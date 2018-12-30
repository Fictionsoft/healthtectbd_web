using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthtechbd.Model.ApiModel
{
    public class ViewDiagnosisTemplates
    {
        public int id { get; set; }
        public string instructions { get; set; }
        public ViewName diagnosis_list { get; set; }
        public List<ViewName> medicines { get; set; }
        public List<ViewName> tests { get; set; }
        public bool will_save { get; set; }
    }       

    public class ViewLocalDiagnosisTemplates
    {
        public string instructions { get; set; }
        public string diagnosis { get; set; }
        public List<string> medicines { get; set; }
        public List<string> tests { get; set; }
        public DateTime created { get; set; }
        public int is_sync { get; set; }
    }

    public class ViewName
    {
        public string name { get; set; }
    }
    
    public class DiagnosisTemplateSucessMessage
    {
        public string status { get; set; }
        public List<long> will_sync_ids { get; set; }
        public int online_success { get; set; }
        public int online_duplicate { get; set; }
    }
}
