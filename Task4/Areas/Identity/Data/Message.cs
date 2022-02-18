using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task4.Areas.Identity.Data
{
    [PersonalData]
    public class Message
    {
        public virtual string Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public virtual string? Title { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public virtual string? Text { get; set; }

        public virtual DateTime CreateDate { get; set; }

        public virtual string? UserSenderId { get; set; }

        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

        public Message()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Message(string title) : this()
        {
            Title = title;
        }

        public Message(string title, string text) : this(title)
        {
            Text = text;
        }
    }
}
