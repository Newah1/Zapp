using System.Text.Json;
using Cocona;
using Zapp.Models.PPL;
using Zapp.Services;

namespace ZappCli.Commands;

public class PPLCommands : IPPLCommands
{
    private readonly IPPLService _pplService;

    public PPLCommands(IPPLService pplService)
    {
        _pplService = pplService;
    }

    [Command("get")]
    public async Task Get()
    {
        _pplService.GetCookies();
        BillToDateModel billToDate = await _pplService.GetBillToDate();
        string jsonBillToDate = JsonSerializer.Serialize(billToDate);
        Console.WriteLine(jsonBillToDate);
    }
}