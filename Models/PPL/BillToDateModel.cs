namespace Zapp.Models.PPL;

public class BillToDateModel
{
    public int UsageToDate { get; set; }
    public DateTime AsOfDate { get; set; }
    public int DaysSoFar { get; set; }
    public double AverageDailyCost { get; set; }
    public DateTime BillCycleEndDate { get; set; }
    public int EstimatedDaysLeft { get; set; }
    public string Id { get; set; }
    public double EstimatedBill { get; set; }
    public string Message { get; set; }
}
