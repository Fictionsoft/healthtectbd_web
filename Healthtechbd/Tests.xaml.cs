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
    /// Interaction logic for Tests.xaml
    /// </summary>
    public partial class Tests : Page
    {
        public Tests()
        {
            InitializeComponent();

            loadTests();
        }

        model.ContextDb db = new model.ContextDb();
        model.test test = new model.test();

        private void ButtonAddTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("AddTest.xaml", UriKind.Relative));
        }

        void loadTests()
        {
            var tests = db.tests.ToList();
            dataGridTests.ItemsSource = tests;
        }

        private void btnDeleteTestRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You Sure ?", "Confirm",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int testId = (dataGridTests.SelectedItem as model.test).id;
                test = db.tests.FirstOrDefault(x => x.id == testId);
                //test = (from x in db.tests where x.id == test_id select x).First();

                db.tests.Remove(test);
                db.SaveChanges();
                loadTests();
                MessageBox.Show("Delete Successfully");
            }
        }

        private void btnEditTestRow_Click(object sender, RoutedEventArgs e)
        {
            int testId = (dataGridTests.SelectedItem as model.test).id;
            EditTest editTest = new EditTest(testId);
            NavigationService.Navigate(editTest);
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string searchBy = searchField.Text.ToString();

            var tests = db.tests.Where(x => x.name.Trim().StartsWith(searchBy)).ToList();
            dataGridTests.ItemsSource = tests;                    
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            searchField.Clear();
            loadTests();
        }
    }
}
