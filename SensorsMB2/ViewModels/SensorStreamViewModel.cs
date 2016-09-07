using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using SensorsMB2.Services;
using SensorsMB2.Utility;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using SensorsMB2.Models;
using SensorsMB2.Properties;

namespace SensorsMB2.ViewModels
{
    public class SensorStreamViewModel : INotifyPropertyChanged
    {
        private SensorStream _sensorStream;

        public SensorStream SensorStream
        {
            get { return _sensorStream; }
            set
            {
                if (Equals(value, _sensorStream)) return;
                _sensorStream = value;
                OnPropertyChanged();
            }
        }

        public ICommand StartCommand { get; set; }
        public IBandService BandService { get; set; }
        public IBandClient BandClient { get; set; }

        public SensorStreamViewModel()
        {
            SensorStream = new SensorStream();
            StartCommand = new CustomCommand(Execute, CanExecute);
            BandService = new BandService();
        }

        private bool CanExecute(object o)
        {
            return true;
        }

        private async void Execute(object o)
        {
            if (BandClient != null) return;
            BandClient = await BandService.InitTask();

            BandClient.SensorManager.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            BandClient.SensorManager.Accelerometer.ReportingInterval =
                SensorStream.Accelerometer.ReportingInterval(Accelerometer.SupportedValues.High);
            await BandClient.SensorManager.Accelerometer.StartReadingsAsync(new CancellationToken());

            BandClient.SensorManager.Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            BandClient.SensorManager.Gyroscope.ReportingInterval =
                SensorStream.Gyroscope.ReportingInterval(Gyroscope.SupportedValues.High);
            await BandClient.SensorManager.Gyroscope.StartReadingsAsync(new CancellationToken());
            //await Task.Delay(TimeSpan.FromSeconds(5));
            //await BandClient.SensorManager.Accelerometer.StopReadingsAsync();
        }

        private async void Gyroscope_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandGyroscopeReading> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SensorStream.Gyroscope.X = e.SensorReading.AngularVelocityX;
                SensorStream.Gyroscope.Y = e.SensorReading.AngularVelocityY;
                SensorStream.Gyroscope.Z = e.SensorReading.AngularVelocityZ;
            });
        }

        private async void Accelerometer_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 SensorStream.Accelerometer.X = e.SensorReading.AccelerationX;
                 SensorStream.Accelerometer.Y = e.SensorReading.AccelerationY;
                 SensorStream.Accelerometer.Z = e.SensorReading.AccelerationZ;
             });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
