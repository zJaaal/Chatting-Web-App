using System;

namespace ChattingWebApp.Shared.Models
{
    public class Profile
    {
        public int ProfileID { get; set; }
        public int UserID { get; set; }
        public string Nickname { get; set; }
        public DateTime LastTimeConnected { get; set; }
        public bool Status { get; set; }
        public string ProfilePhoto { get; set; }
        public string AboutMe { get; set; }
    }
}
