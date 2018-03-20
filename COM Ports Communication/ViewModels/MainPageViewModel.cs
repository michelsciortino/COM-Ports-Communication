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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Diagnostics;


namespace COM_Ports_Communication.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        #region define
        public static Brush ConnectedColor = Brushes.GreenYellow;
        public static Brush NotConnectedColor = Brushes.Red;
        public static int DefaultTimeout = 1000;
        #endregion

        #region Private Variables
        //private Mutex listeningMutex = new Mutex();
        private bool IsListening = false;
        private Thread listener;
        private Models.SerialPortCommunication serialPort;
        private bool end;
        private int timeout= DefaultTimeout;
        #endregion

        #region Private Properties
        private string _timeout = "";
        private string _continuousButton_text = "Read Continuously";
        private string _receivedData="";
        private string _sendingData;
        private bool _canOpenPort = false;
        private bool _canClosePort = false;
        private bool _canClear = false;
        private bool _canReadContinuously = false;
        private bool _canReadLine = false;
        private bool _canSetTimeout = false;
        private bool _canSendData= false;
        private bool _canSelectPort = true;
        private bool _canSelectBoundRate = true;
        private string _selectedPort;
        private string _selectedBoundRate;
        private Brush _statusColor = NotConnectedColor;

        #endregion

        #region Public Properties

        public string Timeout
        {
            get { return _timeout; }
            set
            {
                if (value != _timeout)
                {
                    if (value == "")
                    {
                        _timeout = "";
                        timeout = DefaultTimeout;
                    }
                    else
                    try
                    {
                        timeout = int.Parse(value);
                        _timeout = value;
                        RaisePropertyChanged(nameof(Timeout));
                    }
                    catch //non è un numero
                    {
                        Timeout = _timeout;
                        RaisePropertyChanged(nameof(Timeout));
                    }
                }
            }
        }
        public string ContinuousButton_text
        {
            get { return _continuousButton_text; }
            set
            {
                if (_continuousButton_text != value)
                {
                    _continuousButton_text = value;
                    RaisePropertyChanged(nameof(ContinuousButton_text));
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
                RaisePropertyChanged(nameof(ReceivedData));                
            }
        }
        public string SendingData
        {
            get { return _sendingData?.ToString(); }
            set
            {
                _sendingData = value;
                RaisePropertyChanged(nameof(SendingData));
            }
        }

        public bool CanOpenPort
        {
            get { return _canOpenPort; }
            set {
                if(_canOpenPort != value)
                {
                    _canOpenPort = value;
                    RaisePropertyChanged(nameof(CanOpenPort));
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
                    RaisePropertyChanged(nameof(CanClosePort));
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
                    RaisePropertyChanged(nameof(CanClear));
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
                    RaisePropertyChanged(nameof(CanReadContinuously));
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
                    RaisePropertyChanged(nameof(CanReadLine));
                }
            }
        }
        public bool CanSetTimeout
        {
            get { return _canSetTimeout; }
            set
            {
                if (value != _canSetTimeout)
                {
                    _canSetTimeout = value;
                    RaisePropertyChanged(nameof(CanSetTimeout));
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
                    RaisePropertyChanged(nameof(CanSendData));
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
                    RaisePropertyChanged(nameof(CanSelectPort));
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
                    RaisePropertyChanged(nameof(CanSelectBoundRate));
                }
            }
        }
        public string SelectedPort {
            get { return _selectedPort; }
            set {
                if (_selectedPort != value)
                {
                    _selectedPort = value;
                    RaisePropertyChanged(nameof(SelectedPort));
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
                    RaisePropertyChanged(nameof(SelectedBoundRate));
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
                    RaisePropertyChanged(nameof(StatusColor));
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
                    CanSetTimeout = true;

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
                        if (listener!=null &&
                            (listener.ThreadState == System.Threading.ThreadState.Running ||
                            listener.ThreadState == System.Threading.ThreadState.Background ||
                            listener.ThreadState== System.Threading.ThreadState.WaitSleepJoin))
                        {
                            end = true;
                            listener.Join();
                            ContinuousButton_text = "Read Continuously";
                            IsListening = false;
                        }
                        serialPort.CloseConnection();


                        /* Disabling receiving and sending data buttons */
                        CanReadContinuously = false;
                        CanReadLine = false;
                        CanSendData = false;
                        CanClear = false;

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
                    CanSetTimeout = false;
                    CanReadLine = false;
                    listener.Start();
                    //ListenSerialPort();

                }
                else
                {
                    //listeningMutex.WaitOne();
                    end = true;
                    try
                    {
                        listener.Join();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
                    }
                    //listeningMutex.ReleaseMutex();

                    ContinuousButton_text = "Read Continuously";
                    CanReadLine = true;
                    CanSetTimeout = true;
                    IsListening = false;
                }
            });
            ReadDataClick = new RelayCommand(()=>
            {
                if (!IsListening)
                {
                    try
                    {
                        string receving = serialPort.ReadLine(timeout);
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
                try
                {
                    string received = serialPort.ReadLine(timeout);
                    ReceivedData += received;
                }
                catch { }
                //listeningMutex.WaitOne();
            }
            //listeningMutex.ReleaseMutex();
        }
    }
}