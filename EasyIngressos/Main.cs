using ProjetoCores_1._0;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace EasyIngressos
{
    internal class Main
    {
        public void Synchronize()
        {

        }

        public static bool ValidateTicket(string cod)
        {

            DataTable dataTable = SqliteConn.SelectSearchField(field: "cod", table: "Ticket", search: cod) ;
            try
            {
                if (dataTable.Rows[0].Field<string>("cod") == null)
                {
                    return false;
                }
                if (dataTable.Rows[0].Field<string>("cod") == cod)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                //MessageBox.Show("Error Ticket does not exist!", "error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
                throw;
            }
        }


        public static void ValidateTicket(TextBox txtMessage, Label labelStatus)
        {
            bool validate = ValidateTicket(txtMessage.Text);

            if (!validate)
                labelStatus.Text = "Disapproved!";
            else
            {
                labelStatus.Text = "Approved!";
                txtMessage.Text = string.Empty;
            }
        }

       
    }
}
