using server.Models.SMTPModels;

namespace server.Contracts;

public interface ISMTPService
{

    public Task<bool> SendEmail(sendEmail data);

    public Task<bool> IsSubscribed(string email);

    public Task<bool> updateSubscriptionStatus(updateSubscriptionStatusModel model);

    public Task<bool> SendSingleEmail(sendEmail data);

}