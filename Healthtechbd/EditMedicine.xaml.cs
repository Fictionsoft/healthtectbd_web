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
    /// Interaction logic for EditMedicine.xaml
    /// </summary>
    public partial class EditMedicine : Page
    {
        public EditMedicine()
        {
            InitializeComponent();
        }

        model.ContextDb db = new model.ContextDb();
        model.medicine medicine = new model.medicine();

        private void SubmitUpdateMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (MedicineName.Text != "")
            {
                int medicineId = int.Parse(MedicineId.Text);

                try
                {
                    var haveMedicine = db.medicines.FirstOrDefault(x => x.name == MedicineName.Text && x.id != medicineId);

                    if (haveMedicine == null)
                    {
                        medicine = db.medicines.FirstOrDefault(x => x.id == medicineId);
                        medicine.name = MedicineName.Text.Trim();
                        db.SaveChanges();

                        NavigationService.Navigate(new Uri("Medicines.xaml", UriKind.Relative));
                        MessageBox.Show("Update Successfully", "Success");
                    }
                    else
                    {
                        MessageBox.Show("The medicine already exit", "Alreasy Exit");
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

        private void CancelEditMedicine_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Medicines.xaml", UriKind.Relative));
        }

        public EditMedicine(int id) : this()
        {
            MedicineId.Text = id.ToString();

            try
            {
                medicine = db.medicines.FirstOrDefault(x => x.id == id);
                MedicineName.Text = medicine.name;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }
    }
}
