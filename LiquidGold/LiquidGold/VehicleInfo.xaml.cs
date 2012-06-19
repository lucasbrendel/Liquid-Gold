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
            this.DataContext = this;
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
                VehicleName.Text = _name;
                var FillUpItemsInDB = from ViewModel.FillUp fills in fillUpDB.FillUpItems where fills.VehicleName == _name select fills;
                FillUpItems = new ObservableCollection<ViewModel.FillUp>(FillUpItemsInDB);
                LoadStats();
                HistoryList.ItemsSource = FillUpItems;
            }

            base.OnNavigatedTo(e);
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