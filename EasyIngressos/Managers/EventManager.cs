using EasyIngressos.Entity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using ProjetoCores_1._0;

namespace EasyIngressos.Managers
{
    public class EventManager
    {
        public static EventData[] eventData;

        public static void SetFormData(Label[] labels)
        {
            foreach (var item in labels)
            {
                switch (item.Name)
                {
                    case "label_EventName":
                        item.Text = eventData[0].name;
                        break;
                    case "label_ParentalRating":
                        item.Text = $"CLASSIFICAÇÃO INDICATIVA: {eventData[0].age_classification} ANOS";
                        break;
                    case "label_EventDate":
                        string dateString = eventData[0].due_date;
                        DateTime date = DateTime.Parse(dateString);
                        item.Text = date.ToString("dddd, dd MMMM", new CultureInfo("pt-BR"));
                        break;
                    case "label_EventAddress":
                        item.Text = eventData[0].address;
                        break;
                    case "label_EventHours":
                        string hoursString = eventData[0].due_date;
                        DateTime hours = DateTime.Parse(hoursString);
                        item.Text = $"{hours.ToString("HH:mm")} horas";
                        break;
                    default:
                        break;
                }

            }
        }
        public static void SetSqlData(EventData data)
        {
            string insertSql = $"INSERT INTO EventData (id, name) VALUES ({data.id}, '{data.name}')";

            SqliteConn.ExecuteQuery(insertSql);

        }

    }
}


