using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Steganography_Extract_Text
{
    public partial class Form1 : Form
    {
        private Bitmap _bitmap = null;
        private int _possibleChars = 0;
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        public Form1()
        {
            InitializeComponent();
            toolTip.SetToolTip(btnBrowse, "Browse for your image");
            toolTip.SetToolTip(btnEmbed, "Embed text to the image");            toolTip.SetToolTip(pbSelectedImage, "Selected image will appear here.");
            toolTip.SetToolTip(pbClose, "Exit");
            toolTip.SetToolTip(pbMinimize, "Minimize");
            toolTip.SetToolTip(txtTextToEmbed, "Enter your text here");
            txtPath.Enabled = false;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            pnlSidePanel.Top = btnBrowse.Top;
            pnlSidePanel.Height = btnBrowse.Height;
            
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Image Files (*.png; *._bitmap)|*.png; *._bitmap";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                pbSelectedImage.Image = Image.FromFile(openDialog.FileName);
                txtPath.Text = openDialog.FileName;

                pbSelectedImage.SizeMode = PictureBoxSizeMode.StretchImage;
                toolTip.SetToolTip(pbSelectedImage, "Image you selected");
            }
        }

        private void btnEmbed_Click(object sender, EventArgs e)
        {
            pnlSidePanel.Top = btnEmbed.Top;
            pnlSidePanel.Height = btnEmbed.Height;

            if (txtPath.Text.Equals(""))
            {
                MessageBox.Show(@"Image cannot be empty! Browse for an image and retry.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _bitmap = (Bitmap)pbSelectedImage.Image;

            string extractedText = Steganography.ExtractText(_bitmap);
            if (extractedText.Equals(""))
            {
                txtTextToEmbed.Text = "";
                MessageBox.Show(@"There is no hidden text in this image, retry.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            else
            {
                txtTextToEmbed.Text = extractedText;
                MessageBox.Show(@"The hidden text is: "+extractedText+"", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }


        private void pbClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pbMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
    }
}
