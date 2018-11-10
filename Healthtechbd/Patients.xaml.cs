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
    /// Interaction logic for Patients.xaml
    /// </summary>
    public partial class Patients : Page
    {
        public Patients()
        {
            InitializeComponent();
            loadPatients();
        }

        contextd_db db = new contextd_db();
        user user = new user();

        void loadPatients()   //User = Patient
        {
            try
            {
                var users = db.users.Where(x => x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId).OrderByDescending(x => x.created).Take(40).ToList();
                dataGridPatients.ItemsSource = users; // role_id 3 = Patient 
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAddPatient_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddPatient.xaml", UriKind.Relative));
        }

        private void btnDeletePatientRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
               MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    int patientId = (dataGridPatients.SelectedItem as user).id;
                    user = db.users.FirstOrDefault(x => x.id == patientId);

                    db.users.Remove(user);
                    db.SaveChanges();
                    loadPatients();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnEditPatientRow_Click(object sender, RoutedEventArgs e)
        {
            int patientId = (dataGridPatients.SelectedItem as user).id;
            EditPatient editPatient = new EditPatient(patientId);
            NavigationService.Navigate(editPatient);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchField.Clear();
            loadPatients();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            search();
        }

        public void search()
        {
            if (searchField.Text != "")
            {
                string searchBy = searchField.Text.ToString();

                try
                {
                    var users = db.users.Where(x => (x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId) &&
                                                (x.first_name.Trim().Contains(searchBy) ||
                                                x.last_name.Trim().Contains(searchBy) ||
                                                x.phone.Trim().Contains(searchBy) ||
                                                x.email.Trim().Contains(searchBy) ||
                                                x.age.Trim().Contains(searchBy))
                                               ).Take(40).ToList();

                    dataGridPatients.ItemsSource = users;

                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void btnViewPatientPrescription_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Prescriptions.xaml", UriKind.Relative));

            MainWindow.Session.setPatientId = (dataGridPatients.SelectedItem as user).id;
        }
    }
}
