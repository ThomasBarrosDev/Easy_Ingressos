using EasyIngressos.Entity;
using ProjetoCores_1._0;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EasyIngressos.Managers
{
    public class AppManager
    {

        private static CultureInfo culture = new CultureInfo("en-US");
        public static EventData[] EventsData { get; set; }
        public static EventData SelectedEventData { get; set; }


        public static void SetFormEventData(byte[] imagemData, PictureBox pictureBox, Label[] labels, EventData Event)
        {

            using (MemoryStream ms = new MemoryStream(imagemData))
            {
                pictureBox.Image = Image.FromStream(ms);
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }

            foreach (var item in labels)
            {
                switch (item.Name)
                {
                    case "label_EventName":
                        item.Text = Event.name;
                        break;
                    case "label_ParentalRating":
                        item.Text = $"CLASSIFICAÇÃO INDICATIVA: {Event.age_classification} ANOS";
                        break;
                    case "label_EventDate":
                        string dateString = Event.due_date;
                        DateTime date = DateTime.Parse(dateString);
                        item.Text = date.ToString("dddd, dd MMMM", new CultureInfo("pt-BR"));
                        break;
                    case "label_EventAddress":
                        item.Text = $"{Event.address.street}, {Event.address.city} - {Event.address.uf}";
                        break;
                    case "label_EventHours":
                        string hoursString = Event.due_date;
                        DateTime hours = DateTime.Parse(hoursString);
                        item.Text = $"{hours.ToString("HH:mm")} horas";
                        break;
                    default:
                        break;
                }

            }
        }


        public static void SetFormEventDataOffLine(PictureBox pictureBox, Label[] labels, EventData Event)
        {
            string url = "https://automotivoshopping.com.br/wp-content/uploads/2020/10/maxresdefault-1-1110x577.jpg";

            using (WebClient client = new WebClient())
            {
                byte[] data = client.DownloadData(Event.banner);
                using (MemoryStream ms = new MemoryStream(data))
                {
                    pictureBox.Image = Image.FromStream(ms);
                    pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }

            foreach (var item in labels)
            {
                switch (item.Name)
                {
                    case "label_EventName":
                        item.Text = Event.name;
                        break;
                    case "label_ParentalRating":
                        item.Text = $"CLASSIFICAÇÃO INDICATIVA: {Event.age_classification} ANOS";
                        break;
                    case "label_EventDate":
                        string dateString = Event.due_date;
                        DateTime date = DateTime.Parse(dateString);
                        item.Text = date.ToString("dddd, dd MMMM", new CultureInfo("pt-BR"));
                        break;
                    case "label_EventAddress":
                        item.Text = $"{Event.address.street}, {Event.address.city} - {Event.address.uf}";
                        break;
                    case "label_EventHours":
                        string hoursString = Event.due_date;
                        DateTime hours = DateTime.Parse(hoursString);
                        item.Text = $"{hours.ToString("HH:mm")} horas";
                        break;
                    default:
                        break;
                }

            }
        }

        public static void SetFormTicketData(Label[] labels, TicketDataClass ticket)
        {
            foreach (var item in labels)
            {
                switch (item.Name)
                {
                    case "label_IdIngresso":

                        item.Text = ticket.id.ToString() + " - ";

                        break;
                    case "label_TicketName":

                        if (!string.IsNullOrEmpty(ticket.block_name))
                            item.Text = ticket.block_name.ToUpper();

                        break;
                    case "label_ParticipantName":

                        if (!string.IsNullOrEmpty(ticket.name))
                            item.Text = ticket.name;

                        break;
                    case "label_CPF":
                        if (!string.IsNullOrEmpty(ticket.cpf_rg))
                            item.Text = ticket.cpf_rg;

                        break;
                    case "label_Email":
                        if (!string.IsNullOrEmpty(ticket.email))
                            item.Text = ticket.email;

                        break;
                    case "label_Telefone":
                        if (!string.IsNullOrEmpty(ticket.phone))
                            item.Text = ticket.phone;

                        break;
                    default:
                        break;
                }

            }
        }

        public static void SetFormTicketData(Label[] labels)
        {
            foreach (var item in labels)
            {
                switch (item.Name)
                {
                    case "label_IdIngresso":
                        item.Text = "0 - ";
                        break;
                    case "label_TicketName":
                        item.Text = "NULL";
                        break;
                    case "label_ParticipantName":
                        item.Text = "NULL";
                        break;
                    case "label_CPF":
                        item.Text = "NULL";
                        break;
                    case "label_Email":
                        item.Text = "NULL";
                        break;
                    case "label_Telefone":
                        item.Text = "NULL";
                        break;
                    default:
                        break;
                }

            }
        }

        public static void SetFormTicketValidate(TicketDataClass ticket, Label[] labels, PictureBox pictureBox, Panel panel, TextBox textBox, string code)
        {

            if (ticket == null)
            {
                MessageBox.Show("Error in DataBase Sqlite!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string nomeImagem;
            Color cor;

            if (ticket.code == null)
            {
                nomeImagem = "times-circle.png";

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                {
                    pictureBox.Image = Image.FromStream(stream);
                }

                SetFormTicketData(labels);

                labels[6].Text = "INGRESSO \r\n INEXISTENTE";
                cor = ColorTranslator.FromHtml("#A1432E");
                panel.BackColor = cor;
                return;
            }

            switch (ticket.status)
            {
                case "active":
                case "offline":

                    nomeImagem = "check-circle.png";

                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }

                    SetFormTicketData(labels, ticket);

                    labels[6].Text = "INGRESSO \r\n VÁLIDO";
                    cor = ColorTranslator.FromHtml("#2EA15C");
                    panel.BackColor = cor;

                    ticket.status = "used";

                    TicketSendNewStatusSqlite(ticket);

                    break;
                case "canceled":
                    nomeImagem = "exclamation-circle.png";

                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }

                    SetFormTicketData(labels);

                    labels[6].Text = "INGRESSO \r\n CANCELADO";
                    cor = ColorTranslator.FromHtml("#d3a600");
                    panel.BackColor = cor;

                    break;

                case "used":
                    nomeImagem = "exclamation-circle.png";

                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }

                    SetFormTicketData(labels, ticket);

                    labels[6].Text = "INGRESSO \r\n JÁ USADO";
                    cor = ColorTranslator.FromHtml("#d3a600");
                    panel.BackColor = cor;

                    break;

                case "denied":
                    nomeImagem = "exclamation-circle.png";

                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }

                    SetFormTicketData(labels);

                    labels[6].Text = "INGRESSO \r\n NEGADO";
                    cor = ColorTranslator.FromHtml("#d3a600");
                    panel.BackColor = cor;

                    break;

                case "sold":
                    nomeImagem = "exclamation-circle.png";

                    using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                    {
                        pictureBox.Image = Image.FromStream(stream);
                    }

                    SetFormTicketData(labels);

                    labels[6].Text = "INGRESSO \r\nNÃO FOI VENDIDO";
                    cor = ColorTranslator.FromHtml("#d3a600");
                    panel.BackColor = cor;

                    break;

                default:
                    break;
            }


        }

        public static void SetEventSqlData(EventData data)
        {
            if (SqliteConn.ExistValue("EventData"))
            {
                SqliteConn.ClearDB();
            }

            string url = data.banner;

            using (WebClient client = new WebClient())
            {
                byte[] byt = client.DownloadData(url);

                string insertSql = $"INSERT INTO EventData (id, name, slug, ticket_name, " +
                   $"due_date, alternative_date_start, alternative_date_end, " +
                   $"sales_start_date, sales_end_date, event_time, " +
                   $"ticket_phrase, " +
                   $"adm_tax, tax_percent, featured, age_classification, image, images, about, event_site," +
                   $"info, category_id, provider_id, address_id, status, image_bytes) VALUES " +
                   $"('{data.id}', '{data.name}','{data.slug}', '{data.ticket_name}'," +
                   $"'{data.due_date}','{data.alternative_date_start}','{data.alternative_date_end}'," +
                   $"'{data.sales_start_date}','{data.sales_end_date}','{data.event_time}'," +
                   $"'{data.ticket_phrase}'," +
                   $"'{data.adm_tax}','{data.tax_percent}','{data.featured}','{data.age_classification}','{data.banner}','{data.images}','{data.about}','{data.event_site}'," +
                   $"'{data.info}','{data.category_id}','{data.provider_id}','{data.address_id}','{data.status}', @ImageData)";

                SqliteConn.ExecuteQuery(insertSql, byt);
            }

            data.address.event_id = data.id;

            SetAdressSqlData(data.address);
        }

        public static void SetTicketClasseSqlData(EventData.TicketClass data, int eventId)
        {
            if (!SqliteConn.ExistValueById("TicketClass", data.id.ToString()))
            {
                string insertSql = $"INSERT INTO TicketClass (id, name, type, event_id) VALUES " +
                $"('{data.id}', '{data.name}', '{data.type}', {eventId})";

                SqliteConn.ExecuteQuery(insertSql);

                if (data.ticket_blocks.Length > 0)
                {
                    for (int i = 0; i < data.ticket_blocks.Length; i++)
                    {
                        SetTicketBlockSqlData(data.ticket_blocks[i], data.id);
                    }
                }
            }
        }

        public static void SetTicketBlockSqlData(EventData.TicketBlock data, int idClass)
        {
            float price = 0;
            if (data.price.HasValue)
            {
                price = data.price.Value;
            }

            if (!SqliteConn.ExistValueById("TicketBlock", data.id.ToString()))
            {
                string insertSql = $"INSERT INTO TicketBlock (id, name, price, type, expires_at, class_id) VALUES " +
                $"('{data.id}', '{data.name}', '{price.ToString("F2", culture)}', '{data.type}', '{data.expires_at}', '{idClass}')";

                SqliteConn.ExecuteQuery(insertSql);
            }
        }

        public static void SetTicketDataSqlData(EventData.TicketData data, int TicketId)
        {
            if (!SqliteConn.ExistValueById("TicketData", data.id.ToString()))
            {
                string insertSql = $"INSERT INTO TicketData (name, email, phone, cpf_rg, ticket_id) VALUES " +
                $"('{data.name}', '{data.email}', '{data.phone}', '{data.cpf_rg}', '{TicketId}')";

                SqliteConn.ExecuteQuery(insertSql);
            }
        }

        public static void SetAdressSqlData(EventData.Address data)
        {
            if (!SqliteConn.ExistValueByname("Adress", data.name))
            {
                string insertSql = $"INSERT INTO Adress (name, image, street, number, complement, district, city, uf, zipcode, lat, lon, event_id) VALUES " +
                $"('{data.name}', '{data.image}', '{data.street}', '{data.number}', '{data.complement}', '{data.district}', '{data.city}', '{data.uf}', '{data.zipcode}', '{data.lat}'" +
                $", '{data.lon}', '{data.event_id}')";

                SqliteConn.ExecuteQuery(insertSql);
            }
        }

        public static void SetTicketSqlData(EventData.Ticket data)
        {
            if (!SqliteConn.ExistValueById("Ticket", data.id.ToString()))
            {
                string insertSql = $"INSERT INTO Ticket (id, code, subtotal, tax_value, " +
                $"total, status, class_id, block_id, event_id, created_at, " +
                $"updated_at, deleted_at) VALUES " +
                $"('{data.id}', '{data.code}','{data.subtotal.ToString("F2", culture)}', '{data.tax_value.ToString("F2", culture)}'," +
                $"'{data.total.ToString("F2", culture)}','{data.status}'," +
                $"'{data.class_id}',{data.block_id},'{data.event_id}','{data.created_at}'," +
                $"'{data.updated_at}', '{data.deleted_at}')";
                SqliteConn.ExecuteQuery(insertSql);

                if (data.ticket_data == null)
                    return;

                SetTicketDataSqlData(data.ticket_data, data.id);
            }
        }

        public static void TicketSendNewStatusSqlite(TicketDataClass Ticket)
        {
            if (!SqliteConn.UpdateStatusTicket(Ticket))
            {
                MessageBox.Show("Error in Ticket Status!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
