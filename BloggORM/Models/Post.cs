using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BloggORM.Models
{
    public class Post
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Body { get; set; }
        public User User { get; set; }
        public String CreationDate { get; set; }
        public IList<Comment> Comments { get; set; }

        public Post()
        {

        }
    }
}