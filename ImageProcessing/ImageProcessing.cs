using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Imaging;
using Accord.Imaging.Filters;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ImageProcessing
{
    public partial class ImageProcessing : Form
    {
        private string age, menopause, breast, irradiat, tumorSize;
        private MachineLearning c1;

        public ImageProcessing()
        {
            c1 = new MachineLearning();
            InitializeComponent();

           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void picOrginal_Click(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofile = new OpenFileDialog
            {

                Filter = " (*.jpg)| *.jpg|(*.png)|*.png"
            };
            if (DialogResult.OK == ofile.ShowDialog())
            {
                this.picOrginal.Image = ResizeImage(new Bitmap(ofile.OpenFile()), 160, 214);

            }
        }

        private void btnGray_Click(object sender, EventArgs e)
        {

            Bitmap finalResult;
            Bitmap originalPic = new Bitmap((Bitmap)this.picOrginal.Image);


            intensify(originalPic, 0.9);



            Median median = new Median();

            Rectangle rect = new Rectangle(0, 0, originalPic.Width, originalPic.Height);

            System.Drawing.Imaging.BitmapData copyData = originalPic.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                PixelFormat.Format8bppIndexed);

            Bitmap erPic = median.Apply(copyData);


            picResult.Image = erPic;

            originalPic.UnlockBits(copyData);

            Threshold threshold = new Threshold();

            finalResult = threshold.Apply(erPic);

            thBox.Image = finalResult;

            int tumorSize = TumorSize(finalResult);
            string tumorSizeStr = "";

            if (tumorSize <= 4)
                tumorSizeStr = "0-4";
            else
                if(tumorSize<=9)
                    tumorSizeStr = "5-9";
            else
                if (tumorSize <= 14)
                tumorSizeStr = "10-14";
            else
                if (tumorSize <= 19)
                tumorSizeStr = "15-19";
            else
                if (tumorSize <= 24)
                tumorSizeStr = "20-24";
            else
                if (tumorSize <= 29)
                tumorSizeStr = "25-29";
            else
                if (tumorSize <= 34)
                tumorSizeStr = "30-34";
            else
                if (tumorSize <= 39)
                tumorSizeStr = "35-39";
            else
                if (tumorSize <= 44)
                tumorSizeStr = "40-44";
            else
                if (tumorSize <= 49)
                tumorSizeStr = "45-49";
            else
                if (tumorSize <= 54)
                tumorSizeStr = "50-54";
            else
                if (tumorSize <= 59)
                tumorSizeStr = "55-59";

             

            SobelEdgeDetector sed = new SobelEdgeDetector();
           
            sobelBox.Image = sed.Apply(finalResult);

            

            if(age == null || menopause == null || tumorSizeStr == null || breast ==null || irradiat==null)
                System.Windows.Forms.MessageBox.Show("Eksik giriş");
            else { 
                int result = c1.process(age,menopause,tumorSizeStr,breast,irradiat);
                if(result == 0)
                    System.Windows.Forms.MessageBox.Show("iyi huylu");
                else
                    System.Windows.Forms.MessageBox.Show("kötü huylu");
            }

        }



        void intensify(Bitmap bmp, double multiplier)
        {
            for (int i = 0; i < bmp.Width; i++)
            {
                // Iterates over all the pixels
                for (int j = 0; j < bmp.Height; j++)
                {
                    // Gets the current pixel
                    var currentPixel = bmp.GetPixel(i, j);

                    //  bmp.GetPixel(i,j).GetBrightness

                    // Assigns each value the multiply, or the max value 255
                    var newPixel = Color.FromArgb(
                        Math.Min((byte)255, (byte)(currentPixel.R * multiplier)),
                        Math.Min((byte)255, (byte)(currentPixel.G * multiplier)),
                        Math.Min((byte)255, (byte)(currentPixel.B * multiplier))
                        );

                    // Sets the pixel 
                    bmp.SetPixel(i, j, newPixel);
                }
            }
        }

        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            menopause = cmb.SelectedItem.ToString();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            breast = cmb.SelectedItem.ToString();
            if (breast == "Sol")
                breast = "left";
            else
                breast = "right";

           

        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            irradiat = cmb.SelectedItem.ToString();
            if (irradiat == "Maruz Kalmamış")
                irradiat = "no";
            else
                irradiat = "yes";

            

        }

        public static int TumorSize(Bitmap bmp)
        {
            int minx = bmp.Width, maxx = 0, miny = bmp.Height, maxy = 0;

            for (int i = 0; i < bmp.Width; i++)


                for (int j = 0; j < bmp.Height; j++)

                    if (bmp.GetPixel(i, j).Name == "ffffffff")
                    {
                        if (maxx < i)
                            maxx = i;
                        if (minx > i)
                            minx = i;
                        if (maxy < j)
                            maxy = j;
                        if (miny > j)
                            miny = j;

                    }







            return Math.Max((maxx - minx), (maxy - miny));

        }


        /*
        public static Boolean nodeCaps(Bitmap bmp)
        {
            int counter = 0;
            for (int i = 0; i < bmp.Width; i++)


                for (int j = 0; j < bmp.Height; j++)

                    if (bmp.GetPixel(i, j).Name == "ffffffff")
                    {
                        counter++;
                    }

            return false;
        } */

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cmb = (ComboBox)sender;
            age = cmb.SelectedItem.ToString();
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
