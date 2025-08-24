using System.Text;
using System.Text.Json;
using EvCreditor.Abstractions.Models;
using EvCreditor.Abstractions.Services;
using EvCreditor.Adapters.Billit.Models;
using Microsoft.Extensions.Options;

namespace EvCreditor.Adapters.Billit.Services;

public class BillitService : ICreditNoteService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BillitOptions _options;

    public BillitService(IHttpClientFactory httpClientFactory, IOptions<BillitOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    public async Task CreateCreditNoteAsync(CreditNoteRequest creditNote, CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Add("ApiKey", _options.ApiKey);

        var json = JsonSerializer.Serialize(creditNote);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("https://api.billit.be/v1/orders", content, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
