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
    /// Interaction logic for EditDiagnosis.xaml
    /// </summary>
    public partial class EditDiagnosis : Page
    {
        public EditDiagnosis()
        {
            InitializeComponent();
        }

        model.ContextDb db = new model.ContextDb();
        model.diagnosis diagnosis = new model.diagnosis();

        public EditDiagnosis(int id) : this()
        {
            DiagnosisId.Text = id.ToString();

            try
            {
                diagnosis = db.diagnosis.FirstOrDefault(x => x.id == id);
                DiagnosisName.Text = diagnosis.name;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void CancelEditDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Diagnosis.xaml", UriKind.Relative));
        }

        private void SubmitUpdateDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            if(DiagnosisName.Text != "")
            {                
                int diagnosisId = int.Parse(DiagnosisId.Text);

                try
                {
                    var haveDiagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisName.Text && x.id != diagnosisId);

                    if (haveDiagnosis == null)
                    {
                        diagnosis = db.diagnosis.FirstOrDefault(x => x.id == diagnosisId);
                        diagnosis.name = DiagnosisName.Text.Trim();
                        db.SaveChanges();

                        NavigationService.Navigate(new Uri("Diagnosis.xaml", UriKind.Relative));
                        MessageBox.Show("Update Successfully");
                    }
                    else
                    {
                        MessageBox.Show("The Diagnosis already exist", "Already Exit");
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                               
            }
            else
            {
                MessageBox.Show("Diagnosis name is required", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }       
    }
}
