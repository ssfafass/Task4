using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;
using Task4.Areas.Identity.Data;
using Task4.Models;
using Task4.SignalRHub;

namespace Task4.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUserMessageStore<ApplicationUser, Message> _userMessageStore;
        private readonly IMessageStore<Message> _messageStore;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<SignalRServer> _hub;

        public HomeController(IUserMessageStore<ApplicationUser, Message> userMessageStore,
            IMessageStore<Message> messageStore,
            IUserStore<ApplicationUser> userStore,
            UserManager<ApplicationUser> userManager, IHubContext<SignalRServer> hub)
        {
            _userMessageStore = userMessageStore;
            _messageStore = messageStore;
            _userStore = userStore;
            _userManager = userManager;
            _hub = hub;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User?.Identity?.Name);

            var messages = await _userMessageStore.GetMessagesUserAsync(user, CancellationToken.None);

            var messagesOrderbyTime = messages?.OrderByDescending(m => m.CreateDate).ToList();

            var vm = new List<MessageViewModel>();

            if (messagesOrderbyTime == null)
            {
                return View(vm);
            }

            foreach (var messsage in messagesOrderbyTime)
            {
                var tempUser = await _userManager.FindByIdAsync(messsage.UserSenderId);

                vm.Add(new MessageViewModel()
                {
                    CreateDate = messsage.CreateDate,
                    MessageId = messsage.Id,
                    Text = messsage.Text,
                    Title = messsage.Title,
                    UserSenderId = messsage.UserSenderId,
                    UserFullName = tempUser?.FullName ?? "Deleted"
                });
            }

            return View(vm);
        }

        public async Task<IActionResult> Create(string? userSenderId)
        {
            if (userSenderId == null)
            {
                return View();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(userSenderId);

            if (user == null)
            {
                return View();
            }

            var cvm = new MessageCreateViewModel
            {
                Emails = user.Email,
                UserName = user.FullName
            };

            return View(cvm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MessageCreateViewModel vm)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(vm.Emails))
            {
                return RedirectToAction("Create");
            }
            
            string[] emails = vm.Emails.Split(",")
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(u => u.Trim())
                .Distinct()
                .ToArray();

            List<ApplicationUser> receiveUser = new();

            foreach (var email in emails)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(email);

                if (user == null)
                {
                    continue;
                }

                receiveUser.Add(user);
            }

            ApplicationUser identityUser = await _userManager.FindByNameAsync(User?.Identity?.Name);

            Message message = new();

            await _messageStore.SetMessageTitleAsync(message, vm.Title, CancellationToken.None);
            await _messageStore.SetMessageTextAsync(message, vm.Message, CancellationToken.None);
            await _messageStore.SetMessageCreateDateAsync(message, DateTime.Now, CancellationToken.None);
            await _messageStore.SetMessageSenderAsync(message, identityUser.Id, CancellationToken.None);
            

            foreach (var user in receiveUser)
            {
                await _userMessageStore.AddMessageAsync(user, message, CancellationToken.None);              
            }

            await _messageStore.CreateAsync(message, CancellationToken.None);

            foreach(var user in receiveUser)
            {
                await _hub.Clients.User(user.Id).SendAsync("Message", message.Id);
                await _hub.Clients.User(user.Id).SendAsync("Toast", identityUser.FullName);
            }

            return Redirect("/");
        }

        public async Task<IActionResult> GetMessage(string? messageId)
        {
            if (messageId == null)
            {
                return NotFound();
            }

            Message message = await _messageStore.FindByIdAsync(messageId, CancellationToken.None);

            if (message == null)
            {
                return NotFound();
            }

            ApplicationUser sender = await _userStore.FindByIdAsync(message.UserSenderId, CancellationToken.None);

            MessageViewModel vm = new()
            {
                MessageId = message.Id,
                CreateDate = message.CreateDate,
                Text = message.Text,
                Title = message.Title,
                UserSenderId = message.UserSenderId,
                UserFullName = sender.FullName
            };

            return PartialView("_MessagePartial", vm);
        }

        public IActionResult AutocompleteSearch(string term)
        {
            if (term == null)
            {
                var result = _userManager.Users.Select(u => u.Email).ToArray();

                return Json(result);
            }

            var resultObject = _userManager.Users.Where(u => u.Email.StartsWith(term)).Select(u => u.Email).ToArray();

            return Json(resultObject);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}