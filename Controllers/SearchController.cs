using System.Text.Json.Serialization;
using csharp.Attribute;
using csharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace csharp.Controllers;

[Route("api/search")]
public class SearchController : Controller {

    private readonly AppDbContext _context;

    public SearchController(AppDbContext context) {
        _context = context;
    }

    [HttpGet]
    [Route("")]
    [HasSession]
    public ActionResult MakeSearch(string query) {
        var users = _context.Users.Where(u => u.Username.Contains(query)).Take(4).ToList();

        var posts = _context.Posts
            .Where(p => p.Content.Contains(query))
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

        return Json(new {
            users = users,
            posts = posts
        }, new System.Text.Json.JsonSerializerOptions {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            MaxDepth = 32
        });
    }

}