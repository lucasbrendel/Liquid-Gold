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

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            List<ViewModel.Vehicle> vehicles = new List<ViewModel.Vehicle>();
            vehicles.Add(new ViewModel.Vehicle() { Name = "Mazda", Image = "Icons/appbar.add.dark.png" });
            vehicles.Add(new ViewModel.Vehicle() { Name = "Toyota", Image = "Icons/appbar.cancel.dark.png" });

            VehicleList.ItemsSource = vehicles;
        }
    }
}