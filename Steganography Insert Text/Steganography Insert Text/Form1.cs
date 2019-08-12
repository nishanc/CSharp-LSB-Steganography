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

namespace Steganography_Insert_Text
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
            toolTip.SetToolTip(btnEmbed, "Embed text to the image");
            toolTip.SetToolTip(btnSave, "Save image with embedded text");
            toolTip.SetToolTip(pbSelectedImage, "Selected image will appear here.");
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
            openDialog.Filter = "Image Files (*.jpeg; *.png; *._bitmap)|*.jpg; *.png; *._bitmap";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                pbSelectedImage.Image = Image.FromFile(openDialog.FileName);
                txtPath.Text = openDialog.FileName;

                pbSelectedImage.SizeMode = PictureBoxSizeMode.StretchImage;
                toolTip.SetToolTip(pbSelectedImage, "Image you selected");

                _bitmap = (Bitmap)pbSelectedImage.Image;
                int chars = (_bitmap.Height * _bitmap.Width * 3) / 8;
                lblRes.Text = "Resolution: "+ _bitmap.Height.ToString() + " x "+ _bitmap.Width.ToString() + " | Possible Characters: "+ chars.ToString() + "";
                //Resolution: Possible Characters: 
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

            string text = txtTextToEmbed.Text;

            if (text.Equals(""))
            {
                MessageBox.Show(@"The text you want to hide can't be empty!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _possibleChars = (_bitmap.Height * _bitmap.Width * 3) / 8;
            if (_possibleChars < text.Length)
            {
                MessageBox.Show(@"Text is too lengthy for the selected image. Insufficient space", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _bitmap = Steganography.EmbedText(text, _bitmap);

            DialogResult dialogResult = MessageBox.Show(@"Your text was hidden in the image successfully!" + Environment.NewLine + "Would you like to save image now? then click 'Yes', if you want to retry click 'No'.", "Done", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                btnSave_Click(sender, e);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            pnlSidePanel.Top = btnSave.Top;
            pnlSidePanel.Height = btnSave.Height;

            if (pbSelectedImage.Image == null)
            {
                MessageBox.Show(@"There is no picture to save! Browse for a picture and retry.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Png Image|*.png|Bitmap Image|*._bitmap";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 0:
                    {
                        _bitmap.Save(saveFileDialog.FileName, ImageFormat.Png);
                    }
                        break;
                    case 1:
                    {
                        _bitmap.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                    }
                        break;
                }

                pbSelectedImage.Image = null;
                txtTextToEmbed.Text = "";
                txtTextToEmbed.Focus();
                txtPath.Text = "";
                lblRes.Text = "Resolution: 0 | Possible Characters: 0";
                toolTip.SetToolTip(pbSelectedImage, "Selected image will appear here.");
                MessageBox.Show(@"Your image with embedded text has been saved successfully.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
