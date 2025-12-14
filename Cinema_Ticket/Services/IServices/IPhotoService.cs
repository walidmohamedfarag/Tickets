namespace Cinema_Ticket.Services.IServices
{
    public interface IPhotoService
    {
        Task<ActorPhoto> AddPhoto(IFormFile file , string? folder = null);
        Task<bool> DeletePhoto(string file);
    }
}
