using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task4.Data;

namespace Task4.Areas.Identity.Data
{
    public class UserMessageStore<TMessage>
        : UserMessageStore<TMessage, DbContext>
        where TMessage : Message
    {
        public UserMessageStore(AuthDbContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer) { }
    }

    public class UserMessageStore<TMessage, TContext>
        : UserMessageStore<TMessage, TContext, ApplicationUser>
        where TMessage : Message
        where TContext : DbContext
    {
        public UserMessageStore(TContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer) { }
    }

    public class UserMessageStore<TMessage, TContext, TUser>
        : IMessageStore<TMessage>,
        IUserMessageStore<TUser, TMessage>
        where TMessage : Message
        where TContext : DbContext
        where TUser : ApplicationUser
    {
        public UserMessageStore(TContext context, IdentityErrorDescriber? describer = null)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            ErrorDescriber = describer ?? new IdentityErrorDescriber();
        }

        private bool _disposed;

        public virtual IQueryable<TMessage> Messages => Context.Set<TMessage>();

        public virtual IQueryable<TUser> Users => Context.Set<TUser>();

        public virtual TContext Context { get; private set; }

        public IdentityErrorDescriber ErrorDescriber { get; set; }

        public bool AutoSaveChanges { get; set; } = true;

        protected virtual async Task SaveChanges(CancellationToken cancellationToken = default)
        {
            if (AutoSaveChanges)
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
        }

        public virtual async Task<IdentityResult> CreateAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Context.Add(message);

            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> UpdateAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Context.Update(message);
            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public virtual async Task<IdentityResult> DeleteAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            Context.Remove(message);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            return IdentityResult.Success;
        }

        public virtual Task<string> GetMessageIdAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return Task.FromResult(message.Id);
        }

        public virtual void Dispose() => _disposed = true;

        public virtual Task<DateTime> GetCreateDateAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return Task.FromResult(message.CreateDate);
        }

        public virtual Task<string?> GetMessageTextAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return Task.FromResult(message.Text);
        }

        public virtual Task<string?> GetMessageTitleAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return Task.FromResult(message.Title);
        }

        public virtual Task SetMessageTextAsync(TMessage message,
            string? text,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Text = text;

            return Task.CompletedTask;
        }

        public virtual Task SetMessageTitleAsync(TMessage message,
            string? title,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.Title = title;

            return Task.CompletedTask;
        }

        public virtual Task SetMessageCreateDateAsync(TMessage message,
            DateTime createDate,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.CreateDate = createDate;

            return Task.CompletedTask;
        }

        public virtual Task<TMessage?> FindByIdAsync(string id,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();
            return Messages.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public virtual Task<TMessage?> FindByTitleAsync(string title,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            return Messages.FirstOrDefaultAsync(u => u.Title == title, cancellationToken);
        }

        protected virtual void ThrowIsDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        public virtual Task AddMessagesAsync(TUser user, TMessage[] messages,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            user.Messages.AddRange(messages);
            return Task.FromResult(false);
        }

        public virtual Task AddMessageAsync(TUser user, TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            user.Messages.Add(message);

            return Task.FromResult(false);
        }

        public virtual async Task<IList<TMessage>?> GetMessagesUserAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            TUser? temp = await Users.Where(u => u.Id == user.Id)
                .Include(u => u.Messages)
                .FirstOrDefaultAsync(cancellationToken);

            return temp?.Messages as IList<TMessage>;
        }

        public virtual async Task<IList<TUser>?> GetUsersInMessageAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            TMessage? temp = await Messages
                .Where(m => m.Id == message.Id)
                .Include(m => m.Users)
                .FirstOrDefaultAsync(cancellationToken);

            return temp?.Users as IList<TUser>;
        }

        public virtual Task SetMessageSenderAsync(TMessage message, string userId,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            message.UserSenderId = userId;

            return Task.CompletedTask;
        }

        public virtual async Task<TUser?> GetMessageSenderAsync(TMessage message,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return await Users.FirstOrDefaultAsync(u => u.Id == message.UserSenderId, cancellationToken);
        }

        public virtual async Task<IList<TMessage>> GetMessagesSenderUserAsync(TUser user,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIsDisposed();

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return await Messages
                .Where(m => m.UserSenderId == user.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
