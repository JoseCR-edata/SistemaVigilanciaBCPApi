using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SistemaVigilanciaBCPApi.Models;
using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using VideovigilanciaDB.Models.VideovigilanciaBCP;
using static System.Net.Mime.MediaTypeNames;

namespace SistemaVigilanciaBCPApi.Services
{
    public class Alarma : IAlarma
    {
        private readonly videovigilanciaBCPContext _videovigilanciaContext;
        private readonly BCPSistemaVigilanciaContext _bcpContext;
        private readonly IConfiguration _configuration;
        //private readonly ImageService _imageService;

        public Alarma(videovigilanciaBCPContext videovigilanciaContext, IConfiguration configuration, BCPSistemaVigilanciaContext bcpContext)
        {
            _videovigilanciaContext = videovigilanciaContext;
            _configuration = configuration;
            _bcpContext = bcpContext;
            //_imageService = imageService;
        }

        public async Task<AlarmaDetalle> GetAlarma(int id)
        {
            var alarma = await _videovigilanciaContext.AlarmaAnalitica.Where(x => x.Id == id).FirstOrDefaultAsync();
            AlarmaDetalle alarmaDetalle = new AlarmaDetalle();
            alarmaDetalle.NombreCamara = alarma.AlarmInName;
            alarmaDetalle.TipoAlarma = alarma.AlarmInType;
            alarmaDetalle.HoraAlarma = alarma.OccurTime;
            alarmaDetalle.NivelAlarma = alarma.AlarmLevelValue;
            try
            {
                //////var imgUrl = "https://scontent.flim38-1.fna.fbcdn.net/v/t39.30808-6/436261674_952301133570322_2028530909555042935_n.jpg?_nc_cat=106&ccb=1-7&_nc_sid=6ee11a&_nc_ohc=oWoXGRmUGfYQ7kNvgEUV90n&_nc_ht=scontent.flim38-1.fna&oh=00_AYBseKCS_UT-jqireeHJHmz1zlp_T2RNH-9ihsp9BVAljg&oe=6690C6AE";
                //var imgUrl = "https://images.pexels.com/photos/842711/pexels-photo-842711.jpeg";
                //////Generar token
                //////var jsessionid = await ObtenerJsessionidAsync();
                ////var jsessionid = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ123456";
                ////if (jsessionid != null)
                ////{
                ////    Console.WriteLine($"JSESSIONID: {jsessionid}");
                ////    var baseImage = await RealizarPeticionConJsessionidAsync(jsessionid, imgUrl);
                ////    if (baseImage != null)
                ////    {
                ////        Console.WriteLine($"Imagen en Base64: {baseImage}");
                ////    }
                ////}
                //////Hacer peticion de Imagen

                ////var imageBytes = await GetImageFromUrlAsync(imgUrl);
                ////var base64Image = ConvertImageToBase64(imageBytes);

                ////Generar token
                //var jsessionid = await ObtenerJsessionidAsync();
                //var imagen = "";
                ////var jsessionid = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ123456";
                //if (jsessionid != null)
                //{
                //    imagen = await RealizarPeticionConJsessionidAsync(jsessionid, imgUrl);
                //}
                //alarmaDetalle.imagen = imagen;

                alarmaDetalle.imagen = null;
            }
            catch (Exception ex)
            {
                alarmaDetalle.imagen = null;
            }


            return alarmaDetalle;
        }

        public List<dynamic> GetAlarmas(AlarmaFiltro alarmaFiltro)
        {
            bool contieneFecha= false;
            try
            {
                var alarmasLista = new List<AlarmaAnalitica>();
                if (alarmaFiltro.FechaInicio != "" || alarmaFiltro.FechaFin != "")
                {
                    if (alarmaFiltro.FechaInicio != "" && alarmaFiltro.FechaFin == "")
                    {
                        var fechaInicio = DateTime.ParseExact(alarmaFiltro.FechaInicio, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                        alarmasLista = _videovigilanciaContext.AlarmaAnalitica.AsEnumerable().Where(x => Convert.ToDateTime(x.OccurTime) >= fechaInicio).ToList();
                    }
                    if (alarmaFiltro.FechaInicio == "" && alarmaFiltro.FechaFin != "")
                    {
                        var fechaFin = DateTime.ParseExact(alarmaFiltro.FechaFin, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                        alarmasLista = _videovigilanciaContext.AlarmaAnalitica.AsEnumerable().Where(x => Convert.ToDateTime(x.OccurTime) <= fechaFin).ToList();
                    }
                    if (alarmaFiltro.FechaInicio != "" && alarmaFiltro.FechaFin != "")
                    {
                        var fechaInicio = DateTime.ParseExact(alarmaFiltro.FechaInicio, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                        var fechaFin = DateTime.ParseExact(alarmaFiltro.FechaFin, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                        alarmasLista = _videovigilanciaContext.AlarmaAnalitica.AsEnumerable().Where(x => Convert.ToDateTime(x.OccurTime) >= fechaInicio && Convert.ToDateTime(x.OccurTime) <= fechaFin).ToList();
                    }
                    contieneFecha = true;
                }
                if(alarmasLista.Count == 0 && contieneFecha)
                {
                    return alarmasLista.Cast<dynamic>().ToList(); ;
                }
                if (alarmasLista.Count == 0)
                {
                    alarmasLista = _videovigilanciaContext.AlarmaAnalitica.ToList();
                }
                var alarmasQuery = alarmasLista.AsQueryable();
                if (alarmaFiltro.NombreCamara != "")
                {
                    string nombreCamaraFiltro = alarmaFiltro.NombreCamara.ToLower();
                    alarmasQuery = alarmasQuery.Where(x => x.AlarmInName.ToLower().Contains(nombreCamaraFiltro));
                }
                if (alarmaFiltro.TipoAlerta != "")
                {
                    alarmasQuery = alarmasQuery.Where(x => x.AlarmInType == Convert.ToInt16(alarmaFiltro.TipoAlerta));
                }
                if (alarmaFiltro.NivelAlarma != "")
                {
                    alarmasQuery = alarmasQuery.Where(x => x.AlarmLevelValue == Convert.ToInt16(alarmaFiltro.NivelAlarma));
                }
                var alarmas = alarmasQuery.AsEnumerable().Select(x => new
                {
                    id = x.Id,
                    alarmid = x.Id,
                    cameraname = x.AlarmInName,
                    alarmtime = x.OccurTime,
                    alarmlevel = x.AlarmLevelValue,
                    type = x.AlarmInType
                }).ToList();
                return alarmas.OrderByDescending(r => r.id).Take(1000).Cast<dynamic>().ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
            //try
            //{
            //    var alarmasQuery = _videovigilanciaContext.Alarmas.AsQueryable();
            //    if (alarmaFiltro.NombreCamara != "")
            //    {
            //        string nombreCamaraFiltro=alarmaFiltro.NombreCamara.ToLower();
            //        alarmasQuery = alarmasQuery.Where(x => x.Cameraname.ToLower().Contains(nombreCamaraFiltro));
            //    }
            //    if (alarmaFiltro.FechaInicio != "")
            //    {
            //        var fechaInicio= DateTime.ParseExact(alarmaFiltro.FechaInicio, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            //        alarmasQuery = alarmasQuery.Where(x => x.Alarmtime >= fechaInicio);
            //    }
            //    if (alarmaFiltro.FechaFin != "")
            //    {
            //        var fechaFin = DateTime.ParseExact(alarmaFiltro.FechaFin, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
            //        alarmasQuery = alarmasQuery.Where(x => x.Alarmtime <= fechaFin);
            //    }
            //    if (alarmaFiltro.TipoAlerta != "")
            //    {
            //        alarmasQuery = alarmasQuery.Where(x => x.Type == Convert.ToInt16(alarmaFiltro.TipoAlerta));
            //    }
            //    if (alarmaFiltro.NivelAlarma != "")
            //    {
            //        alarmasQuery = alarmasQuery.Where(x => x.Alarmlevel == Convert.ToInt16(alarmaFiltro.NivelAlarma));
            //    }
            //    var alarmas = alarmasQuery.AsEnumerable().Select(x=>new
            //    {
            //        id = x.Idalarma,
            //        alarmid = x.Alarmid,
            //        cameraname = x.Cameraname,
            //        alarmtime = x.Alarmtime,
            //        alarmlevel = x.Alarmlevel,
            //        type = x.Type
            //    }).ToList();
            //    return alarmas.Cast<dynamic>().ToList();
            //}
            //catch (Exception ex)
            //{
            //    return null;
            //}
        }
        public List<TipoAlarma> GetAlarmasTipo()
        {
            return _bcpContext.TipoAlarma.ToList();
        }
        public List<NivelAlarma> GetAlarmasNivel()
        {
            return _bcpContext.NivelAlarma.ToList();
        }
        #region
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
        public string ConvertImageToBase64(byte[] imageBytes)
        {
            return Convert.ToBase64String(imageBytes);
        }

        private static string GetJsessionIdFromCookie(string cookie)
        {
            var parts = cookie.Split(';');
            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim() == "JSESSIONID")
                {
                    return keyValue[1].Trim();
                }
            }
            return null;
        }

        public async Task<string> ObtenerJsessionidAsync()
        {
            var urlLogin = _configuration["ImagenesVigilancia:urlLogin"];
            //var payload = new FormUrlEncodedContent(new[]
            //{
            //    new KeyValuePair<string, string>("account", _configuration["ImagenesVigilancia:account"]),
            //    new KeyValuePair<string, string>("pwd", _configuration["ImagenesVigilancia:pwd"]),
            //    new KeyValuePair<string, string>("clientIP", _configuration["ImagenesVigilancia:clientIP"]),
            //    new KeyValuePair<string, string>("dstIp", _configuration["ImagenesVigilancia:dstIp"])
            //});

            try
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(IgnoreCertificateValidation);

                using (var webClient = new WebClient())
                {
                    var postData = new NameValueCollection
                    {
                        { "account", _configuration["ImagenesVigilancia:account"] },
                        { "pwd", _configuration["ImagenesVigilancia:pwd"] }
                    };
                    try
                    {
                        // Hacer la solicitud POST
                        Console.WriteLine(postData.ToString());
                        byte[] responseBytes = webClient.UploadValues(urlLogin, "POST", postData);
                        Console.WriteLine(responseBytes.ToString());
                        // Leer el encabezado 'pruebas' de la respuesta
                        string responseBody = Encoding.UTF8.GetString(responseBytes);
                        Console.WriteLine(responseBody.ToString());

                        foreach (string key in webClient.ResponseHeaders.AllKeys)
                        {
                            string value = webClient.ResponseHeaders[key];
                            Console.WriteLine($"{key}: {value}");
                        }

                        // Leer y guardar el header específico
                        string headerValue = webClient.ResponseHeaders["Set-Cookie"];
                        Console.WriteLine(headerValue.ToString());
                        return headerValue.ToString();
                        //Console.WriteLine($"Valor del header específico: {headerValue}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                        return "";
                    }
                }
                // Ignorar la validación del certificado SSL
                //using (var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true })
                //using (var client = new HttpClient(handler))
                //{
                //    var response = await client.PostAsync(urlLogin, payload);

                //    if (response.IsSuccessStatusCode)
                //    {
                //        if (response.Headers.TryGetValues("Cookie", out var cookies))
                //        {
                //            foreach (var cookie in cookies)
                //            {
                //                Console.WriteLine(cookie);
                //                var jsessionid = GetJsessionIdFromCookie(cookie);
                //                if (!string.IsNullOrEmpty(jsessionid))
                //                {
                //                    Console.WriteLine(jsessionid);
                //                    return jsessionid;
                //                }
                //            }
                //        }

                //        Console.WriteLine("Login exitoso, pero no se encontró JSESSIONID en las cookies.");
                //    }
                //    else
                //    {
                //        Console.WriteLine($"Error al loguearse. Código de estado HTTP: {response.StatusCode}");
                //    }
                //}
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error al intentar conectar con la API: {e.Message}");
            }
            return "";
        }

        private static async Task<string> RealizarPeticionConJsessionidAsync(string jsessionid, string imgUrl)
        {
            Console.WriteLine(jsessionid);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(IgnoreCertificateValidation);
            Console.WriteLine("LLego");
            using (var webClient = new WebClient())
            {
                Console.WriteLine("LLego1");
                try
                {
                    // Agregar el encabezado Cookie
                    webClient.Headers.Add(HttpRequestHeader.Cookie, jsessionid);
                    Console.WriteLine("Llego2");
                    // Descargar la imagen
                    string responseBody = webClient.DownloadString(imgUrl);
                    Console.WriteLine(responseBody.ToString());
                    return responseBody;
                    //var imageBytes = await webClient.DownloadStringAsync(imgUrl);
                    //Console.WriteLine(imageBytes.ToString());
                    //// Convertir la imagen a una cadena base64
                    //var base64String = Convert.ToBase64String(imageBytes);
                    //Console.WriteLine(base64String.ToString());
                    //return base64String; // Devuelve la cadena base64
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                    return "";
                }
            }

            //var url = imgUrl;
            //var payload = new StringContent("{\"your\": \"data\"}", System.Text.Encoding.UTF8, "application/json");

            //try
            //{
            //    using (var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true })
            //    using (var client = new HttpClient(handler))
            //    {
            //        client.DefaultRequestHeaders.Add("Cookie", $"JSESSIONID={jsessionid}");

            //        var response = await client.PostAsync(url, payload);

            //        if (response.IsSuccessStatusCode)
            //        {
            //            var imageBytes = await response.Content.ReadAsByteArrayAsync();
            //            var base64String = Convert.ToBase64String(imageBytes);
            //            return base64String;
            //        }
            //        else
            //        {
            //            Console.WriteLine($"Error al realizar la petición. Código de estado HTTP: {response.StatusCode}");
            //        }
            //    }
            //}
            //catch (HttpRequestException e)
            //{
            //    Console.WriteLine($"Error al intentar conectar con la API: {e.Message}");
            //}

            //return null;
        }
        private static bool IgnoreCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // Ignorar todos los errores de certificación SSL
        }

        public async Task<string> PruebaImagenAlarma(string id)
        {
            try
            {
                var jsessionid = "0";
                if (id == "0")
                {
                    jsessionid = await ObtenerJsessionidAsync();
                }
                else
                {
                    jsessionid = id;
                }
                //var jsessionid = "123";
                var imgurl = "https://10.162.115.238:1201/imgu?Action=Download&NvrCode=706c76c021cc40f0ad5aad37cd6eb401&PictureID=MDMjMDAwNyMped%2BZm7lwAi0TYLyARIolOEHLOS%2B9T8tMJFbFDvRKv0JJ8xT3lH4%2BoXg%3D&auth_type=vcm&vcm_domain=4dcd1e6e33904775837d38d8b2c62a09";
                //var imgurl = "https://images.pexels.com/photos/842711/pexels-photo-842711.jpeg";
                var imagen = await RealizarPeticionConJsessionidAsync(jsessionid, imgurl);
                return imagen;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ex.Message;
            }
            
        }
        #endregion
    }
}
