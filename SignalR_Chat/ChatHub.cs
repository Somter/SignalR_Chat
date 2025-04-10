// ChatHub.cs
using Microsoft.AspNetCore.SignalR;
using SignalR_Chat.Models;
using Microsoft.EntityFrameworkCore;

namespace SignalR_Chat
{
    public class ChatHub : Hub
    {
        private readonly ChatDbContext _db;

        public ChatHub(ChatDbContext db)
        {
            _db = db;
        }

        public async Task Send(string username, string message)
        {
            var msg = new Message
            {
                UserName = username,
                Text = message,
                SentAt = DateTime.Now
            };

            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            await Clients.All.SendAsync("AddMessage", username, message);
        }

        public async Task Connect(string userName)
        {
            var id = Context.ConnectionId;

            var existingUsers = await _db.Users
                .Where(u => u.Name == userName || u.ConnectionId == id)
                .ToListAsync();

            if (existingUsers.Any())
            {
                _db.Users.RemoveRange(existingUsers);
                await _db.SaveChangesAsync();
            }

            var user = new User { ConnectionId = id, Name = userName };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var allUsers = await _db.Users.ToListAsync();

            await Clients.Caller.SendAsync("Connected", id, userName, allUsers);
            await GetMessageHistory();
            await Clients.AllExcept(id).SendAsync("NewUserConnected", id, userName);
        }



        public async Task Logout()
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();

                await Clients.AllExcept(Context.ConnectionId).SendAsync("UserDisconnected", user.ConnectionId, user.Name);
                await Clients.Caller.SendAsync("LoggedOut");
            }
        }

        public async Task GetMessageHistory()
        {
            var messages = await _db.Messages.OrderBy(m => m.SentAt).ToListAsync();

            foreach (var msg in messages)
            {
                await Clients.Caller.SendAsync("AddMessage", msg.UserName, msg.Text);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                _db.Users.Remove(user);
                await _db.SaveChangesAsync();
                await Clients.All.SendAsync("UserDisconnected", user.ConnectionId, user.Name);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
