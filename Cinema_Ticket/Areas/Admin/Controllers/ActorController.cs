
using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    public class ActorController : Controller
    {
        private readonly IRepositroy<Actor> actorRepo;
        private readonly IRepositroy<MovieActor> movieActorRepo;
        private readonly IRepositroy<Movie> movieRepo;

        public ActorController(IRepositroy<Actor> _actorRepo , IRepositroy<MovieActor> _movieActorRepo , IRepositroy<Movie> _movieRepo)
        {
            actorRepo = _actorRepo;
            movieActorRepo = _movieActorRepo;
            movieRepo = _movieRepo;
        }

        public async Task<IActionResult> ShowAll(CancellationToken cancellationToken)
        {
            var actors =await movieActorRepo.GetAsync(includes: [ma => ma.Movie, ma => ma.Actor], cancellationToken: cancellationToken);
            return View(actors);
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var movies =await movieRepo.GetAsync(cancellationToken : cancellationToken);
            return View(movies);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile img ,int movieId , CancellationToken cancellationToken)
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
            await actorRepo.AddAsync(actor , cancellationToken: cancellationToken);
            await actorRepo.CommitAsync( cancellationToken);
            await movieActorRepo.AddAsync(new MovieActor { MovieId = movieId, ActorId = actor.Id } , cancellationToken: cancellationToken);
            await movieActorRepo.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(ShowAll));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken cancellationToken)
        {
            var actor =await actorRepo.GetOneAsync(a => a.Id == id , cancellationToken: cancellationToken);
            if (actor is null)
                return View("ErrorPage", "Home");
            ViewBag.MovieActotId =await movieActorRepo.GetOneAsync(ma => ma.ActorId == id , cancellationToken: cancellationToken);
            ViewBag.AllMovie =await movieRepo.GetAsync(cancellationToken:cancellationToken);
            return View(actor);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor , IFormFile img, int? movieId , int oMovieId , CancellationToken cancellationToken)
        {
            var oldActor = await actorRepo.GetOneAsync(a=>a.Id == actor.Id , tracked:false , cancellationToken : cancellationToken);
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
            actorRepo.Update(actor);
            await actorRepo.CommitAsync(cancellationToken);
            if (movieId is null)
                return View("ErrorPage", "Home");
            var newMovie =await movieRepo.GetOneAsync(m => m.Id == movieId , cancellationToken: cancellationToken);
            movieActorRepo.Delete(new MovieActor { ActorId = actor.Id, MovieId = oMovieId });
            await movieActorRepo.CommitAsync(cancellationToken);
            await movieActorRepo.AddAsync(new MovieActor { ActorId = actor.Id, MovieId = newMovie.Id } , cancellationToken: cancellationToken);
            await movieActorRepo.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(ShowAll));
        }
        public async Task<IActionResult> Delete(int id , CancellationToken cancellationToken)
        {
            var oldImg =await actorRepo.GetOneAsync(a => a.Id == id , cancellationToken: cancellationToken);
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ActorImage", oldImg.Img);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            actorRepo.Delete(oldImg);
            await actorRepo.CommitAsync(cancellationToken);
            return RedirectToAction(nameof(ShowAll));
        }
    }
}
