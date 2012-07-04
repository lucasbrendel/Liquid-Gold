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

        private bool _isEdit;

        ViewModel.FillUp EditFill;

        private int ind;
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
                ViewModel.FillUp fill = new ViewModel.FillUp()
                {
                    VehicleName = _vehicle.Name,
                    Odometer = Convert.ToInt32(Odo_txt.Text),
                    Quantity = Convert.ToDouble(Quantity_txt.Text),
                    Cost = Convert.ToDouble(Cost_txt.Text),
                    Date = date.Date.ToString(),
                    Notes = Notes_txt.Text
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