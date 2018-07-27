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
    /// Interaction logic for AddDiagnosis.xaml
    /// </summary>
    public partial class AddDiagnosis : Page
    {
        public AddDiagnosis()
        {
            InitializeComponent();
        }

        model.ContextDb db = new model.ContextDb();
        model.diagnosis diagnosis = new model.diagnosis();

        private void SubmitAddDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            if(DiagnosisName.Text != "")
            {
                try
                {
                    var haveDiagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisName.Text);

                    if (haveDiagnosis == null)
                    {

                        NavigationService.Navigate(new Uri("Diagnosis.xaml", UriKind.Relative));

                        diagnosis.name = DiagnosisName.Text.Trim();
                        diagnosis.status = true;
                        diagnosis.created = DateTime.Now;
                        db.diagnosis.Add(diagnosis);
                        DiagnosisName.Clear();
                        db.SaveChanges();

                        MessageBox.Show("Diagnosis Save Successfully.");
                    }
                    else
                    {
                        MessageBox.Show("The Diagnosis already exist.", "Already Exit");
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
            else
            {
                MessageBox.Show("Diagnosis name is required.", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                                   
        }

        private void CancelAddDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Diagnosis.xaml", UriKind.Relative));
        }       
    }
}
