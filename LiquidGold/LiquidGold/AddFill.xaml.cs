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

namespace LiquidGold
{
    public partial class AddFill : PhoneApplicationPage
    {
        private ViewModel.VehicleDataContext vehicleDb;
        private ViewModel.FillUpDataContext fillUpDb;

        private ObservableCollection<ViewModel.Vehicle> _vehicles;
        private ObservableCollection<ViewModel.FillUp> _fills;

        /// <summary>
        /// 
        /// </summary>
        public AddFill()
        {
            InitializeComponent();

            vehicleDb = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
            fillUpDb = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string index = String.Empty;
            NavigationContext.QueryString.TryGetValue("selectedIndex", out index);

            var vehItemsInDB = from ViewModel.Vehicle veh in vehicleDb.VehicleItems select veh;
            var fillItemsInDB = from ViewModel.FillUp fill in fillUpDb.FillUpItems select fill;
            _vehicles = new ObservableCollection<ViewModel.Vehicle>(vehItemsInDB);
            _fills = new ObservableCollection<ViewModel.FillUp>(fillItemsInDB);

            VehiclesList.ItemsSource = _vehicles;

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
                ViewModel.FillUp fill = new ViewModel.FillUp()
                {
                    VehicleName = _vehicle.Name,
                    Odometer = Convert.ToInt32(Odo_txt.Text),
                    Quantity = Convert.ToDouble(Quantity_txt.Text),
                    Cost = Convert.ToDouble(Cost_txt.Text),
                    Date = date.Date.ToString()
                };

                fillUpDb.FillUpItems.InsertOnSubmit(fill);
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
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