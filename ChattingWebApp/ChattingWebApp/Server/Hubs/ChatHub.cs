using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ChattingWebApp.Shared.Models;
namespace ChattingWebApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(Message message)
        {
            var users = new string[] { message.FromUserID.ToString(), message.ToUserID.ToString() };
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}
