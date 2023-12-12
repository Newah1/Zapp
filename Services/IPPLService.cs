using Zapp.Models.PPL;

namespace Zapp.Services;

public interface IPPLService
{
    string GetRecaptchaToken();

    Task<BillToDateModel> GetBillToDate();

    Task<DailyUsageModel> GetDailyUsage(DateTime startDate, DateTime endDate);
}
