using MLApp;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.IO;
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

            // Get the number of camera devices connected to the system
            var cameraCount = 0;

            // Check up to 10 cameras
            for (var i = 0; i < 10; i++)
            {
                using var cap = new VideoCapture(i, VideoCaptureAPIs.ANY);

                if (!cap.IsOpened())
                {
                    break;
                }

                var isVirtual = cap.Get(VideoCaptureProperties.Mode) == -1;
                if (!isVirtual)
                {
                    cameraComboBox.Items.Add(cameraCount);
                    cameraCount++;
                }
            }
        } // END MainForm

        private void startButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            stopButton.Enabled = true;

            capture = new VideoCapture(cameraComboBox.SelectedIndex);
            Mat frame = new Mat();
            Bitmap image = null;
            
            while (true)
            {
                capture.Read(frame);

                if (frame.Empty())
                {
                    break;
                }

                // Resize the frame to match the size of the PictureBox
                Cv2.Resize(frame, frame, new Size(cameraDisplay.Width, cameraDisplay.Height));
                image = BitmapConverter.ToBitmap(frame);
                cameraDisplay.Image = image;

                Application.DoEvents();

                if (capture == null)
                {
                    break;
                }
            }
        } // END startButton_Click

        private void stopButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = true;
            stopButton.Enabled = false;

            capture = null;

            cameraDisplay.Image = null;
        } // END stopButton_Click

        private void resetButton_Click(object sender, EventArgs e)
        {
            startButton.Enabled = true;
            stopButton.Enabled = false;

            cameraDisplay.Image = null;

            informationLabel.Text = "Information :";
        } // END resetButton_Click

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
        } // END uploadButton_Click

        private void cameraComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void screenshotButton_Click(object sender, EventArgs e)
        {
            if (cameraDisplay.Image != null)
            {
                // Generate a unique file name for the screenshot
                string fileName = "screenshot_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

                // Save the image to the application directory
                string filePath = Path.Combine("D:\\Projects_C#\\Coconut-Pest-Identifier\\Screenshots\\", fileName);
                cameraDisplay.Image.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);

                // Use the image path for MLModel input
                var sampleData = new MLModel.ModelInput()
                {
                    ImageSource = filePath,
                };

                var result = MLModel.Predict(sampleData);

                informationLabel.Text = "Information : \n" +
                                         result.Prediction;

                startButton.Enabled = true;
                stopButton.Enabled = false;

                capture = null;
            }
        }
    }
}