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
using System.Collections;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace LiquidGold
{
    public partial class Charts : PhoneApplicationPage
    {
        private StatsTimeline Timeline;
        string VehicleName;

        public Charts()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ChartList.ItemsSource = (App.Current as App).StatisticsArray.ToList<string>();
            string _veh;
            string _index;

            if (NavigationContext.QueryString.TryGetValue("Name", out _veh))
            {
                VehicleName = _veh;
                Timeline = new StatsTimeline(VehicleName);

                NavigationContext.QueryString.TryGetValue("Index", out _index);
                ChartList.SelectedIndex = Int16.Parse(_index);
                CalculateStat(ChartList.SelectedIndex);               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChartList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateStat(ChartList.SelectedIndex);            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Index"></param>
        private void CalculateStat(int Index)
        {
            if (Timeline != null)
            {
                ObservableCollection<Axis> Statistic = new ObservableCollection<Axis>(); ;

                switch (Index)
                {
                    case 0:
                        Statistic = Timeline.CalculateAverageMileage();
                        break;
                    case 1:
                        Statistic = Timeline.CalculateWorstMileage();
                        break;
                    case 2:
                        Statistic = Timeline.CalculateBestMileage();
                        break;
                    case 3:
                        Statistic = Timeline.CalculateAverageDistance();
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 10:
                        break;
                    case 11:
                        break;
                    case 12:
                        break;
                    default:
                        break;
                }

                FillChart(Statistic); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Statistic"></param>
        private void FillChart(ObservableCollection<Axis> Statistic)
        {
            Chart.Series[0].ItemsSource = Statistic;
        }
    }
}