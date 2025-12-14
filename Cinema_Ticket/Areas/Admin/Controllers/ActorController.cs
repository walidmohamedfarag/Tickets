

using Cinema_Ticket.Services.IServices;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ActorController : Controller
    {
        private readonly IRepositroy<Actor> actorRepo;
        private readonly IRepositroy<MovieActor> movieActorRepo;
        private readonly IRepositroy<Movie> movieRepo;
        private readonly IPhotoService photoService;

        public ActorController(IRepositroy<Actor> _actorRepo, IRepositroy<MovieActor> _movieActorRepo, IRepositroy<Movie> _movieRepo, IPhotoService _photoService)
        {
            actorRepo = _actorRepo;
            movieActorRepo = _movieActorRepo;
            movieRepo = _movieRepo;
            photoService = _photoService;
        }

        public async Task<IActionResult> ShowAll(CancellationToken cancellationToken)
        {
            var actors = await actorRepo.GetAsync(cancellationToken: cancellationToken);
            return View(actors);
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var actors = await actorRepo.GetAsync(cancellationToken: cancellationToken);
            return View(actors);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, ActorBirthDateVM actorBirthDateVM, IFormFile img, CancellationToken cancellationToken)
        {
            var photo = photoService.AddPhoto(img);
            actor.Img = photo.Url;
            actor.ImgPublicId = photo.PublicId;
            var birthDate = new DateOnly(actorBirthDateVM.Year, actorBirthDateVM.Month, actorBirthDateVM.Day);
            actor.BirthDate = birthDate;
            await actorRepo.AddAsync(actor, cancellationToken: cancellationToken);
            await actorRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Actor created successfully.";
            return RedirectToAction(nameof(ShowAll));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var actor = await actorRepo.GetOneAsync(a => a.Id == id, cancellationToken: cancellationToken);
            if (actor is null)
                return View("ErrorPage", "Home");
            ViewBag.MovieActotId = await movieActorRepo.GetOneAsync(ma => ma.ActorId == id, cancellationToken: cancellationToken);
            ViewBag.AllMovie = await movieRepo.GetAsync(cancellationToken: cancellationToken);
            return View(actor);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile img, ActorBirthDateVM actorBirthDateVM, CancellationToken cancellationToken)
        {
            var oldActor = await actorRepo.GetOneAsync(a => a.Id == actor.Id, tracked: false, cancellationToken: cancellationToken);
            if (img is not null && img.Length > 0)
            {
                if (oldActor!.Img is not null)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\ActorImage", oldActor.Img);
                    if (Path.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

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
            var birthDate = new DateOnly(actorBirthDateVM.Year, actorBirthDateVM.Month, actorBirthDateVM.Day);
            actor.BirthDate = birthDate;
            actorRepo.Update(actor);
            await actorRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Actor Edited Successfully.";
            return RedirectToAction(nameof(ShowAll));
        }
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var oldImg = await actorRepo.GetOneAsync(a => a.Id == id, cancellationToken: cancellationToken);
            if (oldImg!.Img is not null)
                await photoService.DeletePhoto(oldImg.ImgPublicId!);
            actorRepo.Delete(oldImg);
            await actorRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Actor Deleted Successfully.";
            return RedirectToAction(nameof(ShowAll));
        }
        public async Task<IActionResult> ActorDetails(int id, CancellationToken cancellationToken)
        {
            var _actor = await movieActorRepo.GetAsync(a => a.ActorId == id, tracked: false, cancellationToken: cancellationToken);
            var actor = await actorRepo.GetOneAsync(a => a.Id == id, cancellationToken: cancellationToken);
            List<Movie> movies = new List<Movie>();
            foreach (var item in _actor)
            {
                var movie = await movieRepo.GetOneAsync(m => m.Id == item.MovieId, includes: [m => m.Cinema], cancellationToken: cancellationToken);
                movies.Add(movie);
            }
            return View(new ActorVM
            {
                Actor = actor,
                Movies = movies,
            });
        }
    }
}
