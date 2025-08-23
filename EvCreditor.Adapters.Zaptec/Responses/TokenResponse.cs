namespace EvCreditor.Adapters.Zaptec.Responses;

public record TokenResponse(
    string access_token,
    string token_type,
    int expires_in,
    string? scope
);