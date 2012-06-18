using System;
using System.Net;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace LiquidGold.ViewModel 
{
    [Table]
    public class Vehicle : INotifyPropertyChanged, INotifyPropertyChanging
    {
        /// <summary>
        /// 
        /// </summary>
        private string _name;

        /// <summary>
        /// 
        /// </summary>
        private string _image;

        /// <summary>
        /// 
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
        /// 
        /// </summary>
        [Column]
        public string VehImage
        {
            get { return _image; }
            set
            {
                if (value != _image)
                {
                    NotifyPropertyChanging("VehImage");
                    _image = value;
                    NotifyPropertyChanged("VehImage");
                }
            }
        }

        [Column(IsVersion = true)]
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
    public class VehicleDataContext : DataContext
    {
        /// <summary>
        /// 
        /// </summary>
        public static string VehicleConnectionString = "Data Source=isostore:/Vehicles.sdf";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionString"></param>
        public VehicleDataContext(string ConnectionString)
            : base(ConnectionString)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public Table<Vehicle> VehicleItems;
    }
}
