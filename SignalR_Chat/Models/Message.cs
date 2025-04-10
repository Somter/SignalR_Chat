namespace SignalR_Chat.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Text { get; set; } = null!;
        public DateTime SentAt { get; set; } = DateTime.Now;
    }

}
