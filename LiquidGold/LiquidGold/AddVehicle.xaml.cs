using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Microsoft.Phone.Controls;

namespace LiquidGold
{
    public partial class AddVehicle : PhoneApplicationPage
    {
        private ViewModel.VehicleDataContext vehicleDb;

        private ObservableCollection<ViewModel.Vehicle> _vehicles;

        private bool IsValueAdded;

        /// <summary>
        /// Constructor
        /// </summary>
        public AddVehicle()
        {
            InitializeComponent();
            IsValueAdded = false;

            vehicleDb = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
        }

        /// <summary>
        /// Override on navigation to
        /// </summary>
        /// <param name="e">Navigation event argument</param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            var vehItemsInDB = from ViewModel.Vehicle veh in vehicleDb.VehicleItems select veh;
            _vehicles = new ObservableCollection<ViewModel.Vehicle>(vehItemsInDB);

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// Add a new vehicle to data source
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void AddBtn_Click(object sender, EventArgs e)
        {
            ViewModel.Vehicle vehicle = new ViewModel.Vehicle { Name = Name_txt.Text, Make = Make_txt.Text, Model = Model_txt.Text, InitOdometer = Double.Parse(InitOdo_txt.Text) };

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

        /// <summary>
        /// Event on navigating from page
        /// </summary>
        /// <param name="e">Navigation event argument</param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (IsValueAdded)
            {
                vehicleDb.SubmitChanges();
            }
        }

        /// <summary>
        /// Cancel adding vehicle
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//MainPage.xaml", UriKind.Relative));
        }
    }
}