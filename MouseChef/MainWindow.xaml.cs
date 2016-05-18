using System.Windows;

namespace MouseChef
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            Closed += (o, e) => DataContext.Dispose();
        }

        public new MainViewModel DataContext
        {
            get { return (MainViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}
