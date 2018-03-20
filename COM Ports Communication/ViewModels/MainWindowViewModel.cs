using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
namespace COM_Ports_Communication.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        #region private
        private Frame _mainWindowFrame;

        #endregion

        #region public properties
        public Frame MainWindowFrame
        {
            get { return _mainWindowFrame; }
            set
            {
                _mainWindowFrame = value;
                RaisePropertyChanged(nameof(MainWindowFrame));
            }
        }
        public Page CurrentPage { get; set; } = new Views.MainPage();

        #endregion

        public MainWindowViewModel()
        {
        }
    }
}
