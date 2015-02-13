using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace maQx.Models
{
    public class GenericEmail : Email
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
    }

    public class AdminConfirmationEmail : GenericEmail
    {
        public string ConfirmationCode { get; set; }
    }

    public class UserInvite : GenericEmail
    {
        public string OrganizationName { get; set; }
        public string AcivationLink { get; set; }
        public string Position { get; set; }
    }
}