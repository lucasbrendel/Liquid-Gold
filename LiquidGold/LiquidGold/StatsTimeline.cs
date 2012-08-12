using System;
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
        /// <param name="fill"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateWorstMileage()
        {
            ObservableCollection<Axis> WorstMileage = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            double mileage;

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                mileage = fill.Distance / fill.Quantity;
                points.Add(mileage);
                point.Y = points.Min();

                WorstMileage.Add(point);
            }

            return WorstMileage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateBestMileage()
        {
            ObservableCollection<Axis> BestMileage = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            double mileage;

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                mileage = fill.Distance / fill.Quantity;
                points.Add(mileage);
                point.Y = points.Max();

                BestMileage.Add(point);
            }

            return BestMileage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateAverageDistance()
        {
            ObservableCollection<Axis> AvgDistance = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Distance);
                point.Y = points.Average();

                AvgDistance.Add(point);
            }

            return AvgDistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateShortestDistance()
        {
            ObservableCollection<Axis> MinDistance = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Distance);
                point.Y = points.Min();

                MinDistance.Add(point);
            }

            return MinDistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateLongestDistance()
        {
            ObservableCollection<Axis> MaxDistance = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Distance);
                point.Y = points.Max();

                MaxDistance.Add(point);
            }

            return MaxDistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateTotalDistance()
        {
            ObservableCollection<Axis> TotalDistance = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Distance);
                point.Y = points.Sum();

                TotalDistance.Add(point);
            }

            return TotalDistance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateAverageQuantity()
        {
            ObservableCollection<Axis> Quantity = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Quantity);
                point.Y = points.Average();

                Quantity.Add(point);
            }

            return Quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateSmallestQuantity()
        {
            ObservableCollection<Axis> Quantity = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Quantity);
                point.Y = points.Min();

                Quantity.Add(point);
            }

            return Quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateLargestQuantity()
        {
            ObservableCollection<Axis> Quantity = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Quantity);
                point.Y = points.Max();

                Quantity.Add(point);
            }

            return Quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateTotalQuantity()
        {
            ObservableCollection<Axis> Quantity = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Quantity);
                point.Y = points.Sum();

                Quantity.Add(point);
            }

            return Quantity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateAverageTotalCost()
        {
            ObservableCollection<Axis> TotalCost = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.TotalCost);
                point.Y = points.Average();

                TotalCost.Add(point);
            }

            return TotalCost;
        }

        public ObservableCollection<Axis> CalculateSmallestTotalCost()
        {
            ObservableCollection<Axis> TotalCost = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.TotalCost);
                point.Y = points.Min();

                TotalCost.Add(point);
            }

            return TotalCost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateLargestTotalCost()
        {
            ObservableCollection<Axis> TotalCost = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.TotalCost);
                point.Y = points.Max();

                TotalCost.Add(point);
            }

            return TotalCost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateTotalTotalCost()
        {
            ObservableCollection<Axis> TotalCost = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.TotalCost);
                point.Y = points.Sum();

                TotalCost.Add(point);
            }

            return TotalCost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateAverageCostPerGallon()
        {
            ObservableCollection<Axis> CostGallon = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Cost);
                point.Y = points.Average();

                CostGallon.Add(point);
            }

            return CostGallon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateSmallestCostPerGallon()
        {
            ObservableCollection<Axis> CostGallon = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Cost);
                point.Y = points.Min();

                CostGallon.Add(point);
            }

            return CostGallon;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Axis> CalculateLargestCostPerGallon()
        {
            ObservableCollection<Axis> CostGallon = new ObservableCollection<Axis>();
            ObservableCollection<double> points = new ObservableCollection<double>();

            foreach (ViewModel.FillUp fill in _fills)
            {
                Axis point = new Axis();
                point.X = DateTime.Parse(fill.Date);
                points.Add(fill.Cost);
                point.Y = points.Max();

                CostGallon.Add(point);
            }

            return CostGallon;
        }
    }
}
