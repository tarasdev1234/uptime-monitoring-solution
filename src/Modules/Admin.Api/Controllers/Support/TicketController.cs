using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Admin.Api.ViewModels.Ticket;
using Admin.Api.ViewModels;
using Microsoft.Extensions.Options;
using Dapper;
using Uptime.Data;
using Uptime.Data.Config;
using Uptime.Data.Models.Support;
using Uptime.Events.EventHandlers.Tickets;
using Uptime.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Reliablesite.Authority.Authentication;
using Reliablesite.Authority.Client;
using Reliablesite.Service.Model.Dto;

namespace Admin.Api.Controllers.Support
{
    [Route("api/tickets")]
    [Authorize(Roles = "Admin")]
    public class TicketController : BaseController {
        private readonly IOptions<DbSettings> settings;
        private readonly IEnumerable<ITicketEventsHandler> eventHandlers;
        private readonly IConfiguration Configuration;
        private readonly IUsersClient users;

        public TicketController (
            ApplicationDbContext context,
            IConfiguration config,
            IEnumerable<ITicketEventsHandler> events,
            IOptions<DbSettings> appSettings,
            IUsersClient users
        ) : base(context) {
            this.users = users;
            Configuration = config;
            settings = appSettings;
            eventHandlers = events;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedItemsVm<Ticket>), (int)HttpStatusCode.OK)]
        [Authorize/*(Policy = Constants.Permissions.READ_TICKETS)*/]
        public async Task<IActionResult> GetTickets ([FromQuery]TicketQuery ticketQuery, [FromQuery]PagedQuery pagination, [FromQuery(Name = "s")] string search = "") {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = dbContext.Tickets
                        .Include(t => t.Status)
                        .Include(t => t.Department)
                        .AsQueryable();

            if (!string.IsNullOrEmpty(search)) {
                query = query.Where(t => t.CustomerEmail.Contains(search));
            }

            if (ticketQuery.Queue.GetValueOrDefault()) {
                query = query.Where(t => t.Status.IsOpen && t.UserId == null);
            } else {
                var isOpen = ticketQuery.Open.GetValueOrDefault();
                var isActive = ticketQuery.Active.GetValueOrDefault();

                query = query.Where(t => t.Status.IsOpen == isOpen && t.Status.IsActive == isActive && t.UserId != null);

                if (!ticketQuery.Global.GetValueOrDefault()) {
                    var usrId = User.GetId();

                    query = query.Where(t => t.UserId == usrId);
                }
            }

            var count = await query.LongCountAsync();
            var tckts = await query
                        .Select(t => new Ticket() {
                            Id = t.Id,
                            DepartmentName = t.Department.Name,
                            Subject = t.Subject,
                            UserId = t.UserId,
                            CustomerId = t.CustomerId,
                            CustomerEmail = t.CustomerEmail,
                            DepartmentId = t.DepartmentId,
                            StatusId = t.StatusId,
                            IsDeleted = t.IsDeleted,
                            DateOpened = t.DateOpened,
                            DateClosed = t.DateClosed
                        })
                        .OrderBy(p => p.Id)
                        .Paged(pagination)
                        .ToListAsync();

            var view = new PaginatedItemsVm<Ticket>(pagination.PageIndex.Value, pagination.PageSize.Value, count, tckts);

            return Ok(view);
        }

        [Authorize/*(Policy = Constants.Permissions.READ_TICKETS)*/]
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Ticket), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetTicket (long id) {
            var ticket = await dbContext.Tickets
                                .Include(t => t.Department)
                                .Include(t => t.Status)
                                .Include(t => t.Messages)
                                .Include(t => t.Comments)
                                    .ThenInclude(c => c.User)
                                .Include(t => t.Events)
                                .Where(t => t.Id == id)
                                .FirstOrDefaultAsync();

            if (ticket == null) {
                return NotFound();
            }

            ticket.Messages = ticket.Messages.OrderByDescending(m => m.Id).ToList();
            ticket.Comments = ticket.Comments.OrderByDescending(c => c.Id).ToList();

            return Ok(ticket);
        }

        [Route("{id}/comment")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddComment (long id, [FromForm] string text) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var usrId = User.GetId();

            var type = await dbContext.CommentTypes.FirstOrDefaultAsync();

            var comment = new TicketComment() {
                UserId = usrId,
                Text = text,
                CommentTypeId = type.Id,
                TicketId = id,
                Date = DateTime.Now,
            };

            dbContext.TicketComments.Add(comment);

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Route("update/{id:long}")]
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> AssignTicket (long id, [FromForm]TicketOptionsViewModel options) {
            var assigned = false;
            var statusChanged = false;
            TicketStatus oldStatus = null;

            var ticket = await dbContext.Tickets
                            .Include(t => t.Status)
                            .Where(t => t.Id == id)
                            .FirstOrDefaultAsync();

            if (ticket == null) {
                return NotFound();
            }

            if (options.AgentId.HasValue) {
                try
                {
                    await users.IsExistAsync(options.AgentId.Value);
                }
                catch (ApiException e) when (e.StatusCode == 404)
                {
                    return Forbid();
                }

                assigned = options.AgentId != ticket.UserId;
            }

            if (ticket.StatusId != options.StatusId) {
                statusChanged = true;
                oldStatus = ticket.Status;
            }

            ticket.UserId = options.AgentId;
            ticket.StatusId = options.StatusId;
            ticket.DepartmentId = options.DepartmentId;

            await dbContext.SaveChangesAsync();

            if (statusChanged || assigned) {
                foreach (var handler in eventHandlers) {
                    if (statusChanged) {
                        await handler.StatusChanged(ticket, oldStatus);
                    }

                    if (assigned) {
                        await handler.Assigned(ticket, options.AgentId.Value);
                    }
                }
            }

            return Ok();
        }

        [Route("take/{id:long}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> TakeTicket (long id) {
            var userId = User.GetId();

            var ticket = await dbContext.Tickets.FindAsync(id);

            if (ticket == null) {
                return NotFound();
            }

            ticket.UserId = userId;

            await dbContext.SaveChangesAsync();

            return Ok();
        }

        [Route("reply/{id:long}")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Reply (long id, [FromForm]TicketReplyViewModel reply) {
            var userId = User.GetId();

            var ticket = await dbContext.Tickets
                                .Include(t => t.Status)
                                .Where(t => t.Id == id && t.UserId == userId)
                                .FirstOrDefaultAsync();

            if (ticket == null) {
                return NotFound();
            }

            var statusChanged = ticket.StatusId != reply.NewStatusId;
            var oldStatus = ticket.Status;

            ticket.StatusId = reply.NewStatusId;

            var message = new TicketMessage() {
                TicketId = ticket.Id,
                UserId = userId,
                To = reply.To,
                From = User.GetEmail(),
                Direction = TicketMessage.MessageDirection.FromSupport,
                Body = reply.Message,
                Subject = reply.Subject,
                Date = DateTime.Now
            };

            dbContext.TicketMessages.Add(message);

            await dbContext.SaveChangesAsync();

            if (statusChanged) {
                foreach (var handler in eventHandlers) {
                    await handler.StatusChanged(ticket, ticket.Status);
                }
            }

            return Ok();
        }
        
        [Route("create")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> StartTicket ([FromBody] TicketCreateViewModel vm) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var dprtmnt = await dbContext.Departments.FindAsync(vm.DepartmentId);

            if (dprtmnt == null) {
                ModelState.AddModelError("", "Invalid Department");
                return BadRequest(ModelState);
            }

            var userId = User.GetId();
            var status = await dbContext.TicketStatuses.Where(ts => ts.Name == TicketStatus.OPEN).FirstOrDefaultAsync();
            
            var tckt = new Ticket() {
                Subject = vm.Subject,
                CustomerEmail = vm.Email,
                DepartmentId = vm.DepartmentId,
                CustomerId = userId,
                StatusId = status.Id,
                DateOpened = DateTime.Now
            };

            tckt.Messages = new List<TicketMessage>() {
                new TicketMessage() {
                    Subject = vm.Subject,
                    Body = vm.Message,
                    UserId = userId
                }
            };

            dbContext.Tickets.Add(tckt);

            await dbContext.SaveChangesAsync();

            foreach (var handler in eventHandlers) {
                await handler.Created(tckt);
            }

            return CreatedAtAction(nameof(GetTicket), new { id = tckt.Id }, null);
        }

        [HttpGet("summary")]
        [ProducesResponseType(typeof(TicketSummaryViewModel), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Summary () {
            var sql = @"SELECT sum(case when StatusId = @waiting AND UserId = @userid then 1 else 0 end) as Waiting,
			                   sum(case when StatusId = @open AND UserId = @userid then 1 else 0 end) as Active,
			                   sum(case when StatusId = @open AND UserId IS NULL then 1 else 0 end) as Queue,
			                   sum(case when StatusId = @open AND UserId IS NOT NULL then 1 else 0 end) as ActiveGlobal,
			                   sum(case when StatusId = @waiting AND UserId IS NOT NULL then 1 else 0 end) as WaitingGlobal
                        FROM Tickets";

            var userId = User.GetId();
            var statuses = await dbContext.TicketStatuses.ToListAsync();
            var open = statuses.Where(s => s.Name == TicketStatus.OPEN).FirstOrDefault().Id;
            var waiting = statuses.Where(s => s.Name == TicketStatus.WAITING).FirstOrDefault().Id;

            var summary = new TicketSummaryViewModel();

            var connection = dbContext.Database.GetDbConnection();
            var result = await connection.QueryAsync<dynamic>(sql, new { userId, open, waiting });

            if (result.AsList().Count > 0)
            {
                var res = result as dynamic;

                summary.Waiting = res[0].Waiting;
                summary.Active = res[0].Active;
                summary.Queue = res[0].Queue;
                summary.ActiveGlobal = res[0].ActiveGlobal;
                summary.WaitingGlobal = res[0].WaitingGlobal;
            }

            return Ok(summary ?? new TicketSummaryViewModel());
        }

        [Authorize(Policy = Constants.Permissions.DELETE_TICKETS)]
        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeleteTicket (long id) {
            var t = await dbContext.Tickets.FindAsync(id);

            if (t == null) {
                return NotFound();
            }

            dbContext.Tickets.Remove(t);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("statuses")]
        [ProducesResponseType(typeof(PaginatedItemsVm<TicketStatus>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetStatuses () {
            var statuses = await dbContext.TicketStatuses.ToListAsync();

            return Ok(statuses);
        }
    }
}
