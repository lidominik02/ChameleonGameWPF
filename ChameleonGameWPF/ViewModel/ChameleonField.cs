using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;

namespace ChameleonGameWPF.ViewModel
{
    class ChameleonField : ViewModelBase
    {
        private int _color;
        private int _chameleon;
        private string _source;
        private bool _isEnable;

        public int Color
        {
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Chameleon
        {
            get => _chameleon;
            set
            {
                if (_chameleon != value)
                {
                    _chameleon = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Source
        {
            get => _source;
            set
            {
                if (_source != value)
                {
                    _source = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                if (_isEnable != value)
                {
                    _isEnable = value;
                    OnPropertyChanged();
                }
            }
        }
        public int Number { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public DelegateCommand StepCommand { get; set; }
    }
}
