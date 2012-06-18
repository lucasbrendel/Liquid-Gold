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

        private ObservableCollection<ViewModel.Vehicle> _vehicles;

        public AddFill()
        {
            InitializeComponent();

            vehicleDb = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
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
            _vehicles = new ObservableCollection<ViewModel.Vehicle>(vehItemsInDB);

            VehiclesList.ItemsSource = _vehicles;

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddFill_Click(object sender, EventArgs e)
        {
            string _name = "";
            ViewModel.FillUp fill = new ViewModel.FillUp(){
                VehicleName = _name, 
                Odometer = Convert.ToInt32(Odo_txt.Text), 
                Quantity = Convert.ToDouble(Quantity_txt.Text), 
                Cost = Convert.ToDouble(Cost_txt.Text), 
                Date = (DateTime)FillDate.Value 
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelFill_Click(object sender, EventArgs e)
        {

        }
    }
}