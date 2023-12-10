using Microsoft.AspNetCore.Mvc;
using Zapp.Models.PPL;
using Zapp.Services;

namespace Zapp.Controllers;

[ApiController]
[Route("[controller]")]
public class ZappController : ControllerBase
{
    private readonly IPPLService _pplService;

    public ZappController(IPPLService pplService)
    {
        _pplService = pplService;
        Console.WriteLine("Big, big chungus...");
    }

    [HttpGet(Name = "Zapp")]
    public async Task<BillToDateModel> Zapp()
    {
        var cookie = _pplService.GetRecaptchaToken();
        var billToDateModel = await _pplService.GetBillToDate();
        return billToDateModel;
    }
}
