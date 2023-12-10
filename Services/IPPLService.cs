namespace Zapp.Services;

public interface IPPLService
{
    string GetRecaptchaToken();

    Task<Zapp.Models.PPL.BillToDateModel> GetBillToDate();
}
