using System.Text;
using EvCreditor.Abstractions;
using EvCreditor.Abstractions.Models;
using EvCreditor.Abstractions.Services;
using EvCreditor.Adapters.Zaptec;
using Microsoft.Extensions.Options;

namespace EvCreditor;

public class GetMonthlyChargingUsageJob : BackgroundService
{
    private readonly ILogger<GetMonthlyChargingUsageJob> _logger;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IChargingReportService _reportService;
    private readonly ICreditNoteService _creditNoteService;
    private readonly ZaptecOptions _zaptecOptions;

    public GetMonthlyChargingUsageJob(
        ILogger<GetMonthlyChargingUsageJob> logger,
        IHostApplicationLifetime lifetime,
        IOptions<ZaptecOptions> zaptecOptions,
        IChargingReportService reportService,
        ICreditNoteService creditNoteService)
    {
        _logger = logger;
        _lifetime = lifetime;
        _reportService = reportService;
        _creditNoteService = creditNoteService;
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
            _logger.LogInformation("Creating credit note...");
            var creditNote = BuildCreditNote(report);
            await _creditNoteService.CreateCreditNoteAsync(creditNote, stoppingToken);
            _logger.LogInformation("Successfully created credit note.");
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
    
    private static CreditNoteRequest BuildCreditNote(ChargingReport report)
    {
        var creditNote = new CreditNoteRequest
        {
            OrderDate = DateTime.Today.ToString("yyyy-MM-dd"),
            Customer = new Customer
            {
                Name = report.SiteName,
                Addresses = new List<Address>
                {
                    new()
                    {
                        Street = report.SiteAddress,
                        City = report.SiteCity,
                        CountryCode = "BE" // Assuming Belgium
                    }
                }
            }
        };

        foreach (var aggregate in report.Aggregates)
        {
            creditNote.OrderLines.Add(new OrderLine
            {
                Description = $"Charging costs for {aggregate.EntityName}",
                Quantity = (decimal)aggregate.TotalEnergyKWh,
                UnitPriceExcl = 0.25m, // Assuming a price of €0.25/kWh
                VATPercentage = 21 // Assuming 21% VAT
            });
        }

        return creditNote;
    }
}
