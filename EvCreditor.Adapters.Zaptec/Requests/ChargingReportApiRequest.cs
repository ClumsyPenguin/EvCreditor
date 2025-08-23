namespace EvCreditor.Adapters.Zaptec.Requests;

public sealed record ChargingReportApiRequest(
    GroupBy GroupBy,
    DateTime FromDate,
    DateTime EndDate,
    Guid InstallationId
);

public enum GroupBy
{
    User,
    Charger,
    ChargeCardName
}