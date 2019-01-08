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
    /// Interaction logic for AddTest.xaml
    /// </summary>
    public partial class AddTest : Page
    {
        public AddTest()
        {
            InitializeComponent();
        }

        contextd_db db = new contextd_db();
        test test = new test();

        private void SubmitAddTest_Click(object sender, RoutedEventArgs e)
        {
            if(TestName.Text != "")
            {
                try
                {
                    var haveTest = db.tests.FirstOrDefault(x => x.name == TestName.Text);

                    if (haveTest == null)
                    {
                        NavigationService.Navigate(new Uri("Tests.xaml", UriKind.Relative));

                        test.name = TestName.Text;
                        test.status = true;
                        test.created = DateTime.Now;
                        db.tests.Add(test);
                        TestName.Clear();
                        db.SaveChanges();
                        MessageBox.Show("The test has been saved", "Success");
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

        private void CancelAddTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("Tests.xaml", UriKind.Relative));
        }        
    }           
}
