using System.Text.Json.Serialization;

namespace Zapp.Models.PPL;

public class DailyUsageModel
{
    [JsonPropertyName("d")]
    public List<IntervalUsage> d { get; set; }
}

public class IntervalUsage
{
    [JsonPropertyName("__type")]
    public string Type { get; set; }

    [JsonPropertyName("AccountId")]
    public string AccountId { get; set; }

    [JsonPropertyName("MeterNo")]
    public string MeterNo { get; set; }

    [JsonPropertyName("Multiplier")]
    public int Multiplier { get; set; }

    [JsonPropertyName("Manufacturer")]
    public object Manufacturer { get; set; }

    [JsonPropertyName("UsageDate")]
    public string UsageDate { get; set; }

    [JsonPropertyName("ApprovedIntervalUsage")]
    public ApprovedIntervalUsage ApprovedIntervalUsage { get; set; }

    [JsonPropertyName("DeliveredIntervalUsage")]
    public object DeliveredIntervalUsage { get; set; }

    [JsonPropertyName("ReceivedIntervalUsage")]
    public object ReceivedIntervalUsage { get; set; }

    [JsonPropertyName("PeakMaxData")]
    public List<object> PeakMaxData { get; set; }

    [JsonPropertyName("PeakMinData")]
    public List<object> PeakMinData { get; set; }

    [JsonPropertyName("IntervalType")]
    public int IntervalType { get; set; }

    [JsonPropertyName("AverageTemperature")]
    public int AverageTemperature { get; set; }

    [JsonPropertyName("IsNetMeter")]
    public bool IsNetMeter { get; set; }

    [JsonPropertyName("IsTou")]
    public bool IsTou { get; set; }

    [JsonPropertyName("IsTouHolidayOrWeekend")]
    public bool IsTouHolidayOrWeekend { get; set; }
}

public class ApprovedIntervalUsage
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("IntervalData")]
    public List<object> IntervalData { get; set; }

    [JsonPropertyName("MinUsage")]
    public object MinUsage { get; set; }

    [JsonPropertyName("MaxUsage")]
    public object MaxUsage { get; set; }

    [JsonPropertyName("NetUsageRead")]
    public double NetUsageRead { get; set; }

    [JsonPropertyName("NetUsageReadOnPeak")]
    public object NetUsageReadOnPeak { get; set; }

    [JsonPropertyName("NetUsageReadOffPeak")]
    public object NetUsageReadOffPeak { get; set; }

    [JsonPropertyName("DeliveredUsageRead")]
    public int DeliveredUsageRead { get; set; }

    [JsonPropertyName("DeliveredUsageReadOnPeak")]
    public object DeliveredUsageReadOnPeak { get; set; }

    [JsonPropertyName("DeliveredUsageReadOffPeak")]
    public object DeliveredUsageReadOffPeak { get; set; }

    [JsonPropertyName("ReceivedUsageRead")]
    public int ReceivedUsageRead { get; set; }

    [JsonPropertyName("ReceivedUsageReadOnPeak")]
    public object ReceivedUsageReadOnPeak { get; set; }

    [JsonPropertyName("ReceivedUsageReadOffPeak")]
    public object ReceivedUsageReadOffPeak { get; set; }

    [JsonPropertyName("UsageType")]
    public object UsageType { get; set; }
}
