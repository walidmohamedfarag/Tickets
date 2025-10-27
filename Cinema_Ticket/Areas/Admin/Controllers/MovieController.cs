
using Microsoft.CodeAnalysis;
using System.Threading.Tasks;

namespace Movie_Ticket.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        private readonly IRepositroy<Movie> movieRepo;
        private readonly IRepositroy<Cinema> cinemaRepo;
        private readonly IRepositroy<MovieSubImg> movieSubImgRepo;


        public MovieController(IRepositroy<Movie> _movieRepo, IRepositroy<Cinema> _cinemaRepo , IRepositroy<MovieSubImg> _movieSubImgRepo)
        {
            movieRepo = _movieRepo;
            cinemaRepo = _cinemaRepo;
            movieSubImgRepo = _movieSubImgRepo;
        }

        public async Task<IActionResult> ShowAll(CancellationToken cancellationToken)
        {
            var movies =await movieRepo.GetAsync(includes: [m => m.Cinema], cancellationToken: cancellationToken);
            return View(movies);
        }
        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var cinemas =await cinemaRepo.GetAsync(cancellationToken:cancellationToken);
            return View(cinemas);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Movie movie , IFormFile MainImg , IEnumerable<IFormFile> subImgs , CancellationToken cancellationToken)
        {
            #region Add Main Image And Movie
            if (MainImg is not null && MainImg.Length > 0)
            {
                var mainimgName = Guid.NewGuid().ToString() + Path.GetExtension(MainImg.FileName);
                var pathMainImg = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg", mainimgName);
                using(var stream = System.IO.File.Create(pathMainImg))
                {
                    MainImg.CopyTo(stream);
                }
                movie.MainImg = mainimgName;
            }
            await movieRepo.AddAsync(movie, cancellationToken:cancellationToken);
            await movieRepo.CommitAsync(cancellationToken);
            #endregion

            #region Add Sub Images
            if (subImgs is not null && subImgs.Any())
            {
                foreach(var subimg in subImgs)
                {
                    var subimgName = Guid.NewGuid() + Path.GetExtension(subimg.FileName);
                    var pathSubImg = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", subimgName);
                    using(var stream = System.IO.File.Create(pathSubImg))
                    {
                        subimg.CopyTo(stream);
                    }
                    await movieSubImgRepo.AddAsync(new MovieSubImg { Img = subimgName, MovieId = movie.Id } , cancellationToken:cancellationToken);
                }
                await movieSubImgRepo.CommitAsync(cancellationToken);
            }
            #endregion


            return RedirectToAction(nameof(ShowAll));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken cancellationToken)
        {
            var movie =await movieRepo.GetOneAsync(c => c.Id == id,cancellationToken:cancellationToken);
            var movieImgs =await movieSubImgRepo.GetAsync(mv=>mv.MovieId == id , cancellationToken:cancellationToken);
            var cinemas =await cinemaRepo.GetAsync();
            return View(new ModelVM { Movie = movie , subImgs = movieImgs , Cinemas = cinemas});
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile mainimg , IEnumerable<IFormFile> subImgs , CancellationToken cancellationToken)
        {
            // Get Old Movie To Update
            var oldMovie =await movieRepo.GetOneAsync(c => c.Id == movie.Id  ,tracked:false);

            #region Edit Main Image
            if (mainimg is not null && mainimg.Length > 0)
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg", oldMovie.MainImg);
                if (Path.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
                var mainimgName = Guid.NewGuid() + Path.GetExtension(mainimg.FileName);
                var mainimgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg", mainimgName);
                using (var stream = System.IO.File.Create(mainimgPath))
                {
                    mainimg.CopyTo(stream);
                }
                movie.MainImg = mainimgName;
            }
            else
                movie.MainImg = oldMovie.MainImg;
            movieRepo.Update(movie);
            await movieRepo.CommitAsync(cancellationToken);
            #endregion

            #region Edit Sub Image
            if (subImgs is not null && subImgs.Any())
            {
                //Get Old Image To Delete 
                var oldSubImgs =await movieSubImgRepo.GetAsync(mv => mv.MovieId == movie.Id , tracked:false , cancellationToken : cancellationToken);
                if(!oldSubImgs.Any())
                    return View("ErrorPage", "Home");
                // Loop To Delete Old Sub Image
                foreach(var oimg in oldSubImgs)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", oimg.Img);
                    if (Path.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                    movieSubImgRepo.Delete(oimg);
                }
                await movieSubImgRepo.CommitAsync(cancellationToken);
                // Add New Sub Image
                foreach (var subimg in subImgs)
                {
                    var subimgName = Guid.NewGuid() + Path.GetExtension(subimg.FileName);
                    var pathSubImg = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", subimgName);
                    using (var stream = System.IO.File.Create(pathSubImg))
                    {
                        subimg.CopyTo(stream);
                    }
                    await movieSubImgRepo.AddAsync(new MovieSubImg { Img = subimgName, MovieId = movie.Id } , cancellationToken:cancellationToken);
                }
                await movieSubImgRepo.CommitAsync(cancellationToken);

            }
            #endregion

            return RedirectToAction(nameof(ShowAll));
        }
        public async Task<IActionResult> Delete(int id , CancellationToken cancellationToken)
        {

            #region Delete Movie With Main And Sub Images
            var movie =await movieRepo.GetOneAsync(c => c.Id == id , cancellationToken: cancellationToken);
            if (movie is null)
                return View("ErrorPage", "Home");
            // Delete Main Image 
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg", movie.MainImg);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            movieRepo.Delete(movie);
            await movieRepo.CommitAsync(cancellationToken);
            // Delete Sub Images 
            var movieSubImges =await movieSubImgRepo.GetAsync(m=>m.MovieId == id , cancellationToken:cancellationToken);
            foreach (var oimg in movieSubImges)
            {
                var oldSubImgs = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", oimg.Img);
                if (Path.Exists(oldSubImgs))
                    System.IO.File.Delete(oldSubImgs);
                movieSubImgRepo.Delete(oimg);
            }
            await movieSubImgRepo.CommitAsync(cancellationToken);
            #endregion

            return RedirectToAction(nameof(ShowAll));

        }
        public async Task<IActionResult> DeleteSubImg(int id , string img , CancellationToken cancellationToken)
        {
            #region Delete Specifiy Sub Image
            var movieSubImg =await movieSubImgRepo.GetOneAsync(c => c.MovieId == id && c.Img == img , cancellationToken:cancellationToken);
            if (movieSubImg is null)
                return View("ErrorPage", "Home");
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", movieSubImg.Img);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            movieSubImgRepo.Delete(movieSubImg);
            await movieSubImgRepo.CommitAsync(cancellationToken);
            #endregion

            return RedirectToAction(nameof(Edit), new { id = id });

        }


    }
}
