using BlogMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BlogMat.Controllers
{
    public class PostController : Controller
    {
        private BlogDbEntities1 blogModel = new BlogDbEntities1();
        private const int PostPerPage = 4;
        public bool IsAdmin { get { return true; } }//Session["IsAdmin"] != null && (bool)Session["IsAdmin"]; } } 

        // GET: Post
        public ActionResult Index(int? id)
        {
            int pageNumber = id ?? 0;
            IEnumerable<Post> posts =
                (from post in blogModel.Post
                where post.DateTime < DateTime.Now
                orderby post.DateTime descending
                select post).Skip(pageNumber * PostPerPage).Take(PostPerPage + 1);
            ViewBag.IsPreviousLinkVisible = pageNumber > 0;
            ViewBag.IsNextLinkVisible = posts.Count() > PostPerPage;
            ViewBag.PageNumber = pageNumber;
            ViewBag.IsAdmin = IsAdmin;

            return View(posts.Take(PostPerPage));
        }
        public ActionResult Details(int id)
        {
            Post post = GetPost(id);
            ViewBag.IsAdmin = IsAdmin;
            return View(post);
        }
        [ValidateInput(false)]
        public ActionResult Comment(int id, string name, string email, string body)
        {
            Post post = GetPost(id);
            Comments comment = new Comments();
            comment.Post = post;
            comment.Name = name;
            comment.DateTime = DateTime.Now;
            comment.Email = email;
            comment.Body = body;
            blogModel.Comments.Add(comment);
            blogModel.SaveChanges();
            return RedirectToAction("Details", new { id = id });
        }
        public ActionResult Tags(string id)
        {
            Tags tag = GetTag(id);
            ViewBag.IsAdmin = IsAdmin;
            return View("Index", tag.Post);
        }
        public ActionResult Update(int? id, string title, string body, DateTime dateTime, string tags)
        {
            if (!IsAdmin)
            {
                return RedirectToAction("Index");
            }
            Post post = GetPost(id);
            post.Title = title;
            post.Body = body;
            post.DateTime = dateTime;
            post.Tags.Clear();

            tags = tags ?? string.Empty;
            string[] tagNames = tags.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string tagName in tagNames)
            {
                post.Tags.Add(GetTag(tagName));
            }
            if (!id.HasValue)
            {                
                blogModel.Post.Add(post);
            }
            blogModel.SaveChanges();
            return RedirectToAction("Details", new { id = post.Id });
        }
        public ActionResult Delete(int id)
        {
            if (IsAdmin)
            {
                Post post = GetPost(id);
                blogModel.Post.Remove(post);
                blogModel.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult DeleteComment(int id)
        {
            if (IsAdmin)
            {
                Comments comment = blogModel.Comments.Where(x => x.Id == id).First();
                blogModel.Comments.Remove(comment);
                blogModel.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int? id)
        {
            Post post = GetPost(id);
            StringBuilder tagList = new StringBuilder();
            foreach(Tags tag in post.Tags)
            {
                tagList.AppendFormat(tag.Name);
            }
            ViewBag.Tags = tagList.ToString();
            return View(post);
        }
        private Tags GetTag(string tagName)
        {
            return blogModel.Tags.Where(x => x.Name == tagName).FirstOrDefault() ?? new Tags() { Name = tagName };
        }

        private Post GetPost(int? id)
        {
            return id.HasValue ? blogModel.Post.Where(x => x.Id == id).First() : new Post() { Id = -1 };
        }
    }
}