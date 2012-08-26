using System;
using System.Device.Location;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;
using System.Linq;
using System.Collections.ObjectModel;


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
            string index;

            if (NavigationContext.QueryString.TryGetValue("Name", out Name))
            {
                NavigationContext.QueryString.TryGetValue("Index", out index);
                _index = index;

                var fill = from ViewModel.FillUp fil in (App.Current as App).FillUps.FillUpItems where fil.VehicleName == Name select fil;
                var veh = from ViewModel.Vehicle v in (App.Current as App).Vehicles.VehicleItems where v.Name == Name select v;
                ViewModel.Vehicle vehs = new ViewModel.Vehicle();
                vehs = veh.First();
                ObservableCollection<ViewModel.FillUp> fills = new ObservableCollection<ViewModel.FillUp>(fill);
                ViewModel.FillUp f = fills[Int32.Parse(_index)];
                CalcDistance(vehs, fills, f);
                FillControls(Name, f);
                
                if (Double.IsNaN(f.Latitude) || Double.IsNaN(f.Longitude))
                {
                    FillLocationMap.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    _location.Latitude = f.Latitude;
                    _location.Longitude = f.Longitude;
                    LocationPin.Location = _location;
                    FillLocationMap.Center = LocationPin.Location;
                }
            }
            base.OnNavigatedTo(e);
        }

        private void FillControls(string Name, ViewModel.FillUp f)
        {
            PageTitle.Text = Name;
            Odo_txt.Text = f.Odometer.ToString();
            Dist_txt.Text = f.Distance.ToString();
            Quantity_txt.Text = f.Quantity.ToString();
            Mile_txt.Text = Math.Round((f.Distance / f.Quantity), 2).ToString();
            Cost_txt.Text = f.Cost.ToString();
            Date_txt.Text = f.Date.ToString();
            Notes_txt.Text = f.Notes.ToString();
        }

        private void CalcDistance(ViewModel.Vehicle vehs, ObservableCollection<ViewModel.FillUp> fills, ViewModel.FillUp f)
        {
            if (_index.Equals("0"))
            {
                f.Distance = fills[Int32.Parse(_index)].Odometer - vehs.InitOdometer;
            }
            else
            {
                f.Distance = fills[Int32.Parse(_index)].Odometer - fills[Int32.Parse(_index) - 1].Odometer;
            }
        }

        /// <summary>
        /// Edit the current fill
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void EditFillBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddFill.xaml?Name=" + PageTitle.Text + "&IsEdit=1&Index=" + Odo_txt.Text, UriKind.Relative));
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