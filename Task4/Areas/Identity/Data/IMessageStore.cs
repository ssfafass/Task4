using Microsoft.AspNetCore.Identity;

namespace Task4.Areas.Identity.Data
{
    public interface IMessageStore<TMessage> : IDisposable where TMessage : class
    {
        IQueryable<TMessage> Messages { get; }
        Task<IdentityResult> CreateAsync(TMessage message, CancellationToken cancellationToken);
        Task<IdentityResult> UpdateAsync(TMessage message, CancellationToken cancellationToken);
        Task<IdentityResult> DeleteAsync(TMessage message, CancellationToken cancellationToken);
        Task<string> GetMessageIdAsync(TMessage message, CancellationToken cancellationToken);
        Task<string?> GetMessageTitleAsync(TMessage message, CancellationToken cancellationToken);
        Task SetMessageTitleAsync(TMessage message, string? title, CancellationToken cancellationToken);
        Task<string?> GetMessageTextAsync(TMessage message, CancellationToken cancellationToken);
        Task SetMessageTextAsync(TMessage message, string? text, CancellationToken cancellationToken);
        Task<DateTime> GetCreateDateAsync(TMessage message, CancellationToken cancellationToken);
        Task SetMessageCreateDateAsync (TMessage message, DateTime createDate, CancellationToken cancellationToken);
        Task SetMessageSenderAsync(TMessage message, string userId, CancellationToken cancellationToken);
        Task<TMessage?> FindByIdAsync(string id, CancellationToken cancellationToken);
        Task<TMessage?> FindByTitleAsync(string title, CancellationToken cancellationToken);
    }
}
