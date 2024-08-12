namespace SistemaVigilanciaBCPApi.Services
{
    public interface IImageService
    {
        public Task<byte[]> GetImageFromUrlAsync(string imageUrl);
        public string ConvertImageToBase64(byte[] imageBytes);
    }
}
