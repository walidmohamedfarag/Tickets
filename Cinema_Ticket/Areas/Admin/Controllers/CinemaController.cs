
namespace Cinema_Ticket.Areas.Admin.Controllers
{
    public class CinemaController : Controller
    {
        private readonly ApplicationDB DB = new();
        public IActionResult ShowAll()
        {
            var cinemas = DB.Cinemas;
            return View(cinemas);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile img)
        {
            if (img is not null && img.Length > 0)
            {
                var imgName = Guid.NewGuid().ToString() + Path.GetExtension(img.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\CinemaImg", imgName);
                using (var stream = System.IO.File.Create(imgPath))
                {
                    img.CopyTo(stream);
                }
                cinema.Img = imgName;
            }
            if (ModelState.IsValid)
            {
                DB.Cinemas.Add(cinema);
                DB.SaveChanges();
                return RedirectToAction(nameof(ShowAll));
            }
            return View();
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = DB.Cinemas.FirstOrDefault(c=>c.Id == id);
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema , IFormFile img)
        {
            var oldCinema = DB.Cinemas.AsNoTracking().FirstOrDefault(c=>c.Id ==cinema.Id);
            if (img is not null && img.Length > 0)
            {
                var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\CinemaImg", oldCinema.Img);
                if(Path.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
                var imgName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                var imgPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\CinemaImg", imgName);
                using (var stream = System.IO.File.Create(imgPath))
                {
                    img.CopyTo(stream);
                }
                cinema.Img = imgName;
            }
            else
                cinema.Img = oldCinema.Img;
            DB.Update(cinema);
            DB.SaveChanges();
            return RedirectToAction(nameof(ShowAll));
        }
        public IActionResult Delete(int id)
        {
            var cinema = DB.Cinemas.FirstOrDefault(c=>c.Id == id);
            if (cinema is null)
                return View("ErrorPage", "Home");
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\CinemaImg", cinema.Img);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            DB.Remove(cinema);
            DB.SaveChanges();
            return RedirectToAction(nameof(ShowAll));

        }

    }
}
