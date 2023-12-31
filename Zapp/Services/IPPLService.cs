using Zapp.Models.PPL;

namespace Zapp.Services;

public interface IPPLService
{
    Dictionary<string, string> GetCookies();

    Task<BillToDateModel> GetBillToDate();

    Task<DailyUsageModel> GetDailyUsage(DateTime startDate, DateTime endDate);
}
