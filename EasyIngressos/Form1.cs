using SuperSimpleTcp;
using System.Text;

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

        private void Form1_Load(object sender, EventArgs e)
        {

            btnSend.Enabled = false;
        }
        private void Events_Disconnected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server disconnected. {Environment.NewLine}";
            });
        }

        private void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"Server connected. {Environment.NewLine}";
            });
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort}: {Encoding.UTF8.GetString(e.Data)}{Environment.NewLine}";
            });
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} disconnected.{Environment.NewLine}";
                m_listClientIp.Remove(e.IpPort);
            });
        }

        private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate
            {
                txtInfo.Text += $"{e.IpPort} Connected. {Environment.NewLine}";
                m_listClientIp.Add(e.IpPort);
            });

        }

        private void btnSend_Click(object sender, EventArgs e)
        {

            if (m_IsServer)
            {
                if (m_server.IsListening)
                {
                    for (int i = 0; i < m_listClientIp.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(txtMessage.Text) && m_listClientIp[i] != null)
                        {
                            m_server.Send(m_listClientIp[i], txtMessage.Text);
                            txtInfo.Text += $"Server: {txtMessage.Text}{Environment.NewLine}";
                            txtMessage.Text = string.Empty;
                        }

                    }
                }
            }
            else
            {
                if (m_client.IsConnected)
                {
                    if (!string.IsNullOrEmpty(txtMessage.Text))
                    {
                        m_client.Send(txtMessage.Text);
                        txtInfo.Text += $"Me: {txtMessage.Text}{Environment.NewLine}";
                        txtMessage.Text = string.Empty;
                    }
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

            m_server = new SimpleTcpServer(txtIP.Text, 9090);
            m_client = new SimpleTcpClient(txtIP.Text, 9090);

            m_client.Events.Connected += Events_Connected;
            m_client.Events.Disconnected += Events_Disconnected;
            m_client.Events.DataReceived += Events_DataReceived;

            m_server.Events.ClientConnected += Events_ClientConnected;
            m_server.Events.ClientDisconnected += Events_ClientDisconnected;
            m_server.Events.DataReceived += Events_DataReceived;

            try
            {
                m_client.Connect();

                if (m_client.IsConnected)
                {
                    m_IsClient = true;
                    MessageBox.Show("Conectado com sucesso no servidor :" + txtIP.Text, "Connected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("Nao foi possivel conectar com o servidor :" + txtIP.Text, "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                m_server.Start();
                txtInfo.Text += $"Starting...{Environment.NewLine}";

                MessageBox.Show("Criado servidor para o IP" + txtIP.Text, "Server created successfully", MessageBoxButtons.OK, MessageBoxIcon.Information);

                m_IsServer = true;
            }
            finally
            {
                btnStart.Enabled = false;
                btnSend.Enabled = true;
            }
        }

        private void txtIP_TextChanged(object sender, EventArgs e)
        {

        }
    }
}