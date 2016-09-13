using Windows.UI.Xaml.Controls;
using SensorsMB2.ViewModels;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SensorsMB2.Views
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SensorStreamView : Page
    {
        public SensorStreamView()
        {
            DataContext = new SensorStreamViewModel();
            InitializeComponent();
        }
    }
}