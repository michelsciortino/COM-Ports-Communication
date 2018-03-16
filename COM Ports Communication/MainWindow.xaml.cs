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
using System.IO.Ports;
namespace COM_Ports_Communication
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public void getAvailablePortNames()
        {
            String[] ports = SerialPort.GetPortNames();
            Ports.Items.Clear();
            foreach(string port in ports)
            {
                ComboBoxItem cbi = new ComboBoxItem();
                cbi.Content = port;
                Ports.Items.Add(cbi);
            }
        }

        private void PortsDropDownOpened(object sender, EventArgs e)
        {
            getAvailablePortNames();
        }

        private void ConnectionSettingSelected(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty((string)Ports.SelectedValue?.ToString()) && !string.IsNullOrEmpty((string)RateBounds.SelectedValue?.ToString()))
            {
                OpenPortButton.IsEnabled = true;
            }
            else
            {
                OpenPortButton.IsEnabled = false;
                ClosePortButton.IsEnabled = false;
            }
        }

        private void OpenPortButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClosePortButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
