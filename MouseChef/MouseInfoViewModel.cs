using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MouseChef
{
    public class MouseInfoViewModel : INotifyPropertyChanged
    {
        private string _caption;

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
