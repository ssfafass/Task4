using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Task4.Areas.Identity.Data;

namespace Task4.SignalRHub
{
    [Authorize]
    public class SignalRServer : Hub
    {
    }
}
