using System; 
using System.Drawing; 
using System.Text; 
using System.Windows.Forms; 
using System.IO;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.Net;
using System.Net.Sockets; 
using Microsoft.Win32;
using System.Threading;

namespace Barcode128
{
    public partial class Form1 : Form
    {
        //static Socket serverSocket =  new Socket(AddressFamily.InterNetwork,
    //SocketType.Stream, ProtocolType.Tcp);
      //  static private string guid = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        ReportParameter[] lim = new ReportParameter[2];
        delegate void SetTextCallback(string text);

        private const int BUFFER_SIZE = 1024;
        private const int PORT_NUMBER = 8082;
        static ASCIIEncoding encoding = new ASCIIEncoding();
        public Form1()
        {
            InitializeComponent();
        }


        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lbPort.Text = "Port: " + PORT_NUMBER;
           SetStartup();
            socket2();
            this.Visible = false;
            this.ShowInTaskbar = false;
            notifyIcon1.ShowBalloonTip(1000);

        }
 

        private void SetStartup()
        {
            
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk.SetValue(this.ProductName, Application.ExecutablePath);

        }



        //private void Socket()
        //{
        //    serverSocket.Bind(new IPEndPoint(IPAddress.Any, 8082));
        //    serverSocket.Listen(100);
        //    serverSocket.BeginAccept(null, 0, OnAccept, null);
        //}

        //private  void OnAccept(IAsyncResult result)
        //{

        //    byte[] buffer = new byte[1024];
        //    try
        //    {
        //        Socket client = null;
        //        string headerResponse = "";
        //        if (serverSocket != null && serverSocket.IsBound)
        //        {
        //            client = serverSocket.EndAccept(result);
        //            var i = client.Receive(buffer);
        //            headerResponse = (System.Text.Encoding.UTF8.GetString(buffer)).Substring(0, i);
        //            // write received data to the console
        //           // Console.WriteLine(headerResponse);
        //           // Console.WriteLine("=====================");
        //        }
        //        if (client != null)
        //        {
        //            /* Handshaking and managing ClientSocket */
        //            var key = headerResponse.Replace("ey:", "`")
        //                      .Split('`')[1]                     // dGhlIHNhbXBsZSBub25jZQ== \r\n .......
        //                      .Replace("\r", "").Split('\n')[0]  // dGhlIHNhbXBsZSBub25jZQ==
        //                      .Trim();

        //            // key should now equal dGhlIHNhbXBsZSBub25jZQ==
        //            var test1 = AcceptKey(ref key);

        //            var newLine = "\r\n";

        //            var response = "HTTP/1.1 101 Switching Protocols" + newLine
        //                 + "Upgrade: websocket" + newLine
        //                 + "Connection: Upgrade" + newLine
        //                 + "Sec-WebSocket-Accept: " + test1 + newLine + newLine
        //                 //+ "Sec-WebSocket-Protocol: chat, superchat" + newLine
        //                 //+ "Sec-WebSocket-Version: 13" + newLine
        //                 ;

        //            client.Send(System.Text.Encoding.UTF8.GetBytes(response));
        //            var i = client.Receive(buffer); // wait for client to send a message
        //            string browserSent = GetDecodedData(buffer, i);
        //            Console.WriteLine("BrowserSent: " + browserSent);
        //            string[] dataget = new String[4];

        //            dataget = browserSent.Split('|'); 
        //            //Console.WriteLine(dataget[1]);
        //            //  txtBarcode.Text = dataget[0].ToString();
        //            SetTextCode(dataget[0].ToString());
        //            SetTextHoTen(dataget[1].ToString());
        //            SetTextG(dataget[2].ToString());
        //            SetTextX(dataget[3].ToString());
        //            SetTextCopy(dataget[4].ToString());
        //            SetTextPrinter(dataget[5].ToString());
        //            PrinterSettings settings = new PrinterSettings();
        //            printDocument1.PrinterSettings.PrinterName = settings.PrinterName;
        //            if (dataget[5].ToString()!="")
        //            {
        //                printDocument1.PrinterSettings.PrinterName = lbPrinter.Text;
        //            }
        //            Int16 x = 1;
        //            if (txtCopy.Text!="")
        //            {
        //                x= (Int16)Int32.Parse(txtCopy.Text);

        //            } 
        //            printDocument1.PrinterSettings.Copies = x;
        //            //System.Threading.Thread.Sleep(100);
        //            printDocument1.Print();
        //            //Barcode128(dataget[0], dataget[1], dataget[2], dataget[3]);

        //            //pictureBox1_Click(new object(), new EventArgs());
        //            // Console.WriteLine("=====================");
        //            //now send message to client
        //            client.Send(GetFrameFromString("server sent."));
        //            //System.Threading.Thread.Sleep(5000);//wait for message to be sent



        //        }
        //    }
        //    catch (SocketException exception)
        //    {
        //        throw exception;
        //    }
        //    finally
        //    {
        //        if (serverSocket != null && serverSocket.IsBound)
        //        {
        //            serverSocket.BeginAccept(null, 0, OnAccept, null);
        //        }
        //    }
        //}
        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily == AddressFamily.InterNetwork)
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }

        private void socket2()
        {
            new Thread(() =>
            {
                try {
                IPAddress address = IPAddress.Parse(GetIP4Address());

                TcpListener listener = new TcpListener(address, PORT_NUMBER);

                // 1. listen
                listener.Start();

                Console.WriteLine("Server started on " + listener.LocalEndpoint);
                Console.WriteLine("Waiting for a connection...");

                Socket socket = listener.AcceptSocket();
                Console.WriteLine("Connection received from " + socket.RemoteEndPoint);

                var stream = new NetworkStream(socket);
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream);
                writer.AutoFlush = true;

                while (true)
                {
                    // 2. receive
                    string browserSent = reader.ReadLine();
                    Console.WriteLine("BrowserSent: " + browserSent);
                    string[] dataget = new String[4];

                    dataget = browserSent.Split('|');
                    //Console.WriteLine(dataget[1]);
                    //  txtBarcode.Text = dataget[0].ToString();
                    SetTextCode(dataget[0].ToString());
                    SetTextHoTen(dataget[1].ToString());
                    SetTextG(dataget[2].ToString());
                    SetTextX(dataget[3].ToString());
                    SetTextCopy(dataget[4].ToString());
                    SetTextPrinter(dataget[5].ToString());
                    PrinterSettings settings = new PrinterSettings();
                    printDocument1.PrinterSettings.PrinterName = settings.PrinterName;
                    if (dataget[5].ToString() != "")
                    {
                        printDocument1.PrinterSettings.PrinterName = lbPrinter.Text;
                    }
                    Int16 x = 1;
                    if (txtCopy.Text != "")
                    {
                        x = (Int16)Int32.Parse(txtCopy.Text);

                    }
                    printDocument1.PrinterSettings.Copies = x;
                    //System.Threading.Thread.Sleep(100);
                    printDocument1.Print();
                    // 3. send
                    //socket.Send(encoding.GetBytes("Hello "+str));

                    break;
                    // 3. send
                  //  writer.WriteLine("Hello " + browserSent);
                }
               

                 
                


                // 4. close
                stream.Close();
                socket.Close();
                listener.Stop();
                socket2();

            }
            catch (Exception ex) {
                if (chboxShowError.Checked == true)
                {
                    MessageBox.Show(ex.ToString());
                }
                
            }
            }).Start();
        }

        //public static T[] SubArray<T>(T[] data, int index, int length)
        //{
        //    T[] result = new T[length];
        //    Array.Copy(data, index, result, 0, length);
        //    return result;
        //}

        //private static string AcceptKey(ref string key)
        //{
        //    string longKey = key + guid;
        //    byte[] hashBytes = ComputeHash(longKey);
        //    return Convert.ToBase64String(hashBytes);
        //}

        //static SHA1 sha1 = SHA1CryptoServiceProvider.Create();
        //private static byte[] ComputeHash(string str)
        //{
        //    return sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(str));
        //}

        ////Needed to decode frame
        //public static string GetDecodedData(byte[] buffer, int length)
        //{
        //    byte b = buffer[1];
        //    int dataLength = 0;
        //    int totalLength = 0;
        //    int keyIndex = 0;

        //    if (b - 128 <= 125)
        //    {
        //        dataLength = b - 128;
        //        keyIndex = 2;
        //        totalLength = dataLength + 6;
        //    }

        //    if (b - 128 == 126)
        //    {
        //        dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
        //        keyIndex = 4;
        //        totalLength = dataLength + 8;
        //    }

        //    if (b - 128 == 127)
        //    {
        //        dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
        //        keyIndex = 10;
        //        totalLength = dataLength + 14;
        //    }

        //    if (totalLength > length)
        //        throw new Exception("The buffer length is small than the data length");

        //    byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

        //    int dataIndex = keyIndex + 4;
        //    int count = 0;
        //    for (int i = dataIndex; i < totalLength; i++)
        //    {
        //        buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
        //        count++;
        //    }

        //    return Encoding.UTF8.GetString(buffer, dataIndex, dataLength);
        //}

        ////function to create  frames to send to client 
        ///// <summary>
        ///// Enum for opcode types
        ///// </summary>
        //public enum EOpcodeType
        //{
        //    /* Denotes a continuation code */
        //    Fragment = 0,

        //    /* Denotes a text code */
        //    Text = 1,

        //    /* Denotes a binary code */
        //    Binary = 2,

        //    /* Denotes a closed connection */
        //    ClosedConnection = 8,

        //    /* Denotes a ping*/
        //    Ping = 9,

        //    /* Denotes a pong */
        //    Pong = 10
        //}

        ///// <summary>Gets an encoded websocket frame to send to a client from a string</summary>
        ///// <param name="Message">The message to encode into the frame</param>
        ///// <param name="Opcode">The opcode of the frame</param>
        ///// <returns>Byte array in form of a websocket frame</returns>
        //public static byte[] GetFrameFromString(string Message, EOpcodeType Opcode = EOpcodeType.Text)
        //{
        //    byte[] response;
        //    byte[] bytesRaw = System.Text.Encoding.UTF8.GetBytes(Message);

        //    byte[] frame = new byte[10];

        //    int indexStartRawData = -1;
        //    int length = bytesRaw.Length;

        //    frame[0] = (byte)(128 + (int)Opcode);
        //    if (length <= 125)
        //    {
        //        frame[1] = (byte)length;
        //        indexStartRawData = 2;
        //    }
        //    else if (length >= 126 && length <= 65535)
        //    {
        //        frame[1] = (byte)126;
        //        frame[2] = (byte)((length >> 8) & 255);
        //        frame[3] = (byte)(length & 255);
        //        indexStartRawData = 4;
        //    }
        //    else
        //    {
        //        frame[1] = (byte)127;
        //        frame[2] = (byte)((length >> 56) & 255);
        //        frame[3] = (byte)((length >> 48) & 255);
        //        frame[4] = (byte)((length >> 40) & 255);
        //        frame[5] = (byte)((length >> 32) & 255);
        //        frame[6] = (byte)((length >> 24) & 255);
        //        frame[7] = (byte)((length >> 16) & 255);
        //        frame[8] = (byte)((length >> 8) & 255);
        //        frame[9] = (byte)(length & 255);

        //        indexStartRawData = 10;
        //    }

        //    response = new byte[indexStartRawData + length];

        //    int i, reponseIdx = 0;

        //    //Add the frame bytes to the reponse
        //    for (i = 0; i < indexStartRawData; i++)
        //    {
        //        response[reponseIdx] = frame[i];
        //        reponseIdx++;
        //    }

        //    //Add the data bytes to the response
        //    for (i = 0; i < length; i++)
        //    {
        //        response[reponseIdx] = bytesRaw[i];
        //        reponseIdx++;
        //    }

        //    return response;
        //}







        private void reportViewer1_Load(object sender, EventArgs e)
        {
           
        }

        private void button2_Click(object sender, EventArgs e)
        {
           
            if (printPreviewDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
            //printDocument1.Print();
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            
            Zen.Barcode.Code128BarcodeDraw bc = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
           
           
            //  MessageBox.Show(txtBarcode.Text);
            var picImage = bc.Draw(txtBarcode.Text, 25);
            // e.Graphics.DrawImage(pictureBox1.Image, new PointF(100,100));

            pictureBox1.Image = picImage;
            // Print in the upper left corner at its full size.
            e.Graphics.DrawImage(picImage,
                  //e.PageBounds.Left + (e.MarginBounds.Width / 2) - (picImage.Width / 2), 10,
                 ((e.PageBounds.Width / 2)) - (picImage.Width / 2), 65,
                picImage.Width, picImage.Height);


            float height = 0;
            height = picImage.Height + 25;

            using (var sf = new StringFormat())
            {
                if (lbhoten.Text=="")
                {
                    lbhoten.Text = "DEMO";
                }

                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(txtBarcode.Text + " " + lbhoten.Text + "\n" +  lbG.Text + "\n" +   lbX.Text, new Font(this.Font.Name, 9), new SolidBrush(txtBarcode.ForeColor), e.PageBounds.Left + (e.PageBounds.Width / 2),30, sf);
            }
           





        }

        private void printPreviewControl1_Click(object sender, EventArgs e)
        {
          
        }

        private void SetTextCode(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtBarcode.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextCode);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtBarcode.Text = text;
            }
        }
        private void SetTextHoTen(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lbhoten.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextHoTen);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lbhoten.Text = text;
            }
        }

        private void SetTextG(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lbG.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextG);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lbG.Text = text;
            }
        }

        private void SetTextX(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lbX.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextX);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lbX.Text = text;
            }
        }

        private void SetTextPrinter(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.lbPrinter.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextPrinter);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.lbPrinter.Text = text;
            }
        }

        private void SetTextCopy(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtCopy.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetTextCopy);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.txtCopy.Text = text;
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //if the form is minimized  
            //hide it from the task bar  
            //and show the system tray icon (represented by the NotifyIcon control)  
            if (this.WindowState == FormWindowState.Minimized)
            {
                
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                
                e.Cancel = true;
                Hide();
                
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000);
            }
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show("Bạn chắc chắn muốn tắt phần mềm in barcode này không??",
                                     "Thông báo!!",
                                     MessageBoxButtons.YesNo);
            if (confirmResult == DialogResult.Yes)
            {
                Application.Exit();
            } 
        }
    }
}
