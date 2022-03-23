using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace T
{
    public partial class Form1 : Form
    {
        int btnFlag = 0;
        public Form1()
        {
            InitializeComponent();
            Text = "Овчинников Никита";
            this.MouseClick+= new MouseEventHandler(Form1_MouseClick);
            m1 = new List<Point>();
            m2 = new List<Point>();
            m3 = new List<Point>();
        }

       
        Point p0 = new Point(); 
        int sizeP;  
        int size=0 ;  // количество точек исходного множества
        List<Point>A= new List<Point>(); // множество точек для которых строится оболочка
        List<Point> m1, m2, m3;
        private Button button1;
        private Button button2;
        private Button button3;
        int iter = 0;    // счетчик точек при вводе
        private Label label1;
        Pen pen = new Pen(Color.Black);
        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            
            Graphics g = CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //заносим координаты новой точки по щелчку мыши
            //A[iter] = new Point(PointToClient(Control.MousePosition).X, PointToClient(Control.MousePosition).Y);    
            //g.FillEllipse(Brushes.DarkBlue, A[iter].X - 3, A[iter].Y - 3, 6, 6);
            
            //
            if (btnFlag == 1)
            {
                m1.Add(new Point(PointToClient(Control.MousePosition).X, PointToClient(Control.MousePosition).Y));
                //A.Add(new Point(PointToClient(Control.MousePosition).X, PointToClient(Control.MousePosition).Y));
                iter++;
                size++;
            }
            else if (btnFlag == 2)
            {
                m2.Add(new Point(PointToClient(Control.MousePosition).X, PointToClient(Control.MousePosition).Y));
                //A.Add(new Point(PointToClient(Control.MousePosition).X, PointToClient(Control.MousePosition).Y));
                iter++;
                size++;
            }
            // рисуем саму оболочку
            /*if (iter >= size) {
                Point[] S = new Point[size];
                S = convex_build(A);    
                int j = 1,i=0;
                while (j < size && (S[j].X != 0 || S[j].Y != 0)) j++;
                Point[] polygon = new Point[j];
                for (i = 0; i < j; i++) polygon[i] = S[i];
                g.FillPolygon(Brushes.Violet, polygon);

                
                for (i = 0; i < size; i++)
                    g.FillEllipse(Brushes.DarkBlue, A[i].X - 3, A[i].Y - 3, 6, 6);
                iter = 0;
                
            }*/
            g.Clear(Color.White);
            draw(g);

        }
        public void draw(Graphics g) //метод для рисования на pictureBox
        {
            
            if (m1.Count > 0)
            {
                pen = new Pen(Color.Blue,2);
                for (int i = 0; i < m1.Count - 1; i++)
                {
                    g.DrawEllipse(pen, (m1[i].X - 3), (m1[i].Y - 3), 6, 6);
                    g.DrawLine(pen, m1[i], m1[i + 1]);
                }

                g.DrawEllipse(pen, (m1[m1.Count - 1].X - 3), (m1[m1.Count - 1].Y - 3), 6, 6);
                g.DrawLine(pen, m1[0], m1[m1.Count - 1]);
            }
            if (m2.Count > 0)
            {
                pen = new Pen(Color.Green, 2);
                for (int i = 0; i < m2.Count - 1; i++)
                {
                    g.DrawEllipse(pen, (m2[i].X - 3), (m2[i].Y - 3), 6, 6);
                    g.DrawLine(pen, m2[i], m2[i + 1]);
                }

                g.DrawEllipse(pen, (m2[m2.Count - 1].X - 3), (m2[m2.Count - 1].Y - 3), 6, 6);
                g.DrawLine(pen, m2[0], m2[m2.Count - 1]);


            }

            //sender.Image = bmp;
        }

        Point[] sort(Point[] P) // сортирует все точки множества в порядке возрастания полярного угла по отнощению к р0
        {
            bool t = true;
            while (t)
            {
                t = false;
                for (int j = 0; j < sizeP - 1; j++)
                    if (alpha(P[j]) > alpha(P[j + 1]))
                    {
                        Point tmp = new Point();
                        tmp = P[j];
                        P[j] = P[j + 1];
                        P[j + 1] = tmp;
                        t = true;
                    
                }else
                    if (alpha(P[j]) == alpha(P[j + 1]))
                    {
                        if (P[j].X > P[j + 1].X)
                        {
                            for (int k = j + 2; k < sizeP; k++)
                                P[k - 1] = P[k];
                            sizeP--;
                            t = true;
                        }
                        else
                            if(P[j+1].X > P[j].X)
                        {
                            for (int k = j + 1; k < sizeP; k++)
                                P[k - 1] = P[k];
                            sizeP--;
                            t = true;
                        } 
                       
                        
                    }
            }
            return P;
        }

        double angle(Point t0, Point t1, Point t2)  // через векторное произведение определяет поворот
                                                    //(если величина отрицательная - поворот против часовой стрелки, и наоборот)
        {
            return (t1.X - t0.X) * (t2.Y - t0.Y) - (t2.X - t0.X) * (t1.Y - t0.Y);
        }

        double alpha(Point t)   // считает полярный угол данной точки по отнощению к р0
        {
            t.X -= p0.X;
            t.Y = p0.Y - t.Y;
            double alph;
            if (t.X == 0) alph = Math.PI / 2;
            else
            {

                if (t.Y == 0) alph = 0;
                else alph = Math.Atan(Convert.ToDouble( t.Y) / Convert.ToDouble( t.X) );
                if (t.X < 0) alph += Math.PI;
                
            }
            return alph;
        }

        Point[] convex_build(List<Point> Q) //построение саммой оболочки(удаление) "лишних" вершин
        {
            //p0 - точка с минимальной координатой у или самая левая из таких точек при наличии совпадений

            p0 = Q[0];
            int ind = 0;
            for (int i = 1; i < size; i++)
                if (Q[i].Y > p0.Y) { p0 = Q[i]; ind = i; }
                else if (Q[i].Y == p0.Y && Q[i].X < p0.X) { p0 = Q[i]; ind = i; }

            //P остальные точки (все Q кроме р0) 
            sizeP = size - 1;
            Point[] P = new Point[sizeP];
            int j = 0;
            for (int i = 0; i < size; i++)
                if (i != ind)
                { P[j] = Q[i]; j++; }

            P = sort(P);  //сортируем Р в порядке возрастания полярного угла,измеряемого против часовой стрелки относительно р0

            Point[] S = new Point[size];  //стек ,который будет содержать вершины оболочки против часовой стрелки 
            S[0] = p0; S[1] = P[0]; S[2] = P[1];
            int last = 2;
            for (int i = 2; i < sizeP; i++)
            {
                while (last > 0 && angle(S[last - 1], S[last], P[i]) >= 0) last--;
                last++;
                S[last] = P[i];
            }
            return S;
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(108, 65);
            this.button1.TabIndex = 0;
            this.button1.Text = "Создать первый выпуклый многоугольник";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(13, 82);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(108, 65);
            this.button2.TabIndex = 1;
            this.button2.Text = "Создать второй выпуклый многоугольник";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(13, 153);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(108, 65);
            this.button3.TabIndex = 2;
            this.button3.Text = "Выпуклая оболочка их объединения";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(602, 511);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(313, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Выполнил студент ПРО-228 Овчинников Никита Евгеньевич";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(927, 533);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnFlag = 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            btnFlag = 2;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Graphics g = CreateGraphics();
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            for(int t = 0; t < m1.Count; t++)
            {
                A.Add(new Point(m1[t].X, m1[t].Y));
            }
            for (int t = 0; t < m2.Count; t++)
            {
                A.Add(new Point(m2[t].X, m2[t].Y));
            }
            //заносим координаты новой точки по щелчку мыши
            //A[iter] = new Point(PointToClient(Control.MousePosition).X, PointToClient(Control.MousePosition).Y);    
            //g.FillEllipse(Brushes.DarkBlue, A[iter].X - 3, A[iter].Y - 3, 6, 6);
            //btnFlag = 0;
            Point[] S = new Point[size];
            S = convex_build(A);
            int j = 1, i = 0;
            while (j < size && (S[j].X != 0 || S[j].Y != 0)) j++;
            Point[] polygon = new Point[j];
            for (i = 0; i < j; i++) polygon[i] = S[i];
            g.FillPolygon(Brushes.Violet, polygon);


            for (i = 0; i < size; i++)
                g.FillEllipse(Brushes.DarkBlue, A[i].X - 3, A[i].Y - 3, 6, 6);
            iter = 0;
            draw(g);

        }
    }
}
