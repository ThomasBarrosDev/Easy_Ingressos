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
using System.Net.NetworkInformation;
using System.Runtime.ConstrainedExecution;
using System.Timers;
using System.Net;
using System.Runtime;
using System.Text.Json;

namespace EasyIngressos
{
    public partial class Form1 : Form
    {
        private SimpleTcpServer m_server;
        private SimpleTcpClient m_client;

        private bool m_IsServer = false;
        private bool m_IsClient = false;
        private List<string> m_listClientIp = new();
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            System.Timers.Timer timer = new System.Timers.Timer(10000);
            timer.Elapsed += CheckInternetConnection;
            timer.Start();

            Label[] labels = { label_IdIngresso, label_TicketName, label_CPF, label_ParticipantName, label_Email, label_Telefone, label_TicketValidate };
            AppManager.SetFormTicketData(labels);

            comboBox_Events.SelectedIndex = 0;

            CheckInternetConnection();

            if (CheckConnection())
            {
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
            else
            {
                comboBox_Events.Enabled = false;

                EventData lEventdata = SqliteConn.SelectEvent();

                AppManager.SetFormEventData(lEventdata.image_bytes, pictureBox_Event, labels, lEventdata);

            }
            textBox_TicketCode.Focus();
        }

        public void CheckInternetConnection(Object source, ElapsedEventArgs e)
        {
            CheckInternetConnection();
        }

        public void CheckInternetConnection()
        {
            string nomeImagem;
            Color cor;

            if (CheckConnection())
            {
                if (labelStatus.InvokeRequired)
                {
                    labelStatus.Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = "Online";
                        cor = ColorTranslator.FromHtml("#47ac70");
                        nomeImagem = "Online.png";
                        labelStatus.ForeColor = cor;

                        if (pictureStatus.InvokeRequired)
                        {
                            pictureStatus.Invoke((MethodInvoker)delegate
                            {
                                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                                {
                                    pictureStatus.Image = Image.FromStream(stream);
                                }
                            });
                        }
                        else
                        {
                            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                            {
                                pictureStatus.Image = Image.FromStream(stream);
                            }
                        }
                    });
                }
                else
                {
                    labelStatus.Text = "Online";
                    cor = ColorTranslator.FromHtml("#47ac70");
                    nomeImagem = "Online.png";
                    labelStatus.ForeColor = cor;

                    if (pictureStatus.InvokeRequired)
                    {
                        pictureStatus.Invoke((MethodInvoker)delegate
                        {
                            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                            {
                                pictureStatus.Image = Image.FromStream(stream);
                            }
                        });
                    }
                    else
                    {
                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                        {
                            pictureStatus.Image = Image.FromStream(stream);
                        }
                    }
                }

            }
            else
            {
                if (labelStatus.InvokeRequired)
                {
                    labelStatus.Invoke((MethodInvoker)delegate
                    {
                        labelStatus.Text = "Offline";
                        cor = ColorTranslator.FromHtml("#a1432e");
                        nomeImagem = "Offline.png";
                        labelStatus.ForeColor = cor;

                        if (pictureStatus.InvokeRequired)
                        {
                            pictureStatus.Invoke((MethodInvoker)delegate
                            {
                                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                                {
                                    pictureStatus.Image = Image.FromStream(stream);
                                }
                            });
                        }
                        else
                        {
                            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                            {
                                pictureStatus.Image = Image.FromStream(stream);
                            }
                        }
                    });
                }
                else
                {
                    labelStatus.Text = "Offline";
                    cor = ColorTranslator.FromHtml("#a1432e");
                    nomeImagem = "Offline.png";
                    labelStatus.ForeColor = cor;

                    if (pictureStatus.InvokeRequired)
                    {
                        pictureStatus.Invoke((MethodInvoker)delegate
                        {
                            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                            {
                                pictureStatus.Image = Image.FromStream(stream);
                            }
                        });
                    }
                    else
                    {
                        using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EasyIngressos.Resources." + nomeImagem))
                        {
                            pictureStatus.Image = Image.FromStream(stream);
                        }
                    }
                }

            }
        }
        public static bool CheckConnection()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var result = ping.Send("www.google.com", 1000);
                    return result.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
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

                if (m_IsServer)
                {
                    string code = Encoding.UTF8.GetString(e.Data);

                    TicketDataClass ticket = SqliteConn.SelectTicket(code);

                    string json = JsonSerializer.Serialize(ticket);

                    m_server.Send(e.IpPort ,json);

                }

                if (m_IsClient)
                {
                    string data = Encoding.UTF8.GetString(e.Data);

                    TicketDataClass ticket = JsonSerializer.Deserialize<TicketDataClass>(data);

                    Label[] labels = { label_IdIngresso, label_TicketName, label_CPF, label_ParticipantName, label_Email, label_Telefone, label_TicketValidate };
                    AppManager.SetFormTicketValidate(ticket, labels, pictureBox_TicketValidate, panel_TicketValidate, textBox_TicketCode, textBox_TicketCode.Text);

                }

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

                            m_client.Events.Connected += Events_Connected;
                            m_client.Events.Disconnected += Events_Disconnected;
                            m_client.Events.DataReceived += Events_DataReceived;
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

                        m_server.Events.ClientConnected += Events_ClientConnected;
                        m_server.Events.ClientDisconnected += Events_ClientDisconnected;
                        m_server.Events.DataReceived += Events_DataReceived;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnStart.Text = "Conectar";
                    }

                }
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

            textBox_TicketCode.Text = string.Empty;
            textBox_TicketCode.Focus();
        }

        private void ValidateTicket(TextBox textBox)
        {
            if (!m_IsServer && !m_IsClient)
            {
                MessageBox.Show("Você precisa criar um servidor local ou se conectar a um para acessar o banco de dados!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (m_IsServer)
            {
                SendValidateTicket(textBox.Text);
            }
            if (m_IsClient)
            {
                if (m_client.IsConnected)
                {
                    m_client.Send(textBox_TicketCode.Text);
                }
            }

            textBox.Text = string.Empty;
            textBox_TicketCode.Focus();
        }

        private void SendValidateTicket(string code)
        {
            TicketDataClass ticket = SqliteConn.SelectTicket(code);
            Label[] labels = { label_IdIngresso, label_TicketName, label_CPF, label_ParticipantName, label_Email, label_Telefone, label_TicketValidate };
            AppManager.SetFormTicketValidate(ticket, labels, pictureBox_TicketValidate, panel_TicketValidate, textBox_TicketCode, code);
        }

        private async void LinkScronizar_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox_Events.SelectedIndex != 0)
                {
                    await ConectionServer.PostEvent();

                    await ConectionServer.GetEvent(AppManager.EventsData[(comboBox_Events.SelectedIndex - 1)].id);

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

            textBox_TicketCode.Focus();
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

                using (WebClient client = new WebClient())
                {
                    byte[] data = client.DownloadData(AppManager.EventsData[(comboBox_Events.SelectedIndex - 1)].banner);
                    AppManager.SetFormEventData(data, pictureBox_Event, labels, AppManager.EventsData[(comboBox_Events.SelectedIndex - 1)]);
                }

                textBox_TicketCode.Focus();
            }
        }

    }
}