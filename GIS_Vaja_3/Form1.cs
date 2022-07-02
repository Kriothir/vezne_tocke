using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GIS_Vaja_3
{
    public partial class Form1 : Form
    {
        Graphics g;
        Pen p;
        public Form1()
        {
            InitializeComponent();
            g = this.CreateGraphics();
            p = new Pen(Color.Blue, 2);
        }
        string pathImage_1 = "";
        string pathImage_2 = "";

        int counter_1 = 0;
        int counter_2 = 0;

        int image_1_x = 0;
        int image_1_y = 0;

        int image_2_x = 0;
        int image_2_y = 0;
        Point p1;
        Point p2;

        Point p1_1;
        Point p2_1;

        float angleImage1 = 0;
        float angleImage2 = 0;
        private Bitmap cutWhite(string path)
        {
            Bitmap source = new Bitmap(path);
            source.MakeTransparent();
            for (int x = 0; x < source.Width; x++)
            {
                for (int y = 0; y < source.Height; y++)
                {
                    Color currentColor = source.GetPixel(x, y);
                    if (currentColor.R >= 255 && currentColor.G >= 255 && currentColor.B >= 255)
                    {
                        source.SetPixel(x, y, Color.Transparent);
                    }
                }
            }
            return source;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    pathImage_2 = openFileDialog.FileName;

                    Image img = Image.FromFile(pathImage_2);
                    pictureBox2.Width = img.Width;
                    pictureBox2.Height = img.Height;

                }
                pictureBox2.Image = Image.FromFile(pathImage_2);
            }
        }
       
        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";


                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    pathImage_1 = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();
                    Image img = Image.FromFile(pathImage_1);
                    pictureBox1.Width = img.Width;
                    pictureBox1.Height = img.Height;
                    pictureBox1.Image = Image.FromFile(pathImage_1);

                }
            }
        }
        public static Point RotatePoints(float xRotationCenter, float yRotationCenter, float angleInDegree, int x, int y)
        {
            var angleInRadiant = (angleInDegree / 180.0) * Math.PI;

            var cosa = (float)Math.Cos(angleInRadiant);
            var sina = (float)Math.Sin(angleInRadiant);

            // First translation
            int t1x = (int)(x - xRotationCenter);
            int t1y = (int)(y - yRotationCenter);

            // Rotation
            int rx = (int)(t1x * cosa - t1y * sina);
            int ry = (int)(t1x * sina + t1y * cosa);

            // seconde translation
            x = (int)(rx + xRotationCenter);
            y = (int)(ry + yRotationCenter);

            Point newPoint = new Point((int)x, (int)y);

            return newPoint;

            //Console.WriteLine(x);
           // Console.WriteLine(y);
        }
        private static Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }
        private float getAngle(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {
            float angle1 = (float)Math.Atan2(y2 - y1, x1 - x2);
            float angle2 = (float)Math.Atan2(y4 - y3, x3 - x4);
            Console.WriteLine("Radians" + (angle1 - angle2));
            float calculatedAngle = (float)(180 / Math.PI) * (angle1 - angle2);
            if (calculatedAngle < 0) calculatedAngle += 360;
            return (360 - calculatedAngle);
        }
        private static void mergeImages(Image big, Image small, int x_small, int y_small, int x_big, int y_big)
        {
            Image img = new Bitmap(big.Width + 300, big.Height + 300);
            using (Graphics gr = Graphics.FromImage(img))
            {
                gr.DrawImage(big, new Rectangle(0 + 300, 0 + 300, img.Width, img.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, null);
                gr.DrawImage(small, new Rectangle(300 + x_big - x_small, 300 + y_big - y_small, small.Width, small.Height), 0, 0, small.Width, small.Height, GraphicsUnit.Pixel, null);
            }
            img.Save(@"C:\Users\Admin\Desktop\Ortofoto\output33333.png", ImageFormat.Png);

        }
        int small_first_x = 0;
        int small_first_y = 0;
        int small_second_x = 0;
        int small_second_y = 0;

        int big_first_x = 0;
        int big_first_y = 0;
        int big_second_x = 0;
        int big_second_y = 0;

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;

            if (counter_1 == 0)
            {
                Point coordinates = me.Location;
                p1_1 = me.Location;
                textBox1.Text = coordinates.X + ", " + coordinates.Y;
                counter_1++;

                small_first_x = coordinates.X;
                small_first_y = coordinates.Y;

            }
            else if(counter_1 == 1)
            {
                Point coordinates = me.Location;
                p2_1 = me.Location;

                textBox3.Text = coordinates.X + ", " + coordinates.Y;
                counter_1++;
                textBox5.Text = Convert.ToString(CalculateDistance(p1_1, p2_1));

                small_second_x = coordinates.X;
                small_second_y = coordinates.Y;

            }
            
        }

        private void pictureBox2_Click_1(object sender, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
           
            if (counter_2 == 0)
            {
                Point coordinates = me.Location;
                p1 = me.Location;
                image_1_x = coordinates.X;
                image_1_y = coordinates.Y;

                big_first_x = coordinates.X;
                big_first_y = coordinates.Y;

                textBox2.Text = coordinates.X + ", " + coordinates.Y;
                counter_2++;

            }
            else if (counter_2 == 1)
            {
                Point coordinates = me.Location;
                p2 = me.Location;

                image_2_x = coordinates.X;
                image_2_y = coordinates.Y;

                big_second_x = coordinates.X;
                big_second_y = coordinates.Y;

                textBox4.Text = coordinates.X + ", " + coordinates.Y;
                counter_2++;
                textBox6.Text =  Convert.ToString(CalculateDistance(p1, p2));
                pictureBox2.Refresh();
            }

        }
        double CalculateDistance(Point p1, Point p2)
        {
            double dist = (int)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));

            return dist;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            g.DrawLine(p, image_2_x, image_2_y, image_1_x, image_1_y);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Image big = Image.FromFile(pathImage_2);
            Image small = Image.FromFile(pathImage_1);
            small = cutWhite(pathImage_1);
            float angle = getAngle(big_first_x, big_first_y, big_second_x, big_second_y, small_first_x, small_first_y, small_second_x, small_second_y);
            small = RotateImage((Bitmap)small, angle);
            Point newPoint = RotatePoints(small.Height / 2, small.Width / 2, angle, small_first_x, small_first_y);
            mergeImages(big, small, newPoint.X, newPoint.Y, big_first_x, big_first_y);
        }
    }
}
