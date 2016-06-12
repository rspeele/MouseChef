using System.Windows;

namespace MouseChef
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public GraphWindow(Graphable xStats, Graphable yStats)
        {
            InitializeComponent();
            DataContext = new GraphViewModel(xStats, yStats);
        }

        public new GraphViewModel DataContext
        {
            get { return (GraphViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}
