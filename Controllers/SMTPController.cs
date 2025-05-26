using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using server.Models.SMTPModels;
using static server.Models.ProjectEnum;

namespace server.Controllers;

[ApiController]
[Route("SMTP")]
public class SMPTController : ControllerBase
{

    private readonly ISMTPService SMTPService;

    public SMPTController(ISMTPService _SMTPService)
    {
        SMTPService = _SMTPService;
    }

    [HttpPost("sendEmail")]
    public async Task<bool> SendEmail([FromBody] sendEmail data)
    {
        bool isSent = await SMTPService.SendEmail(data);

        if (!isSent) return false;

        return true;
    }


    [HttpGet("isSubscribed/{email}")]
    public async Task<bool> IsSubscribed([FromRoute] string email)
    {
        bool isSubscribed = await SMTPService.IsSubscribed(email);

        return isSubscribed;
    }


    [HttpPost("updateSubscriptionStatus")]
    public async Task<bool> updateSubscriptionStatus([FromBody] updateSubscriptionStatusModel model)
    {
        bool isSubscribed = await SMTPService.updateSubscriptionStatus(model);

        if (!isSubscribed) return false;

        return true;

    }


}