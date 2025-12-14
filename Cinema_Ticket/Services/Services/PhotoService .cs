using Cinema_Ticket.Services.IServices;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

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

        public string AddPhotoForUser(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if(file is not null && file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream)
                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }
            return uploadResult.Url.ToString();
        }
    }
}
