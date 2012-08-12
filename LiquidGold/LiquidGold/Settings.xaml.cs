using Microsoft.Phone.Controls;

namespace LiquidGold
{
    public partial class Settings : PhoneApplicationPage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Settings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override function to handle when page is navigated to
        /// </summary>
        /// <param name="e">Navigation event argument</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            LocationSwitch.IsChecked = (App.Current as App).LocationAware;

            if ((App.Current as App).UserUnits == App.Units.Imperial)
            {
                ImperialRad.IsChecked = true;
            }
            else
            {
                MetricRad.IsChecked = true;
            }
        }
    }
}