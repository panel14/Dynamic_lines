using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Dynamic_lines
{
    public partial class Form1 : Form
    {
        int width, height;
        int wMid, hMid;
        int stepSize;
        double lastAngle = 90;
        double angleMultiply = 3;
        int axisCount;
        bool isParty;

        CheckBox[] checks;
        List<Point> startPoints = new List<Point>();
        List<Axis> axes = new List<Axis>();
        Graphics g;
        Pen pen;
        Color backColor;
        Color penColor;

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel) return;
            penColor = colorDialog1.Color;
            pen.Color = penColor;
            label4.ForeColor = penColor;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            g.Clear(backColor);
            if (axisCount > numericUpDown1.Value)
            {
                axes.RemoveAt(axes.Count - 1);

                if (axes.Count % 2 == 0)
                    lastAngle *= 2;

                axisCount--;
            }

            else
            {
                double newAngle;
                if (axes.Count % 2 == 0)
                {
                    newAngle = lastAngle / 2;
                    lastAngle = newAngle;
                }
                else
                {
                    newAngle = lastAngle * angleMultiply;
                }

                int xStart = (int)(wMid - wMid * Math.Sin(newAngle));
                int yStart = (int)(hMid - hMid * Math.Cos(newAngle));
                int xEnd = width - xStart;
                int yEnd = width - yStart;

                Axis axis = new Axis(new Point(xStart, yStart), new Point(xEnd, yEnd), newAngle);
                axes.Add(axis);
                axisCount++;
            }

            foreach (Axis ax in axes)
                ax.Draw(ref g, ref pen);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.Cancel) return;
            g.Clear(colorDialog2.Color);
            backColor = colorDialog2.Color;
            label5.ForeColor = colorDialog2.Color;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isParty = checkBox1.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            wMid = width / 2;
            hMid = height / 2;
            stepSize = int.Parse(textBox1.Text);

            //Will be deleted
            startPoints.Add(new Point(wMid, 0));
            startPoints.Add(new Point(0, hMid));
            startPoints.Add(new Point(wMid, height));
            startPoints.Add(new Point(width, hMid));

            axes = new List<Axis>();
            axes.Add(new Axis(new Point(wMid, 0), new Point(wMid, height), 0));
            axes.Add(new Axis(new Point(0, hMid), new Point(width, hMid), 1.57));

            g = pictureBox1.CreateGraphics();
            pen = new Pen(Color.Black);

            foreach (Axis axis in axes)
                axis.Draw(ref g, ref pen);

            lastAngle = 1.57;
            axisCount = 2;
            backColor = Color.White;
            penColor = Color.Black;
            isParty = false;
            checks = new CheckBox[4] { checkBox4, checkBox3, checkBox2, checkBox5 };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            g.Clear(backColor);
            foreach (Axis axis in axes)
                axis.Draw(ref g, ref pen);

            if (!Int32.TryParse(textBox1.Text, out stepSize))
            {
                textBox1.Text = "Uncorrect.";
                return;
            }
            var sorted = axes.OrderBy(ax => ax.angle).ToArray();

            bool[] enabled = new bool[checks.Length];
            for (int i = 0; i < enabled.Length; i++)
                enabled[i] = !checks[i].Checked;

            pen.Color = penColor;

            for (int i = 0; i < axes.Count; i++)
            {        
                int next = (i + 1 == sorted.Length) ? 0 : i + 1;

                Axis first = sorted[i];
                Axis second = sorted[next];

                if (i + 1 == sorted.Length) 
                    second.InvertAxis();

                Axis.MergeAxes(first, second, stepSize, ref g, ref pen, isParty, enabled);

                if (i + 1 == sorted.Length)
                    second.InvertAxis();
            }
        }
    }
}
