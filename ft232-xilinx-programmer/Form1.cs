using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FTD2XX_NET;
using System.IO;

namespace ft232
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
       
        private void Form1_Load(object sender, EventArgs e)
        {
            
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
           
            textBox1.Text= openFileDialog1.FileName;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            byte[] program = new byte[1];
            byte[] clk = new byte[16]{16,24,16,24,16,24,16,24,16,24,16,24,16,24,16,24};
            byte[] nil = new byte[1];
            byte[] read = new byte[1];
            program[0] = 16;
            
            nil[0] = 0;
            FTDI myFtdiDevice = new FTDI();
            UInt32 numBytesWritten = 0;
            UInt32 numBytesRead = 0;
            if (myFtdiDevice.OpenByIndex(0) == FTDI.FT_STATUS.FT_OK)
            {
                
                myFtdiDevice.SetBitMode(0x19, FTDI.FT_BIT_MODES.FT_BIT_MODE_SYNC_BITBANG);

                myFtdiDevice.SetBaudRate((uint)Convert.ToInt32(textBox3.Text));
                myFtdiDevice.Write(program, 1, ref numBytesWritten);
                System.Threading.Thread.Sleep(1);
                myFtdiDevice.Write(nil, 1, ref numBytesWritten);
                System.Threading.Thread.Sleep(1);
                int n = 0;

                while (true)
                {
                    myFtdiDevice.Write(program, 1, ref numBytesWritten);
                    myFtdiDevice.Read(read, 1, ref numBytesRead);
                    if ((read[0] & 2) == 2)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(10);
                    read[0] = 0;
                    numBytesRead = 0;
                   // n++;
                    if (n > 100) break;
                }
                if (n > 100) { label3.Text = "Нет сигнала INIT"; }
                else
                {
                    
                    myFtdiDevice.SetBitMode(0x19, FTDI.FT_BIT_MODES.FT_BIT_MODE_ASYNC_BITBANG);
                    System.Threading.Thread.Sleep(1);
                    byte[] file = new byte[Convert.ToInt32(textBox2.Text)];
                    byte[] data = new byte[Convert.ToInt32(textBox2.Text)*16];
                    int i = 0;
                    var fil = File.ReadAllBytes(textBox1.Text);
                    Array.Copy(fil,70, file, 0,Convert.ToInt32(textBox2.Text));
                    while (true)
                    {
                        data[i] = (byte)((byte)(file[i / 16] & 128) / 128 + 16);
                        i++;
                        data[i] = (byte)((data[i - 1] ^ 8) | 16);
                        file[i / 16] = (byte)(file[i / 16] << 1);
                        i++;
                        if (i > Convert.ToInt32(textBox2.Text)*16-1) break;
                    }



                    myFtdiDevice.Write(data, data.Length, ref numBytesWritten);
                    myFtdiDevice.Write(clk, 16, ref numBytesWritten);
                    if (myFtdiDevice.IsOpen)
                        myFtdiDevice.Close();
                    label3.Text = "Все ОК";
                }
            }
            else { label3.Text = "Устройство не подключено"; }
        }
    }
}
