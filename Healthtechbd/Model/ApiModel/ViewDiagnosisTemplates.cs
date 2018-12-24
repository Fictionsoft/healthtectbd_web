using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthtechbd.Model.ApiModel
{
    public class ViewDiagnosisTemplates
    {
        public string instructions { get; set; }
        public List<ViewDiagnosisList> diagnosis_list { get; set; }
        //public List<ViewTests> tests { get; set; }
        //public List<ViewMedicines> medicines { get; set; }
    }

    public class ViewDiagnosisList
    {
        public string name { get; set; }
    }

    public class ViewTests
    {
        public string name { get; set; }
    }

    public class ViewMedicines
    {
        public string name { get; set; }
    }
}
