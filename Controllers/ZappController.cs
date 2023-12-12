using Microsoft.AspNetCore.Mvc;
using Zapp.Models;
using Zapp.Models.PPL;
using Zapp.Services;

namespace Zapp.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class ZappController : ControllerBase
{
    private readonly IPPLService _pplService;

    public ZappController(IPPLService pplService)
    {
        _pplService = pplService;
    }

    [HttpGet(Name = "Zapp")]
    public async Task<BillToDateModel> Zapp()
    {
        _pplService.GetRecaptchaToken();
        var billToDateModel = await _pplService.GetBillToDate();
        return billToDateModel;
    }

    [HttpPost(Name = "DailyUsage")]
    public async Task<DailyUsageModel> GetDailyUsage(DailyUsageRequestModel request)
    {
        _pplService.GetRecaptchaToken();
        var dailyUsage = await _pplService.GetDailyUsage(request.StartDate, request.EndDate);
        return dailyUsage;
    }
}
