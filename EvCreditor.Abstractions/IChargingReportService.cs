using EvCreditor.Abstractions.Models;

namespace EvCreditor.Abstractions;

public interface IChargingReportService
{
    Task<ChargingReport> GetChargingReport(ChargingReportRequest request, CancellationToken cancellationToken);
}