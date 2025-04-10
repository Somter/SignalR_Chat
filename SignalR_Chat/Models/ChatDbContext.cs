using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace SignalR_Chat.Models
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Message> Messages => Set<Message>();
    }
}
