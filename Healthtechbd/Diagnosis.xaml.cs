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
    /// Interaction logic for Diagnosis.xaml
    /// </summary>
    public partial class Diagnosis : Page
    {
        public Diagnosis()
        {
            InitializeComponent();

            loadDiagnosis();
        }

        model.ContextDb db = new model.ContextDb();
        model.diagnosis diagnosis = new model.diagnosis();

        void loadDiagnosis()
        {
            try
            {
                var diagnosis = db.diagnosis.OrderByDescending(x => x.created).ToList();
                dataGridDiagnosis.ItemsSource = diagnosis;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }           
        }

        private void ButtonAddDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddDiagnosis.xaml", UriKind.Relative));
        }

        private void btnDeleteDiagnosisRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int diagnosisId = (dataGridDiagnosis.SelectedItem as model.diagnosis).id;
                try
                {
                    diagnosis = db.diagnosis.FirstOrDefault(x => x.id == diagnosisId);

                    db.diagnosis.Remove(diagnosis);
                    db.SaveChanges();
                    loadDiagnosis();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
        }

        private void btnEditDiagnosisRow_Click(object sender, RoutedEventArgs e)
        {
            int diagnosisId = (dataGridDiagnosis.SelectedItem as model.diagnosis).id;
            EditDiagnosis editDiagnosis = new EditDiagnosis(diagnosisId);
            NavigationService.Navigate(editDiagnosis);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchField.Clear();
            loadDiagnosis();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var diagnosis = db.diagnosis.Where(x => x.name.Trim().StartsWith(searchBy)).OrderByDescending(x => x.created).ToList();

                if (diagnosis.Count == 0)
                {
                    MessageBox.Show("Diagnosis not found");
                }
                else
                {
                    dataGridDiagnosis.ItemsSource = diagnosis;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                      
        }       
    }
}
