using System.ComponentModel.DataAnnotations;

namespace Task4.Models
{
    public class MessageViewModel
    {
        public string? MessageId { get; set; }

        [Display(Name = "Title")]
        public string? Title { get; set; }
        [Display(Name = "Message")]
        public string? Text { get; set; }
        public DateTime CreateDate { get; set; }
        public string? UserSenderId { get; set; }

        [Display(Name = "Email")]
        public string? UserFullName { get; set; }
    }
}
