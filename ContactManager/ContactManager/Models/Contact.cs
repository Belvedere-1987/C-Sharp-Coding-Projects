using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContactManager.Models
{
    public class Contact
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public int phoneNumber { get; set; }
    }
}