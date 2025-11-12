
using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly IRepositroy<Cinema> cinemaRepo;

        public CinemaController(IRepositroy<Cinema> _cinemaRepo)
        {
            cinemaRepo = _cinemaRepo;
        }

        public async Task<IActionResult> ShowAll(CancellationToken cancellationToken)
        {
            var cinemas =await cinemaRepo.GetAsync(cancellationToken: cancellationToken);
            return View(cinemas);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile img , CancellationToken cancellationToken)
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
                await cinemaRepo.AddAsync(cinema , cancellationToken : cancellationToken);
                await cinemaRepo.CommitAsync(cancellationToken);
                TempData["success-notification"] = "Cinema Created Successfully";
                return RedirectToAction(nameof(ShowAll));
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id , CancellationToken cancellationToken)
        {
            var cinema =await cinemaRepo.GetOneAsync(c=>c.Id == id , cancellationToken: cancellationToken);
            return View(cinema);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema , IFormFile img , CancellationToken cancellationToken)
        {
            var oldCinema =await cinemaRepo.GetOneAsync(c=>c.Id ==cinema.Id , tracked:false , cancellationToken: cancellationToken);
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
            cinemaRepo.Update(cinema);
            await cinemaRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Cinema Edited Successfully";
            return RedirectToAction(nameof(ShowAll));
        }
        public async Task<IActionResult> Delete(int id , CancellationToken cancellationToken)
        {
            var cinema =await cinemaRepo.GetOneAsync(c=>c.Id == id , cancellationToken: cancellationToken);
            if (cinema is null)
                return View("ErrorPage", "Home");
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\CinemaImg", cinema.Img);
            if (Path.Exists(oldPath))
                System.IO.File.Delete(oldPath);
            cinemaRepo.Delete(cinema);
            await cinemaRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Cinema Deleted Successfully";
            return RedirectToAction(nameof(ShowAll));

        }

    }
}
