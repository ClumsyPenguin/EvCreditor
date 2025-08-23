using System.Net.Http.Headers;
using System.Net.Http.Json;
using EvCreditor.Adapters.Zaptec.Requests;
using EvCreditor.Adapters.Zaptec.Responses;
using EvCreditor.Abstractions;
using EvCreditor.Abstractions.Models;
using Microsoft.Extensions.Options;

namespace EvCreditor.Adapters.Zaptec;

public class ZapTecReportService: IChargingReportService
{
    private readonly HttpClient _httpClient;
    private readonly string _username;
    private readonly string _password;
    

    public ZapTecReportService(HttpClient httpClient, IOptions<ZaptecOptions> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.zaptec.com/");
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        var zaptecOptions = options.Value;
        _username = zaptecOptions.Username;
        _password = zaptecOptions.Password;
    }
    
    public async Task<ChargingReport> GetChargingReport(ChargingReportRequest request, CancellationToken cancellationToken)
    {
        var token = await GetOauthToken(cancellationToken);

        var reportApiRequest = new ChargingReportApiRequest(GroupBy.User, request.FromDate, request.EndDate, request.InstallationId);
        
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        using var response = await _httpClient.PostAsJsonAsync("api/chargehistory/installationreport", reportApiRequest, cancellationToken);
        
        response.EnsureSuccessStatusCode();

        var zaptec = await response.Content.ReadFromJsonAsync<ChargingReportApiResponse>(cancellationToken)
                     ?? throw new InvalidOperationException("Empty installation report response");

        var aggregates = zaptec.totalUserChargerReportModel.Select(m =>
            new ChargingAggregate(
                EntityId: null,
                EntityName: null,
                SessionCount: m.TotalChargeSessionCount,
                TotalEnergyKWh: m.TotalChargeSessionEnergy,
                TotalDuration: TimeSpan.FromHours(m.TotalChargeSessionDuration)
            )
        ).ToList();

        return new ChargingReport(
            SiteName: zaptec.InstallationName,
            SiteAddress: zaptec.InstallationAddress,
            SiteCity: zaptec.InstallationCity,
            SiteZipCode: zaptec.InstallationZipCode,
            TimeZone: zaptec.InstallationTimeZone,
            GroupedBy: zaptec.GroupedBy,
            FromDate: zaptec.Fromdate,
            ToDate: zaptec.Enddate,
            Aggregates: aggregates
        );
    }
    
    private async Task<string> GetOauthToken(CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "oauth/token");
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["username"] = _username,
            ["password"] = _password
        });
        
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Zaptec OAuth failed: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {err}");
        }

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken)
                    ?? throw new InvalidOperationException("Empty token response");
        return token.access_token;
    }
}