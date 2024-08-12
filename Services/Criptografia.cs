
using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using Newtonsoft.Json;
using SistemaVigilanciaBCPApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace SistemaVigilanciaBCPApi.Services
{
    public class Criptografia: ICriptografia
    {
        private static readonly int KeySize = 32;
        private static readonly int BlockSize = 16;
        private static readonly int Iterations = 10000;
        private readonly IConfiguration _configuration;

        public Criptografia(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //private const string EncryptionKey = "f3a8e5b1c7d9123fa2b9c8d1e4f56789";

        public string Encrypt(string plainText )
        {
            var EncryptionKey = _configuration["JwtSettings:Key"];
            byte[] key = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] iv = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            AesManaged aes = new AesManaged();
            aes.Key = key;
            aes.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(),CryptoStreamMode.Write);
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] encrypted = memoryStream.ToArray();
            return Convert.ToBase64String(encrypted);
        }
        public string Decrypt(string encryptedText)
        {
            var EncryptionKey = _configuration["JwtSettings:Key"];
            byte[] key = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] iv = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            AesManaged aes = new AesManaged();
            aes.Key = key;
            aes.IV = iv;
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write);

            byte[] inputBytes = Convert.FromBase64String(encryptedText);
            cryptoStream.Write(inputBytes, 0, inputBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] decrypted = memoryStream.ToArray();
            return UTF8Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);
        }
        public DatosToken ObtenerDatosToken(string tokenAutenticacion)
        {
            var token = tokenAutenticacion;
            if (token != null)
            {
                DatosToken datosToken = new DatosToken();
                var jwtToken = new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken;

                var claims = jwtToken?.Claims;

                // Ejemplo de cómo obtener el valor de un claim específico
                var userId = claims?.FirstOrDefault(claim => claim.Type == "sub")?.Value;
                var tokenInfo = JsonConvert.DeserializeObject<UsuariosConectado>(claims?.FirstOrDefault(claim => claim.Type == "UsuarioConectado")?.Value);
                datosToken.Usuario = tokenInfo.Usuario;
                datosToken.Grupo = tokenInfo.Grupo;
                return datosToken;
            }
            return null;
        }
        //public static byte[] Encryptv(string plainText)
        //{
        //    byte[] key = new byte[16];

        //    byte[] iv = new byte[16];
        //    byte[] cipheredtext;
        //    using (Aes aes = Aes.Create())
        //    {
        //        ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);
        //        using (MemoryStream memoryStream = new MemoryStream())
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        //            {
        //                using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
        //                {
        //                    streamWriter.Write(plainText);
        //                }

        //                cipheredtext = memoryStream.ToArray();
        //            }
        //        }
        //    }
        //    return cipheredtext;
        //}
        //public static string Decryptv(byte[] cipheredtext)
        //{
        //    byte[] key = new byte[16];

        //    byte[] iv = new byte[16];

        //    string simpletext = String.Empty;
        //    using (Aes aes = Aes.Create())
        //    {
        //        ICryptoTransform decryptor = aes.CreateDecryptor(key, iv);
        //        using (MemoryStream memoryStream = new MemoryStream(cipheredtext))
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader streamReader = new StreamReader(cryptoStream))
        //                {
        //                    simpletext = streamReader.ReadToEnd();
        //                }
        //            }
        //        }
        //    }
        //    return simpletext;
        //}

    }
}
