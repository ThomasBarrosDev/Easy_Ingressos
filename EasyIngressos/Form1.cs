using ProjetoCores_1._0;
using SuperSimpleTcp;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using EasyIngressos.Conection;
using System.Globalization;
using EasyIngressos.Managers;
using System.Linq.Expressions;
using EasyIngressos.Entity;
using System.Reflection;

namespace EasyIngressos
{
    public partial class Form1 : Form
    {
        private SimpleTcpServer m_server;
        private SimpleTcpClient m_client;

        private bool m_IsServer = false;
        private bool m_IsClient = false;
        private float time = 0;
        private bool isTime = false;
        private List<string> m_listClientIp = new();
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
           
            comboBox_Events.SelectedIndex = 0;

            try
            {
                await ConectionServer.AuthenticateBackend();

                for (int i = 0; i < AppManager.EventsData.Length; i++)
                {
                    comboBox_Events.Items.Add($"{AppManager.EventsData[i].id} - {AppManager.EventsData[i].name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {

            }
        }

        public static DialogResult DialogBox(string title, string promptText, ref string value, string button1 = "OK", string button2 = "Cancel", string button3 = null)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button button_1 = new Button();
            Button button_2 = new Button();
            Button button_3 = new Button();

            int buttonStartPos = 228; //Standard two button position

            if (button3 != null)
                buttonStartPos = 228 - 81;
            else
            {
                button_3.Visible = false;
                button_3.Enabled = false;
            }

            form.Text = title;

            // Label
            label.Text = promptText;
            label.SetBounds(9, 20, 372, 13);
            label.Font = new Font("Microsoft Tai Le", 10, FontStyle.Regular);

            // TextBox
            if (value == null)
            {
            }
            else
            {
                textBox.Text = value;
                textBox.SetBounds(12, 36, 372, 20);
                textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            }

            button_1.Text = button1;
            button_2.Text = button2;
            button_3.Text = button3 ?? string.Empty;
            button_1.DialogResult = DialogResult.OK;
            button_2.DialogResult = DialogResult.Cancel;
            button_3.DialogResult = DialogResult.Yes;

            button_1.SetBounds(buttonStartPos, 72, 75, 23);
            button_2.SetBounds(buttonStartPos + 81, 72, 75, 23);
            button_3.SetBounds(buttonStartPos + (2 * 81), 72, 75, 23);

            label.AutoSize = true;
            button_1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button_3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, button_1, button_2 });
            if (button3 != null)
                form.Controls.Add(button_3);
            if (value != null)
                form.Controls.Add(textBox);

            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = button_1;
            form.CancelButton = button_2;

            DialogResult dialogResult = form.ShowDialog();
            form.TopMost = true;
            value = textBox.Text;

            return dialogResult;

        }


        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //txtInfo.Text += $"Server disconnected. {Environment.NewLine}";
            });
        }

        private void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //txtInfo.Text += $"Server connected. {Environment.NewLine}";
            });
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //txtInfo.Text += $"{e.IpPort}: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
            });
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //txtInfo.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                m_listClientIp.Remove(e.IpPort);
            });
        }

        private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                //txtInfo.Text += $"{e.IpPort} Connected. {Environment.NewLine}";
                m_listClientIp.Add(e.IpPort);
            });

        }

        // essa funcao eu tinha feito é pra mandar texto de um programa para o outro via soket, agora vou usar ela pra mandar as informaçoes sobre o ingresso por exemplo se foi validado ou nao etc
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (m_IsServer)
            {
                if (m_server.IsListening)
                {
                    for (int i = 0; i < m_listClientIp.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(textBox_TicketCode.Text) && m_listClientIp[i] != null)
                        {
                            m_server.Send(m_listClientIp[i], textBox_TicketCode.Text);
                            //txtInfo.Text += $"Server: {txtMessage.Text}{Environment.NewLine}";
                            textBox_TicketCode.Text = string.Empty;
                        }

                    }
                }
            }
            else
            {
                if (m_client.IsConnected)
                {
                    if (!string.IsNullOrEmpty(textBox_TicketCode.Text))
                    {
                        m_client.Send(textBox_TicketCode.Text);
                        //txtInfo.Text += $"Me: {txtMessage.Text}{Environment.NewLine}";
                        textBox_TicketCode.Text = string.Empty;
                    }
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            this.TopMost = false;

            if (!m_IsServer && !m_IsClient)
            {
                m_server = new SimpleTcpServer(txtIP.Text, 9090);
                m_client = new SimpleTcpClient(txtIP.Text, 9090);

                m_client.Events.Connected += Events_Connected;
                m_client.Events.Disconnected += Events_Disconnected;
                m_client.Events.DataReceived += Events_DataReceived;

                m_server.Events.ClientConnected += Events_ClientConnected;
                m_server.Events.ClientDisconnected += Events_ClientDisconnected;
                m_server.Events.DataReceived += Events_DataReceived;



                MessageBoxButtons messageBoxButtons = MessageBoxButtons.YesNo;

                string value = null;



                DialogResult result = DialogBox("Conectar ao Servidor", "Conectar a um servidor local ou criar um servidor local?", ref value, "Conectar", "Criar");


                if (result == DialogResult.OK)
                {
                    try
                    {
                        m_client.Connect();

                        if (m_client.IsConnected)
                        {
                            m_IsClient = true;
                            MessageBox.Show("Conectado com sucesso no servidor :" + txtIP.Text, "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            btnStart.Text = "Desconectar";
                        }
                        else
                            MessageBox.Show("Nao foi possivel conectar com o servidor :" + txtIP.Text, "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnStart.Text = "Conectar";
                    }
                }
                else
                {
                    try
                    {
                        m_server.Start();

                        MessageBox.Show("Criado servidor para o IP" + txtIP.Text, "Server created successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        btnStart.Text = "Desconectar";

                        m_IsServer = true;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnStart.Text = "Conectar";
                    }

                }

                this.TopMost = true;
            }
            else
            {
                if (m_IsServer)
                {
                    m_server.Stop();
                    m_IsServer = false;
                }

                if (m_IsClient)
                {
                    m_client.Disconnect();
                    m_IsClient = false;
                }

                btnStart.Text = "Conectar";
            }
        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {

        }

        public async Task DelayedAction(int millisecondsDelay, Action action)
        {
            await Task.Delay(millisecondsDelay);
            action?.Invoke();
        }

        private void ValidateTicket(TextBox textBox)
        {
            TicketDataClass ticket = AppManager.ValidateTicket(textBox.Text);

            string nomeImagem;

            Label[] labels = { label_IdIngresso, label_TicketName, label_CPF, label_ParticipantName, label_Email, label_Telefone, label_TicketValidate };
            Color cor;
            if (ticket == null)
            {

                nomeImagem = "times-circle.png";

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                {
                    pictureBox_TicketValidate.Image = Image.FromStream(stream);
                }

                AppManager.SetFormTicketData(labels);

                label_TicketValidate.Text = "INGRESSO \r\n INEXISTENTE";
                cor = ColorTranslator.FromHtml("#A1432E");
                panel_TicketValidate.BackColor = cor;


                textBox.Text = string.Empty;

            }
            else if (ticket.code == textBox.Text)
            {
                switch (ticket.status)
                {
                    case "active":
                    case "offline":

                        nomeImagem = "check-circle.png";

                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                        {
                            pictureBox_TicketValidate.Image = Image.FromStream(stream);
                        }

                        AppManager.SetFormTicketData(labels, ticket);

                        label_TicketValidate.Text = "INGRESSO \r\n VÁLIDO";
                        cor = ColorTranslator.FromHtml("#2EA15C");
                        panel_TicketValidate.BackColor = cor;

                        textBox.Text = string.Empty;

                        break;
                    case "canceled":
                        nomeImagem = "exclamation-circle.png";

                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                        {
                            pictureBox_TicketValidate.Image = Image.FromStream(stream);
                        }

                        AppManager.SetFormTicketData(labels);

                        label_TicketValidate.Text = "INGRESSO \r\n CANCELADO";
                        cor = ColorTranslator.FromHtml("#2EA15C");
                        panel_TicketValidate.BackColor = cor;

                        textBox.Text = string.Empty;
                        break;

                    case "used":
                        nomeImagem = "exclamation-circle.png";

                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                        {
                            pictureBox_TicketValidate.Image = Image.FromStream(stream);
                        }

                        AppManager.SetFormTicketData(labels);

                        label_TicketValidate.Text = "INGRESSO \r\n JÁ USADO";
                        cor = ColorTranslator.FromHtml("#2EA15C");
                        panel_TicketValidate.BackColor = cor;

                        textBox.Text = string.Empty;
                        break;

                    case "denied":
                        nomeImagem = "exclamation-circle.png";

                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                        {
                            pictureBox_TicketValidate.Image = Image.FromStream(stream);
                        }

                        AppManager.SetFormTicketData(labels);

                        label_TicketValidate.Text = "INGRESSO \r\n NEGADO";
                        cor = ColorTranslator.FromHtml("#2EA15C");
                        panel_TicketValidate.BackColor = cor;

                        textBox.Text = string.Empty;


                        break;

                    default:
                        break;
                }



            }
        }

        private async void LinkScronizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox_Events.SelectedIndex != 0)
                {
                    await ConectionServer.GetEvent(comboBox_Events.SelectedIndex);
                    DialogResult result = MessageBox.Show("Sincronizado com o servidor", "Sincronized", MessageBoxButtons.OK);

                    if (result == DialogResult.OK)
                    {
                        AppManager.SetEventSqlData(AppManager.SelectedEventData);

                        for (int i = 0; i < AppManager.SelectedEventData.ticket_classes.Length; i++)
                        {
                            AppManager.SetTicketClasseSqlData(AppManager.SelectedEventData.ticket_classes[i], AppManager.SelectedEventData.id);
                        }

                        for (int i = 0; i < AppManager.SelectedEventData.tickets.Length; i++)
                        {
                            AppManager.SetTicketSqlData(AppManager.SelectedEventData.tickets[i]);

                        }
                    }
                }
                else
                {
                    DialogResult result = MessageBox.Show("Selecione um Evento, antes de Sincronizar com o servidor", "Error when syncing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {

            }

        }

        private async void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                await ConectionServer.GetEvent();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                throw;
            }
        }

        private void ImageScronizar_Click(object sender, EventArgs e)
        {

        }

        private void textBox_TicketCode_TextChanged(object sender, EventArgs e)
        {

            if (textBox_TicketCode.Text.Length >= 12)
            {
                ValidateTicket(textBox_TicketCode);
            }
        }

        private void comboBox_Events_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_Events.SelectedIndex > 0)
            {
                Label[] labels = { label_ParentalRating, label_EventName, label_EventDate, label_EventHours, label_EventAddress };
                AppManager.SetFormEventData(labels, (comboBox_Events.SelectedIndex - 1));

                
            }
        }

    }
}