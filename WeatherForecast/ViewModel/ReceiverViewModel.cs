using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WeatherForecast.Model;
using WeatherForecast.Services;
using WeatherForecast.ViewModel.Command;

namespace WeatherForecast.ViewModel
{
    internal class ReceiverViewModel : INotifyPropertyChanged
    {
        private int port;
        private readonly Logger _logger;
        private JsonWriter writer;
        private WMTReceiver receiver;
        private Forecast forecast = new Forecast("WMT700", 0, 0);

        #region Binding
        public Forecast Forecast
        {
            get => forecast;
            set
            {
                forecast = value;
                OnPropertyChanged("Date");
                OnPropertyChanged("WindSpeed");
                OnPropertyChanged("WindDirection");
            }
        }
        public string Date { get => forecast.Date.ToString(); }
        public string WindSpeed { get => $"{forecast.AverageWindSpeed} м/с"; }
        public string WindDirection { get => $"{forecast.AverageWindDirection}\u00B0"; }
        public string Port
        {
            get => port.ToString();
            set
            {
                int.TryParse(value, out port);
                OnPropertyChanged("Port");
            }
        }
        public string State 
        { 
            get
            {
                return receiver?.opened ?? false ? "Opened" : "Closed";
            } 
        }
        public ICommand SelectOutput { get; }
        public ICommand SelectPort { get; }
        #endregion

        public ReceiverViewModel()
        {
            SelectPort = new LambdaCommand(SelectCOMPort);
            SelectOutput = new LambdaCommand(SelectDirectory);

            try
            {
                _logger = Logger.Instance;
                writer = new JsonWriter("Result", "forecast.json");
            }
            catch
            {
                MessageBox.Show("Ошибка инифиализации компонентов");
            }
        }

        private void SelectCOMPort(object value)
        {
            _logger?.LogInfo<ReceiverViewModel>($"Select port: COM{port}");
            try
            {
                if (receiver != null)
                {
                    receiver.Close();
                }

                receiver = new WMTReceiver($"COM{port}");
                receiver.ReceiveForecastData += GetData;
                receiver.Open();
            }
            catch (Exception ex)
            {
                _logger?.LogError<ReceiverViewModel>(ex);
                MessageBox.Show(ex.Message);
            }
            OnPropertyChanged("State");
        }

        private void SelectDirectory(object value)
        {
            _logger?.LogInfo<ReceiverViewModel>($"Select output directory");
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JSON files (*.json)|*.json";
                saveFileDialog.InitialDirectory = @"c:\";
                if (saveFileDialog.ShowDialog() == true)
                {
                    writer = new JsonWriter(saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError<ReceiverViewModel>(ex);
                MessageBox.Show(ex.Message);
            }
        }

        private void GetData(string data)
        {
            Regex dataValidRegex = new Regex("^[$]{1}[0-9]+[.]{1}[0-9]+[,]{1}[0-9]+[.]{1}[0-9]+\u000D$");

            if (dataValidRegex.IsMatch(data))
            {
                _logger?.LogInfo<ReceiverViewModel>($"Data received: {data}");
                var parts = data.Replace("$", "").Replace("\u0011", "").Split(',');
                if (parts.Length != 2) return;

                if (double.TryParse(parts[0].Replace(".", ","), out double speed)
                    && double.TryParse(parts[1].Replace(".", ","), out double direction))
                {
                    Forecast forecast = new Forecast("WMT700", speed, direction);
                    Forecast = forecast;
                    writer.Append(forecast);
                }
            }
        }

        public void Close()
        {
            receiver?.Close();
            writer?.Close();
            _logger?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
