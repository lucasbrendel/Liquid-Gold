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
            }

            var FillUpItemsInDB = from ViewModel.FillUp fills in fillUpDB.FillUpItems where fills.VehicleName == _name select fills;

            FillUpItems = new ObservableCollection<ViewModel.FillUp>(FillUpItemsInDB);

            base.OnNavigatedTo(e);
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
}