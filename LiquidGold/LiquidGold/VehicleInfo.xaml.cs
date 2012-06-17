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

namespace LiquidGold
{
    public partial class VehicleInfo : PhoneApplicationPage
    {
        public VehicleInfo()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            string _name;
            if (NavigationContext.QueryString.TryGetValue("Name", out _name))
            {
                VehicleName.Text = _name;
            }
        }
    }
}