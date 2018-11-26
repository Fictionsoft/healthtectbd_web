using Healthtechbd.prescriptionTemplates;
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
    /// Interaction logic for PrescriptionTemplates.xaml
    /// </summary>
    public partial class PrescriptionTemplates : Page
    {
        public PrescriptionTemplates()
        {
            InitializeComponent();

            //CheckSelectedRadioBtn();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CheckSelectedRadioBtn();
        }

        public void CheckSelectedRadioBtn()
        {
            int prescriptionTemId = MainWindow.Session.doctorPrescriptionTemId;
            if (prescriptionTemId == 1)
            {
                BtnDefault.IsChecked = true;
            }
            if (prescriptionTemId == 2)
            {
                BtnStandard.IsChecked = true;
            }
            if(prescriptionTemId == 3)
            {
                BtnClassic.IsChecked = true;
            }
            if(prescriptionTemId == 4)
            {
                BtnCustom.IsChecked = true;
            }
            if (prescriptionTemId == 5)
            {
                BtnGeneral.IsChecked = true;
            }
        }

        contextd_db db = new contextd_db();
        user doctor = new user();

        private void BtnStandard_Click(object sender, RoutedEventArgs e)
        {            
            Frame mainContent = AdminPanelWindow.mainContent;
            mainContent.Content = new PrescriptionTemplates();

            doctor = db.users.FirstOrDefault(x => x.id == MainWindow.Session.doctorId);

            RadioButton radioButton = sender as RadioButton;

            doctor.prescription_template_id = int.Parse(radioButton.DataContext.ToString());
            db.SaveChanges();

            MainWindow.Session.doctorPrescriptionTemId = int.Parse(radioButton.DataContext.ToString());
        }
       
        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;

            //MessageBox.Show(image.DataContext.ToString());
            
            MainWindow.Session.imagePath = image.DataContext.ToString();

            AdminPanelWindow.mainContent.Content = new TemImageView();

        }
    }
}
