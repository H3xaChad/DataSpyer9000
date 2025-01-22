using FairDataGetter.Client.Class;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;

namespace FairDataGetter.Client
{
    /// <summary>
    /// Interaction logic for UserControl_CustomerPicture.xaml
    /// </summary>
    public partial class UserControl_CustomerPicture : UserControl
    {
        private Customer newCustomer;
        private Company newCompany;

        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentFrame;
        private Bitmap customerPicture;
        private bool isWebcamRunning = false;

        public UserControl_CustomerPicture(Customer customer, Company company = null)
        {
            newCustomer = customer;
            newCompany = company;

            InitializeComponent();
            if (newCompany != null)
            {
                SecondStepBorder.Background = System.Windows.Media.Brushes.LightGreen;
            }
            else
            {
                SecondStepBorder.Background = System.Windows.Media.Brushes.LightGray;
            }
        }

        // Start webcam when the page is loaded
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get available video devices (webcams)
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No video devices found.");
                    return;
                }

                // Initialize the first video device (webcam)
                videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;

                // Start the video capture
                videoSource.Start();
                isWebcamRunning = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error starting webcam: " + ex.Message);
            }
        }

        // New frame event from webcam, capture each frame and display it
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {

            // Dispose previous frame if any
            if (currentFrame != null)
            {
                currentFrame.Dispose();
            }

            // Capture the current frame
            currentFrame = (Bitmap)eventArgs.Frame.Clone();

            // Use Dispatcher.Invoke to safely update UI elements on the UI thread
            Dispatcher.Invoke(() =>
            {
                // Convert the current frame to a BitmapSource for WPF Image control
                WebCamImage.Source = BitmapToImageSource(currentFrame);

                // Enable the TakePicture and ResetPicture Buttons
                if (isWebcamRunning)
                {
                    TakePictureButton.IsEnabled = true;
                    ResetPictureButton.IsEnabled = true;
                }
            });


        }

        // Take a picture when the "Take Picture" button is clicked
        private void TakePictureButtonClicked(object sender, RoutedEventArgs e)
        {
            if (isWebcamRunning)
            {
                try
                {
                    // Clone the current frame to take a snapshot
                    customerPicture = (Bitmap)currentFrame.Clone();

                    // Convert the snapshot to a BitmapSource and display in takenPicture
                    WebCamImage.Source = BitmapToImageSource(customerPicture);
                    StopWebcam();
                    ContinueButton.IsEnabled = true;
                    TakePictureButton.IsEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error capturing picture: " + ex.Message);
                }
            }
        }

        // Reset the picture when the "Reset Picture" button is clicked
        private void ResetPictureButtonClicked(object sender, RoutedEventArgs e)
        {
            // Restart the webcam feed
            if (!isWebcamRunning)
            {
                videoSource.Start();
                isWebcamRunning = true;

                // Enable the Take Picture button and disable the Continue button
                ContinueButton.IsEnabled = false;
                TakePictureButton.IsEnabled = false;

                // Clear the image to show the live feed
                WebCamImage.Source = null;
            }
        }

        // Stop the webcam when the user navigates away (Return or Continue)
        private void StopWebcam()
        {
            if (isWebcamRunning)
            {
                videoSource.SignalToStop();
                isWebcamRunning = false;
            }
        }

        // Convert a Bitmap to BitmapSource for WPF Image control
        public BitmapSource BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapSource bitmapSource = BitmapFrame.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                return bitmapSource;
            }
        }

        // Handle "Return" button click
        private void ReturnButtonClicked(object sender, RoutedEventArgs e)
        {
            StopWebcam(); // Stop the webcam before navigating away
            MainWindow.UpdateView(new UserControl_CustomerProductGroups(newCustomer, newCompany));
        }

        // Handle "Continue" button click
        private void ContinueButtonClicked(object sender, RoutedEventArgs e)
        {
            StopWebcam(); // Stop the webcam before navigating away
            newCustomer.ImageBase64 = ConvertImageToBase64(customerPicture);
            MainWindow.UpdateView(new UserControl_CustomerSubmit(newCustomer, newCompany));
        }

        private string ConvertImageToBase64(Bitmap image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Save as JPEG or any other format
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }
    }
}