using System.Windows.Controls;

namespace MouseChef
{
    /// <summary>
    /// Interaction logic for MouseInfoControl.xaml
    /// </summary>
    public partial class MouseInfoControl : UserControl
    {
        public MouseInfoControl()
        {
            InitializeComponent();
        }

        public new MouseInfoViewModel DataContext
        {
            get { return (MouseInfoViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }
    }
}
