using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChattingWebApp.Shared.Models
{
    public class AuthenticationRequest
    {
        public string Nickname { get; set; }
        public string Password { get; set; }
    }
}
