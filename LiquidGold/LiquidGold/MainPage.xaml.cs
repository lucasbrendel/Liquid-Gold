using System;
using System.Collections;
using System.Collections.Generic;
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

namespace LiquidGold
{
    public partial class MainPage : PhoneApplicationPage
    {
        List<ViewModel.Vehicle> vehicles = new List<ViewModel.Vehicle>();

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            vehicles.Add(new ViewModel.Vehicle() { Name = "Mazda", Image = "Icons/appbar.add.dark.png" });
            vehicles.Add(new ViewModel.Vehicle() { Name = "Toyota", Image = "Icons/appbar.cancel.dark.png" });

            VehicleList.ItemsSource = vehicles;
        }

        private void VehicleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (VehicleList.SelectedIndex != -1)
            {
                NavigationService.Navigate(new Uri("/VehicleInfo.xaml?Name=" + vehicles[VehicleList.SelectedIndex].Name, UriKind.Relative));
            }
        }
    }
}