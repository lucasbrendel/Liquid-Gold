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
    public partial class Settings : PhoneApplicationPage
    {
        /// <summary>
        /// 
        /// </summary>
        public Settings()
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

            LocationSwitch.IsChecked = (App.Current as App).LocationAware;

            if ((App.Current as App).UserUnits == App.Units.Imperial)
            {
                ImperialRad.IsChecked = true;
            }
            else
            {
                MetricRad.IsChecked = true;
            }
        }
    }
}