using AutoMapper;
using Uptime.Monitoring.Model.Models;
using Uptime.Plugin.Dto;

namespace Uptime.Plugin
{
    internal sealed class WebDtoMapperProfile : Profile
    {
        public WebDtoMapperProfile()
        {
            CreateMap<Monitor, MonitorDto>();
            CreateMap<MonitoringEvent, MonitoringSample>()
                .ForMember(x => x.EventId, me => me.MapFrom(src => src.Id))
                .ForMember(x => x.From, me => me.MapFrom(src => src.Created))
                .ForMember(x => x.State, me => me.MapFrom(src => src.Type));
            CreateMap<EventType, MonitoringState>()
                .ConvertUsing((src, dst) =>
                src switch
                {
                    EventType.Up => MonitoringState.Up,
                    EventType.Down => MonitoringState.Down,
                    _ => MonitoringState.Off
                });
        }
    }
}
