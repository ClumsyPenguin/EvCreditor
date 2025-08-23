namespace EvCreditor.Adapters.Zaptec;

public class ZaptecOptions
{
    public const string SectionName = "Zaptec";

    public required string Username { get; init; }
    public required string Password { get; init; }
    public required Guid InstallationId { get; init; }
}
