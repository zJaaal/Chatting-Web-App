namespace ChattingWebApp.Shared.Models
{
    public class Message
    {
        public int MessageID { get; set; }
        public int FromUserID { get; set; }
        public int ToUserID { get; set; }
        public string MessageText { get; set; }

    }
}
