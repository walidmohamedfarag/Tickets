namespace Cinema_Ticket.Services.IServices
{
    public interface IPhotoService
    {
        ActorPhoto AddPhoto(IFormFile file);
        Task<bool> DeletePhoto(string file);
    }
}
