using System.Text.Json.Serialization;
using csharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp.Controllers;

[Route("profile/")]
public class ProfileController : Controller {

    private readonly AppDbContext _context;

    public ProfileController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    [Route("me")]
    public ActionResult GetSelfProfile(bool json = false) {
        var userId = HttpContext.Session.GetInt32("userId");

        if (userId == null) {
            return Unauthorized();
        }

        var user = _context.Users.SingleOrDefault(u => u.Id == userId);

        if (user == null) {
            return NotFound();
        }

        user.Followers = [.. _context.Follows.Where(f => f.FollowedId == user.Id)];
        user.Following = [.. _context.Follows.Where(f => f.FollowerId == user.Id)];

        var posts = _context.Posts
            .Where(p => p.UserId == user.Id)
            .Include(p => p.Files)
            .Include(p => p.User)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.PostedAt)
            .ToList();

        foreach (var post in posts) {
            foreach (var file in post.Files) {
                file.FilePath = file.FilePath.Substring(file.FilePath.IndexOf("uploads"))
                                        .Replace("\\", "/");
            }
        }

        if (json){
            return Json(new {
                User = user,
                Posts = posts
            }, new System.Text.Json.JsonSerializerOptions {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                MaxDepth = 32
            });
        }

        return View("GetProfile", new UserProfileViewModel {
            User = user,
            Posts = posts
        });
    }

    [HttpGet]
    [Route("{username}/follow")]
    public ActionResult FollowProfile(string username){
        var user = _context.Users.SingleOrDefault(u => u.Username == username);

        if (user == null) {
            return NotFound();
        }

        var userId = HttpContext.Session.GetInt32("userId");

        if (userId == null) {
            return Unauthorized();
        }

        var follower = _context.Follows.SingleOrDefault(f => f.FollowerId == userId && f.FollowedId == user.Id);

        if (follower != null) {
            _context.Follows.Remove(follower);
            _context.SaveChanges();
            return Ok(new { message = "unfollowed"});
        }

        _context.Follows.Add(new Models.FollowModel {
            FollowerId = userId.Value,
            FollowedId = user.Id
        });

        _context.SaveChanges();

        return Ok(new { message = "followed"});
    }


    [HttpGet]
    [Route("{username}")]
    public ActionResult GetProfile(string username, bool json = false) {
        var user = _context.Users.SingleOrDefault(u => u.Username == username);

        if (user == null) {
            return NotFound();
        }

        user.Followers = [.. _context.Follows.Where(f => f.FollowedId == user.Id)];
        user.Following = [.. _context.Follows.Where(f => f.FollowerId == user.Id)];

        var posts = _context.Posts
            .Where(p => p.UserId == user.Id)
            .Include(p => p.Files)
            .Include(p => p.User)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.PostedAt)
            .ToList();

        foreach (var post in posts) {
            foreach (var file in post.Files) {
                file.FilePath = file.FilePath.Substring(file.FilePath.IndexOf("uploads"))
                                        .Replace("\\", "/");
            }
        }

        if (json ){
            return Json(new {
                User = user,
                Posts = posts
            }, new System.Text.Json.JsonSerializerOptions {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                MaxDepth = 32
            });
        }

        return View(new UserProfileViewModel {
            User = user,
            Posts = posts
        });

    }
}