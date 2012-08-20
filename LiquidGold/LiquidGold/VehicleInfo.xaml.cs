using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;

namespace LiquidGold
{
    public partial class VehicleInfo : PhoneApplicationPage, INotifyPropertyChanged
    {        
        private ViewModel.FillUpDataContext fillUpDB;

        private ViewModel.VehicleDataContext vehicleDB;

        private ViewModel.Vehicle CurrentVehicle;

        private bool _delete;

        private int Index;

        private ObservableCollection<Stats> StatList = new ObservableCollection<Stats>();

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
        /// Constructor
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
        /// Override of navigation to page
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
        /// Populates all visual data fields
        /// </summary>
        private void FillInfo()
        {
            VehicleName.Text = CurrentVehicle.Name;
            VehicleMake.Text = CurrentVehicle.Make;
            VehicleModel.Text = CurrentVehicle.Model;
            AverageMileage.Text = AvgMileage().ToString();
            EntryCount.Text = FillUpItems.Count.ToString();
            if (FillUpItems.Count > 0 && Gauge.MaxValue > BestMileage())
            {
                Gauge.MaxValue = Math.Ceiling(AvgMileage() / 10) * 10;
            }
            ValueIndicator.Value = AvgMileage();
            DaysCount.Text = (DateTime.Parse(FillUpItems.Max(f => f.Date)) - DateTime.Parse(FillUpItems.Min(f => f.Date))).Days.ToString();
        }

        /// <summary>
        /// Override of navigation from page
        /// </summary>
        /// <param name="e">Navigation event arguments</param>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            if (_delete)
            {
                fillUpDB.SubmitChanges();
                vehicleDB.SubmitChanges();
            }
        }

        /// <summary>
        /// Load all statistics
        /// </summary>
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
            StatList.Add(new Stats { Name = "Avg. $/gallon", Value = AvgCost() });
            StatList.Add(new Stats { Name = "Max $/gallon", Value = MaxCost() });
            StatList.Add(new Stats { Name = "Min $/gallon", Value = MinCost() });

            StatsList.ItemsSource = StatList;
        }

        /// <summary>
        /// Calculate distance for each fill
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
            if (_fillUpItems.Count > 0)
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
            if (_fillUpItems.Count > 0)
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
        /// Event for a changed property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify all listeners of changed property
        /// </summary>
        /// <param name="propertyName">Name of changed property</param>
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
        /// Event to navigate to settings page
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void settings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//Settings.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Event to add a new fill
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void addFillBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//AddFill.xaml?Name=" + VehicleName.Text.ToString(), UriKind.Relative));
        }

        /// <summary>
        /// Event to delete vehicle and all fills
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
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

        /// <summary>
        /// Event when selection of history fills changes
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void HistoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.FillUp fill = (ViewModel.FillUp)HistoryList.SelectedItem;

            try
            {
                NavigationService.Navigate(new Uri("//ViewFill.xaml?Name=" + VehicleName.Text + "&Odo=" + fill.Odometer.ToString() +
                                                    "&Date=" + fill.Date.ToString() + "&Cost=" + fill.Cost.ToString() +
                                                    "&Notes=" + fill.Notes.ToString() + "&Quantity=" + fill.Quantity.ToString() + 
                                                    "&Lat=" + fill.Latitude.ToString() + "&Lon=" + fill.Longitude.ToString() + 
                                                    "&Index=" + HistoryList.SelectedIndex.ToString(), UriKind.Relative));
            }
            catch (NullReferenceException) { }
        }

        /// <summary>
        /// Event when a stat item is selected
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void StatsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {       
            NavigationService.Navigate(new Uri("//Charts.xaml?Name=" + CurrentVehicle.Name + "&Index=" + StatsList.SelectedIndex, UriKind.Relative));
        }

        /// <summary>
        /// Event when back key is pressed
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Event to pin tile to start screen
        /// </summary>
        /// <param name="sender">Object of event sender</param>
        /// <param name="e"> Event argument</param>
        private void pinBtn_Click(object sender, EventArgs e)
        {
            LiveTileHelper.CreateTile(new RadExtendedTileData() { Title = VehicleName.Text.ToString(), BackVisualElement = this.AverageMileage, BackTitle = "Average Mileage" }, new Uri("/VehicleInfo.xaml?Name=" + VehicleName.Text.ToString(), UriKind.RelativeOrAbsolute));
        }

        #endregion Events

        private void ContextEdit_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = (MenuItem)sender;
            int index = HistoryList.ItemContainerGenerator.IndexFromContainer((HistoryList.ItemContainerGenerator.ContainerFromItem((sender as MenuItem).DataContext) as ListBoxItem));
            if (index != -1)
            {
                if (menu.Header.ToString().Equals("edit"))
                {
                    ViewModel.FillUp fill = (ViewModel.FillUp)HistoryList.SelectedItem;

                    try
                    {
                        NavigationService.Navigate(new Uri("//AddFill.xaml?Name=" + VehicleName.Text + "&IsEdit=1&Index=" + index, UriKind.Relative));
                    }
                    catch (NullReferenceException)
                    { }
                }
            }
        }
    }

    public class Stats
    {
        private string _name;

        private double _value;

        /// <summary>
        /// Constructor
        /// </summary>
        public Stats()
        {
            _name = String.Empty;
            _value = Double.NaN;
        }

        /// <summary>
        /// Name property of statistic
        /// </summary>
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

        /// <summary>
        /// Value property of statistic
        /// </summary>
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