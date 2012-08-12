using System;
using Microsoft.Phone.Controls;

namespace LiquidGold
{
    public partial class ViewFill : PhoneApplicationPage
    {
        public ViewFill()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            string Name;
            string Odo;
            string Quan;
            string Cost;
            string Date;
            string Notes;

            if (NavigationContext.QueryString.TryGetValue("Name", out Name))
            {
                NavigationContext.QueryString.TryGetValue("Odo", out Odo);
                NavigationContext.QueryString.TryGetValue("Quantity", out Quan);
                NavigationContext.QueryString.TryGetValue("Cost", out Cost);
                NavigationContext.QueryString.TryGetValue("Date", out Date);
                NavigationContext.QueryString.TryGetValue("Notes", out Notes);

                PageTitle.Text = Name;
                Odo_txt.Text = Odo;
                Quantity_txt.Text = Quan;
                Cost_txt.Text = Cost;
                Date_txt.Text = Date;
                Notes_txt.Text = Notes;
            }
            base.OnNavigatedTo(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditFillBtn_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("//AddFill.xaml?Name=" + PageTitle.Text + "&IsEdit=1&Index=0", UriKind.Relative));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteFillBtn_Click(object sender, EventArgs e)
        {

        }
    }
}