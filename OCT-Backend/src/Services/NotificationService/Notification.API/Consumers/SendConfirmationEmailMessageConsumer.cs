using AutoMapper;
using MassTransit;
using Messages.NotificationServices;
using Notification.Contract;
using Notification.Core;
using Notification.Core.Configuration;

namespace Notification.API.Consumers
{
    public class SendConfirmationEmailMessageConsumer : IConsumer<SendConfirmationEmailMessage>
    {
        private readonly INotificationService<EmailDetails> _notificationService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IMapper _mapper;

        public SendConfirmationEmailMessageConsumer(INotificationService<EmailDetails> notificationService,
            EmailConfiguration emailConfiguration,
            IMapper mapper)
        {
            _notificationService = notificationService;
            _emailConfiguration = emailConfiguration;
            _mapper = mapper;
        }
        
        public async Task Consume(ConsumeContext<SendConfirmationEmailMessage> context)
        {
            var emailDetails = _mapper.Map<EmailDetails>(context.Message);

            emailDetails.From = _emailConfiguration.SenderEmail;
            emailDetails.Subject = _emailConfiguration.EmailConfirmationSubject;

            await _notificationService.SendEmailConfirmationMailAsync(emailDetails)
                .ConfigureAwait(false);
        }
    }
}
