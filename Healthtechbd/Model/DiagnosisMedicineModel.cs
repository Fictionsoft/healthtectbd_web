using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfChosenControl;
using WpfChosenControl.model;

namespace Healthtechbd.Model
{
    class DiagnosisMedicineModel : ViewModelBase
    {
        private ObservableCollection<IdNameModel> _testsLists;
        public ObservableCollection<IdNameModel> TestsLists
        {
            get
            {
                return _testsLists;
            }
            set
            {
                _testsLists = value;
                NotifyPropertyChanged("TestsLists");
            }
        }

        private List<IdNameModel> _selectedTests;
        public List<IdNameModel> SelectedTests
        {
            get
            {
                return _selectedTests;
            }
            set
            {
                _selectedTests = value;
                NotifyPropertyChanged("SelectedTests");
            }
        }

        contextd_db db = new contextd_db();
        public DiagnosisMedicineModel()
        {
            var tests = new ObservableCollection<IdNameModel>();

            var testsLists = db.tests.OrderByDescending(x => x.created).Take(10).ToList();

            foreach (var test in testsLists)
            {
                tests.Add(new IdNameModel() { Id = test.id, Name = test.name });
            }

            TestsLists = tests;

            int ItemId = MainWindow.Session.editRecordId;
            if (ItemId > 0)
            {
                LoadExistingItems(ItemId);
            }
            else
            {
                var selected_tests = new List<IdNameModel>();
                SelectedTests = selected_tests;
            }
        }

        public void LoadExistingItems(int ItemId)
        {
            var existing_items = new ObservableCollection<IdNameModel>();
            var existing_tests = db.diagnosis_tests
            .Where(x => x.diagnosis_id == ItemId)
            .Select(x => new
            {
                Id = x.test.id,
                Name = x.test.name
            })
            .ToList();

            if (existing_tests.Count() > 0)
            {
                foreach (var existing_test in existing_tests)
                {
                    existing_items.Add(new IdNameModel() { Id = existing_test.Id, Name = existing_test.Name });
                }
            }
            TestsLists = existing_items;
            var tests = new List<IdNameModel>();
            tests.AddRange(existing_items);
            this.SelectedTests = tests;
        }
    }
}
