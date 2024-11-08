using System.Text.Json.Serialization;
using csharp.Attribute;
using csharp.Data;
using csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp.Controllers;

[Route("api/posts")]
public class PostController : Controller {
    private readonly AppDbContext _context;

    public PostController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    [HasSession]
    [Route("get/{postId}")]
    public ActionResult GetPost(int postId) {

        var post = _context.Posts
            .Include(p => p.Files)
            .Include(p => p.User)
            .Include(p => p.Likes)
            .SingleOrDefault(p => p.PostId == postId);

        if (post == null) {
            return this.StatusCode(400, new { message = "Este post não existe." });
        }

        foreach (var file in post.Files) {
            file.FilePath = file.FilePath.Substring(file.FilePath.IndexOf("uploads"))
                                    .Replace("\\", "/");
        }

        return Json(new { post }, new System.Text.Json.JsonSerializerOptions {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 32
        });
    }

    [HttpGet]
    [HasSession]
    [Route("/post/{postId}")]
    public ActionResult GetComments(int postId, bool json = false) {
        var post = _context.Posts
        .Include(p => p.User)
        .Include(p => p.Likes)
        .Include(p=> p.Comments)
        .Include(p => p.Files)
        .SingleOrDefault(p => p.PostId == postId);

        if (post == null) {
            return this.StatusCode(400, new { message = "Este post não existe." });
        }

        foreach (var comment in post.Comments){
            comment.User = _context.Users.SingleOrDefault(u => u.Id == comment.UserId);
        }

        foreach (var file in post.Files) {
            file.FilePath = file.FilePath.Substring(file.FilePath.IndexOf("uploads"))
                                    .Replace("\\", "/");
        }

        if (json ){
            return Json(new {
                Post = post,
            }, new System.Text.Json.JsonSerializerOptions {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                MaxDepth = 32
            });
        }

        return View(new CommentsViewModel { Post = post });
    }

    [HttpPost]
    [HasSession]
    [Route("comment/{postId}")]
    public ActionResult CommentPost(int postId, string content) {
        var currentUser = HttpContext.Session.GetInt32("userId");
        if(!currentUser.HasValue) return Unauthorized();

        var post = _context.Posts.SingleOrDefault(p => p.PostId == postId);

        if (post == null) {
            return this.StatusCode(400, new { message = "Este post não existe." });
        }

        var comment = new CommentModel {
            PostId = postId,
            UserId = (int)currentUser,
            Content = content
        };

        _context.Comments.Add(comment);
        _context.SaveChanges();

        comment.User = _context.Users.SingleOrDefault(u => u.Id == comment.UserId);

        return Json(new { comment }, new System.Text.Json.JsonSerializerOptions {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 32
        });
    }


    [HttpGet]
    [HasSession]
    [Route("{postId}/interaction/like")]
    public ActionResult LikePost(int postId) {
        var currentUser = HttpContext.Session.GetInt32("userId");
        if(!currentUser.HasValue) return Unauthorized();

        var post = _context.Posts.SingleOrDefault(p => p.PostId == postId);

        if (post == null) {
            return this.StatusCode(400, new { message = "Este post não existe." });
        }

        var like = _context.Likes.SingleOrDefault(l => l.PostId == postId && l.UserId == currentUser);

        if (like != null) {
            _context.Likes.Remove(like);
            _context.SaveChanges();

            return this.Ok(new { message = "removed", count = post.Likes.Count });
        }

        var newLike = new LikeModel {
            PostId = postId,
            UserId = (int)currentUser
        };

        _context.Likes.Add(newLike);
        _context.SaveChanges();

        return this.Ok(new { message = "added", count = post.Likes.Count });
    }
    
    [HttpGet]
    [Route("")]
    [HasSession]
    public ActionResult GetPosts() {
        var currentUser = HttpContext.Session.GetInt32("userId");

        var getFollowersFromUser = _context.Follows.Where(f => f.FollowerId == currentUser).Select(f => f.FollowedId).ToList();

        var posts = _context.Posts
            .Where(p => getFollowersFromUser.Contains(p.UserId))
            .Include(p => p.Files)
            .Include(p => p.User)
            .OrderByDescending(p => p.PostedAt)
            .ToList();

        foreach (var post in posts) {
            foreach (var file in post.Files) {
                file.FilePath = file.FilePath.Substring(file.FilePath.IndexOf("uploads"))
                                        .Replace("\\", "/");
            }
        }
        
        return Json(new { posts }, new System.Text.Json.JsonSerializerOptions {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 32
        });
    }

    [HttpPost]
    [HasSession]
    [Route("create")]
    public ActionResult CreatePost(string content, List<IFormFile> files){

        if (string.IsNullOrEmpty(content) && files.Count == 0)
        {
            return this.StatusCode(400, new { message = "Os campos não podem estar vazios." }); 
        }

        var currentUser = HttpContext.Session.GetInt32("userId");
        var getUserFromDb = _context.Users.SingleOrDefault(u => u.Id == currentUser);

        if (getUserFromDb == null)
        {
            return this.StatusCode(400, new { message = "Esta conta não existe" }); 
        }

        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var uploadedFiles = new List<FilesModel>();

        if (files.Count > 1){
            return this.StatusCode(400, new { message = "Apenas é permitido um ficheiro por post." });
        }

        foreach (var file in files) {
            if (file.Length == 0){
                continue;
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var newFile = new FilesModel(filePath);
            uploadedFiles.Add(newFile);
        }

        var post = new PostModel{
            UserId = getUserFromDb.Id,
            Content = content ?? "",
        };

        _context.Posts.Add(post);
        _context.SaveChanges();

        foreach (var file in uploadedFiles) {
            file.PostId = post.PostId;
        }

        _context.Files.AddRange(uploadedFiles);
        _context.SaveChanges();

        return this.Ok(new {message = "Post criado com sucesso."});
    }
}