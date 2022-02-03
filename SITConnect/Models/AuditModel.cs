using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace SITConnect.Models
{
    public class AuditModel
    {

        public string Id { get; set; }
        public string Action { get; set; }
        public string ActionName { get; set; }
        public string Role { get; set; }
        public string IpAddress { get; set; }
        public string LoggedInAt { get; set; }
        public string LoggedOutAt { get; set; }
        public string LoginStatus { get; set; }
        public string PageAccessed { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public string Time { get; set; }

    }

}
