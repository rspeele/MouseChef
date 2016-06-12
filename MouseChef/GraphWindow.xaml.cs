using System.Windows;
using MouseChef.Analysis;

namespace MouseChef
{
    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        public GraphWindow(IStats xStats, IStats yStats)
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
