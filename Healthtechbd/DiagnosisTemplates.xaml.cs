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
        }

        public static int offline_total;
        public static int offline_success;
        public static int offline_duplicate;
        public static int online_total;
        public static int online_success;
        public static int online_duplicate;
        public static List<long> will_sync_true_diagnosis_template_ids = new List<long>();

        contextd_db db = new contextd_db();
        diagnosis_template diagnosis_template = new diagnosis_template();
        diagnosis diagnosis = new diagnosis();
        medicine medicine = new medicine();
        test test = new test();
        diagnosis_medicine diagnosis_medicine = new diagnosis_medicine();
        diagnosis_test diagnosis_test = new diagnosis_test();

        AddDiagnosisTemplate addDiagnosisTemplate = new AddDiagnosisTemplate();

        void loadDiagnosisTemplates()
        {
            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.doctor_id == MainWindow.Session.doctor_id)
                    .OrderByDescending(x => x.id)
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

            MainWindow.Session.edit_record_id = 0;
            MedicineChosenControl.selectedIds.Clear();
            TestChosenControl.selectedIds.Clear();
        }

        private void btnDeleteDiagnosisTemplateRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int diagnosis_template_id = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;

                try
                {
                    //Remove Diagnosis Template
                    diagnosis_template = db.diagnosis_templates.FirstOrDefault(x => x.id == diagnosis_template_id);
                    db.diagnosis_templates.Remove(diagnosis_template);

                    //Remove Diagnosis_Tests
                    var diagnosis_tests = db.diagnosis_tests.Where(x => x.diagnosis_id == diagnosis_template_id).ToList();
                    db.diagnosis_tests.RemoveRange(diagnosis_tests);

                    //Remove Diagnosis_Medicines
                    var diagnosis_medicines = db.diagnosis_medicines.Where(x => x.diagnosis_id == diagnosis_template_id).ToList();
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
            ClearChosenControlSelectedIds();

            int diagnosis_template_id = (dataGridDiagnosisTemplates.SelectedItem as diagnosis_template).id;
            MainWindow.Session.edit_record_id = diagnosis_template_id;
            EditDiagnosisTemplate editDiagnosisTemplate = new EditDiagnosisTemplate(diagnosis_template_id);
            NavigationService.Navigate(editDiagnosisTemplate);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            loadDiagnosisTemplates();
            searchField.Clear();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var diagnosis_templates = db.diagnosis_templates.Where(x => x.diagnosis.name.Contains(searchBy) && x.doctor_id == MainWindow.Session.doctor_id).OrderByDescending(x => x.id).Take(10).ToList();

                dataGridDiagnosisTemplates.ItemsSource = diagnosis_templates;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SyncDiagnosisTemplate_Click(object sender, RoutedEventArgs e)
        {
            ClearChosenControlSelectedIds();

            if (MainWindow.Internet.CheckForInternetConnection() == true)
            {
                GetonlineDiagnosisTemplates();
                GetLocalDiagnosisTemplates();
                loadDiagnosisTemplates();
                MessageBox.Show("Online to offline: \n Total : " + offline_total + "\n Success : " + offline_success + "\n Duplicate : " + offline_duplicate
                                 + "\n \n Offline to online: \n Total : " + online_total + "\n Success : " + online_success + "\n Duplicate : " + online_duplicate,
                                 "Diagnosis templates sync report", MessageBoxButton.OK);

            }
            else
            {
                MessageBox.Show("Check your internet connection, Please try again", "Warning");
            }

            offline_total = offline_success = offline_duplicate = online_total = online_success = online_duplicate = 0; // Clear Value
            will_sync_true_diagnosis_template_ids.Clear(); // Clear Value
        }

        public void GetonlineDiagnosisTemplates()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(MainWindow.Session.api_base_url);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync("admin/diagnosis/get-online-diagnosis-templates?doctor_email=" + MainWindow.Session.doctor_email).Result;
                if (response.IsSuccessStatusCode)
                {
                    var diagnosisTemplates = response.Content.ReadAsStringAsync();
                    diagnosisTemplates.Wait();
                    var online_diagnosis_templates = JsonConvert.DeserializeObject<List<ViewDiagnosisTemplates>>(diagnosisTemplates.Result);

                    //count total sync diagnosis templates from online
                    offline_total = online_diagnosis_templates.Count();

                    if (offline_total > 0)
                    {
                        if (SaveOnlineDiagnosisTemplatesToLocal(online_diagnosis_templates))
                        {
                            HttpResponseMessage change_is_sync_response = client.PostAsJsonAsync("admin/diagnosis/get-online-diagnosis-templates", will_sync_true_diagnosis_template_ids).Result;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        public bool SaveOnlineDiagnosisTemplatesToLocal(List<ViewDiagnosisTemplates> online_diagnosis_templates)
        {
            foreach (var online_diagnosis_template in online_diagnosis_templates)
            {
                var have_diagnosis = db.diagnosis.FirstOrDefault(x => x.name == online_diagnosis_template.diagnosis_list.name);

                if(have_diagnosis != null)
                {
                    bool have_template = false;

                    //Duplicate
                    if (have_diagnosis.diagnosis_template.Count() > 0 && have_diagnosis.diagnosis_template.FirstOrDefault(x => x.doctor_id == MainWindow.Session.doctor_id) != null)
                    {
                        have_template = true;
                    }

                    if (!have_template)
                    {
                        will_sync_true_diagnosis_template_ids.Add(online_diagnosis_template.id);

                        //Create diagnosis template
                        int diagnosis_template_id = addDiagnosisTemplate.CreateDiagnosisTemplate(have_diagnosis.id, online_diagnosis_template.instructions, 1);

                        if (diagnosis_template_id > 0)
                        {
                            online_diagnosis_template.id = diagnosis_template_id;
                            online_diagnosis_template.will_save = true;
                        }

                        //count success sync diagnosis templates from online
                        offline_success++;
                    }
                    else if (have_template)
                    {
                        have_diagnosis.diagnosis_template.FirstOrDefault().is_sync = 1;
                        db.SaveChanges();

                        will_sync_true_diagnosis_template_ids.Add(online_diagnosis_template.id);
                        offline_duplicate++;
                    }
                }               
            }

            foreach (var online_diagnosis_template in online_diagnosis_templates)
            {
                if (online_diagnosis_template.will_save == true)
                {
                    if (online_diagnosis_template.medicines.Count() > 0)
                    {
                        SaveOnlineDiagnosisMedicinesToLocal(online_diagnosis_template.medicines, online_diagnosis_template.id);
                    }

                    if (online_diagnosis_template.tests.Count() > 0)
                    {
                        SaveOnlineDiagnosisTestsToLocal(online_diagnosis_template.tests, online_diagnosis_template.id);
                    }
                }
            }

            if (will_sync_true_diagnosis_template_ids.Count() > 0)
            {
                return true;
            }
            return false;
        }

        public void SaveOnlineDiagnosisMedicinesToLocal(List<ViewName> online_diagnosis_medicines, int online_diagnosis_template_id)
        {
            foreach (var online_diagnosis_medicine in online_diagnosis_medicines)
            {
                var have_medicine = db.medicines.FirstOrDefault(x => x.name == online_diagnosis_medicine.name);

                if (have_medicine != null) // have 
                {
                    MedicineChosenControl.selectedIds.Add(have_medicine.id);
                }
            }
            addDiagnosisTemplate.CreateDiagnosisMedicine(online_diagnosis_template_id);
            MedicineChosenControl.selectedIds.Clear();
        }

        public void SaveOnlineDiagnosisTestsToLocal(List<ViewName> online_diagnosis_tests, int online_diagnosis_template_id)
        {
            foreach (var online_diagnosis_test in online_diagnosis_tests)
            {
                var have_test = db.tests.FirstOrDefault(x => x.name == online_diagnosis_test.name);

                if (have_test != null) // have
                {
                    TestChosenControl.selectedIds.Add(have_test.id);
                }
            }
            addDiagnosisTemplate.CreateDiagnosisTest(online_diagnosis_template_id);
            TestChosenControl.selectedIds.Clear();
        }

        public async void GetLocalDiagnosisTemplates()
        {
            try
            {
                var diagnosis_templates = db.diagnosis_templates
                    .Where(x => x.doctor_id == MainWindow.Session.doctor_id && x.is_sync == 0)
                    .Take(100)
                    .ToList();

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(MainWindow.Session.api_base_url);

                //count total sync Diagnosis Templates from offline
                online_total = diagnosis_templates.Count();

                if (online_total > 0)
                {
                    var json_diagnosis_templates = JsonConvert.SerializeObject(diagnosis_templates, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });

                    HttpResponseMessage response = client.PostAsJsonAsync("admin/diagnosis/get-local-diagnosis-templates?doctor_email=" + MainWindow.Session.doctor_email, json_diagnosis_templates).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var response_local_diagnosis_template_save_to_online = response.Content.ReadAsStringAsync();
                        response_local_diagnosis_template_save_to_online.Wait();
                        var online_response = JsonConvert.DeserializeObject<DiagnosisTemplateSucessMessage>(response_local_diagnosis_template_save_to_online.Result);

                        if (online_response.status == "success")
                        {
                            online_success = online_response.online_success;
                            online_duplicate = online_response.online_duplicate;

                            ChangeIsSyncLocalDiagnosisTemplates(online_response.will_sync_ids);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
                    }
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        public void ChangeIsSyncLocalDiagnosisTemplates(List<long> will_sync_ids)
        {
            foreach (var will_sync_id in will_sync_ids)
            {
                var diagnosis = db.diagnosis_templates.FirstOrDefault(x => x.id == will_sync_id);
                diagnosis.is_sync = 1;
                db.SaveChanges();
            }
        }

        public static void ClearChosenControlSelectedIds()
        {
            MedicineChosenControl.selectedIds.Clear();
            TestChosenControl.selectedIds.Clear();
        }
    }
}
