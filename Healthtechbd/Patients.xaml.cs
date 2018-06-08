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
    /// Interaction logic for Patients.xaml
    /// </summary>
    public partial class Patients : Page
    {
        public Patients()
        {
            InitializeComponent();
        }

        private void ButtonAddPatient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddPatient.xaml", UriKind.Relative));
        }
    }
}
