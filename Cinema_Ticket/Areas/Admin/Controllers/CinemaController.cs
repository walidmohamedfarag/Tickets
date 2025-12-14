
using Cinema_Ticket.Services.IServices;
using Microsoft.Graph.Models;
using System.Threading.Tasks;

namespace Cinema_Ticket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CinemaController : Controller
    {
        private readonly IRepositroy<Cinema> cinemaRepo;
        private readonly IPhotoService photoService;

        public CinemaController(IRepositroy<Cinema> _cinemaRepo, IPhotoService _photoService)
        {
            cinemaRepo = _cinemaRepo;
            photoService = _photoService;
        }

        public async Task<IActionResult> ShowAll(CancellationToken cancellationToken)
        {
            var cinemas = await cinemaRepo.GetAsync(cancellationToken: cancellationToken);
            return View(cinemas);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile img, CancellationToken cancellationToken)
        {
            if (img is not null && img.Length > 0)
            {
                var photo = await photoService.AddPhoto(img, "Cinema-Image");
                cinema.Img = photo.Url;
                cinema.PublicId = photo.PublicId;
            }
            await cinemaRepo.AddAsync(cinema, cancellationToken: cancellationToken);
            await cinemaRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Cinema Created Successfully";
            return RedirectToAction(nameof(ShowAll));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var cinema = await cinemaRepo.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            return View(cinema);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile img, CancellationToken cancellationToken)
        {
            var oldCinema = await cinemaRepo.GetOneAsync(c => c.Id == cinema.Id, tracked: false, cancellationToken: cancellationToken);
            if (img is not null && img.Length > 0)
            {
                await photoService.DeletePhoto(oldCinema!.PublicId!);
                var photo = await photoService.AddPhoto(img, "Cinema");
                cinema.Img = photo.Url;
                cinema.PublicId = photo.PublicId;
            }
            else
                cinema.Img = oldCinema!.Img;
            cinemaRepo.Update(cinema);
            await cinemaRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Cinema Edited Successfully";
            return RedirectToAction(nameof(ShowAll));
        }
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var cinema = await cinemaRepo.GetOneAsync(c => c.Id == id, cancellationToken: cancellationToken);
            if (cinema is null)
                return View("ErrorPage", "Home");
            if (cinema.Img is not null)
                await photoService.DeletePhoto(cinema.PublicId!);
            cinemaRepo.Delete(cinema);
            await cinemaRepo.CommitAsync(cancellationToken);
            TempData["success-notification"] = "Cinema Deleted Successfully";
            return RedirectToAction(nameof(ShowAll));

        }

    }
}
