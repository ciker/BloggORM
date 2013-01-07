using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy;
using Simple.Data;
using Nancy.Session;

namespace BloggORM
{
    public class MainModule : NancyModule
    {
        string connectionString = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Database.sdf");

        string salt = "$2a$10$Sng4hgUrhG.Ik1LMRBTXQ.";

        public MainModule()
        {
            Get["/"] = _ =>
            {
                return View["login"];
            };

            Post["/login"] = _ =>
            {
                var db = Database.OpenFile(connectionString);
                Models.User user = db.Users.FindByUsername(Request.Form.username);
                if (user == null)
                {
                    return View["failed_login"];
                    
                }
                if (BCrypt.Net.BCrypt.Verify(Request.Form.password, user.Password) == false)
                {
                    return View["failed_login"];
                }
                else
                {
                    Session["user"] = user.Id.ToString();
                    return Response.AsRedirect("/index");
                }

            };

            Get["/account_creation"] = _ =>
            {
                return View["account_creation"];
            };

            Post["/create_account"] = _ =>
            {
                var db = Database.OpenFile(connectionString);
                string cipher = BCrypt.Net.BCrypt.HashPassword(Request.Form.password, salt);
                var user = db.Users.FindByUsernameAndPassword(Request.Form.username, cipher);
                if (user == null)
                {
                    db.Users.Insert(Username: Request.Form.username, Password: cipher);
                    return View["created_new_user"];
                }
                else
                {
                    return View["failed_account_creation"];
                }
            };

            Post["/created_an_account"] = _ =>
            {
                return Response.AsRedirect("/");
            };

            Get["/index"] = _ =>
            {
                var userId = (string)Session["user"] ?? "";
                if (userId.Length == 0)
                {
                    return Response.AsRedirect("/");
                }
                else
                {
                    var db = Database.OpenFile(connectionString);
                    List<Models.Post> posts = db.Posts.All().WithUser();
                    var user = db.Users.FindById(userId);
                    return View["index", new { User = user, Posts = posts }];
                }
            };

            Get["/post_creation"] = _ =>
            {
                return View["post_creation"];
            };

            Post["/create_post"] = _ =>
            {
                var db = Database.OpenFile(connectionString);
                var userId = (string)Session["user"];
                var user = db.Users.FindById(userId);
                DateTime date_time = DateTime.Now;
                String.Format("{0:d/M/yyyy HH:mm}", date_time);
                db.Post.Insert(Title: Request.Form.title, Body: Request.Form.body, UserId: userId, CreationDate: date_time);
                return Response.AsRedirect("/index");
            };

            Get["/post/{id}"] = parameters =>
            {
                var userId = (string)Session["user"] ?? "";
                if (userId.Length == 0)
                {
                    return Response.AsRedirect("/");
                }
                else
                {
                    var db = Database.OpenFile(connectionString);
                    List<Models.Post> post = db.Posts.FindAllById(parameters.id).WithUser();
                    List<Models.Comment> comment = db.Comments.FindAllByPostId(parameters.id).WithUser();
                    var user = db.Users.FindById(userId);
                    return View["post", new { Post = post.First(), User = user, Comment = comment}];
                }
            };

            Post["/logout"] = _ =>
            {
                Session.DeleteAll();
                return Response.AsRedirect("/");
            };
        }
    }
}