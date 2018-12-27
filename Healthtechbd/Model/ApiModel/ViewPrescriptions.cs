using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthtechbd.Model.ApiModel
{
    public class ViewPrescriptions
    {
        //Prescription
        public int id { get; set; }
        public ViewNamePhone user { get; set; }
        public string temperature { get; set; }
        public string blood_pressure { get; set; }
        public string doctores_notes { get; set; }
        public string other_instructions { get; set; }

        // Diagnosis Template
        public List<ViewDiagnosis> diagnosis { get; set; } 

        //Prescriptions Medicine
        public List<ViewNameRule> medicines { get; set; }

        //Tests
        public List<ViewName> tests { get; set; }

    }

    //Prescription
    public class ViewNamePhone
    {
        public string first_name { get; set; }
        public string phone { get; set; }
    }

    // Diagnosis Template
    public class ViewDiagnosis
    {
        public string instructions { get; set; }
        public ViewName diagnosis_list { get; set; }
    }

    //Medicine
    public class ViewNameRule
    {
        public string name { get; set; }
        public ViewRule _joinData { get; set; }
    }

    public class ViewRule
    {
        public string rule { get; set; }
    }
}
