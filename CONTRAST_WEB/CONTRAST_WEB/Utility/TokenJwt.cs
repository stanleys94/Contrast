using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CONTRAST_WEB.Models
{
    public class TokenJwt
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AppId { get; set; }
    }
}