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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var vehItemsInDB = from ViewModel.Vehicle veh in vehicleDb.VehicleItems select veh;

            Vehicles = new ObservableCollection<ViewModel.Vehicle>(vehItemsInDB);
            VehicleList.ItemsSource = Vehicles;
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
    }
}