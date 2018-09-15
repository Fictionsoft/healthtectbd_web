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
    /// Interaction logic for Prescriptions.xaml
    /// </summary>
    public partial class Prescriptions : Page
    {
        public Prescriptions()
        {
            InitializeComponent();

            loadPrescriptions();
        }

        contextd_db db = new contextd_db();
        prescription prescription = new prescription();

        void loadPrescriptions()
        {
            try
            {
                var prescriptions = db.presceiptions.Where(x => x.doctor_id  == MainWindow.Session.doctorId)
                                   .OrderByDescending(x => x.created).Take(10).ToList();
                                   dataGridPrescriptions.ItemsSource = prescriptions;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            NavigationService.Navigate(new Uri("AddPrescription.xaml", UriKind.Relative));
        }

        private void btnDeletePrescriptionRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int prescriptionId = (dataGridPrescriptions.SelectedItem as prescription).id;
                try
                {
                    prescription = db.presceiptions.FirstOrDefault(x => x.id == prescriptionId);

                    db.presceiptions.Remove(prescription);
                    db.SaveChanges();
                    loadPrescriptions();
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
                var prescriptions = db.presceiptions.Where(x => x.user.first_name.Trim().Contains(searchBy)).OrderByDescending(x => x.created).ToList();

                if (prescriptions.Count == 0)
                {
                    MessageBox.Show("Prescription not found");
                }
                else
                {
                    dataGridPrescriptions.ItemsSource = prescriptions;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
