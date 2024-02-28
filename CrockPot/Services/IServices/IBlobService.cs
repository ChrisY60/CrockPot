namespace CrockPot.Services.IServices
{
    public interface IBlobService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
