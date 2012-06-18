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
    public partial class AddFill : PhoneApplicationPage
    {
        public AddFill()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string index = String.Empty;
            NavigationContext.QueryString.TryGetValue("selectedIndex", out index);

            base.OnNavigatedTo(e);
        }
    }
}