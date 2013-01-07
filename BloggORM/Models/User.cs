using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloggORM.Models
{
    public class User
    {
        public int Id { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }

        public User()
        {

        }
    }
}