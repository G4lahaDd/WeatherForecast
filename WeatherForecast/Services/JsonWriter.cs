using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace WeatherForecast.Services
{
    public class JsonWriter
    {
        private readonly Regex filenameRegex = new Regex("^[A-Za-zА-Яа-я0-9-_,\\s]+[.]{1}json$");
        private readonly Logger logger;

        private FileStream outStream;

        public JsonWriter(string docPath)
        {
            logger = Logger.Instance;

            if (string.IsNullOrEmpty(docPath))
            {
                throw new ArgumentNullException(nameof(docPath));
            }
            if (!Path.GetExtension(docPath).Equals(".json"))
            {
                throw new ArgumentException("Имя файла должно иметь расширение *.json", nameof(docPath));
            }

            Initialize(docPath);
        }

        public JsonWriter(string subDir, string filename)
        {
            logger = Logger.Instance;

            if (string.IsNullOrEmpty(subDir))
            {
                throw new ArgumentNullException(nameof(subDir));
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            if (!filenameRegex.IsMatch(filename))
            {
                throw new ArgumentException("Имя файла должно содержать только символы 'a-Zа-Я0-9_,' и иметь расширение *.json", nameof(filename));
            }

            string docPath = Environment.CurrentDirectory;
            docPath = Path.Combine(docPath, subDir);

            if (!Directory.Exists(docPath))
            {
                Directory.CreateDirectory(docPath);
            }

            docPath = Path.Combine(docPath, filename);

            Initialize(docPath);
        }

        private void Initialize(string docPath)
        {
            logger.LogInfo<JsonWriter>($"Open json output writer: {docPath}");

            try
            {
                bool exist = File.Exists(docPath);

                outStream = new FileStream(docPath, FileMode.OpenOrCreate);

                if (exist)
                {
                    outStream.Seek(-1, SeekOrigin.End);
                }
                else
                {
                    outStream.WriteByte(91);//[
                }
            }
            catch (Exception ex)
            {
                logger.LogError<JsonWriter>(ex);
                throw ex;
            }
        }
        

        public void Append(Model.Forecast weatherForecast)
        {
            try
            {
                byte[] bytes = Encoding.ASCII.GetBytes(weatherForecast.ToString() + ",");
                outStream.Write(bytes, 0, bytes.Length);
            }
            catch(Exception ex)
            {
                logger.LogError<JsonWriter>(ex);
            }
           
        }

        public void Close()
        {
            logger.LogInfo<JsonWriter>("Close json output writer");

            outStream.Position--;
            int end = outStream.ReadByte();
            if (end == 44)// ','
            {
                outStream.Position--;
                outStream.WriteByte((byte)93);
            }
            else if(end != 93)// ']'
            {
                outStream.WriteByte((byte)93);
            }

            outStream.Close();
        }
    }
}
