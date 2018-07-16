namespace Healthtechbd.model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("wpf_healthtechbd.users")]
    public partial class user
    {
        public int id { get; set; }
        
        [StringLength(222)]
        public string email { get; set; }

        [StringLength(222)]
        public string password { get; set; }      

        [Column(TypeName = "timestamp")]
        public DateTime created { get; set; }
    }
}
