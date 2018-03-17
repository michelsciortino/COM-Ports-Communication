using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace COM_Ports_Communication.ViewModels
{
    public class MainPageViewModel : BaseVireModel
    {
        #region define
        public static Brush ConnectedColor = Brushes.GreenYellow;
        public static Brush NotConnectedColor = Brushes.Red;
        #endregion
        
        #region Private Variables
        //private Mutex listeningMutex = new Mutex();
        private bool IsListening = false;
        private Thread listener;
        private Models.SerialPortCommunication serialPort;
        private bool end;
        #endregion

        #region Private Properties

        private string _continuousButton_text = "Read Continuously";
        private string _receivedData;
        private string _sendingData;
        private bool _canOpenPort = false;
        private bool _canClosePort = false;
        private bool _canClear = false;
        private bool _canReadContinuously = false;
        private bool _canReadLine = false;
        private bool _canSendData= false;
        private bool _canSelectPort = true;
        private bool _canSelectBoundRate = true;
        private string _selectedPort;
        private string _selectedBoundRate;
        private Brush _statusColor = NotConnectedColor;

        #endregion

        #region Public Properties

        public string ContinuousButton_text
        {
            get { return _continuousButton_text; }
            set
            {
                if (_continuousButton_text != value)
                {
                    _continuousButton_text = value;
                    OnPropertyChanged(nameof(ContinuousButton_text));
                }
            }
        }
        public List<string> PortList { get { return Models.SerialPortCommunication.GetAvailablePortNames().ToList<string>(); } }
        public List<string> BoundRates { get { return Models.SerialPortCommunication.BoundRates; } }
        public string ReceivedData
        {
            get { return _receivedData; }
            set
            {
                _receivedData=value;
                OnPropertyChanged(ReceivedData);                
            }
        }
        public string SendingData
        {
            get { return _sendingData?.ToString(); }
            set
            {
                _sendingData = value;
                OnPropertyChanged(SendingData);
            }
        }

        

        public bool CanOpenPort
        {
            get { return _canOpenPort; }
            set {
                if(_canOpenPort != value)
                {
                    _canOpenPort = value;
                    OnPropertyChanged(nameof(CanOpenPort));
                }
            }
        }
        public bool CanClosePort
        {
            get { return _canClosePort; }
            set
            {
                if (_canClosePort != value)
                {
                    _canClosePort = value;
                    OnPropertyChanged(nameof(CanClosePort));
                }
            }
        }
        public bool CanClear
        {
            get { return _canClear; }
            set
            {
                if (_canClear != value)
                {
                    _canClear = value;
                    OnPropertyChanged(nameof(CanClear));
                }
            }
        }
        public bool CanReadContinuously
        {
            get { return _canReadContinuously; }
            set
            {
                if (_canReadContinuously != value)
                {
                    _canReadContinuously = value;
                    OnPropertyChanged(nameof(CanReadContinuously));
                }
            }
        }
        public bool CanReadLine
        {
            get { return _canReadLine; }
            set
            {
                if (_canReadLine != value)
                {
                    _canReadLine = value;
                    OnPropertyChanged(nameof(CanReadLine));
                }
            }
        }
        public bool CanSendData
        {
            get { return _canSendData; }
            set
            {
                if (_canSendData != value)
                {
                    _canSendData = value;
                    OnPropertyChanged(nameof(CanSendData));
                }
            }
        }
        public bool CanSelectPort
        {
            get { return _canSelectPort; }
            set
            {
                if (_canSelectPort != value)
                {
                    _canSelectPort = value;
                    OnPropertyChanged(nameof(CanSelectPort));
                }
            }
        }
        public bool CanSelectBoundRate
        {
            get { return _canSelectBoundRate; }
            set
            {
                if (_canSelectBoundRate != value)
                {
                    _canSelectBoundRate = value;
                    OnPropertyChanged(nameof(CanSelectBoundRate));
                }
            }
        }
        public string SelectedPort {
            get { return _selectedPort; }
            set {
                if (_selectedPort != value)
                {
                    _selectedPort = value;
                    OnPropertyChanged(nameof(SelectedPort));
                }
            }
        }
        public string SelectedBoundRate
        {
            get { return _selectedBoundRate; }
            set
            {
                if (_selectedBoundRate != value)
                {
                    _selectedBoundRate = value;
                    OnPropertyChanged(nameof(SelectedBoundRate));
                }
            }
        }
        public Brush StatusColor
        {
            get { return _statusColor; }
            set
            {
                if (_statusColor != value)
                {
                    _statusColor = value;
                    OnPropertyChanged(nameof(StatusColor));
                }
            }
        }

        #endregion

        #region Constructor
        public MainPageViewModel()
        {
            ConnectionSettingSelected = new RelayCommand(() =>
            {
                if (SelectedPort != null && SelectedBoundRate != null)
                {
                    CanOpenPort = true;
                }
                else
                {
                    CanOpenPort = false;
                    CanClosePort = false;
                }
            });
            OpenPortButtonClick = new RelayCommand(()=> {
                if(SelectedPort == null || SelectedBoundRate == null)
                    return;
                try
                {
                    serialPort = new Models.SerialPortCommunication(SelectedPort.ToString(), Convert.ToInt32(SelectedBoundRate.ToString()));
                    try
                    {
                        serialPort.OpenConnection();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"{ex.Message}", "Error", MessageBoxButton.OK);
                        return;
                    }
                    CanOpenPort = false;
                    CanClosePort = true;
                    
                    /* Port Listening Button Enabling */
                    CanClear = true;
                    CanReadContinuously = true;
                    CanReadLine = true;

                    /* Port Sending Button Enabling */
                    CanSendData = true;

                    /* Disabling Port and Bound Rate Selection */
                    CanSelectPort = false;
                    CanSelectBoundRate = false;

                    /* Updating Connection Status Ribbon */
                    StatusColor = ConnectedColor;
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, $"Error opening port {SelectedPort}", MessageBoxButton.OK);
                }
            });
            ClosePortButtonClick = new RelayCommand(()=>
            {
                if (SelectedPort != null && serialPort.IsOpen == true)
                {
                    try
                    {
                        serialPort.CloseConnection();

                        /* Disabling receiving and sending data buttons */
                        CanReadContinuously = false;
                        CanReadLine = false;
                        CanSendData = false;

                        /* Enabling Port Settings Selection */
                        CanSelectPort = true;
                        CanSelectBoundRate = true;

                        /* Resetting Port Settings */
                        SelectedPort = null;
                        SelectedBoundRate = null;
                        CanOpenPort = false;
                        CanClosePort = false;

                        /* Updating Connection Status Ribbon */
                        StatusColor = NotConnectedColor;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, $"Error closing port {SelectedPort}", MessageBoxButton.OK);
                    }
                }
            });
            ClearButtonClick = new RelayCommand(() =>
            {
                ReceivedData = "";
            });
            ContinuousReading_Click = new RelayCommand(()=>
            {
                if (!IsListening)
                {
                    IsListening = true;
                    listener = new Thread(ListenSerialPort);

                    //listeningMutex.WaitOne();
                    end = false;
                    //listeningMutex.ReleaseMutex();


                    ContinuousButton_text = "Stop";
                    CanReadLine = false;
                    listener.Start();
                }
                else
                {
                    //listeningMutex.WaitOne();
                    end = true;
                    //listeningMutex.ReleaseMutex();
                }
            });
            ReadDataClick = new RelayCommand(()=>
            {
                if (!IsListening)
                {
                    try
                    {
                        string receving = serialPort.ReadLine();
                        ReceivedData += receving;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, $"Error reading Data from {SelectedPort}", MessageBoxButton.OK);
                    }
                }
            });
            SendDataButtonClick = new RelayCommand(() =>
             {
                 if (!IsListening)
                 {
                     try
                     {
                         serialPort.WriteLine(SendingData);
                         SendingData = "";
                     }
                     catch(Exception ex)
                     {
                         MessageBox.Show(ex.Message, $"Error sending Data on {SelectedPort}", MessageBoxButton.OK);
                     }
                 }
             });
        }
        #endregion

        #region Public Commands

        public ICommand ConnectionSettingSelected { get; set; }
        public ICommand OpenPortButtonClick { get; set; }
        public ICommand ClosePortButtonClick { get; set; }
        public ICommand ClearButtonClick { get; set; }
        public ICommand ContinuousReading_Click { get; set; }
        public ICommand ReadDataClick { get; set; }
        public ICommand SendDataButtonClick { get; set; }

        #endregion



        

        private void ListenSerialPort()
        {
            //listeningMutex.WaitOne();
            while (!end)
            {
                //listeningMutex.ReleaseMutex();
                string received = serialPort.ReadLine();
                ReceivedData += received;
                //listeningMutex.WaitOne();
            }
            //listeningMutex.ReleaseMutex();

            ContinuousButton_text = "Read Continuously";
            CanReadLine = true;
            IsListening = false;
        }
    }
}