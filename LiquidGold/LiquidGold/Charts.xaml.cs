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
using Telerik;
using Telerik.Charting;

namespace LiquidGold
{
    public partial class Charts : PhoneApplicationPage
    {
        private StatsTimeline Timeline;
        string VehicleName;

        public Charts()
        {
            InitializeComponent();
            ChartTooltipBehavior behavior = (ChartTooltipBehavior)this.Chart.Behaviors[0];
            behavior.ContextNeeded += new EventHandler<TooltipContextNeededEventArgs>(behavior_ContextNeeded);
        }

        void behavior_ContextNeeded(object sender, TooltipContextNeededEventArgs e)
        {
            e.Context = this.CreateContext(e.DefaultContext);
        }

        private Axis CreateContext(ChartDataContext chartDataContext)
        {
            CategoricalDataPoint DataPoint = (CategoricalDataPoint)chartDataContext.ClosestDataPoint.DataPoint;

            return new Axis()
            {
                X = (DateTime)DataPoint.Category,
                Y = (double)DataPoint.Value
            };
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
            if (ChartList.SelectedIndex != -1)
            {
                CalculateStat(ChartList.SelectedIndex);
            }
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
                        Statistic = Timeline.CalculateShortestDistance();
                        break;
                    case 5:
                        Statistic = Timeline.CalculateLongestDistance();
                        break;
                    case 6:
                        Statistic = Timeline.CalculateTotalDistance();
                        break;
                    case 7:
                        Statistic = Timeline.CalculateAverageQuantity();
                        break;
                    case 8:
                        Statistic = Timeline.CalculateSmallestQuantity();
                        break;
                    case 9:
                        Statistic = Timeline.CalculateLargestQuantity();
                        break;
                    case 10:
                        Statistic = Timeline.CalculateTotalQuantity();
                        break;
                    case 11:
                        Statistic = Timeline.CalculateAverageTotalCost();
                        break;
                    case 12:
                        Statistic = Timeline.CalculateSmallestTotalCost();
                        break;
                    case 13:
                        Statistic = Timeline.CalculateLargestTotalCost();
                        break;
                    case 14:
                        Statistic = Timeline.CalculateTotalTotalCost();
                        break;
                    case 15:
                        Statistic = Timeline.CalculateAverageCostPerGallon();
                        break;
                    case 16:
                        Statistic = Timeline.CalculateSmallestCostPerGallon();
                        break;
                    case 17:
                        Statistic = Timeline.CalculateLargestCostPerGallon();
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
            LineSeries series = ((LineSeries)Chart.Series[0]);
            series.CategoryBinding = new PropertyNameDataPointBinding() { PropertyName = "X" };
            series.ValueBinding = new GenericDataPointBinding<Axis, double>() { ValueSelector = Axis => Axis.Y };
            series.ItemsSource = Statistic;

            //Chart.Series[0].ItemsSource = Statistic;
        }
    }
}