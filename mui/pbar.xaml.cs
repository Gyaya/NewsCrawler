using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace mui
{
    /// <summary>
    /// pbar.xaml 的交互逻辑
    /// </summary>
    public partial class pbar : UserControl
    {
        public pbar()
        {
            InitializeComponent();
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
                       typeof(Timeline),
                           new FrameworkPropertyMetadata { DefaultValue = 100  });
        }
    }
}
