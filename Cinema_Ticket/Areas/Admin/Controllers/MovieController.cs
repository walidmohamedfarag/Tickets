
using Microsoft.CodeAnalysis;

namespace Movie_Ticket.Areas.Admin.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDB _db = new();
        public IActionResult ShowAll()
        {
            var movies = _db.Movies.Include(mv=>mv.Cinema);
            return View(movies);
        }
        [HttpGet]
        public IActionResult Create()
        {
            var cinemas = _db.Cinemas;
            return View(cinemas);
        }
        [HttpPost]
        public IActionResult Create(Movie movie , IFormFile MainImg , IEnumerable<IFormFile> subImgs)
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
            _db.Movies.Add(movie);
            _db.SaveChanges();
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
                    _db.MovieSubImgs.Add(new MovieSubImg { Img = subimgName, MovieId = movie.Id });
                }
                _db.SaveChanges();
            }
            #endregion


            return RedirectToAction(nameof(ShowAll));
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _db.Movies.FirstOrDefault(c => c.Id == id);
            var movieImgs = _db.MovieSubImgs.Where(mv=>mv.MovieId == id);
            var cinemas = _db.Cinemas;
            return View(new ModelVM { Movie = movie , subImgs = movieImgs , Cinemas = cinemas});
        }
        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile mainimg , IEnumerable<IFormFile> subImgs)
        {
            // Get Old Movie To Update
            var oldMovie = _db.Movies.AsNoTracking().FirstOrDefault(c => c.Id == movie.Id);

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
            _db.Update(movie);
            _db.SaveChanges();
            #endregion

            #region Edit Sub Image
            if (subImgs is not null && subImgs.Any())
            {
                //Get Old Image To Delete 
                var oldSubImgs = _db.MovieSubImgs.AsNoTracking().Where(mv => mv.MovieId == movie.Id);
                if(!oldSubImgs.Any())
                    return View("ErrorPage", "Home");
                // Loop To Delete Old Sub Image
                foreach(var oimg in oldSubImgs)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", oimg.Img);
                    if (Path.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                    _db.MovieSubImgs.Remove(oimg);
                }
                _db.SaveChanges();
                // Add New Sub Image
                foreach (var subimg in subImgs)
                {
                    var subimgName = Guid.NewGuid() + Path.GetExtension(subimg.FileName);
                    var pathSubImg = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", subimgName);
                    using (var stream = System.IO.File.Create(pathSubImg))
                    {
                        subimg.CopyTo(stream);
                    }
                    _db.MovieSubImgs.Add(new MovieSubImg { Img = subimgName, MovieId = movie.Id });
                }
                _db.SaveChanges();

            }
            #endregion

            return RedirectToAction(nameof(ShowAll));
        }
        public IActionResult Delete(int id)
        {

            #region Delete Movie With Main And Sub Images
            var movie = _db.Movies.FirstOrDefault(c => c.Id == id);
            if (movie is null)
                return View("ErrorPage", "Home");
            // Delete Main Image 
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg", movie.MainImg);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            _db.Remove(movie);
            _db.SaveChanges();
            // Delete Sub Images 
            var movieSubImges = _db.MovieSubImgs.Where(m=>m.MovieId == id);
            foreach (var oimg in movieSubImges)
            {
                var oldSubImgs = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", oimg.Img);
                if (Path.Exists(oldSubImgs))
                    System.IO.File.Delete(oldSubImgs);
                _db.MovieSubImgs.Remove(oimg);
            }
            _db.SaveChanges();
            #endregion

            return RedirectToAction(nameof(ShowAll));

        }
        public IActionResult DeleteSubImg(int id , string img)
        {
            #region Delete Specifiy Sub Image
            var movieSubImg = _db.MovieSubImgs.FirstOrDefault(c => c.MovieId == id && c.Img == img);
            if (movieSubImg is null)
                return View("ErrorPage", "Home");
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\MovieImg\SubImges", movieSubImg.Img);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            _db.MovieSubImgs.Remove(movieSubImg);
            _db.SaveChanges();
            #endregion

            return RedirectToAction(nameof(Edit), new { id = id });

        }


    }
}
