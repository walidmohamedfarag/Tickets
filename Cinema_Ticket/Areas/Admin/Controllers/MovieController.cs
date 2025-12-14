
using Cinema_Ticket.Models;
using Cinema_Ticket.Services.IServices;
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace Movie_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IRepositroy<Movie> movieRepo;
        private readonly IRepositroy<Cinema> cinemaRepo;
        private readonly IRepositroy<MovieSubImg> movieSubImgRepo;
        private readonly IRepositroy<MovieActor> movieActorRepo;
        private readonly IRepositroy<Actor> actorRepo;
        private readonly IPhotoService photoService;

        public MovieController(IRepositroy<Movie> _movieRepo, IRepositroy<Cinema> _cinemaRepo, IRepositroy<MovieSubImg> _movieSubImgRepo, IRepositroy<MovieActor> _movieActorRepo, IRepositroy<Actor> _actorRepo, IPhotoService _photoService)
        {
            movieRepo = _movieRepo;
            cinemaRepo = _cinemaRepo;
            movieSubImgRepo = _movieSubImgRepo;
            movieActorRepo = _movieActorRepo;
            actorRepo = _actorRepo;
            photoService = _photoService;
        }

        public async Task<IActionResult> ShowAll(CancellationToken cancellationToken)
        {
            var movies = await movieRepo.GetAsync(includes: [m => m.Cinema], cancellationToken: cancellationToken);
            return View(movies);
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var cinemas = await cinemaRepo.GetAsync(cancellationToken: cancellationToken);
            ViewBag.actros = await actorRepo.GetAsync();
            return View(cinemas);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile MainImg, IEnumerable<IFormFile> subImgs, int[] actrosid, CancellationToken cancellationToken)
        {
            #region Add Main Image And Movie
            if (MainImg is not null && MainImg.Length > 0)
            {
                var photo = await photoService.AddPhoto(MainImg, "Movie-Image");
                movie.MainImg = photo.Url;
                movie.PublicId = photo.PublicId;
            }
            await movieRepo.AddAsync(movie, cancellationToken: cancellationToken);
            await movieRepo.CommitAsync(cancellationToken);
            #endregion

            foreach (var item in actrosid)
                await movieActorRepo.AddAsync(new MovieActor { MovieId = movie.Id, ActorId = item }, cancellationToken: cancellationToken);
            await movieActorRepo.CommitAsync(cancellationToken);

            #region Add Sub Images
            if (subImgs is not null && subImgs.Any())
            {
                foreach (var subimg in subImgs)
                {
                    var photo = await photoService.AddPhoto(subimg, "Movie-SubImage");
                    await movieSubImgRepo.AddAsync(new MovieSubImg { Img = photo.Url, PublicId = photo.PublicId, MovieId = movie.Id }, cancellationToken: cancellationToken);
                }
                await movieSubImgRepo.CommitAsync(cancellationToken);
            }
            #endregion

            TempData["success-notification"] = "Movie Created Successfully";
            return RedirectToAction(nameof(ShowAll));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var movie = await movieRepo.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            var movieImgs = await movieSubImgRepo.GetAsync(mv => mv.MovieId == id, cancellationToken: cancellationToken);
            var cinemas = await cinemaRepo.GetAsync();
            return View(new ModelVM { Movie = movie, subImgs = movieImgs, Cinemas = cinemas });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile mainimg, IEnumerable<IFormFile> subImgs, CancellationToken cancellationToken)
        {
            // Get Old Movie To Update
            var oldMovie = await movieRepo.GetOneAsync(c => c.Id == movie.Id, tracked: false);

            #region Edit Main Image
            if (mainimg is not null && mainimg.Length > 0)
            {
                if (oldMovie!.MainImg is not null)
                    await photoService.DeletePhoto(oldMovie.PublicId!);
                var photo = await photoService.AddPhoto(mainimg, "Movie-Image");
                movie.MainImg = photo.Url;
                movie.PublicId = oldMovie.PublicId;
            }
            else
                movie.MainImg = oldMovie.MainImg;
            movieRepo.Update(movie);
            #endregion

            #region Edit Sub Image
            if (subImgs is not null && subImgs.Any())
            {
                //Get Old Image To Delete 
                var oldSubImgs = await movieSubImgRepo.GetAsync(mv => mv.MovieId == movie.Id, tracked: false, cancellationToken: cancellationToken);
                if (!oldSubImgs.Any())
                    return View("ErrorPage", "Home");
                // Loop To Delete Old Sub Image
                foreach (var oimg in oldSubImgs)
                    await photoService.DeletePhoto(oimg.PublicId!);
                // Add New Sub Image
                foreach (var subimg in subImgs)
                {
                    var photo = await photoService.AddPhoto(subimg, "Movie-SubImage");
                    await movieSubImgRepo.AddAsync(new MovieSubImg { Img = photo.Url, PublicId = photo.PublicId, MovieId = movie.Id }, cancellationToken: cancellationToken);
                }

            }
            await movieSubImgRepo.CommitAsync(cancellationToken);
            #endregion

            TempData["success-notification"] = "Movie Edited Successfully";
            return RedirectToAction(nameof(ShowAll));
        }
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {

            #region Delete Movie With Main And Sub Images
            var movie = await movieRepo.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            if (movie is null)
                return View("ErrorPage", "Home");
            // Delete Sub Images 
            var movieSubImges = await movieSubImgRepo.GetAsync(m => m.MovieId == id, cancellationToken: cancellationToken);
            foreach (var oimg in movieSubImges)
            {
                await photoService.DeletePhoto(oimg.PublicId!);
                movieSubImgRepo.Delete(oimg);
            }
            // Delete Main Image 
            await photoService.DeletePhoto(movie.PublicId!);
            movieRepo.Delete(movie);
            await movieRepo.CommitAsync(cancellationToken);
            #endregion

            TempData["success-notification"] = "Movie Deleted Successfully";
            return RedirectToAction(nameof(ShowAll));

        }
        public async Task<IActionResult> DeleteSubImg(int id, string img, CancellationToken cancellationToken)
        {
            #region Delete Specifiy Sub Image
            var movieSubImg = await movieSubImgRepo.GetOneAsync(c => c.MovieId == id && c.Img == img, cancellationToken: cancellationToken);
            if (movieSubImg is null)
                return View("ErrorPage", "Home");
            await photoService.DeletePhoto(movieSubImg.PublicId!);
            movieSubImgRepo.Delete(movieSubImg);
            await movieSubImgRepo.CommitAsync(cancellationToken);
            #endregion

            TempData["success-notification"] = "Sub Image Deleted Successfully";
            return RedirectToAction(nameof(Edit), new { id = id });

        }


    }
}
