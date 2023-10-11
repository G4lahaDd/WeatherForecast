using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Services
{
    public class Logger
    {
        private readonly StreamWriter _infoWriter;
        private readonly StreamWriter _errorWriter;

        public static Logger _instance;
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                }
                return _instance;
            }
        }

        private Logger()
        {
            string folderPath = Environment.CurrentDirectory;
            folderPath = Path.Combine(folderPath, "logs");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            Debug.WriteLine(folderPath);
            _errorWriter = new StreamWriter(Path.Combine(folderPath, "errors.txt"), true);
            _infoWriter = new StreamWriter(Path.Combine(folderPath, "info.txt"), true);
        }

        public void LogInfo(string message)
        {
            _infoWriter.WriteLine($"{DateTime.Now} INFO: {message}");
        }

        public void LogInfo<T>(string message)
        {
            _infoWriter.WriteLine($"{DateTime.Now}, {typeof(T).Name} INFO: {message}");

        }

        public void LogError(string message)
        {
            _infoWriter.WriteLine($"{DateTime.Now}, ERROR: {message}");
            _errorWriter.WriteLine($"{DateTime.Now}, ERROR: {message}");
        }

        public void LogError<T>(string message)
        {
            _infoWriter.WriteLine($"{DateTime.Now}, {typeof(T).Name} ERROR: {message}");
            _errorWriter.WriteLine($"{DateTime.Now}, {typeof(T).Name} ERROR: {message}");
        }

        public void LogError(Exception ex)
        {
            _infoWriter.WriteLine($"{DateTime.Now}, ERROR: {ex.Message}");
            _errorWriter.WriteLine($"{DateTime.Now}, ERROR: {ex.Message}\n{ex.StackTrace}");
        }

        public void LogError<T>(Exception ex)
        {
            _infoWriter.WriteLine($"{DateTime.Now}, {typeof(T).Name} ERROR: {ex.Message}");
            _errorWriter.WriteLine($"{DateTime.Now}, {typeof(T).Name} ERROR: {ex.Message}\n{ex.StackTrace}");
        }

        public void Close()
        {
            _errorWriter.Close();
            _infoWriter.Close();
        }
    }
}
