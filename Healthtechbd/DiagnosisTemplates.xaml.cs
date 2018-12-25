using Healthtechbd.Model.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
using WpfChosenControl;
using WpfChosenControl.model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for DiagnosisTemplates.xaml
    /// </summary>
    public partial class DiagnosisTemplates : Page
    {
        public DiagnosisTemplates()
        {
            InitializeComponent();
            loadDiagnosisTemplates();
            //GetOnlineDiagnosisTemplates();
            GetLocalDiagnosisTemplates();
        }

        contextd_db db = new contextd_db();        
        diagnosis_template diagnosis_template = new diagnosis_template();
        diagnosis diagnosis = new diagnosis();
        medicine medicine = new medicine();
        test test = new test();
        diagnosis_medicine diagnosis_medicine = new diagnosis_medicine();
        diagnosis_test diagnosis_test = new diagnosis_test();

        void loadDiagnosisTemplates()
        {
            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.doctor_id == MainWindow.Session.doctorId)
                    .OrderByDescending(x => x.created)
                    .Take(40)
                    .ToList();

                dataGridDiagnosisTemplates.ItemsSource = diagnosis_templates;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAddDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddDiagnosisTemplate.xaml", UriKind.Relative));

            MainWindow.Session.editRecordId = 0;
            MedicineChosenControl.selectedIds.Clear();
            TestChosenControl.selectedIds.Clear();
        }

        private void btnDeleteDiagnosisTemplateRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int diagnosisTemplateId = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;

                try
                {
                    //Remove Diagnosis Template
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosisTemplateId);
                    db.diagnosis_templates.Remove(diagnosis_template);

                    //Remove Diagnosis_Tests
                    var diagnosis_tests = db.diagnosis_tests.Where(x => x.diagnosis_id == diagnosisTemplateId).ToList();
                    db.diagnosis_tests.RemoveRange(diagnosis_tests);

                    //Remove Diagnosis_Medicines
                    var diagnosis_medicines = db.diagnosis_medicines.Where(x => x.diagnosis_id == diagnosisTemplateId).ToList();
                    db.diagnosis_medicines.RemoveRange(diagnosis_medicines);

                    db.SaveChanges();
                    loadDiagnosisTemplates();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }               
            }
        }

        private void btnEditDiagnosisTemplateRow_Click(object sender, RoutedEventArgs e)
        {
            MedicineChosenControl.selectedIds.Clear();
            TestChosenControl.selectedIds.Clear();

            int diagnosisTemplateId = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;
            MainWindow.Session.editRecordId = diagnosisTemplateId;
            EditDiagnosisTemplate editDiagnosisTemplate = new EditDiagnosisTemplate(diagnosisTemplateId);
            NavigationService.Navigate(editDiagnosisTemplate);           
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            loadDiagnosisTemplates();
            searchField.Clear();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            search();           
        }

        public void search()
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.diagnosis.name.Contains(searchBy) && x.doctor_id == MainWindow.Session.doctorId).OrderByDescending(x => x.created).Take(10).ToList();

                dataGridDiagnosisTemplates.ItemsSource = diagnosis_templates;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SyncDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForInternetConnection() == true)
            {
                GetOnlineDiagnosisTemplates();
            }
            else
            {
                MessageBox.Show("Check your Internet connection", "Warning");
            }
        }

        public void GetOnlineDiagnosisTemplates()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("admin/diagnosis/get-online-diagnosis-templates?doctor_email=" + MainWindow.Session.doctorEmail).Result;
            if (response.IsSuccessStatusCode)
            {
                var diagnosisTemplates = response.Content.ReadAsStringAsync();
                diagnosisTemplates.Wait();
                var onlineDiagnosisTemplates = JsonConvert.DeserializeObject<List<ViewDiagnosisTemplates>>(diagnosisTemplates.Result);

                SaveOnlineDiagnosisTemplatesToLocal(onlineDiagnosisTemplates);
            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }
        
        public void SaveOnlineDiagnosisTemplatesToLocal(List<ViewDiagnosisTemplates> onlineDiagnosisTemplates)
        {  
            foreach (var onlineDiagnosisTemplate in onlineDiagnosisTemplates)
            {
                var haveDiagnosis = db.diagnosis.FirstOrDefault(x => x.name == onlineDiagnosisTemplate.diagnosis_list.name);                

                if(haveDiagnosis != null && haveDiagnosis.diagnosis_template.Count() == 0)
                {
                    //create template
                    diagnosis_template.diagnosis_list_id = haveDiagnosis.id;
                    diagnosis_template.doctor_id = MainWindow.Session.doctorId;
                    diagnosis_template.instructions = onlineDiagnosisTemplate.instructions;
                    diagnosis_template.status = true;
                    diagnosis_template.created = DateTime.Now;
                    db.diagnosis_templates.Add(diagnosis_template);

                    int diagnossisSaveResult = db.SaveChanges();  
                    
                    if(diagnossisSaveResult == 1) // True
                    {
                        if(onlineDiagnosisTemplate.medicines.Count() > 0)
                        {
                            SaveOnlineDiagnosisMedicinesToLocal(onlineDiagnosisTemplate.medicines, diagnosis_template.id);
                        }
                    }
                }                                                           
            }
        }

        public void SaveOnlineDiagnosisMedicinesToLocal(List<ViewName> online_diagnosis_medicines, int online_diagnosis_template_id)
        {
            foreach (var online_diagnosis_medicine in online_diagnosis_medicines)
            {
                var haveMedicines = db.medicines.FirstOrDefault(x => x.name == online_diagnosis_medicine.name);

                if(haveMedicines != null)
                {
                    diagnosis_medicine.diagnosis_id = online_diagnosis_template_id; //Diagnosis Template id
                    diagnosis_medicine.medicine_id = haveMedicines.id;
                    diagnosis_medicine.status = true;
                    diagnosis_medicine.created = DateTime.Now;
                    db.diagnosis_medicines.Add(diagnosis_medicine);
                    db.SaveChanges();
                }                
            }
        }

        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void GetLocalDiagnosisTemplates()
        {
            var diagnosis_templates = db.diagnosis_templates.Where(x => x.doctor_id == MainWindow.Session.doctorId)
                    .OrderByDescending(x => x.created)
                    .Take(40)
                    .ToList();


            var json_diagnosis_templates = JsonConvert.SerializeObject(diagnosis_templates, Formatting.Indented,
            new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });


            //                       config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling
            //= Newtonsoft.Json.ReferenceLoopHandling.Serialize;
            //       config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling
            //            = Newtonsoft.Json.PreserveReferencesHandling.Objects;

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PostAsJsonAsync("admin/diagnosis/get-local-diagnosis-templates?doctor_email=" + MainWindow.Session.doctorEmail, json_diagnosis_templates).Result;
            if (response.IsSuccessStatusCode)
            {
                var diagnosisTemplates = response.Content.ReadAsStringAsync();
                diagnosisTemplates.Wait();
                var onlineDiagnosisTemplates = JsonConvert.DeserializeObject(diagnosisTemplates.Result);
            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }
    }
}
