using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Telerik.Windows.Controls;

namespace LiquidGold
{
    public partial class AddFill : PhoneApplicationPage
    {
        private ViewModel.VehicleDataContext vehicleDb;
        private ViewModel.FillUpDataContext fillUpDb;

        private ObservableCollection<ViewModel.Vehicle> _vehicles;
        private ObservableCollection<ViewModel.FillUp> _fills;

        private bool _isEdit;

        ViewModel.FillUp EditFill;

        private GeoCoordinateWatcher watcher;
        private double _lat = Double.NaN;
        private double _lon = Double.NaN;

        private int ind;
        /// <summary>
        /// 
        /// </summary>
        public AddFill()
        {
            InitializeComponent();

            vehicleDb = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
            fillUpDb = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString);

            FillDate.Value = DateTime.Now;

            if (watcher == null)
            {
                watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
                watcher.MovementThreshold = 50;
                watcher.StatusChanged += new EventHandler<GeoPositionStatusChangedEventArgs>(watcher_StatusChanged);
                watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            }

            watcher.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            _lat = e.Position.Location.Latitude;
            _lon = e.Position.Location.Longitude;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case GeoPositionStatus.Disabled:
                    if (watcher.Permission == GeoPositionPermission.Denied)
                    {
                        RadMessageBox.Show("Location Denied", MessageBoxButtons.OK, "Location services on this device are denied. Please change them in settings to save location.",
                            null, false, true, HorizontalAlignment.Stretch, VerticalAlignment.Top, null);
                    }
                    else
                    {
                        RadMessageBox.Show("Location Issues", MessageBoxButtons.OK, "There are issues with location services. Your location may not save now.",
                            null, false, true, System.Windows.HorizontalAlignment.Stretch, System.Windows.VerticalAlignment.Top, null);
                    }
                    break;
                case GeoPositionStatus.Initializing:
                    break;
                case GeoPositionStatus.Ready:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string index = String.Empty;

            var vehItemsInDB = from ViewModel.Vehicle veh in vehicleDb.VehicleItems select veh;
            var fillItemsInDB = from ViewModel.FillUp fill in fillUpDb.FillUpItems select fill;
            _vehicles = new ObservableCollection<ViewModel.Vehicle>(vehItemsInDB);
            _fills = new ObservableCollection<ViewModel.FillUp>(fillItemsInDB);

            (App.Current as App).FillUps = fillUpDb;
            (App.Current as App).Vehicles = vehicleDb;

            VehiclesList.ItemsSource = _vehicles;
            if (NavigationContext.QueryString.TryGetValue("Name", out index))
            {
                string isEdit = "0";
                string listIndex;
                NavigationContext.QueryString.TryGetValue("IsEdit", out isEdit);
                NavigationContext.QueryString.TryGetValue("Index", out listIndex);
                ViewModel.Vehicle veh = new ViewModel.Vehicle();
                veh.Name = index;
                VehiclesList.SelectedIndex = _vehicles.IndexOf(_vehicles.Single(vehic => vehic.Name.Equals(index)));
                _isEdit = false;
                if (isEdit == "1")
                {
                    _isEdit = true;
                    ind = int.Parse(listIndex);
                    EditFill = _fills[ind];
                    Odo_txt.Text = EditFill.Odometer.ToString();
                    Quantity_txt.Text = EditFill.Quantity.ToString();
                    Cost_txt.Text = EditFill.Cost.ToString();
                    FillDate.Value = DateTime.Parse(EditFill.Date);
                    Notes_txt.Text = EditFill.Notes.ToString();
                }
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            fillUpDb.SubmitChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFill_Click(object sender, EventArgs e)
        {          
            if (Odo_txt.Text != String.Empty && Quantity_txt.Text != String.Empty && Cost_txt.Text != String.Empty)
            {
                ViewModel.Vehicle _vehicle = (ViewModel.Vehicle)VehiclesList.SelectedItem;
                DateTime date = (DateTime)FillDate.Value;

                if (!LocationSwitch.IsChecked)
                {
                    _lat = Double.NaN;
                    _lon = Double.NaN;
                }

                ViewModel.FillUp fill = new ViewModel.FillUp()
                {
                    VehicleName = _vehicle.Name,
                    Odometer = Convert.ToInt32(Odo_txt.Text),
                    Quantity = Convert.ToDouble(Quantity_txt.Text),
                    Cost = Convert.ToDouble(Cost_txt.Text),
                    Date = date.Date.ToString(),
                    Notes = Notes_txt.Text,
                    Latitude = _lat,
                    Longitude = _lon
                };

                if (_isEdit)
                {
                    var _fil = from ViewModel.FillUp f in fillUpDb.FillUpItems where 
                                   f.VehicleName == EditFill.VehicleName &&
                               f.Cost == EditFill.Cost &&
                               f.Date == EditFill.Date &&
                               f.Notes == EditFill.Notes &&
                               f.Quantity == EditFill.Quantity select f;
                    fillUpDb.FillUpItems.DeleteAllOnSubmit(_fil);
                    fillUpDb.SubmitChanges();

                    fillUpDb.FillUpItems.InsertOnSubmit(fill);
                    fillUpDb.SubmitChanges();
                    (App.Current as App).FillUps = fillUpDb;
                }
                else
                {
                    fillUpDb.FillUpItems.InsertOnSubmit(fill);
                }

                watcher.Stop();
                NavigationService.Navigate(new Uri("//VehicleInfo.xaml?Name=" + _vehicle.Name, UriKind.Relative));
            }
            else
            {
                MessageBox.Show("All values must be filled", "ERROR", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelFill_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }   
    }
}