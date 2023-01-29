using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace sockettest
{
    public partial class Form1 : Form
    {
        
        Server s;
        Client client;

        protected bool stopThread = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            s = new Server();
            label1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            s.StartServer();
        }

        class Client
        {
            public Socket socket;
            public const int size = 1024;
            public byte[] buffer;
            Action<string> writeText;

            public Client(Socket s)
            {
                socket = s;
                buffer = new byte[size];
            }
            public Client(int port, Action<string> writeText)
            {
                IPAddress ipAddr = IPAddress.Any;

                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
                socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                //socket.Bind(ipEndPoint);
                buffer = new byte[size];
                this.writeText = writeText;
            }

            public void Connect(IPEndPoint server)
            {
                writeText.Invoke("Connecting...\n");
                try
                {
                    socket.Connect(server);
                    writeText.Invoke("Connected\n");
                    BeginReceive(Receive);
                }
                catch(SocketException ex)
                {
                    writeText.Invoke(ex.Message+"\n");
                }
            }

            private void Receive(IAsyncResult result)
            {
                int bytesReceived = socket.EndReceive(result);
                string data = Encoding.UTF8.GetString(buffer, 0, bytesReceived).Trim();
                writeText.Invoke(data+"\n");
                BeginReceive(Receive);
            }
            public void BeginReceive(AsyncCallback callback)
            {
                socket.BeginReceive(buffer, 0, size, SocketFlags.None, callback, this);
            }

            public void Send(string message)
            {
                if (!socket.Connected) writeText.Invoke("Not connected");
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                socket.Send(bytes);
            }
        }

        class Server
        {
            Thread listenThread;
            List<Client> clients;
            Socket sListener;
            bool isRunning = false;
            bool canAccept = false;

            


            public void StartServer()
            {
                if (isRunning) return;
                canAccept = true;
                isRunning = true;
                IPAddress ipAddr = IPAddress.Any;
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 12345);
                Console.WriteLine(ipAddr);
                sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                clients = new List<Client>();
                listenThread = new Thread(listen);
                Console.WriteLine("Starting thread");
                listenThread.Start();
            }
            private void listen()
            {
                while (isRunning)
                {
                    if (canAccept)
                    {
                        Console.WriteLine("Begin accepting");
                        sListener.BeginAccept(AcceptConnection, sListener);
                        canAccept = false;
                    }
                    
                }
            }
            public void stop()
            {
                if(listenThread!=null && listenThread.IsAlive)
                    listenThread.Abort();
            }
            private void AcceptConnection(IAsyncResult result)
            {
                var client = new Client( sListener.EndAccept(result));
                Console.WriteLine("{0} connected", client.socket.RemoteEndPoint);
                client.BeginReceive(Receive);
                byte[] message = Encoding.UTF8.GetBytes(String.Format("{0} joined the chat",client.socket.RemoteEndPoint));
                foreach (Client c in clients)
                {
                    c.socket.Send(message, message.Length, SocketFlags.None);
                }
                clients.Add(client);
                
                Console.WriteLine("Clients: {0}", clients.Count);
                canAccept = true;
            }
            private void Receive(IAsyncResult result)
            {
                var client = (Client)result.AsyncState;
                if (!client.socket.Connected)
                {
                    Console.WriteLine("Client {0} disconnected", client.socket.RemoteEndPoint);
                    client.socket.Close();
                    clients.Remove(client);
                    client.socket.Dispose();
                    Console.WriteLine("Clients: {0}", clients.Count);
                    return;
                } 
                    
                int bytesReceived = client.socket.EndReceive(result);

                if (bytesReceived == 0)
                {
                    Console.WriteLine("Client {0} disconnected", client.socket.RemoteEndPoint);
                    
                    client.socket.Close();
                    clients.Remove(client);
                    client.socket.Dispose();
                    Console.WriteLine("Clients: {0}", clients.Count);
                    return;
                }
                

                string data = Encoding.UTF8.GetString(client.buffer, 0, bytesReceived).Trim();
                Console.WriteLine("From {0} received {1}", client.socket.RemoteEndPoint, data);
                byte[] message = Encoding.UTF8.GetBytes(String.Format("From {0} received {1}\n", client.socket.RemoteEndPoint, data));
                foreach (Client c in clients)
                {
                    c.socket.Send(message, message.Length, SocketFlags.None);
                }
                client.BeginReceive(Receive);           
            }

        }
   
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            s.stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            client = new Client(25123, AppendText);
            try
            {
                client.Connect(new IPEndPoint(IPAddress.Parse(textBox1.Text), 12345));
            }
            catch(System.FormatException ex)
            {
                AppendText(ex.Message + "\n");
            }
        }
        public void AppendText(string text)
        {
            Action<Label,string> d = delegate (Label l, string t) { l.Text = l.Text + t; };
            BeginInvoke(d,label1,text);          
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (client == null) return;
            client.Send(textBox2.Text);
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            if (client == null) return;
            client.Send(textBox2.Text);
        }
    }
}
