using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;
using System.IO;


namespace enginesim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }


        int angle = 0;
        System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer t1 = new System.Windows.Forms.Timer();
        double x, y, z, ax, ay;
        bool W, A, S, D, Space, C,E;

        //int rev = 0;

        int i = 0;
        private void trackBar3_ValueChanged(object sender, EventArgs e)
        {
           // t1.Interval = 10 + trackBar3.Value;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        Point Vect;
        static CrankShaft crankshaft = new CrankShaft();
        Piston[] p=new Piston[6];

        FileStream fs;
        private void Form1_Load(object sender, EventArgs e)
        {


            // sw = File.AppendText("321.wav");
           // fs = File.OpenWrite("321.wav");
            // crankshaft.angle = 10;
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);// отчитка окна 

           

            Gl.glClearColor(255, 255, 255, 1);// установка порта вывода в соответствии с размерами элемента anT 
            Gl.glViewport(0, 0, AnT.Width, AnT.Height);// настройка проекции 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(90, (float)AnT.Width / (float)AnT.Height, 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();// настройка параметров OpenGL для визуализации 
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            


            t1.Interval = 1;
            t1.Tick += update;
            t1.Start();

            System.Threading.Thread.Sleep(30);
            t.Interval = 30;
            t.Tick += tick;
            t.Start();

            Timer t2 = new Timer();
            t2.Interval = 1000;
            t2.Tick += tachometer;
            t2.Start();

            Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);

            for (int i = 0; i < 1; i++)
            {
                crankshaft.p.Add(new Piston(crankshaft, i));
                crankshaft.r.Add(new Rod(crankshaft, i));
            }

            chart1.ChartAreas[0].AxisY.Maximum = 1;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            crankshaft.accel = 0.05;
            
        }

        private void tachometer(object sender, EventArgs e)
        {
            label1.Text = (crankshaft.rev*2).ToString();
            crankshaft.rev = 0;
        }
        private void tick(object sender, EventArgs e)
        {




            

            if (AnT.Focused)
            {
                Vect = new Point(Cursor.Position.X - Screen.PrimaryScreen.Bounds.Width / 2, Cursor.Position.Y - Screen.PrimaryScreen.Bounds.Height / 2);
                Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
               
                double sin = Math.Sin(ax) / 30;
                double cos = Math.Cos(ax) / 30;
                if (W) { x += sin; z += cos; y += Math.Tan(ay) / 30; }
                if (A) { z -= sin; x += cos; }
                if (S) { x -= sin; z -= cos; y -= Math.Tan(ay) / 30; }
                if (D) { z += sin; x -= cos; }
                
                if (Space) y += 0.01;
                if (C) y -= 0.01;
               
                ax += -(double)Vect.X / 300;
                ay += -(double)Vect.Y / 300;
               
            }
            else
            {
                W = A = S = D = Space = C=E = false;
                
            }
            Gl.glLoadIdentity();
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Glu.gluLookAt(x, y, z, x + Math.Sin(ax), y + Math.Tan(ay), z + Math.Cos(ax), 0, 1, 0);

           

            Gl.glBegin(Gl.GL_QUADS);
           Gl.glColor3d(0.8, 1, 0.8);
            Gl.glVertex3d(-5, 0, -5);
            Gl.glColor3d(0.8, 0.9, 0.8);
            Gl.glVertex3d(-5, 0, 5);
           Gl.glColor3d(0.8, 1, 0.8);
            Gl.glVertex3d(5, 0, 5);
           Gl.glColor3d(0.8, 0.9, 0.8);
            Gl.glVertex3d(5, 0, -5);
            Gl.glEnd();


            Gl.glColor3d(0.5, 0.5, 0.5);

            Gl.glPushMatrix();
            Gl.glTranslated(1, 0, 0);
           // Glut.glutSolidCube(1);
            Gl.glPopMatrix();


            //crankshaft.angle = angle;
            crankshaft.Draw();
           // crankshaft.rF = 0;
            
            
            foreach (Piston p in crankshaft.p)
            {
                p.Draw();
            }
            foreach (Rod r in crankshaft.r)
            {
                r.Draw();
            }
            
            //crankshaft.ang = 0;




            Gl.glFlush();
            AnT.Invalidate();
            


        }

        public void update(object sender, EventArgs e)
        {
            angle += 10;
            if (angle > 359) angle = 0;
            // crankshaft.angle = angle;
            //Gl.glPushMatrix();
            // Gl.glMatrixMode(Gl.GL_PROJECTION);
           
           // label1.Text = ((crankshaft.p[0].fuel * 14.7 / crankshaft.p[0].air).ToString()+"\n" +crankshaft.p[0].air.ToString());
            chart1.Series[0].Points.Add(crankshaft.p[0].vol.T/1000);
            chart1.Series[1].Points.Add(crankshaft.p[0].intakepipe.p /100000);
            chart1.Series[2].Points.Add(Math.Min(50, Math.Max(0, crankshaft.p[0].fuel*14.7/crankshaft.p[0].air/2)));
            if (chart1.Series[0].Points.Count > 100) {
                chart1.Series[0].Points.RemoveAt(0);
                chart1.Series[1].Points.RemoveAt(0);
                chart1.Series[2].Points.RemoveAt(0);
            }

           // trackBar2.Value = (int)(crankshaft.accel*100/ (crankshaft.v/200+1));

            crankshaft.accel = (double)trackBar1.Value* (double)trackBar1.Value / 10000;
            crankshaft.mix= (double)trackBar2.Value / 100;
            crankshaft.addang= trackBar4.Value;
            crankshaft.mult=1/(1+ (double)trackBar3.Value);
            crankshaft.F = -crankshaft.v * trackBar5.Value*10;
            if (E) { crankshaft.F += 50000; }
            crankshaft.Update();
            crankshaft.rF = 0;

            //fs.WriteByte(Convert.ToByte( crankshaft.atmo*64));


            foreach (Piston p in crankshaft.p)
            {
                p.Update();
            }
            foreach (Rod r in crankshaft.r)
            {
                r.Update();
            }

           
        }



        private void AnT_MouseDown(object sender, MouseEventArgs e)
        {           
            this.Focus();
        }
        private void AnT_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                W = false;
            }
            if (e.KeyCode == Keys.A)
            {
                A = false;
            }
            if (e.KeyCode == Keys.S)
            {
                S = false;
            }
            if (e.KeyCode == Keys.D)
            {
                D = false;
            }
            if (e.KeyCode == Keys.E)
            {
                E = false;
            }
            if (e.KeyCode == Keys.Space) Space = false;
            if (e.KeyCode == Keys.C) C = false;

            e.Handled = true;
        }
        private void AnT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                W = true;
            }
            if (e.KeyCode == Keys.A)
            {
                A = true;
            }
            if (e.KeyCode == Keys.S)
            {
                S = true;
            }
            if (e.KeyCode == Keys.D)
            {
                D = true;
            }
            if (e.KeyCode == Keys.E)
            {
                E = true;
            }
            if (e.KeyCode == Keys.Space) Space = true;
            if (e.KeyCode == Keys.C) C = true;
            e.Handled = true;
        }
    }

    class CrankShaft
    {
        public int rev;
        public double angle;
         public double I = 400;
        public double v;
        public double rF;
        public double F;
       public double a;
        public double ang;
        public  double accel=0.5;
        public double mix=0.5;
        public double manifoldAir;
        public double mafair;
        public double mafvalve;
        public double addang = 0;
        public bool inject = false;
        public double mult = 1;
        public double exhaust1;
        public double exhaust2;
        public double atmo;
        public List<Rod> r=new List<Rod>();
       public List<Piston> p=new List<Piston>();
        public Volume mafair1=new Volume(0.000216);
        public Volume mafair2 = new Volume(0.0006);       


        public void Update()
        {           
            F += rF * Math.Sign(-1);
            //F += -v * 0.08 * Math.Abs(v);
            F += -v * 0.06 * Math.Abs(v);
            a = F / I;
            v += a / 100*mult;

            //mafvalve =Math.Max(0, (100000 - mafair1.p) / 100000);
           // mafvalve = 1 - mafair;
           // mafair += (1 - mafair)*mafvalve * mult;
            
           
         
            




            manifoldAir += accel * (1 - manifoldAir) / 2 * mult;
           // mafair-= accel * (mafair - manifoldAir) / 2 * mult;


            angle += v / 0.5 / 100 * mult;
            // angle += v / 0.5 / 100;
            ang = angle / 2;
            //ang += addang;
            if (angle > 719) {

                angle -= 719;
                rev++;
                foreach (Piston p in p)
                {
                    p.intakefuel += mix/100;
                    
                }
            }
            else if (angle < 0) angle += 720;

            if (ang > 719) ang -= 719;
            else if (ang < 0) ang += 719;

        }


        public void Draw()
        {
           




            Gl.glColor3d(0.5, 0.5, 0.5);
            
            Gl.glRotated(90, 0, 1, 0);
           
            Gl.glPushMatrix();
            Gl.glRotated(angle, 0, 0, 1);
            for (int i = 0; i < 1; i++)
            {

                Gl.glPushMatrix();
                Gl.glTranslated(0, 0.4, 0);
                Glut.glutSolidCylinder(0.15, 0.4, 10, 10);
                Gl.glTranslated(0, -0.2, 0);
                Gl.glScaled(0.4, 0.6, 0.2);
                Gl.glColor3d(0.6, 0.6, 0.6);
                Glut.glutSolidCube(1);

                Gl.glTranslated(0, 0, 2);
                Glut.glutSolidCube(1);
                Gl.glColor3d(0.5, 0.5, 0.5);
                Gl.glPopMatrix();
                Gl.glTranslated(0, 0, 0.4);
                Glut.glutSolidCylinder(0.15, 0.4, 10, 10);
                Gl.glTranslated(0, 0, 0.49);
                if (i > 2) Gl.glRotated(120, 0, 0, 1);
                if (i < 2) Gl.glRotated(120, 0, 0, -1);
            }
            
            Gl.glTranslated(0, 0, -0.25);
            Gl.glColor3d(0.5, 0.5, 0.5);
            Glut.glutSolidCylinder(1.2, 0.2, 10, 10);
            Gl.glPopMatrix();
            Gl.glPushMatrix();
            Gl.glTranslated(0, 3, -0.5);
            
            Gl.glRotated(ang, 0, 0, 1);
            Gl.glColor3d(0.55, 0.55, 0.55);
            Glut.glutSolidCylinder(0.5, 0.1, 10, 10);
            Gl.glColor3d(0.5, 0.5, 0.5);
            Glut.glutSolidCylinder(0.1, 1, 10, 10);

           
            Gl.glTranslated(0, 0, 0.4225);
            Gl.glColor3d(0.4, 0.4, 0.4);
            int t = 0;

            for (int i = 0; i < 1; i++)
            {

                switch (i)
                {
                    case 0:
                        t = 0;
                        break;
                    case 1:
                        t = 4;
                        break;
                    case 2:
                        t = 2;
                        break;
                    case 3:
                        t = 5;
                        break;
                    case 4:
                        t = 1;
                        break;
                    case 5:
                        t = 3;
                        break;
                }
            }
                Gl.glPushMatrix();
                Gl.glTranslated(0, 0, 0.89 * t);
                Gl.glRotated(-60*(double)0, 0, 0, 1);

                Gl.glColor3d(0.9, 0.4, 0.4);
                Gl.glPushMatrix();
                Gl.glRotated(-43, 0, 0, 1);
                Gl.glTranslated(0, -0.15, 0);
                Gl.glScaled(0.2, 0.3, 0.2);
                Glut.glutSolidCylinder(0.5, 1, 10, 10);
                //Glut.glutSolidCube(1);
                Gl.glPopMatrix();

                // Gl.glTranslated(0, 0, 0.5);

                Gl.glColor3d(0.4, 0.4, 0.4);
                Gl.glPushMatrix();
                Gl.glRotated(36.75, 0, 0, 1);
                Gl.glTranslated(0, -0.15, 0.3);
                Gl.glScaled(0.2, 0.3, 0.2);
                Glut.glutSolidCylinder(0.5, 1, 10, 10);
                //Glut.glutSolidCube(1);
                Gl.glPopMatrix();
                Gl.glPopMatrix();

                
            
            Gl.glPopMatrix();

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glDepthMask(Gl.GL_FALSE);

            //Gl.glColor4d(0.0, 0.0, manifoldAir, 0.1);
            Gl.glColor4d(0.0, 0.0, mafair2.p/100000, 0.1);

            Gl.glPushMatrix();
            Gl.glTranslated(-1.2, 2.3, -1.15);
            //Glut.glutSolidCylinder(0.3, 2.3, 10, 10);
            Gl.glTranslated(0, 0.6, 1.15);
            Gl.glPushMatrix();
            Gl.glTranslated(0, -0.3, 0);
            Gl.glRotated(accel*90, 0, 0, 1);
            Gl.glRotated(90, 0, 1, 0);
            Gl.glColor3d(0.5, 0.5, 0.5);           
            Glut.glutSolidCylinder(0.3, 0.01, 10, 10);
            Gl.glPopMatrix();

            

            Gl.glPopMatrix();

            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glColor3d(0.6, 0.5, 0.5);
            Gl.glPushMatrix();
            Gl.glTranslated(0.8, 2.5, 0);
            Glut.glutSolidCylinder(0.25, 2.4, 10, 10);
            Gl.glTranslated(0, 0, 2.2);
            Gl.glPushMatrix();
            Gl.glRotated(45, 1, 1, 0);
            Gl.glColor3d(0.5, 0.4, 0.4);
            Glut.glutSolidCylinder(0.25, 0.4, 10, 10);
            Gl.glTranslated(0, 0, 3);
            Gl.glRotated(180, 1, 0, 0);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glDepthMask(Gl.GL_FALSE);

            Gl.glColor4d(0.5, 0.5, 0.5, exhaust1);
            Glut.glutSolidCone(0.5, 3, 10, 10);

            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glPopMatrix();
            Gl.glTranslated(0, 0, 0.5);
            Gl.glColor3d(0.6, 0.5, 0.5);
           // Glut.glutSolidCylinder(0.25, 2.4, 10, 10);
            Gl.glTranslated(0, 0, 2.2);
            Gl.glPushMatrix();
            Gl.glRotated(45, 1, 1, 0);
            Gl.glColor3d(0.5, 0.4, 0.4);
           // Glut.glutSolidCylinder(0.25, 0.4, 10, 10);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glDepthMask(Gl.GL_FALSE);

            Gl.glColor4d(0.5, 0.5, 0.5,exhaust2);
            Gl.glTranslated(0, 0, 3);
            Gl.glRotated(180, 1, 0, 0);
            Glut.glutSolidCone(0.5, 3, 10, 10);

            Gl.glPopMatrix();
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDisable(Gl.GL_BLEND);
            
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glColor3d(0.9, 0.9, 0.9);
            Gl.glTranslated(1, 3, -1);
            Gl.glRotated(90, 0, 1,0 );
            Glut.glutSolidCylinder(0.5, 0.1, 10, 10);
            Gl.glTranslated(0, 0, 0.1);
            Gl.glRotated(90, 1, 0, 0);
            Gl.glRotated(v/10, 0, -1, 0);
            Gl.glColor3d(1, 0, 0);
            Glut.glutSolidCylinder(0.01, 0.5, 10, 10);
            Gl.glPopMatrix();


        }

    }

    class Piston
    {
        public int num;
        public CrankShaft cr;
        public double pos=2;
        public double v;

        public double a;
        public double m = 1;
        public double rF;
        public double F;
        public double p;
        public double air;
        public double fuel;
        public double intakefuel;
        public double gas;
        public double intake;
        public double intakevalve;
        public double exhaustvalve;
        public double exhaust;
        public double pos1=2.15;
        public Volume vol = new Volume(0.000550);
        public Volume intakepipe = new Volume(0.005);



        public double torch;
        public bool fire = false;
        public double ratio = 1;
        public Piston(CrankShaft cr,int num)
        {
            this.cr = cr;
           
            this.num = num;
           // cr.p.Add(this);
        }

        public void Update()
        {

            intakepipe.m += intakefuel;
            intakepipe.fuel += intakefuel;
            intakefuel = 0;
            intakepipe.Update();
            intakepipe.fuelratio = intakepipe.fuel / intakepipe.m;
            fuel = vol.fuel;
            if (fire == false)
            {
                
                air = Math.Max(0, vol.m - fuel);
            }
            ratio = fuel*14.7 / air;
            if (torch > 0.5) fire = true;
            if (fuel<0.000001|air<0.000001) fire = false;
            if (fire)
            {

                vol.E += vol.fuel*50000000*(-5*Math.Pow(ratio,2)+10*ratio-4);
                vol.fuel = 0;
            }
            
           

 
            F = (100000-vol.p)*Math.PI * 0.04425 * 0.04425*5;
        
            a = F / m;
            v += a / 100 * cr.mult;


            //vol.Update();
            double dV = 0;
            
            vol.V = ((1.98 - pos) * 0.1 * Math.PI * 0.04425 * 0.04425);
            dV = vol.V - vol.V2;
            vol.V2 = vol.V;
            vol.Update();
            vol.E = Math.Max(0, vol.E - dV* vol.p);
            
           
                vol.Update();



            intakepipe.Atmo(cr.accel * cr.mult*0.02, 300);
            intakepipe.Transfer(vol,intakevalve * cr.mult*0.5);
            vol.Atmo(Math.Min(1,exhaustvalve)*cr.mult, 300);
            pos1 = pos;




            double ang = cr.ang;

            switch (num)
            {
                case 0:

                    break;
                case 1:
                    ang -= 240;
                    break;
                case 2:
                    ang -= 120;
                    break;
                case 3:
                    ang -= 300;
                    break;
                case 4:
                    ang -= 60;
                    break;
                case 5:
                    ang -= 180;
                    break;
            }


            ang += 5;

            ///* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/

            exhaustvalve = Math.Min(1, Math.Max(0, (Math.Sin((ang  -60) * Math.PI / 180)) - 0.4) * 2) * 1;
            intakevalve = Math.Min(1, Math.Max(0, (Math.Sin((ang - 50 - 79.75) * Math.PI / 180)) - 0.4) * 2) * 1;
            ang += cr.addang;
            torch = Math.Min(1, Math.Max(0, (Math.Sin((ang + 60) * Math.PI / 180)) - 0.9) * 10);
            //vol.E += torch*1000;
            //pos = Math.Min(3,Math.Max(0, pos));


        }

        public void Draw()
        {
            
            ///* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/


            Gl.glPushMatrix();
            Gl.glColor3d(0.5, 0.5, 0.5);
            Gl.glTranslated(0, 0, 0);
            //Glut.glutSolidCube(1);
            Gl.glRotated(90, 0, -1,0 );
            Gl.glRotated(90, -1, 0, 0);
            
            double angle = cr.angle;
            if (num == 1 | num == 4) angle -= 120;
            if (num == 2 | num == 3) angle -= 240;
            Gl.glTranslated(0.89*num+0.2, 0,pos-0.4);
            Glut.glutSolidCylinder(0.4425, 0.8, 10, 10);
            Gl.glTranslated(0, 0, 0.8);

            Gl.glColor4d(1-(-5 * Math.Pow(ratio, 2) + 10 * ratio - 4), 0, (-5 * Math.Pow(ratio, 2) + 10 * ratio - 4), 0.1);

            //Gl.glColor4d(gas/50, fuel*25*500, air, 0.1);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glDepthMask(Gl.GL_FALSE);
           // if (p > 9) { fuel = 0; air = 10; }
            Glut.glutSolidCylinder(0.4425, 1.98 - pos , 10, 10);

            Gl.glPopMatrix();
            Gl.glPushMatrix();
            Gl.glRotated(90, 0, -1, 0);
            Gl.glColor4d(0, 0, 0, 0.1);
            Gl.glTranslated(0.9 * num, 2.6, 0.2);
            //Gl.glRotated(10, 1, 0, 0);
           // Gl.glRotated(0, 0, -1, 0);

            Gl.glColor4d(0, 0, intakepipe.p/100000, 0.1);

            //Gl.glColor4d(0, intakefuel*25*500, intake, 0.1);
            Glut.glutSolidCylinder(0.2, 1, 10, 10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            
            Gl.glColor4d(0, 0, 0, 0.1);
            Gl.glTranslated(0.2, 2.5, 0.9 * num+0.3);
            Gl.glRotated(90, 0, 1, 0);
            //Gl.glRotated(10, 1, 0, 0);
            //Gl.glRotated((num - 2.5) * 15, 0, -1, 0);
            ;
            Gl.glColor4d(0.5, 0.5, 0.5, exhaust+0.1);
            Glut.glutSolidCylinder(0.2, 0.4, 10, 10);
            Gl.glPopMatrix();

            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDisable(Gl.GL_BLEND);

            Gl.glPushMatrix();
            
            Gl.glTranslated(0.2, 2.325, 0.89 * num + 0.3);
            Gl.glRotated(90, -1, 0, 0);
            
            Gl.glColor3d(0.55, 0.55, 0.55);

            Gl.glPushMatrix();
            Gl.glRotated(5, 0, 1, 0);
            Gl.glTranslated(0, 0, -exhaustvalve*0.1+0.1);           
            Glut.glutSolidCylinder(0.05, 0.8, 10, 10);
            Gl.glColor3d(0.6, 0.6, 0.6);
            Glut.glutSolidCylinder(0.2, 0.05, 10, 10);
            Gl.glPopMatrix();
            
            Gl.glPushMatrix();
            Gl.glColor3d(0.65, 0.65, 0.65);
            Gl.glTranslated(-0.1, 0,0.9);
            Gl.glRotated(exhaustvalve * 45, 0, 1, 0);
            Gl.glScaled(0.5, 0.2, 0.1);
            Glut.glutSolidCube(1);
            Gl.glPopMatrix();
            
            Gl.glColor3d(0.55, 0.55, 0.55);
            Gl.glPushMatrix();
            Gl.glTranslated(-0.4, 0.25, 0);
            Gl.glRotated(5, 0, -1, 0);
            Gl.glTranslated(0, 0, -intakevalve*0.1+0.1);            
            Glut.glutSolidCylinder(0.05, 0.8, 10, 10);
            Gl.glColor3d(0.6, 0.6, 0.6);
            Glut.glutSolidCylinder(0.2, 0.05, 10, 10);
            Gl.glPopMatrix();

            Gl.glPushMatrix();
            Gl.glColor3d(0.6, 0.6, 0.6);
            Gl.glTranslated(-0.3, 0.3, 0.9);
            Gl.glRotated(intakevalve * 45, 0, -1, 0);
            Gl.glScaled(0.5, 0.2, 0.1);
            Glut.glutSolidCube(1);
            Gl.glPopMatrix();

            Gl.glPushMatrix();         
            Gl.glTranslated(0, 0.3,0.05);
            Gl.glRotated(25, 0, 1, 0);
            Gl.glColor3d(torch+0.4, torch + 0.4, 0.4);
            Glut.glutSolidCylinder(0.08, 0.2, 10, 10);
            Gl.glColor3d(0.9, 0.9, 0.9);
            Glut.glutSolidCylinder(0.05, 0.5, 10, 10);
            Gl.glPopMatrix();

            Gl.glPopMatrix();

          
            

            

        }


    }

    class Rod
    {
        public Vector3 dir;
        int num;
        CrankShaft cr;
        public double xxx;
        Vector3 pos;
        public Rod(CrankShaft cr,int num)
        {
            this.cr = cr;
            this.num = num;
           // cr.r.Add(this);

        }

        public void Update()
        {
            double angle = cr.angle;
            if (num == 1 | num == 4) angle -= 120;
            if (num == 2 | num == 3) angle -= 240;
           pos = new Vector3(0.4 * Math.Sin(angle / (180 / Math.PI)), 0.4 * Math.Cos(angle / (180 / Math.PI)), 0);
            dir = new Vector3(0, cr.p[num].pos, 0) - pos;

            cr.p[num].pos = Math.Sqrt((1.5 * 1.5) - (0.4 * Math.Sin(angle / (180 / Math.PI)))) + 0.4 * Math.Cos(angle / (180 / Math.PI))
                ;
            //cr.p[num].rF = Vector3.Dot(dir.Normalize(),new Vector3(0,1,0)) *((1.5-dir.Length())*dir.Y * 1000 - cr.p[num].v*Math.Abs(cr.p[num].v)*10);

            Vector3 joint = new Vector3(pos.Y, pos.X, 0);
            xxx = Vector3.Dot(joint.Normalize(), dir.Normalize()) * Vector3.Dot(dir.Normalize(), new Vector3(0, 1, 0)) * cr.p[num].F;
            cr.rF += Vector3.Dot(joint.Normalize(), dir.Normalize()) * Vector3.Dot(dir.Normalize(), new Vector3(0, 1, 0)) * cr.p[num].F;
        }

        public void Draw()
        {
            

            Gl.glPushMatrix();
            ////  Gl.glRotated(90, 1,0, 0);
            Gl.glRotated(90, 0, -1, 0);
            Gl.glTranslated(0.89 * num + 0.2, pos.Y+(dir.Y/2), pos.X+dir.X / 2);
            Gl.glRotated(Math.Acos(Vector3.Dot(new Vector3(pos.Y + (dir.Y / 2), pos.X + dir.X / 2,0).Normalize(),new Vector3(0,1,0))) *(180/Math.PI)+90, 1, 0, 0);
            Gl.glScaled(0.15, 1.5, 0.2);
            //Gl.glColor3d(-cr.p[num].F/1000, 0.0, 0.0);
            Gl.glColor3d(0.4, 0.4, 0.4);
            Glut.glutSolidCube(1);
            
            Gl.glPopMatrix();
        }
    }

    public class Vector3
    {
        public double X, Y, Z;
        Random r = new Random();

        public Vector3(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 RandomVector3()
        {

            return new Vector3((r.NextDouble() * 2) - 1, (r.NextDouble() * 2) - 1, (r.NextDouble() * 2) - 1);
        }
        public Vector3 Normalize()
        {
            double l = Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2));

            return new Vector3(X / l, Y / l, Z / l);
        }

        static public double Dot(Vector3 a, Vector3 b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        }

         public double Length()
        {
            return Math.Sqrt( X * X + Y * Y + Z * Z);

        }
        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }
        public static Vector3 operator *(Vector3 left, double right)
        {
            return new Vector3(left.X * right, left.Y * right, left.Z * right);
        }


    }

   public class Volume
    {
        public double V;
        public double V2;
        public double p;
        public double T = 273;
        public double P;
        public double m;
        public double E;

        public double c;
        public double R;
        public double fuelratio = 0;
        public double fuel = 0;
        public Volume(double Vol)
        {
            this.V = Vol;
            m =100000 * V / (8.31 * T);
            E = 3 * m * 8.31 * T / (2);
        }
        public Volume(double Vol, double mass)
        {
            this.V = Vol;
            m = mass;
            E = 3 * m * 8.31 * T / (2);
        }
        public void Update()
        {
            /*
            pV = mRT /M;

            p= mRT / MV;
            m = MpV / RT;
            T= MpV / Rm;
            */
            T = E / (3 * m * 8.31 / (2));
            p = m * 8.31 * T / (V);


        }
        public void Transfer(Volume vol, double speed)
        {
            double mass = 0;
            double pp = (m + vol.m) * 8.31 * ((m * T + vol.m * vol.T) / (m + vol.m)) / ((V + vol.V));
            if (vol.p > p)
            {
                mass = (vol.p - pp) * vol.V / (8.31 * vol.T) * speed;
                if (mass < 0) return;
                E += 3 * mass * 8.31 * vol.T / (2);
                vol.E -= 3 * mass * 8.31 * vol.T / (2);
               // T = (m * T + mass * vol.T) / (m + mass);
                m += mass;
                fuel += mass * vol.fuelratio;
                vol.m -= mass;
                vol.fuel -= mass * vol.fuelratio;
                fuelratio = fuel / m;
            }
            if (vol.p < p)
            {
                mass = (p-pp) * V / (8.31 * T) * speed;
                if (mass < 0) return;
                E -= 3 * mass * 8.31 * T / (2);
                vol.E += 3 * mass * 8.31 * T / (2);
                //vol.T = (m * T - mass * vol.T) / (m - mass);
                m -= mass;
                fuel -= mass * fuelratio;
                vol.m += mass;
                vol.fuel += mass * fuelratio;
            }
            
            //vol.E = 3 * vol.m * 8.31 * vol.T / (2 * 0.029);
            Update();
            vol.Update();


        }
        public void Atmo(double speed, double Temp)
        {
            double mass = 0;
            if (100000 > p)
            {
                mass = (100000 - p) * V / (8.31 * Temp) * speed;
                T = (m * T + mass * Temp) / (m + mass);
            }
            if (100000 < p)
            {
                mass = (100000 - p) * V / (8.31 * T) * speed;
                fuel += mass * fuelratio;
            }


            m += mass;
            E = 3 * m * 8.31 * T / (2);
            Update();
        }
    }
}
