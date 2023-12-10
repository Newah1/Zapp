using Microsoft.AspNetCore.Mvc;

namespace Zapp.Controllers;

[ApiController]
[Route("[controller]")]
public class ZappController : ControllerBase
{
    public ZappController()
    {
        Console.WriteLine("Big, big chungus...");
    }

    [HttpPost(Name = "Zapp")]
    public string Zapp([FromBody] string carlMarx = "badGuy")
    {
        Console.WriteLine("Calling carl marx" + carlMarx);
        carlMarx = $"{carlMarx} is the bad guy.";
        Console.WriteLine("lemme tell ya something joe rogan");
        return carlMarx;
    }
}
