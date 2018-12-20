using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthtechbd.Model.ApiModel
{
    public class OnlinePatients
    {
        public List<Patients> patients { get; set; }
    }

    public class Patients
    {
        public int id { get; set; }
        public int role_id { get; set; }
        public int doctor_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string address_line1 { get; set; }
        public string address_line2 { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string age { get; set; }
        public string password { get; set; }
        public string clinic_name { get; set; }
        public string specialist { get; set; }
        public string cember_name { get; set; }
        public string cember_address { get; set; }
        public string visiting_time { get; set; }
        public string off_day { get; set; }
        public string website { get; set; }
        public string logo { get; set; }
        public string signature { get; set; }
        public string educational_qualification { get; set; }
        public string profile_picture { get; set; }
        public string token { get; set; }
        public string prescription_template_id { get; set; }
        public string expire_date { get; set; }
        public int is_localhost { get; set; }
        public int is_sync { get; set; }
        public DateTime created { get; set; }
    }

    public class SuccessMessages
    {
        public string status { get; set; }
        public int off_line_total { get; set; }
        public int off_line_success { get; set; }
        public int off_line_duplicate { get; set; }
    }
}