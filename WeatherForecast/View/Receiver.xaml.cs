using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeatherForecast.ViewModel;

namespace WeatherForecast.View
{
    /// <summary>
    /// Логика взаимодействия для Receiver.xaml
    /// </summary>
    public partial class Receiver : Window
    {
        public Receiver()
        {
            InitializeComponent();
            DataContext = new ReceiverViewModel();
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) => (DataContext as ReceiverViewModel).Close();
    }
}
