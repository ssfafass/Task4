namespace Task4.Areas.Identity.Data
{
    public interface IUserMessageStore<TUser, TMessage> where TUser : class
    {
        IQueryable<TUser> Users { get; }
        Task AddMessagesAsync(TUser user, TMessage[] messages, CancellationToken cancellationToken);
        Task AddMessageAsync(TUser user, TMessage message, CancellationToken cancellationToken);
        Task<IList<TMessage>?> GetMessagesUserAsync(TUser user, CancellationToken cancellationToken);
        Task<IList<TUser>?> GetUsersInMessageAsync(TMessage message, CancellationToken cancellationToken);
        Task<TUser?> GetMessageSenderAsync(TMessage message, CancellationToken cancellationToken);
        Task<IList<TMessage>> GetMessagesSenderUserAsync(TUser user, CancellationToken cancellationToken);
    }
}
