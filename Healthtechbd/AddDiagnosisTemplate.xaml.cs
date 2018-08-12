using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// Interaction logic for AddDiagnosisTemplate.xaml
    /// </summary>
    public partial class AddDiagnosisTemplate : Page
    {
        public AddDiagnosisTemplate()
        {
            InitializeComponent();           
            LoadDiagnosisCombobox();

            DiagnosisComboBox.AddHandler(System.Windows.Controls.Primitives.TextBoxBase.TextChangedEvent,
                     new System.Windows.Controls.TextChangedEventHandler(DiagnosisComboBox_TextChanged));
        }

        model.ContextDb db = new model.ContextDb();
        model.diagnosis diagnosis = new model.diagnosis();
        model.diagnosis_templates diagnosis_template = new model.diagnosis_templates();

        private void SubmitAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            if(DiagnosisComboBox.SelectedItem != "Type here..." && Instruction.Text != "")
            {
                try
                {
                    diagnosis = db.diagnosis.FirstOrDefault(x => x.name == DiagnosisComboBox.SelectedItem.ToString());

                    var haveDiagnosisTemplate = db.diagnosis_templates.FirstOrDefault(x => x.diagnosis_id == diagnosis.id);

                    if (haveDiagnosisTemplate == null)
                    {
                        NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));

                        diagnosis_template.diagnosis_id = diagnosis.id;
                        diagnosis_template.instructions = Instruction.Text;
                        diagnosis_template.status = true;
                        diagnosis_template.created = DateTime.Now;
                        db.diagnosis_templates.Add(diagnosis_template);                        
                        db.SaveChanges();

                        MessageBox.Show("Diagnosis Tempalte has been saved", "Save");                       
                    }
                    else
                    {
                        MessageBox.Show("The Diagnosis Template already exist", "Already Exit");
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }               
            }
            else
            {
                MessageBox.Show("Please fill in the required fields", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        void LoadDiagnosisCombobox()
        {
            try
            {
                var diagnosis = db.diagnosis.OrderByDescending(x => x.created).ToList();

                foreach (var item in diagnosis)
                {
                    DiagnosisComboBox.Items.Add(item.name);
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void CancelAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("DiagnosisTemplates.xaml", UriKind.Relative));
        }

        //private void DiagnosisComboBox_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    if (DiagnosisComboBox.Text == "Type here...")
        //    {
        //        DiagnosisComboBox.Text = "";
        //    }
        //}       

        //private void DiagnosisComboBox_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    if (DiagnosisComboBox.Text == "")
        //    {
        //        DiagnosisComboBox.Text = "Type here...";
        //    }
        //}             

        //private void DiagnosisComboBox_Loaded(object sender, RoutedEventArgs e)
        //{
        //    // ... A List.
        //    List<string> data = new List<string>();
        //    data.Add("Book");
        //    data.Add("Computer");
        //    data.Add("Chair");
        //    data.Add("Mug");

        //    // ... Get the ComboBox reference.
        //    var comboBox = sender as ComboBox;

        //    // ... Assign the ItemsSource to the List.
        //    comboBox.ItemsSource = data;

        //    // ... Make the first item selected.
        //    comboBox.SelectedIndex = 0;
        //}

        void DiagnosisComboBox_TextChanged(object sender, RoutedEventArgs e)
        {
            DiagnosisComboBox.IsDropDownOpen = true;

            ComboBox obj = sender as ComboBox;

            var item = obj.Text;

            var diagnosis = db.diagnosis.Where(x => x.name.Contains(item)).OrderByDescending(x => x.created).ToList();
            DiagnosisComboBox.Items.Clear();

            foreach (var data in diagnosis)
            {
                DiagnosisComboBox.Items.Add(data.name);
            }

            if (diagnosis.Count == 0)
            {
                DiagnosisComboBox.Items.Add("No results mached with " + item);
            }
        }

        private void DiagnosisComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            DiagnosisComboBox.IsDropDownOpen = true;
        }
    }
}
