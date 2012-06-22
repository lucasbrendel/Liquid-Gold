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
                CalculateDistance();
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
            StatList.Add(new Stats { Name = "Avg. Mileage", Value = AvgMileage() });
            StatList.Add(new Stats { Name = "Best Mileage", Value = BestMileage() });
            StatList.Add(new Stats { Name = "Worst Mileage", Value = WorstMileage() });
            StatList.Add(new Stats { Name = "Avg. Quantity", Value = 0.00 });
            StatList.Add(new Stats { Name = "Avg. Distance", Value = 0.00 });
            StatList.Add(new Stats { Name = "Max. Distance", Value = 0.00 });
            StatList.Add(new Stats { Name = "Min. Distance", Value = 0.00 });
            StatList.Add(new Stats { Name = "Average Total Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Lowest Total Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Highest Total Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Overall Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Cost Last Year", Value = 0.00 });
            StatList.Add(new Stats { Name = "Cost Last Month", Value = 0.00 });
            StatList.Add(new Stats { Name = "Avg. Yearly Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Avg. Monthly Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Avg. Cost/Distance", Value = 0.00 });
            StatList.Add(new Stats { Name = "Max Cost/Distance", Value = 0.00 });
            StatList.Add(new Stats { Name = "Min Cost/Distance", Value = 0.00 });
            StatList.Add(new Stats { Name = "Avg. Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Max Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Min Cost", Value = 0.00 });
            StatList.Add(new Stats { Name = "Smallest Quantity", Value = 0.00 });
            StatList.Add(new Stats { Name = "Largest Quantity", Value = 0.00 });
            StatList.Add(new Stats { Name = "TotalQuantity", Value = 0.00 });

            StatsList.ItemsSource = StatList;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateDistance()
        {
            for (int i = 0; i < _fillUpItems.Count; i++)
            {
                if (i == 0)
                {
                    _fillUpItems[i].Distance = 0.0;
                }
                else
                {
                    _fillUpItems[i].Distance = _fillUpItems[i].Odometer - _fillUpItems[i - 1].Odometer;
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double AvgMileage()
        {
            double _value = 0.0;
            if (_fillUpItems.Count > 0)
            {
                double _fuel = _fillUpItems.Sum(fillup => fillup.Quantity);
                double _distance = _fillUpItems.Last().Odometer - _fillUpItems.First().Odometer;

                _value = _distance / _fuel;
            }
            return Math.Round(_value, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double BestMileage()
        {
            double best = 0.0;
            double temp = 0.0;
            if (_fillUpItems.Count > 1)
            {
                foreach (ViewModel.FillUp fill in _fillUpItems)
                {
                    temp = fill.Distance / fill.Quantity;
                    if (temp > best)
                    {
                        best = temp;
                    }
                }
            }
            return Math.Round(best, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double WorstMileage()
        {
            double worst = 0.0;
            double temp = 0.0;
            if (_fillUpItems.Count > 1)
            {
                worst = Double.PositiveInfinity;
                foreach (ViewModel.FillUp fill in _fillUpItems)
                {
                    temp = fill.Distance / fill.Quantity;
                    if (temp < worst)
                    {
                        worst = temp;
                    }
                }
            }
            return Math.Round(worst, 2);
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