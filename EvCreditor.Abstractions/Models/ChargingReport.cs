namespace EvCreditor.Abstractions.Models;

public record ChargingReport(
    string SiteName,
    string SiteAddress,
    string SiteCity,
    string SiteZipCode,
    string TimeZone,
    string GroupedBy,
    DateTime FromDate,
    DateTime ToDate,
    IReadOnlyList<ChargingAggregate> Aggregates
);

public record ChargingAggregate(
    string? EntityId,
    string? EntityName, 
    double SessionCount,
    double TotalEnergyKWh,
    TimeSpan TotalDuration
);