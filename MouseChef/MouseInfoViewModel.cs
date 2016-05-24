using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MouseChef.Analysis;
using MouseChef.Models;

namespace MouseChef
{
    public class MouseInfoViewModel : INotifyPropertyChanged
    {
        private readonly MultiAnalyzerModel _multiAnalyzer;
        private readonly bool _isBaseline;

        private string _caption;
        private List<Mouse> _mouseOptions;

        public MouseInfoViewModel(MultiAnalyzerModel multiAnalyzer, bool isBaseline)
        {
            _multiAnalyzer = multiAnalyzer;
            _isBaseline = isBaseline;
        }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; OnPropertyChanged(); }
        }

        public Mouse SelectedMouse
        {
            get { return _isBaseline ? _multiAnalyzer.Baseline : _multiAnalyzer.Subject; }
            set
            {
                if (SelectedMouse == value) return;
                if (_isBaseline) _multiAnalyzer.Baseline = value;
                else _multiAnalyzer.Subject = value;
                OnPropertyChanged();
            }
        }

        public List<Mouse> MouseOptions
        {
            get { return _mouseOptions; }
            set { _mouseOptions = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
