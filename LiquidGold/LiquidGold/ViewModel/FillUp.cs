using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;


namespace LiquidGold.ViewModel
{
    [Table]
    public class FillUp : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private string _vehicleName;

        private int _odo;

        private double _cost;

        private double _quantity;

        private string _date;

        private string _notes;

        public FillUp()
        {
            _vehicleName = String.Empty;
            _odo = 0;
            _cost = 0.0;
            _quantity = 0.0;
            _date = String.Empty;
            _notes = String.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        [Column(CanBeNull=false, IsPrimaryKey=true)] 
        public string VehicleName
        {
            get { return _vehicleName; }
            set
            {
                if (value != _vehicleName)
                {
                    NotifyPropertyChanging("VehicleName");
                    _vehicleName = value;
                    NotifyPropertyChanged("VehicleName");
                }
            }
        }

        [Column(CanBeNull=false, IsPrimaryKey=true)]
        public int Odometer
        {
            get { return _odo; }
            set
            {
                if (value != _odo)
                {
                    NotifyPropertyChanging("Odometer");
                    _odo = value;
                    NotifyPropertyChanged("Odometer");
                }
            }
        }

        [Column(CanBeNull=false)]
        public double Cost
        {
            get { return _cost; }
            set
            {
                if (value != _cost)
                {
                    NotifyPropertyChanging("Cost");
                    _cost = Math.Round(value, 2);
                    NotifyPropertyChanged("Cost");
                }
            }
        }

        [Column(CanBeNull = false)]
        public double Quantity
        {
            get { return _quantity; }
            set
            {
                if (value != _quantity)
                {
                    NotifyPropertyChanging("Quantity");
                    _quantity = value;
                    NotifyPropertyChanged("Quantity");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Column(CanBeNull = false)]
        public string Date
        {
            get { return _date; }
            set
            {
                if (value != _date)
                {
                    NotifyPropertyChanging("Date");
                    DateTime _dates = DateTime.Parse(value);
                    _date = _dates.Date.ToShortDateString();
                    NotifyPropertyChanged("Date");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Column(CanBeNull = true)]
        public string Notes
        {
            get { return _notes; }
            set
            {
                if (value != _notes)
                {
                    NotifyPropertyChanging("Notes");
                    _notes = value;
                    NotifyPropertyChanged("Notes");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double TotalCost
        {
            get { return Math.Round(_cost * _quantity, 2); }
        }

        /// <summary>
        /// 
        /// </summary>
        public double Distance
        {
            get;
            set;
        }

        [Column(IsVersion=true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

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

        #region INotifyPropertyChanging Members

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class FillUpDataContext : DataContext
    {
        /// <summary>
        /// 
        /// </summary>
        public static string DBConnectionString = "Data Source=isostore:/FillUp.sdf";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        public FillUpDataContext(string ConnectionString)
            : base(ConnectionString)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public Table<FillUp> FillUpItems;
    }
}
