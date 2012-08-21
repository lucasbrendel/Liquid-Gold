using System;
using System.Device.Location;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;
using System.Linq;


namespace LiquidGold
{
    public partial class ViewFill : PhoneApplicationPage
    {
        private GeoCoordinate _location = new GeoCoordinate();

        private string _index;

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewFill()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Override of navigation to
        /// </summary>
        /// <param name="e">Navigation event argument</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string Name;
            string Odo;
            string Quan;
            string Cost;
            string Date;
            string Notes;
            string Lat, Lon;
            string index;

            if (NavigationContext.QueryString.TryGetValue("Name", out Name))
            {
                NavigationContext.QueryString.TryGetValue("Odo", out Odo);
                NavigationContext.QueryString.TryGetValue("Quantity", out Quan);
                NavigationContext.QueryString.TryGetValue("Cost", out Cost);
                NavigationContext.QueryString.TryGetValue("Date", out Date);
                NavigationContext.QueryString.TryGetValue("Notes", out Notes);
                NavigationContext.QueryString.TryGetValue("Lat", out Lat);
                NavigationContext.QueryString.TryGetValue("Lon", out Lon);
                NavigationContext.QueryString.TryGetValue("Index", out index);

                PageTitle.Text = Name;
                Odo_txt.Text = Odo;
                Quantity_txt.Text = Quan;
                Cost_txt.Text = Cost;
                Date_txt.Text = Date;
                Notes_txt.Text = Notes;
                _index = index;

                if (Double.IsNaN(Double.Parse(Lat)) || Double.IsNaN(Double.Parse(Lon)))
                {
                    FillLocationMap.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    _location.Latitude = Double.Parse(Lat);
                    _location.Longitude = Double.Parse(Lon);
                    LocationPin.Location = _location;
                    FillLocationMap.Center = LocationPin.Location;
                }
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Edit the current fill
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void EditFillBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//AddFill.xaml?Name=" + PageTitle.Text + "&IsEdit=1&Index=" + _index, UriKind.Relative));
        }

        /// <summary>
        /// Delete the current fill entry
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void DeleteFillBtn_Click(object sender, EventArgs e)
        {
            RadMessageBox.Show("Delete Fill", MessageBoxButtons.OKCancel, "Are you sure you want to delete this fill? This can't be undone.",
                null, false, true, System.Windows.HorizontalAlignment.Stretch, System.Windows.VerticalAlignment.Top, closedHandler: (args) =>
                    {
                        if (args.Result == DialogResult.OK)
                        {
                            DeleteFill();
                        }
                        else
                        {

                        }
                    }
            );
        }
        
        /// <summary>
        /// 
        /// </summary>
        private void DeleteFill()
        {
            var fil = from ViewModel.FillUp fils in (App.Current as App).FillUps.FillUpItems where fils.Odometer == Int32.Parse(Odo_txt.Text) && fils.VehicleName == PageTitle.Text select fils;
            ViewModel.FillUp fill = fil.First();
            (App.Current as App).FillUps.FillUpItems.DeleteOnSubmit(fill);
            (App.Current as App).FillUps.SubmitChanges();
            NavigationService.Navigate(new Uri("/VehicleInfo.xaml?Name=" + PageTitle.Text, UriKind.Relative));
        }
    }
}