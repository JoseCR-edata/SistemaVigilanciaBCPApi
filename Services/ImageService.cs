

using System.Net;

namespace SistemaVigilanciaBCPApi.Services
{
    public class ImageService : IImageService
    {
        //private readonly HttpClient _httpClient;

        //public ImageService(HttpClient httpClient)
        //{
        //    _httpClient = httpClient;
        //}

        public string ConvertImageToBase64(byte[] imageBytes)
        {
            return Convert.ToBase64String(imageBytes);
        }

        public async Task<byte[]> GetImageFromUrlAsync(string imageUrl)
        {
            //var response = await _httpClient.GetAsync(imageUrl);
            //response.EnsureSuccessStatusCode();
            //return await response.Content.ReadAsByteArrayAsync();
            using (var webClient = new WebClient())
            {
                return await webClient.DownloadDataTaskAsync(imageUrl);
            }
        }
    }
}
