using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Data;


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

        public void LoadData(DataGridView datagrid)
        {
            /*SetConnection();
            conexao.Open();
            comando = conexao.CreateCommand();
            string comandtxt = "SELECT * FROM CLIENTES";
            dbadapter = new SQLiteDataAdapter(comandtxt, conexao);
            ds.Reset();
            dbadapter.Fill(ds);
            dt = ds.Tables[0];
            datagrid.DataSource = dt;
            conexao.Close();*/

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
       /* public static SQLiteDataReader SelectFields(string fields, string table)
        {

        }*/

        public static DataTable SelectAll(string field, string table, string search)
        {
            SetConnection();

            conexao.Open();

            DataTable dt = new DataTable();

            comando = conexao.CreateCommand();
            string comandtxt = $"SELECT * FROM {table.ToUpper()} WHERE {field.ToUpper()} = '{search}'";

            dbadapter = new SQLiteDataAdapter(comandtxt, conexao);
            dbadapter.Fill(dt);

            conexao.Close();

            return dt;
        }

    }
}
