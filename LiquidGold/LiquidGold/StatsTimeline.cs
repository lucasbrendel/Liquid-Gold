using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace LiquidGold
{
    public class Axis
    {
        public Axis() { }
        public DateTime X { get; set; }
        public double Y { get; set; }
    }

    public class StatsTimeline
    {
        private ViewModel.FillUpDataContext fillUpDb;
        private ViewModel.VehicleDataContext vehicleDb;

        private ObservableCollection<ViewModel.FillUp> _fills;
        private ObservableCollection<ViewModel.Vehicle> _vehicle;

        private ViewModel.Vehicle CurrentVehicle;

        /// <summary>
        /// 
        /// </summary>
        public StatsTimeline(string VehicleName)
        {
            fillUpDb = new ViewModel.FillUpDataContext(ViewModel.FillUpDataContext.DBConnectionString);
            vehicleDb = new ViewModel.VehicleDataContext(ViewModel.VehicleDataContext.VehicleConnectionString);
            var fillItemsInDB = from ViewModel.FillUp fill in fillUpDb.FillUpItems where fill.VehicleName == VehicleName select fill;
            var vehicleItems = from ViewModel.Vehicle veh in vehicleDb.VehicleItems where veh.Name == VehicleName select veh;
            _fills = new ObservableCollection<ViewModel.FillUp>(fillItemsInDB);
            _vehicle = new ObservableCollection<ViewModel.Vehicle>(vehicleItems);
            CurrentVehicle = _vehicle[0];
            _fills.OrderByDescending(p => p.Odometer);
            _fills.Reverse();
            _fills = CalculateDistance(_fills);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateAverageMileage()
        {
            ObservableCollection<Axis> AvgMileage = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();
            
            double mileage;

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                mileage = fill.Distance / fill.Quantity;
                points.Add(mileage);
                point.Y = points.Average();

                AvgMileage.Add(point);
            }

            return AvgMileage;
        }

        private ObservableCollection<ViewModel.FillUp> CalculateDistance(ObservableCollection<ViewModel.FillUp> fill)
        {
            for (int i = 0; i < fill.Count; i++)
            {
                if (i == 0)
                {
                    fill[i].Distance = fill[i].Odometer - CurrentVehicle.InitOdometer;
                }
                else
                {
                    fill[i].Distance = fill[i].Odometer - fill[i - 1].Odometer;
                }
            }

            return fill;
        }
    }
}
