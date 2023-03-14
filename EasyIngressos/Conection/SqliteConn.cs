using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data;
using EasyIngressos.Entity;
using System.Globalization;
using EasyIngressos.Conection;
using static EasyIngressos.Conection.EventsSynchronize;
using System.Numerics;

namespace ProjetoCores_1._0
{
    class SqliteConn
    {
        private static SQLiteConnection conexao;
        private static SQLiteCommand comando;
        private static SQLiteDataAdapter dbadapter;

        private static string path = Environment.CurrentDirectory;
        private static string dataBaseName = "database.db";

        public static void SetConnection()
        {
            conexao = new SQLiteConnection($"Data Source={path}\\Database\\{dataBaseName};Version=3;New=False;Compress=True;");
        }

        public static void ClearDB()
        {
            SetConnection();
            conexao.Open();
            comando = conexao.CreateCommand();

            string deleteTickets = "DELETE FROM ticket";
            comando.CommandText = deleteTickets;
            comando.ExecuteNonQuery();

            string deleteTicketsData = "DELETE FROM ticketdata";
            comando.CommandText = deleteTicketsData;
            comando.ExecuteNonQuery();

            string deleteBlocks = "DELETE FROM ticketblock";
            comando.CommandText = deleteBlocks;
            comando.ExecuteNonQuery();

            string deleteClass = "DELETE FROM ticketclass";
            comando.CommandText = deleteClass;
            comando.ExecuteNonQuery();

            string deleteAdress = "DELETE FROM adress";
            comando.CommandText = deleteAdress;
            comando.ExecuteNonQuery();

            string deleteEvent = "DELETE FROM EventData";
            comando.CommandText = deleteEvent;
            comando.ExecuteNonQuery();

            conexao.Close();
        }


        public static void ExecuteQuery(string txtQuery)
        {
            SetConnection();
            conexao.Open();
            comando = conexao.CreateCommand();
            comando.CommandText = txtQuery;
            comando.ExecuteNonQuery();
            conexao.Close();
        }

        public static void ExecuteQuery(string txtQuery, byte[] byt)
        {
            SetConnection();
            conexao.Open();
            comando = conexao.CreateCommand();
            comando.CommandText = txtQuery;
            comando.Parameters.Add("@ImageData", DbType.Binary, byt.Length).Value = byt;
            comando.ExecuteNonQuery();
            conexao.Close();
        }

        public static bool ExistValueById(string table, string id)
        {
            SetConnection();

            conexao.Open();

            DataTable dt = new DataTable();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM {table.ToUpper()} WHERE id = '{id}'";

            dbadapter = new SQLiteDataAdapter(comandtxt, conexao);
            dbadapter.Fill(dt);

            conexao.Close();

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public static bool UpdateStatusTicket(TicketDataClass ticketData)
        {
            SetConnection();

            try
            {
                conexao.Open();

                comando = conexao.CreateCommand();
                string comandtxt = $"UPDATE Ticket SET status = '{ticketData.status}' WHERE id = '{ticketData.id}'";

                comando.CommandText = comandtxt;
                comando.ExecuteNonQuery();

                conexao.Close();

                return true;
            }
            catch (Exception)
            {
                // Em caso de exceção, retorna false indicando que a operação falhou
                conexao.Close();
                return false;
            }
        }

        public static bool ExistValue(string table)
        {
            SetConnection();

            conexao.Open();

            DataTable dt = new DataTable();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM {table.ToUpper()}";

            dbadapter = new SQLiteDataAdapter(comandtxt, conexao);
            dbadapter.Fill(dt);

            conexao.Close();

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public static bool ExistValueByname(string table, string name)
        {
            SetConnection();

            conexao.Open();

            DataTable dt = new DataTable();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM {table.ToUpper()} WHERE name = '{name}'";

            dbadapter = new SQLiteDataAdapter(comandtxt, conexao);
            dbadapter.Fill(dt);

            conexao.Close();

            if (dt.Rows.Count > 0)
                return true;
            else
                return false;

        }

        public static TicketDataClass SelectTicket(string code)
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            TicketDataClass t = new TicketDataClass();
            SetConnection();

            conexao.Open();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM Ticket WHERE code = '{code}'";

            comando.CommandText = comandtxt;

            using (SQLiteDataReader reader = comando.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    t.id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    t.code = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);

                    t.subtotal = reader.IsDBNull(2) ? 0 : reader.GetFloat(2);
                    t.tax_value = reader.IsDBNull(3) ? 0 : reader.GetFloat(3);
                    t.total = reader.IsDBNull(4) ? 0 : reader.GetFloat(4);

                    t.status = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                    t.class_id = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                    t.block_id = reader.IsDBNull(7) ? 0 : reader.GetInt32(7);
                    t.event_id = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                    t.created_at = reader.IsDBNull(10) ? string.Empty : reader.GetString(10);
                    t.updated_at = reader.IsDBNull(11) ? string.Empty : reader.GetString(11);
                    t.deleted_at = reader.IsDBNull(12) ? string.Empty : reader.GetString(12);
                }
            }
            
            comando = conexao.CreateCommand();
            comandtxt = $"SELECT * FROM TicketData WHERE ticket_id = '{t.id}'";

            comando.CommandText = comandtxt;

            using (SQLiteDataReader readerData = comando.ExecuteReader())
            {

                if (readerData.HasRows)
                {
                    readerData.Read();
                    t.name = readerData.IsDBNull(1) ? string.Empty : readerData.GetString(1);
                    t.email = readerData.IsDBNull(1) ? string.Empty : readerData.GetString(2);
                    t.phone = readerData.IsDBNull(1) ? string.Empty : readerData.GetString(3);
                    t.cpf_rg = readerData.IsDBNull(1) ? string.Empty : readerData.GetString(4);
                }

            }


            if (t.class_id != 0)
            {
                comando = conexao.CreateCommand();
                comandtxt = $"SELECT * FROM TicketClass WHERE id = '{t.class_id}'";

                comando.CommandText = comandtxt;

                using (SQLiteDataReader readerData = comando.ExecuteReader())
                {

                    if (readerData.HasRows)
                    {
                        readerData.Read();
                        t.class_name = readerData.IsDBNull(1) ? string.Empty : readerData.GetString(1);
                    }
                }
            }

            if (t.block_id != 0)
            {
                comando = conexao.CreateCommand();
                comandtxt = $"SELECT * FROM TicketBlock WHERE id = '{t.block_id}'";

                comando.CommandText = comandtxt;

                using (SQLiteDataReader readerData = comando.ExecuteReader())
                {

                    if (readerData.HasRows)
                    {
                        readerData.Read();
                        t.block_name = readerData.IsDBNull(1) ? string.Empty : readerData.GetString(1);
                    }

                }
            }
            conexao.Close();

            return t;
        }

        public static EventsSynchronize SelectEventSynchronize()
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            EventsSynchronize t = new EventsSynchronize();
            SetConnection();

            conexao.Open();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM EventData";

            comando.CommandText = comandtxt;

            using (SQLiteDataReader reader = comando.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    t.events[0] = new EventSynchronize();
                    t.events[0].id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                }

            }

            comando = conexao.CreateCommand();
            comandtxt = $"SELECT * FROM Ticket";

            comando.CommandText = comandtxt;
            List<object[]> rows = new List<object[]>();

            using (SQLiteDataReader readerData = comando.ExecuteReader())
            {
                while (readerData.Read())
                {
                    object[] row = new object[readerData.FieldCount];
                    readerData.GetValues(row);
                    rows.Add(row);
                }
            }

            conexao.Close();
            t.events[0].tickets = new EventSynchronize.TicketResponse[rows.Count];
            int i = 0;
            foreach (object[] row in rows)
            {
                t.events[0].tickets[i] = new EventSynchronize.TicketResponse();
                t.events[0].tickets[i].id = (int)(long)row[0];
                t.events[0].tickets[i].status = row[5].ToString();
                i++;
            }

            return t;
        }

        public static EventData SelectEvent()
        {
            CultureInfo culture = new CultureInfo("pt-BR");
            EventData e = new EventData();
            e.address = new EventData.Address();
            SetConnection();

            conexao.Open();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM EventData";

            comando.CommandText = comandtxt;

            using (SQLiteDataReader reader = comando.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    e.id = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    e.name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    e.due_date = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                    e.age_classification = reader.IsDBNull(15) ? 0 : reader.GetInt32(15);
                    e.images = reader.IsDBNull(16) ? string.Empty : reader.GetString(16);
                    e.event_site = reader.IsDBNull(19) ? string.Empty : reader.GetString(19);
                    e.image_bytes = reader.IsDBNull(29)? null: (byte[])reader["image_bytes"];
                }

            }

            comando = conexao.CreateCommand();
            comandtxt = $"SELECT * FROM Adress WHERE event_id = '{e.id}'";

            comando.CommandText = comandtxt;

            using (SQLiteDataReader readerData = comando.ExecuteReader())
            {

                if (readerData.HasRows)
                {
                    readerData.Read();
                    e.address.name = readerData.IsDBNull(1) ? string.Empty : readerData.GetString(1);
                    e.address.image = readerData.IsDBNull(2) ? string.Empty : readerData.GetString(2);
                    e.address.street = readerData.IsDBNull(3) ? string.Empty : readerData.GetString(3);
                    e.address.street = readerData.IsDBNull(4) ? string.Empty : readerData.GetString(4);
                    e.address.complement = readerData.IsDBNull(5) ? string.Empty : readerData.GetString(5);
                    e.address.district = readerData.IsDBNull(6) ? string.Empty : readerData.GetString(6);
                    e.address.city = readerData.IsDBNull(7) ? string.Empty : readerData.GetString(7);
                    e.address.uf = readerData.IsDBNull(8) ? string.Empty : readerData.GetString(8);
                    e.address.zipcode = readerData.IsDBNull(9) ? string.Empty : readerData.GetString(9);
                    e.address.lat = readerData.IsDBNull(10) ? string.Empty : readerData.GetString(10);
                    e.address.lon = readerData.IsDBNull(11) ? string.Empty : readerData.GetString(11);
                }


            }

            conexao.Close();
            return e;

        }

    }
}
