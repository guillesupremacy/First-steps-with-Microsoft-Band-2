using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Microsoft.Band;
using Microsoft.Band.Notifications;
using Microsoft.Band.Sensors;
using SensorsMB2.Annotations;
using SensorsMB2.Models;
using SensorsMB2.Services;
using SensorsMB2.Utilities;

namespace SensorsMB2.ViewModels
{
    public class SensorStreamViewModel : INotifyPropertyChanged
    {
        private string _accelerometerSamples;
        private string _countdown;
        private string _gyroscopeSamples;
        private string _statusMessage;

        public Collection<SensorStreamModel> AccelerometerSensorStreamCollection;
        public Collection<SensorStreamModel> GyroscopeSensorStreamCollection;

        public SensorStreamViewModel()
        {
            GyroscopeSensorStreamCollection = new Collection<SensorStreamModel>();
            AccelerometerSensorStreamCollection = new Collection<SensorStreamModel>();
            StartCommand = new CustomCommand(StartCommand_Execute, StartCommand_CanExecute);
            StopCommand = new CustomCommand(StopCommand_Execute, StopCommand_CanExecute);
            ConnectToBand();
        }

        public CustomCommand StartCommand { get; set; }
        public CustomCommand StopCommand { get; set; }
        public IBandService BandService { get; set; }
        public IBandClient BandClient { get; set; }
        public bool IsGettingData { get; set; }

        public string GyroscopeSamples
        {
            get { return _gyroscopeSamples; }
            set
            {
                if (value == _gyroscopeSamples) return;
                _gyroscopeSamples = value;
                OnPropertyChanged();
            }
        }

        public string AccelerometerSamples
        {
            get { return _accelerometerSamples; }
            set
            {
                if (value == _accelerometerSamples) return;
                _accelerometerSamples = value;
                OnPropertyChanged();
            }
        }

        public string Countdown
        {
            get { return _countdown; }
            set
            {
                if (value == _countdown) return;
                _countdown = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                if (value == _statusMessage) return;
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ConnectToBand()
        {
            try
            {
                BandService = new BandService();
                BandClient = await BandService.InitTask();

                BandClient.SensorManager.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
                BandClient.SensorManager.Accelerometer.ReportingInterval =
                    SensorStreamModel.ReportingInterval(SensorStreamModel.SupportedValues.High);

                BandClient.SensorManager.Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
                BandClient.SensorManager.Gyroscope.ReportingInterval =
                    SensorStreamModel.ReportingInterval(SensorStreamModel.SupportedValues.High);
            }
            catch (Exception e)
            {
                StatusMessage = e.Message;
                Debug.WriteLine(e.Message);
            }

            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }


        private bool StopCommand_CanExecute(object obj)
        {
            return BandService.IsConnected && IsGettingData;
        }

        private async void StopCommand_Execute(object obj)
        {
            await BandClient.SensorManager.Accelerometer.StopReadingsAsync(new CancellationToken());
            await BandClient.SensorManager.Gyroscope.StopReadingsAsync(new CancellationToken());

            IsGettingData = false;
            StopCommand.RaiseCanExecuteChanged();
            StartCommand.RaiseCanExecuteChanged();

            SensorStreamModel.SerializeJsonToFile(AccelerometerSensorStreamCollection,
                "AccelerometerData.json");
            SensorStreamModel.SerializeJsonToFile(GyroscopeSensorStreamCollection,
                "GyroscopeData.json");

            Countdown = "Done";
        }

        private bool StartCommand_CanExecute(object o)
        {
            return BandService.IsConnected && !IsGettingData;
        }

        private async void StartCommand_Execute(object o)
        {
            Countdown = "3";
            await Task.Delay(TimeSpan.FromSeconds(1));
            Countdown = "2";
            await Task.Delay(TimeSpan.FromSeconds(1));
            Countdown = "1";

            await BandClient.SensorManager.Accelerometer.StartReadingsAsync(new CancellationToken());
            await BandClient.SensorManager.Gyroscope.StartReadingsAsync(new CancellationToken());

            await BandClient.NotificationManager.VibrateAsync(VibrationType.OneToneHigh);
            Countdown = "Go!";

            IsGettingData = true;
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
        }

        private async void Gyroscope_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandGyroscopeReading> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                GyroscopeSensorStreamCollection.Add(
                    new SensorStreamModel(
                        e.SensorReading.Timestamp.ToUnixTimeMilliseconds(),
                        e.SensorReading.AngularVelocityX,
                        e.SensorReading.AngularVelocityY,
                        e.SensorReading.AngularVelocityZ));

                GyroscopeSamples = "Gyro_Data = " + GyroscopeSensorStreamCollection.Count;
            });
        }

        private async void Accelerometer_ReadingChanged(object sender,
            BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AccelerometerSensorStreamCollection.Add(
                    new SensorStreamModel(
                        e.SensorReading.Timestamp.ToUnixTimeMilliseconds(),
                        e.SensorReading.AccelerationX,
                        e.SensorReading.AccelerationY,
                        e.SensorReading.AccelerationZ));

                AccelerometerSamples = "Acce_Data = " + AccelerometerSensorStreamCollection.Count;
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}