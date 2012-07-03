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

        /// <summary>
        /// 
        /// </summary>
        private ViewModel.VehicleDataContext vehicleDB;

        /// <summary>
        /// 
        /// </summary>
        private ViewModel.Vehicle CurrentVehicle;

        /// <summary>
        /// 
        /// </summary>
        private bool _delete;

        /// <summary>
        /// 
        /// </summary>
        private int Index;

        /// <summary>
        /// 
        /// </summary>
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
                var vehicle = from ViewModel.Vehicle veh in vehicleDB.VehicleItems where veh.Name == _name select veh;
                CurrentVehicle = vehicle.First();
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
                FillInfo();
            }

            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 
        /// </summary>
        private void FillInfo()
        {
            VehicleName.Text = CurrentVehicle.Name;
            VehicleMake.Text = CurrentVehicle.Make;
            VehicleModel.Text = CurrentVehicle.Model;
            AverageMileage.Text = AvgMileage().ToString();
            EntryCount.Text = FillUpItems.Count.ToString();

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
            StatList.Add(new Stats { Name = "Avg. MPG", Value = AvgMileage() });
            StatList.Add(new Stats { Name = "Worst MPG", Value = WorstMileage() });
            StatList.Add(new Stats { Name = "Best MPG", Value = BestMileage() });
            StatList.Add(new Stats { Name = "Avg. Distance", Value = AverageDistance() });
            StatList.Add(new Stats { Name = "Smallest Distance", Value = SmallestDistance() });
            StatList.Add(new Stats { Name = "Longest Distance", Value = LargestDistance() });
            StatList.Add(new Stats { Name = "Total Distance", Value = TotalDistance() });
            StatList.Add(new Stats { Name = "Avg. Quantity", Value = AverageQuantity() });
            StatList.Add(new Stats { Name = "Smallest Quantity", Value = SmallestQuantity() });
            StatList.Add(new Stats { Name = "Largest Quantity", Value = LargestQuantity() });
            StatList.Add(new Stats { Name = "Total Quantity", Value = TotalQuantity() });
            StatList.Add(new Stats { Name = "Average Total Cost", Value = AverageTotalCost() });
            StatList.Add(new Stats { Name = "Smallest Total Cost", Value = SmallestTotalCost() });
            StatList.Add(new Stats { Name = "Largest Total Cost", Value = LargestTotalCost() });
            StatList.Add(new Stats { Name = "Overall Cost", Value = TotalCost()});
            //StatList.Add(new Stats { Name = "Cost Last Year", Value = 0.00 });
            //StatList.Add(new Stats { Name = "Cost Last Month", Value = 0.00 });
            //StatList.Add(new Stats { Name = "Avg. Yearly Cost", Value = 0.00 });
            //StatList.Add(new Stats { Name = "Avg. Monthly Cost", Value = 0.00 });
            //StatList.Add(new Stats { Name = "Avg. Cost/Distance", Value = 0.00 });
            //StatList.Add(new Stats { Name = "Max Cost/Distance", Value = 0.00 });
            //StatList.Add(new Stats { Name = "Min Cost/Distance", Value = 0.00 });
            StatList.Add(new Stats { Name = "Avg. $/gallon", Value = AvgCost() });
            StatList.Add(new Stats { Name = "Max $/gallon", Value = MaxCost() });
            StatList.Add(new Stats { Name = "Min $/gallon", Value = MinCost() });


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
                    _fillUpItems[i].Distance = _fillUpItems[i].Odometer - CurrentVehicle.InitOdometer;
                }
                else
                {
                    _fillUpItems[i].Distance = _fillUpItems[i].Odometer - _fillUpItems[i - 1].Odometer;
                }
            }
        }

        #region stats

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double AvgMileage()
        {
            double _value = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _value = TotalDistance() / TotalQuantity();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double SmallestQuantity()
        {
            double _small = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _small = _fillUpItems.Min(fillup => fillup.Quantity);
            }
            return Math.Round(_small, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double LargestQuantity()
        {
            double _largest = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _largest = _fillUpItems.Max(fillup => fillup.Quantity);
            }
            return Math.Round(_largest, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double AverageQuantity()
        {
            double _avg = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _avg = (TotalQuantity() / _fillUpItems.Count);
            }
            return Math.Round(_avg, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double MinCost()
        {
            double _cost = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _cost = _fillUpItems.Min(fillup => fillup.Cost);
            }
            return Math.Round(_cost, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double MaxCost()
        {
            double _cost = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _cost = _fillUpItems.Max(fillup => fillup.Cost);
            }
            return Math.Round(_cost, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double AvgCost()
        {
            double _cost = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _cost = _fillUpItems.Sum(fillup => fillup.Cost) / _fillUpItems.Count;
            }
            return Math.Round(_cost, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double AverageTotalCost()
        {
            double _cost = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _cost = TotalCost() / _fillUpItems.Count;
            }
            return Math.Round(_cost, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double LargestTotalCost()
        {
            double _cost = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _cost = _fillUpItems.Max(fillup => fillup.TotalCost);
            }
            return Math.Round(_cost, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double SmallestTotalCost()
        {
            double _cost = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _cost = _fillUpItems.Min(fillup => fillup.TotalCost);
            }
            return Math.Round(_cost, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double TotalCost()
        {
            double _total = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _total = _fillUpItems.Sum(fillup => fillup.TotalCost);
            }
            return Math.Round(_total, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double TotalQuantity()
        {
            double _total = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _total = _fillUpItems.Sum(fillup => fillup.Quantity);
            }
            return Math.Round(_total, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double AverageDistance()
        {
            double _distance = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _distance = TotalDistance() / _fillUpItems.Count;
            }
            return Math.Round(_distance, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double SmallestDistance()
        {
            double _distance = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _distance = _fillUpItems.Min(fillup => fillup.Distance);
            }
            return Math.Round(_distance, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double LargestDistance()
        {
            double _distance = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _distance = _fillUpItems.Max(fillup => fillup.Distance);
            }
            return Math.Round(_distance, 2);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private double TotalDistance()
        {
            double _total = 0.0;
            if (_fillUpItems.Count > 0)
            {
                _total = _fillUpItems.Sum(fillup => fillup.Distance);
            }
            return _total;
        }

        #endregion stats

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

        #region Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//Settings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addFillBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//AddFill.xaml?Name=" + VehicleName.Text.ToString(), UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #endregion Events
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