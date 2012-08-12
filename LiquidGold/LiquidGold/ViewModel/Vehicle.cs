using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace LiquidGold.ViewModel 
{
    [Table]
    public class Vehicle : INotifyPropertyChanged, INotifyPropertyChanging
    {
        private string _name;

        private string _make;

        private string _model;

        private double _initOdo;

        /// <summary>
        /// Construcotr
        /// </summary>
        public Vehicle()
        {

        }

        /// <summary>
        /// Overloaded constructor
        /// </summary>
        /// <param name="Name">Name of the vehicle</param>
        public Vehicle(string Name)
        {
            _name = Name;
        }

        /// <summary>
        /// Name of vehicle
        /// </summary>
        [Column(CanBeNull=false, IsPrimaryKey=true)]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Manufacture make of the vehicle
        /// </summary>
        [Column(CanBeNull = false)]
        public string Make
        {
            get { return _make; }
            set
            {
                if (value != _make)
                {
                    NotifyPropertyChanging("Make");
                    _make = value;
                    NotifyPropertyChanged("Make");
                }
            }
        }

        /// <summary>
        /// Model of vehicle make
        /// </summary>
        [Column(CanBeNull = false)]
        public string Model
        {
            get { return _model; }
            set
            {
                if (value != _model)
                {
                    NotifyPropertyChanging("Model");
                    _model = value;
                    NotifyPropertyChanged("Model");
                }
            }
        }

        /// <summary>
        /// The initial odometer reading before filling
        /// </summary>
        [Column(CanBeNull=false)]
        public double InitOdometer
        {
            get { return _initOdo; }
            set
            {
                if (value != _initOdo)
                {
                    NotifyPropertyChanging("InitOdometer");
                    _initOdo = value;
                    NotifyPropertyChanged("InitOdometer");
                }
            }
        }

        /// <summary>
        /// Table value
        /// </summary>
        [Column(IsVersion = true)]
        private Binary _version;

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Property changed event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify all listeners of changed property
        /// </summary>
        /// <param name="propertyName">Name of property that changed</param>
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
        /// Property changing event
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Notify all listeners of changing property
        /// </summary>
        /// <param name="propertyName">Name of property changing</param>
        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }

    public class VehicleDataContext : DataContext
    {
        /// <summary>
        /// String to connect to data source
        /// </summary>
        public static string VehicleConnectionString = "Data Source=isostore:/Vehicles.sdf";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ConnectionString">String of source connection</param>
        public VehicleDataContext(string ConnectionString)
            : base(ConnectionString)
        {

        }

        /// <summary>
        /// Table of database values
        /// </summary>
        public Table<Vehicle> VehicleItems;
    }
}
