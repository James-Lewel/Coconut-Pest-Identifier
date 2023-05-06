using MLApp;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Windows.Forms;
using Size = OpenCvSharp.Size;

namespace MLForms
{
    public partial class MainForm : Form
    {
        private VideoCapture capture;

        public MainForm()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;

            capture = new VideoCapture(0);
            Mat frame = new Mat();
            
            while (true)
            {
                capture.Read(frame);

                if (frame.Empty())
                {
                    break;
                }

                // Resize the frame to match the size of the PictureBox
                Cv2.Resize(frame, frame, new Size(cameraDisplay.Width, cameraDisplay.Height));
                cameraDisplay.Image = BitmapConverter.ToBitmap(frame);

                Application.DoEvents();

                if (capture == null)
                {
                    cameraDisplay.Image = null;
                    break;
                }
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = true;
            stopButton.Enabled = false;

            capture.Dispose();
            capture = null;
        }

        private void resetButton_Click(object sender, EventArgs e)
        {

        }

        private void uploadButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Open Image";
                openFileDialog.Filter = "Image Files(*.jpeg;*.bmp;*.png;*.jpg)|*.jpeg;*.bmp;*.png;*.jpg";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the image
                    Bitmap originalImage = new Bitmap(openFileDialog.FileName);

                    // Resize the image to fit the picture box
                    Bitmap resizedImage = new Bitmap(cameraDisplay.Width, cameraDisplay.Height);
                    Graphics graphics = Graphics.FromImage(resizedImage);
                    graphics.DrawImage(originalImage, 0, 0, cameraDisplay.Width, cameraDisplay.Height);

                    // Display the resized image in the picture box
                    cameraDisplay.Image = resizedImage;

                    // Use the image path for MLModel input
                    var sampleData = new MLModel.ModelInput()
                    {
                        ImageSource = openFileDialog.FileName,
                    };

                    var result = MLModel.Predict(sampleData);

                    informationLabel.Text = "Information : \n" +
                                             result.Prediction;
                }
            }
        }

        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}