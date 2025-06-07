using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using server.Contracts;
using server.Models.SMTPModels;

namespace server.Controllers;

[ApiController]
[Route("SMTP")]
[Authorize]
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

    [HttpPost("sendSingleEmail")]
    [AllowAnonymous]
    public async Task<bool> SendSingleEmail([FromBody] sendEmail data)
    {
        bool isSent = await SMTPService.SendSingleEmail(data);

        if (!isSent) return false;

        return true;
    }


    [HttpGet("isSubscribed/{email}")]
    public async Task<IActionResult> IsSubscribed([FromRoute] string email)
    {
        bool isSubscribed = await SMTPService.IsSubscribed(email);

        return Ok(new { isSubscribed });
    }


    [HttpPost("updateSubscriptionStatus")]
    public async Task<bool> updateSubscriptionStatus([FromBody] updateSubscriptionStatusModel model)
    {
        bool isSubscribed = await SMTPService.updateSubscriptionStatus(model);

        if (!isSubscribed) return false;

        return true;

    }


}