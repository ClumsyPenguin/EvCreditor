using EvCreditor.Abstractions;
using EvCreditor.Abstractions.Models;
using EvCreditor.Adapters.Zaptec;
using Microsoft.Extensions.Options;

namespace EvCreditor;

public class GetMonthlyChargingUsageJob : BackgroundService
{
    private readonly ILogger<GetMonthlyChargingUsageJob> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IChargingReportService _reportService;
    private readonly ZaptecOptions _zaptecOptions;

    public GetMonthlyChargingUsageJob(ILogger<GetMonthlyChargingUsageJob> logger,  
        IHostApplicationLifetime lifetime,
        IChargingReportService reportService,
        IOptions<ZaptecOptions> zaptecOptions)
    {
        _logger = logger;
        _lifetime = lifetime;
        _reportService = reportService;
        _zaptecOptions = zaptecOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("GetMonthlyChargingUsageJob started");

            var installationId = _zaptecOptions.InstallationId;

            var today = DateTime.Today;
            var fromDate = new DateTime(today.Year, today.Month, 1);
            var toDate = fromDate.AddMonths(1).AddDays(-1);

            _logger.LogInformation("Fetching charging report for installation {InstallationId} from {FromDate:yyyy-MM-dd} to {ToDate:yyyy-MM-dd}", installationId, fromDate, toDate);

            var request = new ChargingReportRequest(fromDate, toDate, installationId);
            var report = await _reportService.GetChargingReport(request, stoppingToken);

            _logger.LogInformation("Successfully fetched report for {SiteName}", report.SiteName);
            _logger.LogInformation("Total energy consumed: {TotalKwh} kWh", report.Aggregates.Sum(a => a.TotalEnergyKWh));
            _logger.LogInformation("Total sessions: {TotalSessions}", report.Aggregates.Sum(a => a.SessionCount));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while executing GetMonthlyChargingUsageJob");
        }
        finally
        {
            _logger.LogInformation("GetMonthlyChargingUsageJob finished, stopping application.");
            _lifetime.StopApplication();
        }
    }
}
