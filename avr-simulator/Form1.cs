
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace avr
{




    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //const int SREG = 0x3F;
        string[] lines;
        line q;
        AVR core = new AVR();
        int count = 0;
        int i = 0;
        byte[] ramcopy = new byte[1120];
        byte[] gprcopy = new byte[32];
        DateTime tim = new DateTime();
        int span = 0;
        Thread tr;
        private void Form1_Load(object sender, EventArgs e)
        {

            ReadHex("123.hex");
            WriteRom();
            
            for (int j=0;j<core.rom.Length;j++)
            {
                var ins = core.Decode(j);
                //textBox1.AppendText(ins.pc+" "+ins.opcode + " " + ins.arg1 + " " + ins.arg2+Environment.NewLine);
                dataGridView2.Rows.Add(new string[] { ins.pc.ToString(), ins.opcode + " " + ins.arg1 + " " + ins.arg2 });
            }


            tr = new Thread(tick);
            //tr.Start();
            //tr
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 200;
            t.Tick += upd;


            dataGridView1.Rows.Clear();
            while (i < 96)
            {
                if (i<32)
                {
                    dataGridView1.Rows.Add(new string[] { "R"+Convert.ToString(i), "0x"+Convert.ToString(core.ram[i], 16)+" "+ Convert.ToString(core.ram[i]) });
                }
                else
                    dataGridView1.Rows.Add(new string[] { Convert.ToString(i, 16), Convert.ToString(core.io[i-32], 16) });
                i++;
            }
            t.Start();



            label2.Text = "123";
            /*label3.Text = q.type.ToString();
            label4.Text = q.data[0].ToString();
            label5.Text = q.check.ToString();*/
        }
        void upd(object sender, EventArgs e)
        {

            label1.Text = "CMD = " + Convert.ToString(core.rom[core.PC], 16);
            label2.Text = "PC =" + core.PC.ToString();
            //textBox1.AppendText(Convert.ToString(core.rom[core.PC], 16)+Environment.NewLine);
            // textBox1.AppendText(core.Decode() + Environment.NewLine);
            dataGridView2.Rows[core.PC].Cells[0].Style.BackColor = Color.White;
            dataGridView2.Rows[core.PC].Cells[1].Style.BackColor = Color.White;
            //DecodeAddr(core.PC);
            //var ins= core.Decode(core.PC);
            //core.Execute(ins);
            dataGridView2.Rows[core.PC].Cells[0].Style.BackColor = Color.Yellow;
            dataGridView2.Rows[core.PC].Cells[1].Style.BackColor = Color.Yellow;
            label3.Text = "SREG = " + Convert.ToString(core.ram[0x5F]);

            for (int i = 0; i < core.gpr.Length; i++)
            {

                if (core.gpr[i] != gprcopy[i])
                {
                    //dataGridView1.Rows.RemoveAt(i);
                    //dataGridView1.Rows.Insert(i, new string[] { Convert.ToString(i, 16), Convert.ToString(core.ram[i], 16) });
                    if (i < 32)
                    {
                        dataGridView1.Rows[i].SetValues(new string[] { "R" + Convert.ToString(i), "0x" + Convert.ToString(core.gpr[i], 16) + " " + Convert.ToString(core.gpr[i]) });
                    }
                    else
                        dataGridView1.Rows[i].SetValues(new string[] { Convert.ToString(i, 16), Convert.ToString(core.io[i-32], 16) });
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.BackColor = Color.Yellow;
                    dataGridView1.Rows[i].Cells[1].Style = style;
                }
                else
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[1].Style = style;
                }
            }

            core.gpr.CopyTo(gprcopy, 0);

            label4.Text = count.ToString();
            label5.Text = (double)(count - span)/200000 + " MHz";
            span = count;
            //count++;

        }
        void tick()
        {
            //tim = DateTime.Now;
            Instruction[] ins = new Instruction[core.rom.Length];
            for (int j = 0; j < core.rom.Length; j++)
            {
                ins[j] = core.Decode(j).copy();
            }

            while (true)
            {
                // DecodeAddr(core.PC);
                //for (int i = 0; i < 100; i++) ;
                core.Execute(ins[core.PC]);
                count++;
            }


        }


        void ReadHex(string file)
        {
            if (File.Exists("rom.bin"))
            {
                File.Delete("rom.bin");
            }
            q = new line();
            lines = File.ReadAllLines(file);
            FileStream fs = File.OpenWrite("rom.bin");
            int count = 0;
            int i = 0;
            while (i < lines.Length)
            {
                q = GetLine(i);
                byte l = 0;
                if (q.type == 0)
                {
                    for (l = 0; l < q.length; l++)
                    {
                        fs.WriteByte(q.data[l]);
                        count++;
                    }
                }
                i++;
            }
            while (count < 8192)
            {
                fs.WriteByte(0xFF);
                count++;
            }



            fs.Close();
        }

        void DecodeAddr(int addr)
        {
            bool flag = false;

            ushort cmd = core.rom[addr];
            //label1.Text = Convert.ToString(cmd & 0xFC00,16);
            byte Rd, Rr;
            Rd = (byte)((cmd & 0x1F0) >> 4);
            Rr = (byte)((cmd & 0xF) + ((cmd & 0x200) >> 5));
            switch (cmd & 0xFC00)
            {
                case 0x1C00:
                    core.ADC(Rd, Rr);
                    break;
                case 0x0C00:
                    core.ADD(Rd, Rr);
                    break;
                case 0x2000:
                    core.AND(Rd, Rr);
                    break;
                case 0x1400:
                    core.CP(Rd, Rr);
                    break;
                case 0x0400:
                    core.CPC(Rd, Rr);
                    break;
                case 0x1000:
                    core.CPSE(Rd, Rr);
                    break;
                case 0x2C00:
                    core.MOV(Rd, Rr);
                    break;
                case 0x9C00:
                    core.MUL(Rd, Rr);
                    break;
                case 0x2800:
                    core.OR(Rd, Rr);
                    break;
                case 0x1800:
                    core.SUB(Rd, Rr);
                    break;
                case 0x0800:
                    core.SBC(Rd, Rr);
                    break;
                case 0x2400:
                    core.EOR(Rd, Rr);
                    break;
                default:
                    flag = true;
                    break;
            }
            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0x30) >> 4);
                Rr = (byte)(cmd & 0xF + ((cmd & 0xC0) >> 2));
                switch (cmd & 0xFF00)
                {
                    case 0x9600:
                        core.ADIW(Rd, Rr);
                        break;
                    case 0x9700:
                        core.SBIW(Rd, Rr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }
            if (flag)
            {
                flag = false;
                ushort adr = (ushort)(cmd & 0x0FFF);
                switch (cmd & 0xF000)
                {
                    case 0xC000:
                        core.RJMP(adr);
                        break;
                    case 0xD000:
                        core.RCALL(adr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }
            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0x1F0) >> 4);
                switch (cmd & 0xFE0F)
                {
                    case 0x9405:
                        core.ASR(Rd);
                        break;
                    case 0x9400:
                        core.COM(Rd);
                        break;
                    case 0x940A:
                        core.DEC(Rd);
                        break;
                    case 0x9403:
                        core.INC(Rd);
                        break;
                    case 0x9406:
                        core.LSR(Rd);
                        break;
                    case 0x9401:
                        core.NEG(Rd);
                        break;
                    case 0x900F:
                        core.POP(Rd);
                        break;
                    case 0x920F:
                        core.PUSH(Rd);
                        break;
                    case 0x9407:
                        core.ROR(Rd);
                        break;
                    case 0x9402:
                        core.SWAP(Rd);
                        break;
                    case 0x9004:
                        core.LPM(Rd);
                        break;
                    case 0x9005:
                        core.LPM2(Rd);
                        break;
                    case 0x9006:
                        core.ELPM(Rd);
                        break;
                    case 0x9007:
                        core.ELPM2(Rd);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }

            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0x70) >> 4);

                switch (cmd & 0xFF8F)
                {
                    case 0x9488:
                        core.BCLR(Rd);
                        break;
                    case 0x9408:
                        core.BSET(Rd);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }


            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0x1F0) >> 4);
                if ((cmd & 0xF000) == 0x9000)
                {
                    switch (cmd & 0xFE00)
                    {
                        
                        case 0x9000:
                            core.LD(Rd, (ushort)(cmd & 0x3C0F));
                            break;                                              
                        case 0x9200:
                            core.ST(Rd, (ushort)(cmd & 0x3C0F));
                            break;                      
                        default:
                            flag = true;
                            break;
                    }

                }
                else if ((cmd & 0xD000) == 0x8000)
                {
                    switch (cmd & 0xF200)
                    {
                        case 0x8000:
                            core.LD(Rd, (ushort)(cmd & 0x3C0F));
                            break;                       
                        case 0xA000:
                            core.LD(Rd, (ushort)(cmd & 0x3C0F));
                            break;
                        case 0x8200:
                            core.ST(Rd, (ushort)(cmd & 0x3C0F));
                            break;                        
                        case 0xA200:
                            core.ST(Rd, (ushort)(cmd & 0x3C0F));
                            break;
                        default:
                            flag = true;
                            break;
                    }
                }
                else
                {
                    flag = true;
                }
                
            }
            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0x1F0) >> 4);
                Rr = (byte)(cmd & 0x7);
                switch (cmd & 0xFE08)
                {
                    case 0xF800:
                        core.BLD(Rd, Rr);
                        break;
                    case 0xFA00:
                        core.BST(Rd, Rr);
                        break;
                    case 0xFC00:
                        core.SBRC(Rd, Rr);
                        break;
                    case 0xFE00:
                        core.SBRS(Rd, Rr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }

            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0x3F8) >> 3);
                Rr = (byte)(cmd & 0x7);
                switch (cmd & 0xFC00)
                {
                    case 0xF000:
                        core.BRBS(Rd, Rr);
                        break;
                    case 0xF400:
                        core.BRBC(Rd, Rr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }
            if (flag)
            {
                flag = false;
                switch (cmd)
                {
                    case 0x0000:
                        core.NOP();
                        break;
                    case 0x9409:
                        core.IJMP();
                        break;
                    case 0x9508:
                        core.RET();
                        break;
                    case 0x9518:
                        core.RETI();
                        break;
                    case 0x9588:
                        core.SLEEP();
                        break;
                    case 0x95A8:
                        core.WDR();
                        break;
                    case 0x95C8:
                        core.LPM();
                        break;
                    case 0x95E8:
                        core.SPM();
                        break;
                    case 0x9509:
                        core.ICALL();
                        break;
                    case 0x95D8:
                        core.ELPM();
                        break;
                    default:
                        flag = true;
                        break;
                }
            }

            if (flag)
            {
                flag = false;
                Rd = (byte)(((cmd & 0x600) >> 5) + (cmd & 0xF));
                Rr = (byte)((cmd & 0x1F0) >> 4);
                switch (cmd & 0xF800)
                {
                    case 0xB000:
                        core.IN(Rr, Rd);
                        break;
                    case 0xB800:
                        core.OUT(Rd, Rr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }

            if (flag)
            {
                flag = false;
                Rd = (byte)(((cmd & 0x70) >> 4));
                Rr = (byte)((cmd & 0x7));
                switch (cmd & 0xFFC8)
                {
                    case 0x0300:
                        core.MULSU(Rd, Rr);
                        break;
                    case 0x0308:
                        core.FMUL(Rd, Rr);
                        break;
                    case 0x0380:
                        core.FMULS(Rd, Rr);
                        break;
                    case 0x0388:
                        core.FMULSU(Rd, Rr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }
            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0xF0) >> 4);
                Rr = (byte)(cmd & 0xF);
                switch (cmd & 0xFF00)
                {
                    case 0x0100:
                        core.MOVW(Rd, Rr);
                        break;
                    case 0x0200:
                        core.MULS((byte)(Rd + 16), (byte)(Rr + 16));
                        break;
                    default:
                        flag = true;
                        break;
                }
            }

            if (flag)
            {
                flag = false;
                Rd = (byte)(((cmd & 0xF0) >> 4));
                Rr = (byte)(((cmd & 0xF00) >> 4) + (cmd & 0xF));
                switch (cmd & 0xF000)
                {
                    case 0x3000:
                        core.CPI(Rd, Rr);
                        break;
                    case 0x4000:
                        core.SBCI(Rd, Rr);
                        break;
                    case 0x5000:
                        core.SUBI(Rd, Rr);
                        break;
                    case 0x6000:
                        core.ORI(Rd, Rr);
                        break;
                    case 0x7000:
                        core.ANDI(Rd, Rr);
                        break;
                    case 0xE000:
                        core.LDI(Rd, Rr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }

            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0xF8) >> 3);
                Rr = (byte)(cmd & 0x7);
                switch (cmd & 0xFF00)
                {
                    case 0x9800:
                        core.CBI(Rd, Rr);
                        break;
                    case 0x9900:
                        core.SBIC(Rd, Rr);
                        break;
                    case 0x9A00:
                        core.SBI(Rd, Rr);
                        break;
                    case 0x9B00:
                        core.SBIS(Rd, Rr);
                        break;

                    default:
                        flag = true;
                        break;
                }
            }
            if (flag)
            {
                flag = false;
                Rd = (byte)((cmd & 0xF8) >> 3);
                Rr = (byte)(cmd & 0x7);
                switch (cmd & 0xD208)
                {
                    case 0x8000:
                        core.LDD(Rd, Rr);
                        break;
                    case 0x8008:
                        core.LDD(Rd, Rr);
                        break;
                    case 0x8200:
                        core.STD(Rd, Rr);
                        break;
                    case 0x8208:
                        core.STD(Rd, Rr);
                        break;

                    default:
                        flag = true;
                        break;
                }
            }
            if (flag)
            {
                flag = false;
                int adr = ((cmd & 0x1F1) << 16) + core.rom[addr + 1];

                switch (cmd & 0xFE0E)
                {
                    case 0x940C:
                        core.JMP(adr);
                        break;
                    case 0x940E:
                        core.CALL(adr);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }
            if (flag)
            {
                flag = false;
                int adr = core.rom[addr + 1];
                Rd = (byte)((cmd & 0x1F0) >> 4);
                switch (cmd & 0xFE0F)
                {
                    case 0x9200:
                        core.STS(adr, Rd);
                        break;
                    case 0x9000:
                        core.LDS(adr, Rd);
                        break;
                    default:
                        flag = true;
                        break;
                }
            }


            // label3.Text = core.ram[16].ToString();
            //label4.Text = core.ram[0x5F].ToString();
        }




        void WriteRom()
        {
            if (File.Exists("rom.bin"))
            {
                int i = 0;
                FileStream fs = File.OpenRead("rom.bin");

                while (i < 4096)
                {
                    core.rom[i] = (ushort)(fs.ReadByte() + (fs.ReadByte() << 8));
                    i++;
                }
            }

        }



        line GetLine(int number)
        {
            int i = 0;
            line l = new line();
            l.length = Convert.ToByte(lines[number].Substring(1, 2), 16);
            l.data = new byte[l.length];
            l.adress = Convert.ToInt32(lines[number].Substring(3, 4), 16);
            l.type = Convert.ToByte(lines[number].Substring(7, 2), 16);
            for (i = 0; i < l.length; i++)
            {
                l.data[i] = Convert.ToByte(lines[number].Substring(9 + i * 2, 2), 16);
            }
            l.check = Convert.ToByte(lines[number].Substring(9 + l.length * 2, 2), 16);

            return l;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            DecodeAddr(Convert.ToInt32(numericUpDown1.Value));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            upd(sender, e);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            tr.Abort();
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }

    public class line
    {
        public byte length;
        public int adress;
        public byte type;
        public byte[] data;
        public byte check;


    }

}
