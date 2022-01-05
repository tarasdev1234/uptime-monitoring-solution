using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Reliablesite.Authority.Authentication;
using Uptime.Data;
using Uptime.Monitoring.Data;
using Uptime.Monitoring.Model.Abstractions;
using Uptime.Monitoring.Model.Models;
using Uptime.Mvc.Controllers;
using Uptime.Plugin.Dto;
using Uptime.Plugin.Extensions;
using Uptime.Plugin.ViewModels;
using Uptime.Plugin.ViewModels.Dashboard;
using Uptime.Plugin.ViewModels.Monitor;
using IMonitoringTaskService = Uptime.Plugin.Services.IMonitoringTaskService;
using Reliablesite.Service.Model.Dto;
using System.Net.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Uptime.Plugin.Controllers
{
    [Authorize]
    public class MonitorController : BaseController {
        private readonly UptimeMonitoringContext monitoringContext;
        private readonly IEventsService eventSvc;
        private readonly IMonitoringTaskService monitoringTaskService;
        private readonly IMapper mapper;

        public MonitorController (
            ApplicationDbContext ctx,
            UptimeMonitoringContext monitoringContext,
            IEventsService eventSvc,
            IMonitoringTaskService monitoringTaskService,
            IMapper mapper
        ) : base(ctx) {
            this.monitoringContext = monitoringContext;
            this.eventSvc = eventSvc;
            this.monitoringTaskService = monitoringTaskService;
            this.mapper = mapper;
        }
        
        [HttpGet]
        [Route("/api/monitors")]
        [ProducesResponseType(typeof(PaginatedItemsVm<Monitor>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetMonitors ([FromQuery]PagedQuery p, [FromQuery(Name = "s")] string search = "", int sort = 1) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = User.GetId();

            var query = monitoringContext.Monitors
                .Where(m => m.UserId == userId);

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(m => m.Name.Contains(search));
            }

            switch ((MonitorSortTypes)sort) {
                case MonitorSortTypes.NAME_ASC:
                    query = query.OrderBy(m => m.Name);
                    break;

                case MonitorSortTypes.NAME_DESC:
                    query = query.OrderByDescending(m => m.Name);
                    break;

                case MonitorSortTypes.HTTP_KWRD_PNG_PRT:
                case MonitorSortTypes.KWRD_HTTP_PNG_PRT:
                    query = query.OrderBy(m => m.Type);
                    break;

                case MonitorSortTypes.PNG_PRT_HTTP_KWRD:
                case MonitorSortTypes.PRT_PNG_HTTP_KWRD:
                    query = query.OrderByDescending(m => m.Type);
                    break;

                default:
                    query = query.OrderBy(m => m.Id);
                    break;
            }
            
            var totalItems = await query.LongCountAsync();

            var monitors = (await query
                .Paged(p)
                .ToListAsync())
                .ToDictionary(x => x.Id,
                m => mapper.Map<MonitorDto>(m));

            var weekAgo = DateTimeOffset.UtcNow.AddDays(-7);
            var events = (await eventSvc.GetEventsByMonitorsAsync(userId, monitors.Keys, weekAgo)).Where(x => x.SourceEventId == null).GroupBy(x => x.MonitorId);

            foreach(var group in events)
            {
                monitors[group.Key].MonitoringHistory = group
                    .OrderBy(x => x.Created)
                    .Select(x => mapper.Map<MonitoringSample>(x))
                    .ToList();
            }

            var model = new PaginatedItemsVm<MonitorDto>(p.PageIndex.Value + 1, p.PageSize.Value, totalItems, monitors.Values);

            return Ok(model);
        }

        [HttpPost]
        [Route("/api/monitors")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create ([FromBody]CreateMonitorViewModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = User.GetId();

            var alerts = monitoringContext.AlertContacts.Where(ac => model.AlertContacts.Contains(ac.Id)).ToList();

            var monitor = default(Monitor);

            switch((MonitorType)model.Type)
            {
                case MonitorType.HTTP:
                    monitor = new HttpMonitor();
                    break;

                case MonitorType.PING:
                    monitor = new PingMonitor();
                    break;

                default:
                    return BadRequest("Unknown monitor type");
            }

            monitor.UserId = userId;
            monitor.Name = model.Name;
            monitor.Interval = model.Interval;
            monitor.Url = model.Url;
            monitor.AuthType = model.AuthType;
            monitor.CreatedDate = DateTime.UtcNow;

            var monitorAlerts = alerts.Select(ac => new MonitorAlertContact() {
                AlertContact = ac,
                Monitor = monitor
            }).ToList();

            await monitoringContext.MonitorAlertContacts.AddRangeAsync(monitorAlerts);
            await monitoringContext.Monitors.AddAsync(monitor);

            await monitoringContext.SaveChangesAsync();

            await monitoringTaskService.StartAsync(monitor.Id);

            return CreatedAtAction(nameof(GetMonitor), new { id = monitor.Id }, null);
        }

        [HttpGet]
        [Route("/api/monitors/{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMonitor (long id) {
            if (id <= 0) {
                return BadRequest();
            }

            var userId = User.GetId();

            var monitor = await monitoringContext.Monitors
                .Include(m => m.AlertContacts)
                .Where(m => m.Id == id && m.UserId == userId)
                .FirstOrDefaultAsync();

            if (monitor == null) {
                return NotFound();
            }

            return Ok(new CreateMonitorViewModel() {
                Id = monitor.Id,
                Url = monitor.Url,
                AuthType = monitor.AuthType,
                HttpPassword = "",
                HttpUser = "",
                Interval = monitor.Interval,
                Name = monitor.Name,
                Type = (int)monitor.Type,
                AlertContacts = monitor.AlertContacts.Select(ac => ac.AlertContactId).ToList()
            });
        }

        [HttpDelete]
        [Route("/api/monitors/{id:long}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Delete (long id) {
            var userId = User.GetId();

            var mon = await monitoringContext.Monitors
                .Where(m => m.UserId == userId && m.Id == id)
                .FirstOrDefaultAsync();

            if (mon == null) {
                return NotFound();
            }

            await monitoringTaskService.StopAsync(mon.Id);

            monitoringContext.Monitors.Remove(mon);

            await monitoringContext.SaveChangesAsync();
            await eventSvc.DeleteEventsAsync(mon.UserId, mon.Id);

            return NoContent();
        }

        [HttpPut]
        [Route("/api/monitors/{id:long}/status")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SetStatus (long id, [FromQuery(Name = "status")] int newStatus) {
            try {
                var mon = await monitoringContext
                    .Monitors
                    .Where(m => m.Id == id)
                    .FirstOrDefaultAsync();

                if (mon == null) {
                    return NotFound($"Monitor with id '{id}' not found");
                }

                var status = (MonitorStatus)newStatus;

                switch (status) {
                    case MonitorStatus.Started:
                        await monitoringTaskService.StartAsync(mon.Id);
                        break;
                    case MonitorStatus.Stopped:
                        await monitoringTaskService.StopAsync(mon.Id);
                        break;
                    default:
                        return BadRequest($"Unexpected monitor status: {status}");
                }

            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpPost]
        [Route("/api/monitors/{id:long}/reset")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> Reset (long id) {
            var mon = await monitoringContext.Monitors.FindAsync(id);

            if (mon == null) {
                return NotFound();
            }

            await eventSvc.DeleteEventsAsync(mon.UserId, mon.Id);

            // TODO: do we need to start the monitor if its not started?
            //mon.Status = (int)EventType.STARTED;

            //await monitoringContext.SaveChangesAsync();
            //await eventSvc.InsertSummaryEvent(new MonitoringEvent() {
            //    MonitorId = mon.Id,
            //    MonitorName = mon.Name,
            //    Code = 0,
            //    Description = "OK",
            //    ResponseTime = 0,
            //    Type = EventType.STARTED
            //});

            return NoContent();
        }

        [HttpGet]
        [Route("/api/monitors/summary")]
        [ProducesResponseType(typeof(PaginatedItemsVm<MonitoringEventDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetSummary ([FromQuery]LastEventsRequest request) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var userId = User.GetId();

            var summary = await GetSummaryEvents(request, userId);

            return Ok(new {
                Data = summary,
                PagingState = request.paging != null ? Convert.ToBase64String(request.paging) : ""
            });
        }

        [HttpGet]
        [Route("/api/monitors/{monitorId:long}/logs")]
        public async Task<IActionResult> GetMonitorLogs(long monitorId) {

            var monitor = await monitoringContext.Monitors.FindAsync(monitorId);

            if (monitor == null || monitor.LastExecutor == Guid.Empty)
            {
                return NotFound();
            }

            if (monitor.UserId != User.GetId())
            {
                return Forbid();
            }

            var url = new UriBuilder(Request.GetEncodedUrl());
            url.Path = $"/services/{monitor.LastExecutor}/api/internal/logs/{monitorId}";
            url.Query = string.Empty;

            HttpClient client = new HttpClient();

            var logStream = await client.GetStreamAsync(url.Uri);

            return File(logStream, "text/plain", $"monitor-{monitorId}.log");
        }

        private async Task<IEnumerable<MonitoringEventDto>> GetSummaryEvents (LastEventsRequest request, long userId) {
            IReadOnlyCollection<MonitoringEventDto> result = new List<MonitoringEventDto>();
            var userMonitors = new Dictionary<long, string>();

            if (request.MonitorId > 0) {
                var userMonitor = await monitoringContext.Monitors
                    .FirstOrDefaultAsync(m => m.Id == request.MonitorId && m.UserId == userId);

                if (userMonitor == null) {
                    return result;
                }

                userMonitors.Add(request.MonitorId, userMonitor.Name);
            } else {
                userMonitors = await monitoringContext.Monitors
                   .Where(m => m.UserId == userId)
                   .ToDictionaryAsync(m => m.Id, m => m.Name);
            }

            var pagination = new Pagination { PageSize = request.PageSize };

            if (!string.IsNullOrEmpty(request.PagingState)) {
                pagination.State = Convert.FromBase64String(request.PagingState);
            }

            try
            {
                var events = request.MonitorId > 0
                    ? await eventSvc.GetEventsByMonitorAsync(userId, request.MonitorId, pagination)
                    : await eventSvc.GetRecentEvents(userId, pagination);

                request.paging = pagination.State;

                return events.Select(x => new MonitoringEventDto
                {
                    Id = x.Id,
                    Created = x.Created,
                    Type = x.Type,
                    Duration = x.LastRepeat.HasValue ? x.LastRepeat - x.Created : null,
                    MonitorName = userMonitors.TryGetOrDefault(x.MonitorId, $"{x.MonitorId}"),
                    Description = x.Details.Description
                });

            } catch (Exception ex) {
                throw ex;
            }
        }

        //[HttpGet]
        //[Route("/api/monitors/events")]
        //[ProducesResponseType(typeof(PaginatedItemsVm<MonitoringEvent>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public async Task<IActionResult> GetLastEvents ([FromQuery]LastEventsRequest request) {
        //    if (!ModelState.IsValid) {
        //        return BadRequest(ModelState);
        //    }

        // use userMgr.GetUserId(User)
        //    if (!long.TryParse(identitySvc.GetUserIdentity(), out var userId)) {
        //        return Forbid();
        //    }

        //    var data = request.MonitorId <= 0 ? await GetAllLastEvents(userId) : await GetLastEvents(request, userId);

        //    return Ok(new {
        //        Data = data,
        //        PagingState = request.paging != null ? Convert.ToBase64String(request.paging) : ""
        //    });
        //}

        //private async Task<IEnumerable<MonitoringEvent>> GetAllLastEvents (long userId, string table = "events") {
        //    var userMonitors = await monitoringContext.Monitors
        //        .Where(m => m.UserId == userId)
        //        .Select(m => m.Id)
        //        .ToListAsync();

        //    return await eventSvc.GetAllLastEvents(userMonitors, table);
        //}

        //private async Task<IEnumerable<MonitoringEvent>> GetLastEvents (LastEventsRequest request, long userId) {
        //    var events = new List<MonitoringEvent>();

        //    var userMonitor = await monitoringContext.Monitors
        //        .AnyAsync(m => m.Id == request.MonitorId && m.UserId == userId);

        //    if (!userMonitor) {
        //        return events;
        //    }

        //    PreparedStatement prepared;
        //    IStatement bound;
        //    var startingId = request.StartingId;
        //    var types = string.Join(", ", (int)EventType.UP, (int)EventType.DOWN);

        //    if (string.IsNullOrEmpty(startingId)) {
        //        prepared = await statementCache.GetOrAddAsync(
        //            $"SELECT eventid, monitorid, dateof(eventid) AS date, code, reason, monitorname, response_time, type FROM events WHERE monitorid = ? AND type IN({types})"
        //        );
        //        bound = prepared.Bind(request.MonitorId);
        //    } else {
        //        prepared = await statementCache.GetOrAddAsync(
        //            $"SELECT eventid, monitorid, dateof(eventid) AS date, code, reason, monitorname, response_time, type FROM events WHERE monitorid = ? AND id <= ? AND type IN({types})"
        //        );
        //        bound = prepared.Bind(request.MonitorId, startingId);
        //    }

        //    bound.SetAutoPage(false)
        //         .SetPageSize(request.PageSize);

        //    if (!string.IsNullOrEmpty(request.PagingState)) {
        //        bound.SetPagingState(Convert.FromBase64String(request.PagingState));
        //    }

        //    RowSet rows = await session.ExecuteAsync(bound).ConfigureAwait(false);
        //    request.paging = rows.PagingState;

        //    return rows.Select(eventSvc.MapRowToEvent);
        //}
    }
}
