using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;

namespace LiquidGold
{
    public partial class MainPage : INotifyPropertyChanged
    {
        private ViewModel.VehicleDataContext vehicleDb;

        private ObservableCollection<ViewModel.Vehicle> _vehicles;

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

            vehicleDb = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);

            this.DataContext = vehicleDb;

            LoadStorageVariables();
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadStorageVariables()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            RefreshVehicleList();

            base.OnNavigatedTo(e);
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
            NavigationService.Navigate(new Uri("//AddVehicle.xaml", UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFill_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//AddFill.xaml", UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settings_item_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//Settings.xaml", UriKind.Relative));
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
            ViewModel.FillUpDataContext fills = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString);
            var fil = from fill in fills.FillUpItems where fill.VehicleName == Vehicle.Name select fill;
            ObservableCollection<ViewModel.FillUp> fillDB = new ObservableCollection<ViewModel.FillUp>(fil);
            fills.FillUpItems.DeleteAllOnSubmit(fillDB);
            vehicleDb.VehicleItems.DeleteOnSubmit(Vehicle);
            fills.SubmitChanges();
            vehicleDb.SubmitChanges();

            (App.Current as App).FillUps = fills;
            RefreshVehicleList();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RefreshVehicleList()
        {
            var vehItemsInDB = from ViewModel.Vehicle veh in vehicleDb.VehicleItems select veh;

            Vehicles = new ObservableCollection<ViewModel.Vehicle>(vehItemsInDB);
            VehicleList.ItemsSource = Vehicles;

            (App.Current as App).Vehicles = vehicleDb;

            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            if (btn != null)
            {
                if (Vehicles.Count == 0)
                {
                    btn.IsEnabled = false;
                }
                else
                {
                    btn.IsEnabled = true;
                }
            }
        }
    }
}