using EvCreditor.Abstractions.Models;

namespace EvCreditor.Abstractions.Services;

public interface ICreditNoteService
{
    Task CreateCreditNoteAsync(CreditNoteRequest creditNote, CancellationToken cancellationToken = default);
}
