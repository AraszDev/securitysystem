﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Core;


namespace SecuritySystemUWP
{
    public class UsbCamera : ICamera
    {
        private MediaCapture mediaCapture;
        private static DispatcherTimer takePhotoTimer;
        private MotionSensor pirSensor;
        private CoreDispatcher dispatcher;
        private bool isTimerStarted;
        private static Mutex pictureMutexLock = new Mutex();
        /*******************************************************************************************
        * PUBLIC METHODS
        *******************************************************************************************/
        public async Task Initialize()
        {
            //Initialize Camera
            if (mediaCapture == null)
            {
                // Attempt to get the back camera if one is available, but use any camera device if not
                var cameraDevice = await FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel.Back);

                if (cameraDevice == null)
                {
                    Debug.WriteLine("No camera device found!");
                    return;
                }

                // Create MediaCapture and its settings
                mediaCapture = new MediaCapture();

                var settings = new MediaCaptureInitializationSettings { VideoDeviceId = cameraDevice.Id };
                // Initialize MediaCapture
                try
                {
                    await mediaCapture.InitializeAsync(settings);
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine("The app was denied access to the camera. Ensure webcam capability is added in the manifest.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(string.Format("Exception when initializing MediaCapture with {0}: {1}", cameraDevice.Id, ex.ToString()));
                }
            }
            //Timer controlling camera pictures with motion
            isTimerStarted = false;
            takePhotoTimer = new DispatcherTimer();
            takePhotoTimer.Interval = TimeSpan.FromSeconds(1);
            takePhotoTimer.Tick += TakePhotoTimer_Tick;
            dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;

            //Initialize PIR Sensor
            pirSensor = new MotionSensor();
            pirSensor.OnChanged += PirSensor_OnChanged;
        }

        private async void PirSensor_OnChanged(object sender, GpioPinValueChangedEventArgs e)
        {
            if ((e.Edge == GpioPinEdge.RisingEdge) != isTimerStarted)
            {
                isTimerStarted = !isTimerStarted;
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (isTimerStarted)
                        {
                            takePhotoTimer.Start();
                        }
                        else
                        {
                            takePhotoTimer.Stop();
                        }
                    }).AsTask();                                  
            }
        }

        public void Dispose()
        {
            if (mediaCapture != null)
            {
                mediaCapture.Dispose();
            }

            if (takePhotoTimer != null)
            {
                takePhotoTimer.Stop();
            }

            if (pirSensor != null)
            {
                pirSensor.Dispose();
            }
        }

        /*******************************************************************************************
        * PRIVATE METHODS
        ********************************************************************************************/
        private static async Task<DeviceInformation> FindCameraDeviceByPanelAsync(Windows.Devices.Enumeration.Panel desiredPanel)
        {
            // Get available devices for capturing pictures
            var allVideoDevices = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);

            // Get the desired camera by panel
            DeviceInformation desiredDevice = allVideoDevices.FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desiredPanel);

            // If there is no device mounted on the desired panel, return the first device found
            if(desiredDevice == null)
            {
                Debug.WriteLine("No device was found on the desired panel. First device found was returned.");
                return allVideoDevices.FirstOrDefault();
            }
            return desiredDevice;
        }

        private async void TakePhotoTimer_Tick(object sender, object e)
        {
            await TakePhotoAsync();
        }
        private async Task TakePhotoAsync()
        {
            string imageName = DateTime.UtcNow.Ticks.ToString() + ".jpg";
            var cacheFolder = KnownFolders.PicturesLibrary;
            cacheFolder = await cacheFolder.GetFolderAsync("securitysystem-cameradrop");
            StorageFile image = await cacheFolder.CreateFileAsync(imageName);
            try
            {
                await mediaCapture.CapturePhotoToStorageFileAsync(ImageEncodingProperties.CreateJpeg(), image);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception when taking a photo: {0}", ex.ToString()));
                await image.DeleteAsync();
            }
        }
    }
}

