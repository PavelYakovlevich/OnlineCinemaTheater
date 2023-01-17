namespace Configurations.MessageBroker;

public static class MessageBrokerQueues
{
    public static string ConfirmEmailQueue => "email-confirm-queue";

    public static string ChangePasswordEmailQueue => "email-password-change-queue";

    public static string AccountCreatedQueue => "account-created-queue";

    public static string MediaInfoUpdatedQueue => "media-info-updated-queue";

    public static string MediaInfoDeletedQueue => "media-info-deleted-queue";

    public static string MediaInfoCreatedQueue => "media-info-created-queue";

    public static string UserCreatedQueue => "user-created-queue";

    public static string UserUpdatedQueue => "user-updated-queue";
}