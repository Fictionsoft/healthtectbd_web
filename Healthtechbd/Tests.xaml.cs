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
using Healthtechbd.model;

namespace Healthtechbd
{
    /// <summary>
    /// Interaction logic for Tests.xaml
    /// </summary>
    public partial class Tests : Page
    {
        public Tests()
        {
            InitializeComponent();

            loadTests();
        }

        contextd_db db = new contextd_db();
        test test = new test();

        private void ButtonAddTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddTest.xaml", UriKind.Relative));
        }

        void loadTests()
        {
            try
            {
                var tests = db.tests.OrderByDescending(x => x.created).Take(10).ToList();
                dataGridTests.ItemsSource = tests;
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                       
        }

        private void btnDeleteTestRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int testId = (dataGridTests.SelectedItem as test).id;

                try
                {
                    test = db.tests.FirstOrDefault(x => x.id == testId);

                    db.tests.Remove(test);
                    db.SaveChanges();
                    loadTests();
                    MessageBox.Show("Delete Successfully", "Success");
                }
                catch
                {
                    MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }                
            }
        }

        private void btnEditTestRow_Click(object sender, RoutedEventArgs e)
        {
            int testId = (dataGridTests.SelectedItem as test).id;
            EditTest editTest = new EditTest(testId);
            NavigationService.Navigate(editTest);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchBy = searchField.Text.ToString();

            try
            {
                var tests = db.tests.Where(x => x.name.Trim().StartsWith(searchBy)).OrderByDescending(x => x.created).Take(10).ToList();

                if (tests.Count == 0)
                {
                    MessageBox.Show("Test not found", "Warning");
                }
                else
                {
                    dataGridTests.ItemsSource = tests;
                }
            }
            catch
            {
                MessageBox.Show("There is a problem, Please try again", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }                                        
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchField.Clear();
            loadTests();
        }
    }
}
