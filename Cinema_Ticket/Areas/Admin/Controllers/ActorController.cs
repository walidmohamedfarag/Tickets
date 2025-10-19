using Cinema_Ticket.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    public class ActorController : Controller
    {
        private readonly ApplicationDB context = new();
        public IActionResult ShowAll()
        {
            var actors = context.MovieActors.Include(m=>m.Actor).Include(m=>m.Movie);
            return View(actors);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var movies = context.Movies;
            return View(movies);
        }
        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile img ,int movieId)
        {
            if (img is not null && img.Length > 0)
            {
                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ActorImage", imgName);
                using (var stream = System.IO.File.Create(imgPath))
                {
                    img.CopyTo(stream);
                }
                actor.Img = imgName;
            }
            context.Actors.Add(actor);
            context.SaveChanges();
            context.MovieActors.Add(new MovieActor { MovieId = movieId, ActorId = actor.Id });
            context.SaveChanges();
            return RedirectToAction(nameof(ShowAll));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = context.Actors.FirstOrDefault(a => a.Id == id);
            if (actor is null)
                return View("ErrorPage", "Home");
            ViewBag.MovieActotId = context.MovieActors.FirstOrDefault(ma => ma.ActorId == id);
            ViewBag.AllMovie = context.Movies;
            return View(actor);
        }
        [HttpPost]
        public IActionResult Edit(Actor actor , IFormFile img, int? movieId , int oMovieId)
        {
            var oldActor = context.Actors.AsNoTracking().FirstOrDefault(a=>a.Id == actor.Id);
            if (img is not null && img.Length > 0)
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ActorImage", oldActor.Img);
                if (Path.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ActorImage", imgName);
                using (var stream = System.IO.File.Create(imgPath))
                {
                    img.CopyTo(stream);
                }
                actor.Img = imgName;
            }
            else
                actor.Img = oldActor.Img;
            context.Update(actor);
            context.SaveChanges();
            if (movieId is null)
                return View("ErrorPage", "Home");
            var newMovie = context.Movies.FirstOrDefault(m => m.Id == movieId);
            context.MovieActors.Remove(new MovieActor { ActorId = actor.Id, MovieId = oMovieId });
            context.SaveChanges();
            context.MovieActors.Add(new MovieActor { ActorId = actor.Id, MovieId = newMovie.Id });
            context.SaveChanges();
            return RedirectToAction(nameof(ShowAll));
        }
        public IActionResult Delete(int id)
        {
            var oldImg = context.Actors.FirstOrDefault(a => a.Id == id);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ActorImage", oldImg.Img);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            context.Remove(oldImg);
            context.SaveChanges();
            return RedirectToAction(nameof(ShowAll));
        }
    }
}
