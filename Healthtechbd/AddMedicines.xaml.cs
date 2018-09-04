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
using Healthtechbd.model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for AddMedicines.xaml
    /// </summary>
    public partial class AddMedicines : Page
    {
        public AddMedicines()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        medicine medicine = new medicine();

        private void SubmitAddMedicine_Click(object sender, RoutedEventArgs e)
        {
            if(MedicineName.Text != "")
            {
                try
                {
                    var haveMedicine = db.medicines.FirstOrDefault(x => x.name == MedicineName.Text);

                    if (haveMedicine == null)
                    {
                        NavigationService.Navigate(new Uri("Medicines.xaml", UriKind.Relative));

                        medicine.name = MedicineName.Text.Trim();
                        medicine.status = true;
                        medicine.created = DateTime.Now;
                        db.medicines.Add(medicine);
                        MedicineName.Clear();
                        db.SaveChanges();
                        MessageBox.Show("Medicine Save Successfully");
                    }
                    else
                    {
                        MessageBox.Show("The Medicine already exist", "Already Exit");
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
            else
            {
                MessageBox.Show("Medicine name is required", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelAddMedicine_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Medicines.xaml", UriKind.Relative));
        }
    }
}
