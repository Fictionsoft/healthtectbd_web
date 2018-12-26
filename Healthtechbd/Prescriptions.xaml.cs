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
using WpfChosenControl;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

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
                if (MainWindow.Session.setPatientId > 0)
                {
                    var prescriptions = db.prescriptions.Where(x => x.doctor_id == MainWindow.Session.doctorId && x.user_id == MainWindow.Session.setPatientId)
                                  .OrderByDescending(x => x.created).Take(40).ToList();

                    dataGridPrescriptions.ItemsSource = prescriptions;
                }
                else
                {
                    var prescriptions = db.prescriptions.Where(x => x.doctor_id == MainWindow.Session.doctorId)
                                 .OrderByDescending(x => x.created).Take(40).ToList();                    

                    dataGridPrescriptions.ItemsSource = prescriptions;
                }              
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonAddPrescription_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Session.editRecordId = 0;
            AddPrescription.diagnosisTemplateIds.Clear();
            DiagnosisTestChosenControl.selectedIds.Clear();

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
                    prescription = db.prescriptions.FirstOrDefault(x => x.id == prescriptionId);

                    db.prescriptions.Remove(prescription);

                    //prescription diagnosis delete
                    var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescriptionId);
                    if (prescriptions_diagnosis.Count() > 0)
                    {
                        db.prescriptions_diagnosis.RemoveRange(prescriptions_diagnosis);
                        int delete_result = db.SaveChanges();
                    }

                    //prescription medicines delete
                    var prescriptions_medicines = db.prescriptions_medicines.Where(x => x.prescription_id == prescriptionId);
                    if (prescriptions_medicines.Count() > 0)
                    {
                        db.prescriptions_medicines.RemoveRange(prescriptions_medicines);
                        int delete_result = db.SaveChanges();
                    }

                    //prescription tests delete
                    var prescriptions_tests = db.prescriptions_tests.Where(x => x.prescription_id == prescriptionId);
                    if (prescriptions_tests.Count() > 0)
                    {
                        db.prescriptions_tests.RemoveRange(prescriptions_tests);
                        int delete_result = db.SaveChanges();
                    }

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
            if(searchField.Text != "")
            {
                search();
            }           
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchField.Clear();
            MainWindow.Session.setPatientId = 0;
            loadPrescriptions();
        }

        string PrescriptionTem;
        private void btnViewPrescriptionRow_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Session.editRecordId = (dataGridPrescriptions.SelectedItem as prescription).id;

            int doctorPrescriptionTemId = MainWindow.Session.doctorPrescriptionTemId;

            if (doctorPrescriptionTemId == 1)
            {
                PrescriptionTem = "DefaultTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 2)
            {
                PrescriptionTem = "StandardTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 3)
            {
                PrescriptionTem = "ClassicTemplate.xaml";
            }
            else if (doctorPrescriptionTemId == 4)
            {
                PrescriptionTem = "CustomTemplate.xaml";
            }
            else
            {
                PrescriptionTem = "GeneralTemplate.xaml";
            }

            NavigationService.Navigate(new Uri("prescriptionTemplates/" + PrescriptionTem, UriKind.Relative));
        }

        private void btnEditPrescriptionRow_Click(object sender, RoutedEventArgs e)
        {            
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;

            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            int prescriptionId = (dataGridPrescriptions.SelectedItem as prescription).id;
            MainWindow.Session.editRecordId = prescriptionId;
            EditPrescription editPrescription = new EditPrescription(prescriptionId);

            EditPrescription.diagnosisTemplateIds.Clear();
            var prescriptions_diagnosis = db.prescriptions_diagnosis.Where(x => x.prescription_id == prescriptionId).ToList();
            foreach (var prescriptions_diagnosi in prescriptions_diagnosis)
            {
                EditPrescription.diagnosisTemplateIds.Add(prescriptions_diagnosi.diagnosis_id);
            }

            NavigationService.Navigate(editPrescription);                      
        }

        public void search()
        {
            string searchBy = searchField.Text.ToString();
            List<prescription> prescriptions = new List<prescription>();
            try
            {
                if(MainWindow.Session.setPatientId > 0)
                {
                    prescriptions = db.prescriptions.Where(x => (x.user.first_name.Contains(searchBy) ||
                                                               x.user.phone.Contains(searchBy)) &&
                                                               x.user_id == MainWindow.Session.setPatientId &&
                                                               x.doctor_id == MainWindow.Session.doctorId
                                                        ).Take(40).ToList();
                }
                else
                {
                    prescriptions = db.prescriptions.Where(x => (x.user.first_name.Contains(searchBy) ||
                                                               x.user.phone.Contains(searchBy)) &&
                                                               x.doctor_id == MainWindow.Session.doctorId
                                                         ).Take(40).ToList();

                }
                dataGridPrescriptions.ItemsSource = prescriptions;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(MainWindow.Internet.CheckForInternetConnection() == true)
            {
                GetOnlinePrescriptions();
            }
            else
            {
                MessageBox.Show("Check your Internet connection", "Warning");
            }
        }

        public async void GetOnlinePrescriptions()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("admin/users/get-online-prescriptions?doctor_email=" + MainWindow.Session.doctorEmail).Result;
            if (response.IsSuccessStatusCode)
            {
                var onlinePrescriptions = response.Content.ReadAsStringAsync();
                onlinePrescriptions.Wait();
                //var onlinePatients = JsonConvert.DeserializeObject<List<ViewPatients>>(onlinePrescriptions.Result);               
            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }
    }
}
