using System;
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
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace LiquidGold
{
    public partial class AddVehicle : PhoneApplicationPage, INotifyPropertyChanged
    {
        private ViewModel.VehicleDataContext vehicleDb;

        private ObservableCollection<ViewModel.Vehicle> _vehicles;

        private bool IsValueAdded;

        public AddVehicle()
        {
            InitializeComponent();
            IsValueAdded = false;

            vehicleDb = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var vehItemsInDB = from ViewModel.Vehicle veh in vehicleDb.VehicleItems select veh;
            _vehicles = new ObservableCollection<ViewModel.Vehicle>(vehItemsInDB);

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBtn_Click(object sender, EventArgs e)
        {
            ViewModel.Vehicle vehicle = new ViewModel.Vehicle { Name = Name_txt.Text, Make=Make_txt.Text, Model=Model_txt.Text};

            if (Name_txt.Text != String.Empty)
            {
                if (_vehicles.Contains(vehicle))
                {
                    MessageBox.Show("This vehicle already exists and cannot be duplicated. Pick a different name please.", "ERROR", MessageBoxButton.OK);
                }
                else
                {
                    if (InitOdo_txt.Text != String.Empty)
                    {
                        vehicleDb.VehicleItems.InsertOnSubmit(vehicle);
                        IsValueAdded = true;
                        NavigationService.Navigate(new Uri("//MainPage.xaml", UriKind.Relative));
                    }
                    else
                    {
                        MessageBox.Show("Please enter an intial odometer reading", "ERROR", MessageBoxButton.OK);
                    }
                }
            }
            else
            {
                MessageBox.Show("The name of the vehicle cannot be empty", "ERROR", MessageBoxButton.OK);
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (IsValueAdded)
            {
                vehicleDb.SubmitChanges();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//MainPage.xaml", UriKind.Relative));
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
    }
}