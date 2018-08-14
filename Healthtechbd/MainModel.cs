using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WpfChosenControl;
using WpfChosenControl.model;

namespace Healthtechbd
{
    public class MainModel : ViewModelBase
    {        
        private ObservableCollection<IdNameModel> _medicinesLists;
        private List<IdNameModel> _selectedMedicines;


        public ObservableCollection<IdNameModel> MedicinesLists
        {
            get
            {
                return _medicinesLists;
            }
            set
            {
                _medicinesLists = value;
                NotifyPropertyChanged("MedicinesLists");
            }
        }

        public List<IdNameModel> SelectedMedicines
        {
            get
            {
                return _selectedMedicines;
            }
            set
            {
                _selectedMedicines = value;
                NotifyPropertyChanged("SelectedItems");
            }
        }

        model.ContextDb db = new model.ContextDb();        

        public MainModel()
        {
            var items = new ObservableCollection<IdNameModel>();

            var medicinesLists = db.medicines.OrderByDescending(x => x.created).ToList();

            foreach (var medicine in medicinesLists)
            {
                items.Add(new IdNameModel() { Id = medicine.id, Name = medicine.name });
            }

            MedicinesLists = items;
            var medicines = new List<IdNameModel>();
            medicines.Add(MedicinesLists.First());
            medicines.Add(MedicinesLists.LastOrDefault());

            this.SelectedMedicines = medicines;
        }        

        //private void Submit()
        //{

        //}
        
    }
}

