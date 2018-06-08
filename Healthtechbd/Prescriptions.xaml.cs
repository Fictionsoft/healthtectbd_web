using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for Prescriptions.xaml
    /// </summary>
    public partial class Prescriptions : Page
    {
        public Prescriptions()
        {
            InitializeComponent();
        }

        private void ButtonAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            NavigationService.Navigate(new Uri("AddPrescription.xaml", UriKind.Relative));
        }        
    }
}
