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

namespace Healthtechbd.prescriptionTemplates
{
    /// <summary>
    /// Interaction logic for TemImageView.xaml
    /// </summary>
    public partial class TemImageView : Page
    {
        public TemImageView()
        {
            InitializeComponent();
            LoadTemImageView();
        }       

        void LoadTemImageView()
        {
            TemImage.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory +
                "images/prescription_templates/" + MainWindow.Session.imagePath));
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("PrescriptionTemplates.xaml", UriKind.Relative));
        }
    }    
}
