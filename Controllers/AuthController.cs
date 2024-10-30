namespace server.Controllers;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("auth")]
public class AuthController: ControllerBase{

    [HttpPost("login")]
    // [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public IActionResult Login([FromBody] string data)
    {

        


        return Ok();
    }

}