namespace EvCreditor.Abstractions.Models;

public sealed record ChargingReportRequest(
    DateTime FromDate,
    DateTime EndDate,
    Guid InstallationId
);