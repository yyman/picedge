using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        Bitmap img, imgTrim, imgTrimW, imgTrimH, imgTrimWf, imgTrimHf, imgTrimT, imgP, imgE;
        bool s = false, vf = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            DialogResult result = openDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                //fn = openDialog.SafeFileName;
                img = new Bitmap(openDialog.FileName);
                pictureBox1.Image = img;
                imgTrim = img;
            }
            s = false;
            vf = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int r, g, b, luminance;
            if (s == false)
            {
                for (int i = 0; i < imgTrim.Width; i++)
                    for (int j = 0; j < imgTrim.Height; j++)
                    {
                        r = (int)imgTrim.GetPixel(i, j).R;
                        g = (int)imgTrim.GetPixel(i, j).G;
                        b = (int)imgTrim.GetPixel(i, j).B;
                        luminance = (int)(0.298912 * (double)r + 0.586611 * (double)g + 0.114478 * (double)b);
                        Color col = Color.FromArgb(luminance, luminance, luminance);
                        imgTrim.SetPixel(i, j, col);
                    }
                this.pictureBox1.Image = imgTrim;
                imgTrimW = new Bitmap(imgTrim);
                imgTrimH = new Bitmap(imgTrim);
                imgTrimWf = new Bitmap(imgTrim);
                imgTrimHf = new Bitmap(imgTrim);
                imgTrimT = new Bitmap(imgTrim);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button2_Click(sender, e);
            int nr = 1;
            int[,] filter = new int[,]{
                {-1, 0, 1},
                {-2, 0, 2},
                {-1, 0, 1}
            };
            int[,] fw = new int[imgTrim.Width, imgTrim.Height];
            int[,] fh = new int[imgTrim.Width, imgTrim.Height];
            if (s == false)
            {
                for (int i = 1; i < imgTrim.Width - 1; i++)
                {
                    for (int j = 1; j < imgTrim.Height - 1; j++)
                    {
                        int sumW = 0;
                        int sumH = 0;
                        int sumT = 0;
                        for (int ii = 0; ii < 3; ii++)
                        {
                            for (int jj = 0; jj < 3; jj++)
                            {
                                sumW += imgTrim.GetPixel(i - 1 + ii, j - 1 + jj).R * filter[jj, ii];
                                sumH += imgTrim.GetPixel(i - 1 + ii, j - 1 + jj).R * filter[ii, jj];
                            }
                        }
                        
                        Color col;
                        if(sumW < 0)
                            col = Color.FromArgb(255, 255, -sumW/10);//Colorに変換
                        else
                            col = Color.FromArgb(0, 0, sumW/10);
                        imgTrimWf.SetPixel(i, j, col);
                        if (sumH < 0)
                            col = Color.FromArgb(255, 255, -sumH/10);//Colorに変換
                        else
                            col = Color.FromArgb(0, 0, sumH / 10);//Colorに変換
                        fw[i, j] = sumW;
                        fh[i, j] = sumH;
                        imgTrimHf.SetPixel(i, j, col);
                        sumW = Math.Abs(sumW);
                        sumH = Math.Abs(sumH);
                        sumT = (int)Math.Sqrt(Math.Pow(sumW, 2) + Math.Pow(sumH, 2));
                        sumW = sumW / nr;
                        sumW = (sumW >= 255) ? 255 : (sumW <= 0) ? 0 : sumW;
                        sumH = (sumH >= 255) ? 255 : (sumH <= 0) ? 0 : sumH;
                        sumT = (sumT >= 255) ? 255 : (sumT <= 0) ? 0 : sumT;
                        col = Color.FromArgb(sumW, sumW, sumW);//Colorに変換
                        imgTrimW.SetPixel(i, j, col);
                        col = Color.FromArgb(sumH, sumH, sumH);//Colorに変換
                        imgTrimH.SetPixel(i, j, col);
                        col = Color.FromArgb(sumT, sumT, sumT);//Colorに変換
                        imgTrimT.SetPixel(i, j, col);
                    }
                }
            }
            if (sender.Equals(button4) == true)
                this.pictureBox1.Image = imgTrimW;
            else if (sender.Equals(button5) == true)
                this.pictureBox1.Image = imgTrimH;
            else if (sender.Equals(button3) || sender.Equals(button6) == true)
                this.pictureBox1.Image = imgTrimT;
            s = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            double x, y, vx, vy, tx, ty, ttx, tty, v, tv;

            if (s == true && vf == false)
            {
                imgP = new Bitmap(imgTrimT.Width, imgTrimT.Height);
                imgE = new Bitmap(imgTrimT.Width, imgTrimT.Height);
                Graphics g = Graphics.FromImage(imgP);//imgに描画する
                Graphics ge = Graphics.FromImage(imgE);//imgに描画する
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(Color.White);
                ge.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                ge.Clear(Color.White);
                Pen p = new Pen(Color.FromArgb(50, Color.Black), 1);//ペンの色と太さ

                vf = true;

                for (int i = 1; i < imgTrimT.Width-1; i++)
                {
                    for (int j = 1; j < imgTrimT.Height-1; j++)
                    {
                        if (imgTrimT.GetPixel(i, j).R >= 100)
                        {
                            x = ((imgTrimWf.GetPixel(i, j).R > 0) ? -1 : 1) * imgTrimWf.GetPixel(i, j).B;
                            y = ((imgTrimHf.GetPixel(i, j).R > 0) ? -1 : 1) * imgTrimHf.GetPixel(i, j).B;
                            tx = x * Math.Cos(Math.PI / 2) - y * Math.Sin(Math.PI / 2);
                            ty = x * Math.Sin(Math.PI / 2) + y * Math.Cos(Math.PI / 2);
                            v = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                            tv = Math.Sqrt(Math.Pow(tx, 2) + Math.Pow(ty, 2));
                            vx = i + x / v * 20;
                            vy = j + y / v * 20;
                            ttx = i + tx / tv * 10;
                            tty = j + ty / tv * 10;
                            g.DrawLine(p, new Point(i, j), new Point((int)vx, (int)vy));
                            //g.DrawLine(p, new Point(i, j), new Point(i+(int)vvw, j+(int)vvh));
                            ge.DrawLine(p, new Point(i - (int)(tx / tv * 10), j - (int)(ty / tv * 10)), new Point((int)ttx, (int)tty));

                        }
                    }
                }
            }
            this.pictureBox1.Image = imgP;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (vf)
                this.pictureBox1.Image = imgE;
        }

    }
}
