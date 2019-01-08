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
    /// Interaction logic for EditTest.xaml
    /// </summary>
    public partial class EditTest : Page
    {
        public EditTest()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        test test = new test();

        public EditTest(int id) : this()
        {
            TestId.Text = id.ToString();

            try
            {
                test = db.tests.FirstOrDefault(x => x.id == id);
                TestName.Text = test.name;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void SubmitUpdateTest_Click(object sender, RoutedEventArgs e)
        {
            if(TestName.Text != "")
            {
                int testId = int.Parse(TestId.Text);
                try
                {
                    var haveTest = db.tests.FirstOrDefault(x => x.name == TestName.Text && x.id != testId);

                    if (haveTest == null)
                    {
                        test = db.tests.FirstOrDefault(x => x.id == testId);
                        test.name = TestName.Text;
                        db.SaveChanges();

                        NavigationService.Navigate(new Uri("Tests.xaml", UriKind.Relative));
                        MessageBox.Show("Update Successfully", "Success");
                    }
                    else
                    {
                        MessageBox.Show("The Test already exist", "Already Exit");
                    }
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
            else
            {
                MessageBox.Show("Test name is required", "Required field", MessageBoxButton.OK, MessageBoxImage.Warning);
            }            
        }

        private void CancelEditTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Tests.xaml", UriKind.Relative));
        }        
    }
}
