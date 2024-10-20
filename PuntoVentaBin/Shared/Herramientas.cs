using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PuntoVentaBin.Shared
{
    public static class Cifrado
    {
        private const string cLlavePrivada = "YmsetgDkuZ/pMnhW9MdTgKhnkwT/j3FI0e/hh8cF9Po=";
        private const string cVectorEntrada = "bbUSn8h471ETkOvMLzkIJw==";

        public static string Encriptar(this string cHileraEntrada)
        {

            try
            {
                var key = Convert.FromBase64String(cLlavePrivada);
                var IV = Convert.FromBase64String(cVectorEntrada);

                using (var oMemStream = new MemoryStream())
                using (var rmCrypto = new RijndaelManaged())
                using (var cryptStream = new CryptoStream(oMemStream, rmCrypto.CreateEncryptor(key, IV), CryptoStreamMode.Write))
                using (var sWriter = new StreamWriter(cryptStream, Encoding.UTF8))
                {
                    sWriter.Write(cHileraEntrada);
                    sWriter.Close();
                    var aHileraEncriptada = oMemStream.ToArray();


                    return Convert.ToBase64String(aHileraEncriptada);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public static string DesEncriptar(this string cHileraEntrada)
        {
            if (string.IsNullOrEmpty(cHileraEntrada))
            {
                return string.Empty;
            }
            try
            {
                var key = Convert.FromBase64String(cLlavePrivada);
                var IV = Convert.FromBase64String(cVectorEntrada);

                var aHileraEntrada = Convert.FromBase64String(cHileraEntrada);

                using (var oMemStream = new MemoryStream(aHileraEntrada))
                using (var rmCrypto = new RijndaelManaged())
                using (var cryptStream = new CryptoStream(oMemStream, rmCrypto.CreateDecryptor(key, IV), CryptoStreamMode.Read))
                using (var sWriter = new StreamReader(cryptStream, Encoding.UTF8))
                {
                    var cHileraEncriptada = sWriter.ReadToEnd();

                    return cHileraEncriptada.Replace("//D//", string.Empty).Replace("//#//", string.Empty).Replace("//NULL//", string.Empty);
                }
            }
            catch (Exception)
            {
                return cHileraEntrada;
            }
        }

    }

    public class Sender
    {
        public int Timeout { get; set; } = 10;
        public Sender(string url)
        {

            Url = url;
        }
        public string Url { get; set; }

        #region EnvioRest
        public string PostRest(object objeto)
        {
            try
            {
                var json = JsonSerializer.Serialize(objeto);

                var req = (WebRequest)HttpWebRequest.Create(Url);
                req.Method = "POST";
                req.ContentType = "application/json";

                using (var writer = new StreamWriter(req.GetRequestStream()))
                {
                    writer.Write(json);
                }

                var result = string.Empty;

                using (var reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public T PostRest<T>(object objeto)
        {
            var json = JsonSerializer.Serialize(objeto);

            var req = (WebRequest)HttpWebRequest.Create(Url);
            req.Method = "POST";
            req.ContentType = "application/json; charset=utf-8";
            req.Timeout = 1000 * Timeout * 60;
            using (var writer = new StreamWriter(req.GetRequestStream()))
            {

                writer.Write(json);
            }

            var result = string.Empty;

            using (var reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            var respuesta = JsonSerializer.Deserialize(result, typeof(T));

            return (T)respuesta;
        }

        public string GetRest(string funcion, string parametros)
        {
            try
            {
                //var obj = new { value = objeto };
                //var json = JsonSerializer.Serialize(objeto);

                var req = (WebRequest)HttpWebRequest.Create(Url + "/" + funcion + (string.IsNullOrEmpty(parametros) ? "" : "?" + parametros));
                req.Method = "GET";
                req.ContentType = "application/json; charset=utf-8";

                var result = string.Empty;

                using (var reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                return result;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public string GetRest(string funcion, List<string> parametros)
        {
            try
            {
                var req = (WebRequest)HttpWebRequest.Create(Url + funcion + (parametros.Count == 0 ? "" : "?" + string.Join("&", parametros)));
                req.Method = "GET";
                req.ContentType = "application/json; charset=utf-8";
                var result = string.Empty;

                using (var reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                return result;

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public T GetRest<T>(string funcion, List<string> parametros)
        {
            try
            {
                var req = (WebRequest)HttpWebRequest.Create(Url + funcion + (parametros.Count == 0 ? "" : "?" + string.Join("&", parametros)));
                req.Method = "GET";
                req.ContentType = "application/json; charset=utf-8";
                var result = string.Empty;

                using (var reader = new StreamReader(req.GetResponse().GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }

                var respuesta = JsonSerializer.Deserialize(result, typeof(T));

                return (T)respuesta;

            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public T GetRest<T>(string funcion, string parametros)
        {
            var req = (WebRequest)HttpWebRequest.Create(Url + "/" + funcion + (string.IsNullOrEmpty(parametros) ? "" : "?" + parametros));
            req.Method = "GET";
            req.ContentType = "application/json; charset=utf-8";
            var result = string.Empty;

            using (var reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            var respuesta = JsonSerializer.Deserialize(result, typeof(T));

            return (T)respuesta;
        }

        #endregion


        #region "Seguridad"

        protected const int zBlockSize = 4096;
        protected string cLlavePrivada = "YmsetgDkuZ/pMnhW9MdTgKhnkwT/j3FI0e/hh8cF9Po=";
        protected string cVectorEntrada = "bbUSn8h471ETkOvMLzkIJw==";

        public byte[] DigerirHileraMD5(string cHileraEntrada, Encoding oEncoding)
        {
            try
            {
                if (oEncoding == null)
                {
                    oEncoding = System.Text.Encoding.UTF8;
                }

                var oMD5Hasher = new MD5CryptoServiceProvider();

                byte[] RetHashedDataBytes = oMD5Hasher.ComputeHash(oEncoding.GetBytes(cHileraEntrada));

                return RetHashedDataBytes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] DigerirHileraMD5(string cHileraEntrada, ref string cHileraSalida, Encoding oEncoding)
        {
            try
            {
                if (oEncoding == null)
                {
                    oEncoding = System.Text.Encoding.UTF8;
                }

                var oMD5Hasher = new MD5CryptoServiceProvider();

                byte[] RetHashedDataBytes = oMD5Hasher.ComputeHash(oEncoding.GetBytes(cHileraEntrada));

                cHileraSalida = Convert.ToBase64String(RetHashedDataBytes);

                return RetHashedDataBytes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public byte[] DigerirHileraSha1(string cHileraEntrada, ref string cHileraSalida, Encoding oEncoding)
        {
            try
            {
                if (oEncoding == null)
                {
                    oEncoding = Encoding.UTF8;
                }

                var oSha1Hasher = new SHA1CryptoServiceProvider();

                byte[] RetHashedDataBytes = oSha1Hasher.ComputeHash(oEncoding.GetBytes(cHileraEntrada));

                cHileraSalida = Convert.ToBase64String(RetHashedDataBytes);

                return RetHashedDataBytes;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public byte[] DigerirHileraSha1(string cHileraEntrada, Encoding oEncoding)
        {
            try
            {
                if (oEncoding == null)
                {
                    oEncoding = Encoding.UTF8;
                }

                var oSha1Hasher = new SHA1CryptoServiceProvider();

                byte[] RetHashedDataBytes = oSha1Hasher.ComputeHash(oEncoding.GetBytes(cHileraEntrada));

                return RetHashedDataBytes;

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }

        public string EncriptarRms(string cHileraEntrada)
        {

            try
            {

                var key = Convert.FromBase64String(cLlavePrivada);
                var IV = Convert.FromBase64String(cVectorEntrada);

                using (var oMemStream = new MemoryStream())
                using (var rmCrypto = new RijndaelManaged())
                using (var cryptStream = new CryptoStream(oMemStream, rmCrypto.CreateEncryptor(key, IV), CryptoStreamMode.Write))
                using (var sWriter = new StreamWriter(cryptStream, Encoding.UTF8))
                {
                    sWriter.Write(cHileraEntrada);
                    sWriter.Close();
                    var aHileraEncriptada = oMemStream.ToArray();


                    return Convert.ToBase64String(aHileraEncriptada);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string DesEncriptarRms(string cHileraEntrada)
        {
            if (string.IsNullOrEmpty(cHileraEntrada))
            {
                return string.Empty;
            }
            try
            {
                var key = Convert.FromBase64String(cLlavePrivada);
                var IV = Convert.FromBase64String(cVectorEntrada);

                var aHileraEntrada = Convert.FromBase64String(cHileraEntrada);

                using (var oMemStream = new MemoryStream(aHileraEntrada))
                using (var rmCrypto = new RijndaelManaged())
                using (var cryptStream = new CryptoStream(oMemStream, rmCrypto.CreateDecryptor(key, IV), CryptoStreamMode.Read))
                using (var sWriter = new StreamReader(cryptStream, Encoding.UTF8))
                {


                    var cHileraEncriptada = sWriter.ReadToEnd();

                    return cHileraEncriptada.Replace("//D//", string.Empty).Replace("//#//", string.Empty).Replace("//NULL//", string.Empty);
                }
            }
            catch (Exception)
            {
                return cHileraEntrada;
            }
        }

        public string Compress(string cTextoPlano, Encoding oEncoding)
        {
            byte[] aBytesTarget;
            try
            {

                byte[] aBytesSource = oEncoding.GetBytes(cTextoPlano);
                using (var oStreamSource = new MemoryStream(aBytesSource))
                using (var oStreamTarget = new MemoryStream())
                {
                    //BZip2.Compress(oStreamSource, oStreamTarget, zBlockSize);

                    aBytesTarget = oStreamTarget.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Convert.ToBase64String(aBytesTarget);
        }

        public string DeCompress(string cZipBase64, Encoding oEncoding)
        {
            try
            {
                byte[] aBytes_Target;
                byte[] aBytes_Source = Convert.FromBase64String(cZipBase64);
                using (var oStreamSource = new System.IO.MemoryStream(aBytes_Source))
                using (var oStream_Target = new System.IO.MemoryStream())
                {
                    //BZip2.Decompress(oStreamSource, oStream_Target);
                    aBytes_Target = oStream_Target.ToArray();
                }

                return oEncoding.GetString(aBytes_Target);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion

    }
}
