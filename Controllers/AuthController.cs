namespace Controllers.authController;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

[ApiController]
[Route("auth")]
public class AuthController: ControllerBase{

    [HttpGet("login")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public IActionResult Login()
    {

        var obj = new {
            first = "Ivan",
            last = "Gerdzhikov"
        };

        string output = JsonConvert.SerializeObject(obj);

        return Ok(output);

    }

}