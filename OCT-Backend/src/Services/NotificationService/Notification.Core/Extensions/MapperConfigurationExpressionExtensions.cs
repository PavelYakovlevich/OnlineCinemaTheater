using AutoMapper;
using Messages.NotificationServices;

namespace Notification.Core.Extensions;

public static class MapperConfigurationExpressionExtensions
{
    public static void AddMessagesToModelsMapping(this IMapperConfigurationExpression configure)
    {
        configure.CreateMap<SendConfirmationEmailMessage, EmailDetails>()
            .ForMember(details => details.Body, conf => conf.MapFrom(message => message.ConfirmationLink));

        configure.CreateMap<SendPasswordChangeEmailMessage, EmailDetails>()
            .ForMember(details => details.Body, conf => conf.MapFrom(message => message.ChangePasswordLink));
    }
}
