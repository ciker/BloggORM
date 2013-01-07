using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloggORM.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public String Message { get; set; }
        public User User { get; set; }

        public Comment()
        {

        }
    }
}