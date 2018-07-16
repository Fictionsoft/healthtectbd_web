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
    /// Interaction logic for Medicines.xaml
    /// </summary>
    public partial class Medicines : Page
    {
        public Medicines()
        {
            InitializeComponent();

            loadMedicines();
        }

        model.ContextDb db = new model.ContextDb();
        model.medicine medicine = new model.medicine();

        void loadMedicines()
        {
            var medicines = db.medicines.ToList();
            dataGridMedicines.ItemsSource = medicines;
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
                int medicineId = (dataGridMedicines.SelectedItem as model.medicine).id;
                medicine = db.medicines.FirstOrDefault(x => x.id == medicineId);
                //test = (from x in db.tests where x.id == test_id select x).First();

                db.medicines.Remove(medicine);
                db.SaveChanges();
                loadMedicines();
                MessageBox.Show("Delete Successfully");
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
           
            string searchBy = searchField.Text.ToString();

            var medicines = db.medicines.Where(x => x.name.Trim().StartsWith(searchBy)).ToList();
            dataGridMedicines.ItemsSource = medicines;
            
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            {
                searchField.Clear();
                loadMedicines();
            }
        }

        private void btnEditTestRow_Click(object sender, RoutedEventArgs e)
        {
            int medicineId = (dataGridMedicines.SelectedItem as model.medicine).id;
            EditMedicine editMedicine = new EditMedicine(medicineId);
            NavigationService.Navigate(editMedicine);
        }
    }
}
