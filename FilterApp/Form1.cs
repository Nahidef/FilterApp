using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace FilterApp
{
	public partial class Form1 : Form
	{
		private Bitmap originalImage; 

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			comboBox1.Items.Add("Griye Çevir");
			comboBox1.Items.Add("Gaussian Blur");
			comboBox1.Items.Add("Kenar Algılama");
			comboBox1.SelectedIndex = 0;
			pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
			pictureBox1.BorderStyle = BorderStyle.FixedSingle;
		}

		private void btnLoadImage_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Resim Dosyaları|*.jpg;*.png;*.bmp";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				originalImage = new Bitmap(ofd.FileName); 
				pictureBox1.Image = originalImage;
			}
		}

		private void btnApplyFilter_Click(object sender, EventArgs e)
		{
			if (originalImage == null) return;

			Bitmap filtered = null;
			string selectedFilter = comboBox1.SelectedItem.ToString();

			try
			{
				switch (selectedFilter)
				{
					case "Griye Çevir":
						filtered = ToGrayScale(originalImage);
						break;
					case "Ortalama (Mean) Filtresi":
						filtered = MeanFilter(originalImage);
						break;
					case "Gauss Filtresi":
						filtered = GaussianFilter(originalImage);
						break;
					case "Kenar Tespiti (Sobel)":
						filtered = SobelEdgeDetection(originalImage);
						break;
				}

				if (filtered != null)
					pictureBox1.Image = filtered;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Hata: {ex.Message}");
			}
		}

		
		private Bitmap ToGrayScale(Bitmap source)
		{
			Bitmap bmp = new Bitmap(source.Width, source.Height);
			for (int y = 0; y < source.Height; y++)
			{
				for (int x = 0; x < source.Width; x++)
				{
					Color c = source.GetPixel(x, y);
					int gray = (int)((c.R + c.G + c.B) / 3);
					bmp.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
				}
			}
			return bmp;
		}

		
		private Bitmap MeanFilter(Bitmap source)
		{
			Bitmap bmp = new Bitmap(source.Width, source.Height);
			int[,] kernel = {
				{ 1, 1, 1 },
				{ 1, 1, 1 },
				{ 1, 1, 1 }
			};

			int kernelSum = 9;

			for (int y = 1; y < source.Height - 1; y++)
			{
				for (int x = 1; x < source.Width - 1; x++)
				{
					int r = 0, g = 0, b = 0;

					for (int ky = -1; ky <= 1; ky++)
					{
						for (int kx = -1; kx <= 1; kx++)
						{
							Color c = source.GetPixel(x + kx, y + ky);
							r += c.R * kernel[ky + 1, kx + 1];
							g += c.G * kernel[ky + 1, kx + 1];
							b += c.B * kernel[ky + 1, kx + 1];
						}
					}

					r /= kernelSum;
					g /= kernelSum;
					b /= kernelSum;

					bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
				}
			}
			return bmp;
		}

		
		private Bitmap GaussianFilter(Bitmap source)
		{
			Bitmap bmp = new Bitmap(source.Width, source.Height);
			int[,] kernel = {
				{ 1, 2, 1 },
				{ 2, 4, 2 },
				{ 1, 2, 1 }
			};

			int kernelSum = 16;

			for (int y = 1; y < source.Height - 1; y++)
			{
				for (int x = 1; x < source.Width - 1; x++)
				{
					int r = 0, g = 0, b = 0;

					for (int ky = -1; ky <= 1; ky++)
					{
						for (int kx = -1; kx <= 1; kx++)
						{
							Color c = source.GetPixel(x + kx, y + ky);
							r += c.R * kernel[ky + 1, kx + 1];
							g += c.G * kernel[ky + 1, kx + 1];
							b += c.B * kernel[ky + 1, kx + 1];
						}
					}

					r /= kernelSum;
					g /= kernelSum;
					b /= kernelSum;

					bmp.SetPixel(x, y, Color.FromArgb(r, g, b));
				}
			}
			return bmp;
		}

		
		private Bitmap SobelEdgeDetection(Bitmap source)
		{
			Bitmap gray = ToGrayScale(source);
			Bitmap result = new Bitmap(gray.Width, gray.Height);

			int[,] gx = {
				{ -1, 0, 1 },
				{ -2, 0, 2 },
				{ -1, 0, 1 }
			};

			int[,] gy = {
				{ -1, -2, -1 },
				{  0,  0,  0 },
				{  1,  2,  1 }
			};

			for (int y = 1; y < gray.Height - 1; y++)
			{
				for (int x = 1; x < gray.Width - 1; x++)
				{
					int gxSum = 0, gySum = 0;

					for (int ky = -1; ky <= 1; ky++)
					{
						for (int kx = -1; kx <= 1; kx++)
						{
							int intensity = gray.GetPixel(x + kx, y + ky).R;
							gxSum += gx[ky + 1, kx + 1] * intensity;
							gySum += gy[ky + 1, kx + 1] * intensity;
						}
					}

					int gVal = Math.Min(255, Math.Abs(gxSum) + Math.Abs(gySum));
					result.SetPixel(x, y, Color.FromArgb(gVal, gVal, gVal));
				}
			}

			return result;
		}
	}
}
