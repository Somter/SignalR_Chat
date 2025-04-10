namespace SignalR_Chat.Models
{
    public class User
    {
        public int Id { get; set; }
        public string ConnectionId { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

}
