using AutoMapper;
using MassTransit;
using Messages.NotificationServices;
using Notification.Contract;
using Notification.Core;
using Notification.Core.Configuration;

namespace Notification.API.Consumers
{
    public class SendPasswordChangeEmailMessageConsumer : IConsumer<SendPasswordChangeEmailMessage>
    {
        private readonly INotificationService<EmailDetails> _notificationService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IMapper _mapper;

        public SendPasswordChangeEmailMessageConsumer(
            INotificationService<EmailDetails> notificationService,
            EmailConfiguration emailConfiguration,
            IMapper mapper)
        {
            _notificationService = notificationService;
            _emailConfiguration = emailConfiguration;
            _mapper = mapper;
        }

        public async Task Consume(ConsumeContext<SendPasswordChangeEmailMessage> context)
        {
            var emailDetails = _mapper.Map<EmailDetails>(context.Message);

            emailDetails.From = _emailConfiguration.SenderEmail;
            emailDetails.Subject = _emailConfiguration.ChangePasswordSubject;

            await _notificationService.SendPasswordWasForgottenMailAsync(emailDetails);
        }
    }
}
