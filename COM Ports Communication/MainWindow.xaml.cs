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
        SerialPort port;
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
            if (Ports.SelectedValue!=null && BoundRates.SelectedValue!=null)
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
            if (Ports.SelectedValue == null || BoundRates.SelectedValue == null) return;
            try
            {
                port = new SerialPort();
                port.PortName = ((ComboBoxItem)Ports.SelectedValue).Content.ToString();
                port.BaudRate = int.Parse(((ComboBoxItem)BoundRates.SelectedValue).Content.ToString());
                try
                {
                    port.Open();
                }
                catch(Exception ex)
                {
                    System.Windows.MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK);
                    return;
                }
                OpenPortButton.IsEnabled = false;
                ClosePortButton.IsEnabled = true;
                ReadDataButton.IsEnabled = true;
                SendDataButton.IsEnabled = true;
                Ports.IsEnabled = false;
                BoundRates.IsEnabled = false;
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void ClosePortButton_Click(object sender, RoutedEventArgs e)
        {
            if (port != null && port.IsOpen == true)
            {
                try
                {
                    port.Close();
                    ReadDataButton.IsEnabled = false;
                    SendDataButton.IsEnabled = false;
                    OpenPortButton.IsEnabled = true;
                    ClosePortButton.IsEnabled = false;
                    Ports.IsEnabled = true;
                    BoundRates.IsEnabled = true;
                }
                catch { }
            }

        }

        private void ReadDataButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SendDataButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
