using Microsoft.Phone.Controls;
using Microsoft.Live;
using Telerik.Windows.Controls;

namespace LiquidGold
{
    public partial class Settings : PhoneApplicationPage
    {
        private LiveConnectSession _session;

        private LiveConnectClient _client;

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

        private void BackupBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void RestoreBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkydriveSignIn_SessionChanged(object sender, Microsoft.Live.Controls.LiveConnectSessionChangedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                _session = e.Session;
                _client = new LiveConnectClient(_session);
            }
            else
            {
                _client = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkydriveSignIn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LiveAuthClient auth = new LiveAuthClient(SkydriveSignIn.ClientId);
            auth.LoginCompleted += new System.EventHandler<LoginCompletedEventArgs>(auth_LoginCompleted);
            auth.LoginAsync(new string[] { "wl.signin", "wl.basic" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void auth_LoginCompleted(object sender, LoginCompletedEventArgs e)
        {
            if (e.Status == LiveConnectSessionStatus.Connected)
            {
                _session = e.Session;
                _client = new LiveConnectClient(_session);
            }
            else if (e.Error != null)
            {
                RadMessageBox.Show("Error", MessageBoxButtons.OK, e.Error.ToString(), null, false, false, System.Windows.HorizontalAlignment.Stretch, System.Windows.VerticalAlignment.Top, null);
            }            
        }
    }
}