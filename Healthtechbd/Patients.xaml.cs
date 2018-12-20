using Healthtechbd.Model;
using Healthtechbd.Model.ApiModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        //From Dashboard 
        public Patients(List<user> patients) : this()
        {
            dataGridPatients.ItemsSource = patients; //Patients = Users   
        }

        contextd_db db = new contextd_db();
        user user = new user();
        prescription prescription = new prescription();

        void loadPatients() //User = Patient
        {
            try
            {               
                var patients = db.users.Where(x => x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId).OrderByDescending(x => x.created).Take(40).ToList();// role_id 3 = Patient
                dataGridPatients.ItemsSource = patients; //Patients = Users                
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
                                                (x.first_name.Contains(searchBy) ||
                                                x.last_name.Contains(searchBy) ||
                                                x.phone.Contains(searchBy) ||
                                                x.email.Contains(searchBy) ||
                                                x.age.Contains(searchBy))
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

        private void btnCreatePatientPrescription_Click(object sender, RoutedEventArgs e)
        {
            Grid sidebar = AdminPanelWindow.sidebar;
            sidebar.Visibility = Visibility.Hidden;
            AdminPanelWindow.sidebarColumnDefination.Width = new GridLength(0);

            int patientId = (dataGridPatients.SelectedItem as user).id;

            AddPrescription addPrescription = new AddPrescription(patientId);            
            NavigationService.Navigate(addPrescription);
        }

        public static int OfflineTotal;
        public static int OfflineSuccess;
        public static int OfflineDuplicate;

        public static int OnlineTotal;
        public static int OnlineSuccess;
        public static int OnlineDuplicate;

        private void ButtonSyncPatient_Click(object sender, RoutedEventArgs e)
        {
            if (CheckForInternetConnection() == true)
            {
                GetOnlinePatients();
                GetLocalPatients();

                MessageBox.Show("Patients sync Report:");

                OfflineTotal = OfflineSuccess = OfflineDuplicate = OnlineTotal = OnlineSuccess = OnlineDuplicate = 0; // Clear Value
            }
            else
            {
                MessageBox.Show("Check your Internet connection", "Warning");
            }
        }

        public async void GetOnlinePatients()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(MainWindow.Session.apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("admin/users/get-online-patients?doctor_email="+MainWindow.Session.doctorEmail).Result;
            if (response.IsSuccessStatusCode)
            {
                var patients = response.Content.ReadAsStringAsync();
                patients.Wait();
                var onlinePatients = JsonConvert.DeserializeObject<OnlinePatients>(patients.Result);

                if (SaveOnlinePatientsToLocal(onlinePatients))
                {
                    HttpResponseMessage change_is_sync_response = client.PostAsJsonAsync("admin/users/get-online-patients", onlinePatients.patients).Result;
                }
            }
            else
            {
                MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }
        }

        public bool SaveOnlinePatientsToLocal(OnlinePatients onlinePatients)
        {
            //Count Total Sync Patients From On-line
            OfflineTotal = (onlinePatients.patients.Count() > 0)?onlinePatients.patients.Count():0;

            foreach (var patient in onlinePatients.patients)
            {
                var havePatient = db.users.Where(x => x.first_name == patient.first_name && x.phone == patient.phone && x.doctor_id == MainWindow.Session.doctorId).FirstOrDefault();

                if(havePatient != null)
                {
                    havePatient.is_sync = 1;
                    db.SaveChanges();

                    //Count Duplicate Sync Patients From On-line
                    OfflineDuplicate++;
                }
                else
                {                   
                    user.doctor_id = MainWindow.Session.doctorId;
                    user.role_id = patient.role_id; // role_id 3 = Patient
                    user.first_name = patient.first_name;
                    user.phone = patient.phone;
                    user.email = patient.email;
                    user.age = patient.age;
                    user.address_line1 = patient.address_line1;
                    user.expire_date = patient.expire_date;
                    user.is_sync = 1;
                    user.created = patient.created;

                    db.users.Add(user);
                    db.SaveChanges();

                    //Count Success Sync Patients From On-line
                    OfflineSuccess++;
                }                                              
            }

            if(OnlineSuccess > 0 || OnlineDuplicate > 0)
            {
                return true;
            }
            return false;
        }

        public async void GetLocalPatients()
        {
            //try
            //{
                var LocalPatients = db.users.Where(x => x.role_id == 3 && x.doctor_id == MainWindow.Session.doctorId && x.is_sync == 0).Take(100).ToList();// role_id 3 = Patient                              
                foreach ( var LocalPatient in LocalPatients)
                {
                    LocalPatient.prescription.Clear();
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(MainWindow.Session.apiBaseUrl);

                HttpResponseMessage response = client.PostAsJsonAsync("admin/users/get-local-patients?doctor_email="+MainWindow.Session.doctorEmail, LocalPatients).Result;
                if (response.IsSuccessStatusCode)
                {
                    var ResponseLocalPatientSaveToOnline = response.Content.ReadAsStringAsync();
                    ResponseLocalPatientSaveToOnline.Wait();

                    var OnlineResponse = JsonConvert.DeserializeObject<SuccessMessages>(ResponseLocalPatientSaveToOnline.Result);                    

                    if(OnlineResponse != null && OnlineResponse.status == "success")
                    {
                        OnlineTotal = OnlineResponse.off_line_total;
                        OnlineSuccess = OnlineResponse.off_line_success;
                        OnlineDuplicate = OnlineResponse.off_line_duplicate;

                        ChangeIsSyncLocalPatients(LocalPatients);
                    }                    
                }
                else
                {
                    MessageBox.Show("Error Code " + response.StatusCode + " : Message - " + response.ReasonPhrase);
                }
            //}
            //catch
            //{
            //    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            //}
        }

        public void ChangeIsSyncLocalPatients(List<user> LocalPatients)
        {
            foreach(var LocalPatient in LocalPatients)
            {
                user = db.users.FirstOrDefault(x => x.id == LocalPatient.id);
                user.is_sync = 1;

                //try
                //{
                db.SaveChanges();
                //}
                //catch
                //{
                //    MessageBox.Show("There is a problem, Please try again.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                //}
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
    }
}
