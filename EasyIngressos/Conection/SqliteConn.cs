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


        public static void ExecuteQuery(string txtQuery)
        {
            SetConnection();
            conexao.Open();
            comando = conexao.CreateCommand();
            comando.CommandText = txtQuery;
            comando.ExecuteNonQuery();
            conexao.Close();
        }

        public static bool ExistValue(string table, string key)
        {
            SetConnection();

            conexao.Open();

            DataTable dt = new DataTable();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM {table.ToUpper()} WHERE id = '{key}'";

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
                conexao.Close();
            }

            conexao.Open();

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
            conexao.Close();


            if (t.class_id != 0)
            {
                conexao.Open();

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
                conexao.Close();
            }

            if (t.block_id != 0)
            {
                conexao.Open();

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
                conexao.Close();
            }

            return t;
        }

        public static EventData SelectEvent()
        {

            CultureInfo culture = new CultureInfo("pt-BR");
            EventData e = new EventData();
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
                    e.name = reader.IsDBNull(0) ? 0 : reader.GetString(0);
                    e.age_classification = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                    e.due_date = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                    e.address.street = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
                    e.address.city = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
                    e.address.uf = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
                }
                conexao.Close();
            }

            return e;

        }

    }
}
