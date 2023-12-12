using Newtonsoft.Json;

namespace Zapp.Models.PPL;

public class DailyUsageModel
{
    [JsonProperty("d")]
    public IntervalUsage Data { get; set; }
}

public class IntervalUsage
{
    [JsonProperty("__type")]
    public string Type { get; set; }

    [JsonProperty("AccountId")]
    public string AccountId { get; set; }

    [JsonProperty("MeterNo")]
    public string MeterNo { get; set; }

    [JsonProperty("Multiplier")]
    public int Multiplier { get; set; }

    [JsonProperty("Manufacturer")]
    public object Manufacturer { get; set; }

    [JsonProperty("UsageDate")]
    public string UsageDate { get; set; }

    [JsonProperty("ApprovedIntervalUsage")]
    public ApprovedIntervalUsage ApprovedIntervalUsage { get; set; }

    [JsonProperty("DeliveredIntervalUsage")]
    public object DeliveredIntervalUsage { get; set; }

    [JsonProperty("ReceivedIntervalUsage")]
    public object ReceivedIntervalUsage { get; set; }

    [JsonProperty("PeakMaxData")]
    public List<object> PeakMaxData { get; set; }

    [JsonProperty("PeakMinData")]
    public List<object> PeakMinData { get; set; }

    [JsonProperty("IntervalType")]
    public int IntervalType { get; set; }

    [JsonProperty("AverageTemperature")]
    public int AverageTemperature { get; set; }

    [JsonProperty("IsNetMeter")]
    public bool IsNetMeter { get; set; }

    [JsonProperty("IsTou")]
    public bool IsTou { get; set; }

    [JsonProperty("IsTouHolidayOrWeekend")]
    public bool IsTouHolidayOrWeekend { get; set; }
}

public class ApprovedIntervalUsage
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("IntervalData")]
    public List<object> IntervalData { get; set; }

    [JsonProperty("MinUsage")]
    public object MinUsage { get; set; }

    [JsonProperty("MaxUsage")]
    public object MaxUsage { get; set; }

    [JsonProperty("NetUsageRead")]
    public double NetUsageRead { get; set; }

    [JsonProperty("NetUsageReadOnPeak")]
    public object NetUsageReadOnPeak { get; set; }

    [JsonProperty("NetUsageReadOffPeak")]
    public object NetUsageReadOffPeak { get; set; }

    [JsonProperty("DeliveredUsageRead")]
    public int DeliveredUsageRead { get; set; }

    [JsonProperty("DeliveredUsageReadOnPeak")]
    public object DeliveredUsageReadOnPeak { get; set; }

    [JsonProperty("DeliveredUsageReadOffPeak")]
    public object DeliveredUsageReadOffPeak { get; set; }

    [JsonProperty("ReceivedUsageRead")]
    public int ReceivedUsageRead { get; set; }

    [JsonProperty("ReceivedUsageReadOnPeak")]
    public object ReceivedUsageReadOnPeak { get; set; }

    [JsonProperty("ReceivedUsageReadOffPeak")]
    public object ReceivedUsageReadOffPeak { get; set; }

    [JsonProperty("UsageType")]
    public object UsageType { get; set; }
}
