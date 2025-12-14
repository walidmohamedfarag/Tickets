using Cinema_Ticket.Services.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Threading.Tasks;

namespace Cinema_Ticket.Services.Services
{
    public class PhotoService : IPhotoService
    {
        public readonly IConfiguration _configuration;
        private CloudinarySettings _cloudinarySettings;
        private Cloudinary cloudinary;

        public PhotoService(IConfiguration configuration)
        {
            _configuration = configuration;
            _cloudinarySettings = _configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>()!;
            var account = new Account(_cloudinarySettings.CloudName, _cloudinarySettings.ApiKey, _cloudinarySettings.ApiSecret);
            cloudinary = new Cloudinary(account);
        }

        public async Task<ActorPhoto> AddPhoto(IFormFile file , string? folder = null)
        {
            var uploadResult = new ImageUploadResult();
            if(file is not null && file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Folder = folder
                    };
                    uploadResult = await cloudinary.UploadAsync(uploadParams);
                }
            }
            return new ActorPhoto 
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.Url.ToString() 
            };
        }
        public async Task<bool> DeletePhoto(string file)
        {
            if(file is null || file == "")
                return false;
            var deleteImage = new DeletionParams(file);
            var result = await cloudinary.DestroyAsync(deleteImage);
            return result.Result == "ok";
        }
    }
}
