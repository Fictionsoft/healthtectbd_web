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
using WpfChosenControl.model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for Medicines.xaml
    /// </summary>
    public partial class Medicines : Page
    {
        public Medicines()
        {
            InitializeComponent();

            loadMedicines();
        }

        contextd_db db = new contextd_db();
        medicine medicine = new medicine();

        void loadMedicines()
        {
            try
            {
                var medicines = db.medicines.OrderByDescending(x => x.created).Take(10).ToList();
                dataGridMedicines.ItemsSource = medicines;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void ButtonAddMedicine_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddMedicines.xaml", UriKind.Relative));
        }

        private void btnDeleteMedicineRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int medicineId = (dataGridMedicines.SelectedItem as medicine).id;

                try
                {
                    medicine = db.medicines.FirstOrDefault(x => x.id == medicineId);

                    db.medicines.Remove(medicine);
                    db.SaveChanges();
                    loadMedicines();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }               
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
           
            string searchBy = searchField.Text.ToString();

            try
            {
                var medicines = db.medicines.Where(x => x.name.Trim().Contains(searchBy)).OrderByDescending(x => x.created).Take(10).ToList();

                if (medicines.Count == 0)
                {
                    MessageBox.Show("Medicine not found", "Warning");
                }
                else
                {
                    dataGridMedicines.ItemsSource = medicines;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                                 
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {            
            searchField.Clear();
            loadMedicines();            
        }

        private void btnEditTestRow_Click(object sender, RoutedEventArgs e)
        {
            int medicineId = (dataGridMedicines.SelectedItem as medicine).id;
            EditMedicine editMedicine = new EditMedicine(medicineId);
            NavigationService.Navigate(editMedicine);
        }
    }
}
