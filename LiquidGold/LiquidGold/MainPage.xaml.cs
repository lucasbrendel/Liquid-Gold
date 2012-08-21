using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Telerik.Windows.Controls;
using System.IO.IsolatedStorage;

namespace LiquidGold
{
    public partial class MainPage : INotifyPropertyChanged
    {
        //private ViewModel.VehicleDataContext vehicleDb;

        private ObservableCollection<ViewModel.Vehicle> _vehicles;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ViewModel.Vehicle> Vehicles
        {
            get { return _vehicles; }
            set
            {
                if (value != _vehicles)
                {
                    _vehicles = value;
                    NotifyPropertyChanged("Vehicles");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            ShellTile tile = LiveTileHelper.GetTile(new Uri("/AddFill.xaml", UriKind.RelativeOrAbsolute));

            if (tile == null)
            {
                ApplicationBarMenuItem item = (ApplicationBarMenuItem)ApplicationBar.MenuItems[1];
                item.IsEnabled = true;
            }
            else
            {
                ApplicationBarMenuItem item = (ApplicationBarMenuItem)ApplicationBar.MenuItems[1];
                item.IsEnabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var veh = from ViewModel.Vehicle vehs in (App.Current as App).Vehicles.VehicleItems select vehs;
            this.Vehicles = new ObservableCollection<ViewModel.Vehicle>(veh);
            RefreshVehicleList();
            if ((App.Current as App).AskUserForLocation)
            {
                AskUserForLocation();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AskUserForLocation()
        {
            RadMessageBox.Show("Access Location", MessageBoxButtons.YesNo, "Liquid Gold wants to check to make you want location awareness on by default. Choose 'Yes', if you want it to save your location.",
                null, false, true, System.Windows.HorizontalAlignment.Stretch, System.Windows.VerticalAlignment.Top, closedHandler: (args) =>
                    {
                        if (args.Result == DialogResult.OK)
                        {
                            (App.Current as App).LocationAware = true;
                        }
                        else
                        {
                            (App.Current as App).LocationAware = false;
                        }
                    }
            );

            IsolatedStorageSettings.ApplicationSettings["LocationAware"] = (App.Current as App).LocationAware.ToString();
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VehicleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VehicleList.SelectedIndex != -1)
            {
                ViewModel.Vehicle vehicle = (ViewModel.Vehicle)VehicleList.SelectedItem;
                NavigationService.Navigate(new Uri("/VehicleInfo.xaml?Name=" + vehicle.Name, UriKind.Relative));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddVeh_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddVehicle.xaml", UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFill_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/AddFill.xaml", UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settings_item_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VehicleList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            VehicleList.SelectedItem = sender;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            int index = VehicleList.ItemContainerGenerator.IndexFromContainer((VehicleList.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem));
            if (index != -1)
            {
               ViewModel.Vehicle SelectedVehicle = (ViewModel.Vehicle)VehicleList.Items[index];

                //Selected context menu item is to pin to start
                if (menu.Header.ToString().Equals("pin to start"))
                {
                    LiveTileHelper.CreateOrUpdateTile(new RadExtendedTileData() { Title = SelectedVehicle.Name.ToString(), BackgroundImage = new Uri("/Images/Car.png", UriKind.Relative) }, new Uri("/VehicleInfo.xaml?Name=" + SelectedVehicle.Name.ToString(), UriKind.RelativeOrAbsolute)); 
                }
                //Selected context menu item is to delete the vehicle
                else if (menu.Header.ToString().Equals("delete"))
                {
                    DeleteVehicle(SelectedVehicle);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteVehicle(ViewModel.Vehicle Vehicle)
        {
            ShellTile Tile = ShellTile.ActiveTiles.FirstOrDefault(x => x.NavigationUri.ToString().Contains("Name=" + Vehicle.Name.ToString()));

            if (Tile != null)
            {
                Tile.Delete();
            }
            //ViewModel.FillUpDataContext fills = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString);
            var fil = from fill in (App.Current as App).FillUps.FillUpItems where fill.VehicleName == Vehicle.Name select fill;
            ObservableCollection<ViewModel.FillUp> fillDB = new ObservableCollection<ViewModel.FillUp>(fil);
            (App.Current as App).FillUps.FillUpItems.DeleteAllOnSubmit(fillDB);
            (App.Current as App).Vehicles.VehicleItems.DeleteOnSubmit(Vehicle);
            (App.Current as App).FillUps.SubmitChanges();
            (App.Current as App).Vehicles.SubmitChanges();

            var veh = from ViewModel.Vehicle vehs in (App.Current as App).Vehicles.VehicleItems select vehs;
            this.Vehicles = new ObservableCollection<ViewModel.Vehicle>(veh);
            //(App.Current as App).FillUps = fills;
            RefreshVehicleList();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshVehicleList()
        {
            VehicleList.ItemsSource = this.Vehicles;

            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            ApplicationBarIconButton vehbtn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (btn != null)
            {
                if (VehicleList.Items.Count == 0)
                {
                    btn.IsEnabled = false;
                }
                else
                {
                    btn.IsEnabled = true;
                }

                if (VehicleList.Items.Count == 5)
                {
                    vehbtn.IsEnabled = false;
                }
                else
                {
                    vehbtn.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PinQuickFill_Click(object sender, EventArgs e)
        {
            LiveTileHelper.CreateOrUpdateTile(new RadExtendedTileData() { Title = "Add Fill", BackgroundImage= new Uri("/Images/AddFillIcon.png", UriKind.RelativeOrAbsolute) }, new Uri("/AddFill.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}