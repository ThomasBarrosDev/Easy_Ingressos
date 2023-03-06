using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyIngressos.Entity
{
    public class TicketDataClass
    {
        public int id { get; set; }
        public string? code { get; set; }
        public float subtotal { get; set; }
        public float tax_value { get; set; }
        public float total { get; set; }
        public string? status { get; set; }
        public int class_id { get; set; }
        public string class_name { get; set; }
        public int block_id { get; set; }
        public string block_name { get; set; }
        public int event_id { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string? deleted_at { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string cpf_rg { get; set; }

    }
}
