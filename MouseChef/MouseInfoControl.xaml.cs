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
            DataContext = new MouseInfoViewModel
            {
                Caption = Caption,
            };
        }

        public string Caption { get; set; }
    }
}
