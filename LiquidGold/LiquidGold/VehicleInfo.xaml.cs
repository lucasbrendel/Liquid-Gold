using System;
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
using System.Data;
using System.Data.Linq;
using System.ComponentModel;

namespace LiquidGold
{
    public partial class VehicleInfo : PhoneApplicationPage, INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        private ViewModel.FillUpDataContext fillUpDB;

        private ViewModel.VehicleDataContext vehicleDB;

        private ViewModel.Vehicle CurrentVeh;

        private bool _delete;

        private int Index;

        private ObservableCollection<Stats> StatList = new ObservableCollection<Stats>();

        /// <summary>
        /// 
        /// </summary>
        private ObservableCollection<ViewModel.FillUp> _fillUpItems;

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ViewModel.FillUp> FillUpItems
        {
            get { return _fillUpItems; }
            set
            {
                if (value != _fillUpItems)
                {
                    _fillUpItems = value;
                    NotifyPropertyChanged("FillUpItems");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public VehicleInfo()
        {
            InitializeComponent();
            fillUpDB = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString);
            vehicleDB = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
            this.DataContext = this;
            _delete = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string _name;
            if (NavigationContext.QueryString.TryGetValue("Name", out _name))
            {
                int i = 0;
                var FillUpItemsInDB = from ViewModel.FillUp fills in fillUpDB.FillUpItems where fills.VehicleName == _name select fills;
                FillUpItems = new ObservableCollection<ViewModel.FillUp>(FillUpItemsInDB);
                foreach (ViewModel.FillUp fill in FillUpItems)
                {
                    if (fill.VehicleName == _name)
                    {
                        break;
                    }
                    i++;
                }
                Index = i;
                VehicleName.Text = _name;
                LoadStats();
                HistoryList.ItemsSource = FillUpItems;
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

            if (_delete)
            {
                fillUpDB.SubmitChanges();
                vehicleDB.SubmitChanges();
            }
        }

        private void LoadStats()
        {
            StatList.Add(new Stats{Name="Avg. Mileage", Value = 0.00});
            StatList.Add(new Stats{Name="Avg. Quantity", Value = 0.00});
            StatList.Add(new Stats{Name="Worst Mileage", Value = 0.00});
            StatList.Add(new Stats{Name="Best Mileage", Value = 0.00});
            StatList.Add(new Stats{Name="Avg. Distance", Value = 0.00});
            StatList.Add(new Stats{Name="Max. Distance", Value = 0.00});
            StatList.Add(new Stats{Name="Min. Distance", Value = 0.00});
            StatList.Add(new Stats{Name="Average Total Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Lowest Total Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Highest Total Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Overall Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Cost Last Year", Value = 0.00});
            StatList.Add(new Stats{Name="Cost Last Month", Value = 0.00});
            StatList.Add(new Stats{Name="Avg. Yearly Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Avg. Monthly Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Avg. Cost/Distance", Value = 0.00});
            StatList.Add(new Stats{Name="Max Cost/Distance", Value = 0.00});
            StatList.Add(new Stats{Name="Min Cost/Distance", Value = 0.00});
            StatList.Add(new Stats{Name="Avg. Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Max Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Min Cost", Value = 0.00});
            StatList.Add(new Stats{Name="Smallest Quantity", Value = 0.00});
            StatList.Add(new Stats{Name="Largest Quantity", Value = 0.00});
            StatList.Add(new Stats{Name="TotalQuantity", Value = 0.00});

            StatsList.ItemsSource = StatList;
        }

        private double AvgMileage()
        {
            double _value = 0.0;

            foreach(ViewModel.FillUp fill in FillUpItems)
            {
                
            }

            return _value;
        }

        private double BestMileage()
        {
            double _value = 0.0;

            return _value;
        }

        private double WorstMileage()
        {
            double _value = 0.0;

            return _value;
        }

        #region INotifyPropertyChanged
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
        #endregion
        private void settings_Click(object sender, EventArgs e)
        {

        }

        private void addFillBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//AddFill.xaml?Name=" + VehicleName.Text.ToString(), UriKind.Relative));
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            MessageBoxResult results = MessageBox.Show("Are you sure you want to delete " + VehicleName.Text.ToUpper() + "?", "Delete", MessageBoxButton.OKCancel);

            if (results == MessageBoxResult.OK)
            {
                fillUpDB.FillUpItems.DeleteAllOnSubmit(FillUpItems);
                
                var VehiclesInDB = from ViewModel.Vehicle veh in vehicleDB.VehicleItems select veh;
                ObservableCollection<ViewModel.Vehicle> vehicles = new ObservableCollection<ViewModel.Vehicle>(VehiclesInDB);
                vehicleDB.VehicleItems.DeleteOnSubmit(vehicles[Index]);
                _delete = true;
                NavigationService.Navigate(new Uri("//MainPage.xaml", UriKind.Relative));
            }
        }
    }

    public class Stats
    {
        private string _name;

        private double _value;

        public Stats()
        {

        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                }
            }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                if (value != _value)
                {
                    _value = value;
                }
            }
        }
    }
}