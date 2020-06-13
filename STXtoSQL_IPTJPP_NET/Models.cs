using System;
using System.Collections.Generic;
using System.Text;


namespace STXtoSQL.Models
{
    public class IPTJPP
    {
        
        public int job_no { get; set; }
        public int itm { get; set; }
        public int sbitm { get; set; }
        public string invt_typ { get; set; }
        public decimal wdth { get; set; }
        public string ord_info { get; set; }
        public int cus_id { get; set; }
        public string part { get; set; }
        public int pcs { get; set; }             
    }
}
