using SistemaVigilanciaBCPApi.Models;

namespace SistemaVigilanciaBCPApi.Services
{
    public interface ICriptografia
    {
        public string Encrypt(string plainText);
        public string Decrypt(string encryptedText);
        public DatosToken ObtenerDatosToken(string tokenAutenticacion);
    }
}
