using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyIngressos.Entity
{
    [Serializable]
    public class EventData
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string ticket_name { get; set; }
        public string due_date { get; set; }
        public string alternative_date_start { get; set; }
        public string alternative_date_end { get; set; }
        public string sales_start_date { get; set; }
        public string sales_end_date { get; set; }
        public string event_time { get; set; }
        public string ticket_phrase { get; set; }
        public int? adm_tax { get; set; }
        public int? tax_percent { get; set; }
        public int? featured { get; set; }
        public int? age_classification { get; set; }
        public int? category_id { get; set; }
        public int? provider_id { get; set; }
        public int? address_id { get; set; }
        public string status { get; set; }
        public string? banner { get; set; }
        public string images { get; set; }
        public string[] event_images { get; set; }
        public string about { get; set; }
        public string event_site { get; set; }
        public string info { get; set; }
        public string provider { get; set; }
        public Category category { get; set; }
        public Address address { get; set; }
        public string metadata { get; set; }
        public TicketClass[] ticket_classes { get; set; }
        public string[] sales_points { get; set; }
        public Ticket[] tickets { get; set; }


        [Serializable]
        public class Category
        {
            public int id { get; set; }
            public string name { get; set; }
            public string slug { get; set; }
        }

        [Serializable]
        public class Address
        {
            public string name { get; set; }
            public string image { get; set; }
            public string street { get; set; }
            public string number { get; set; }
            public string complement { get; set; }
            public string district { get; set; }
            public string city { get; set; }
            public string uf { get; set; }
            public string zipcode { get; set; }
            public string lat { get; set; }
            public string lon { get; set; }

        }

        [Serializable]
        public class TicketClass
        {
            public int id { get; set; }
            public string? name { get; set; }
            public string? type { get; set; }
            public TicketBlock[] ticket_blocks { get; set; }
        }

        [Serializable]
        public class TicketBlock
        {
            public int id { get; set; }
            public string? name { get; set; }
            public float? price { get; set; }
            public string? type { get; set; }
            public string expires_at { get; set; }
        }

        [Serializable]
        public class Ticket
        {
            public int id { get; set; }
            public string? code { get; set; }
            public float subtotal { get; set; }
            public float tax_value { get; set; }
            public float total { get; set; }
            public string? status { get; set; }
            public int class_id { get; set; }
            public int block_id { get; set; }
            public int event_id { get; set; }
            public string created_at { get; set; }
            public string updated_at { get; set; }
            public string? deleted_at { get; set; }
            public TicketData ticket_data { get; set; }
        }

        [Serializable]
        public class TicketData
        {
            public int id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string phone { get; set; }
            public string cpf_rg { get; set; }
        }

    }
}
