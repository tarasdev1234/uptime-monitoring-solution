using AutoMapper;
using Uptime.Notifications.Model.Models;
using Uptime.Notifications.Web.Dto;

namespace Uptime.Notifications.Web
{
    internal sealed class DtoMapperProfile : Profile
    {
        public DtoMapperProfile()
        {
            CreateMap<Template, TemplateShort>();
            CreateMap<Template, TemplateFull>()
                .ReverseMap();
        }
    }
}
