using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using SensorsMB2.Models;
using SensorsMB2.Properties;
using SensorsMB2.Services;
using SensorsMB2.Utilities;

namespace SensorsMB2.ViewModels
{
    [DataContract]
    public class SensorStreamViewModel : INotifyPropertyChanged
    {
        private SensorStreamModel _accelerometerSensorStream;
        private SensorStreamModel _gyroscopeSensorStream;

        [DataMember] public Collection<SensorStreamModel> AccelerometerSensorStreamCollection;

        [DataMember] public Collection<SensorStreamModel> GyroscopeSensorStreamCollection;

        public SensorStreamViewModel()
        {
            GyroscopeSensorStreamCollection = new Collection<SensorStreamModel>();
            AccelerometerSensorStreamCollection = new Collection<SensorStreamModel>();
            AccelerometerSensorStream = new SensorStreamModel();
            GyroscopeSensorStream = new SensorStreamModel();
            StartCommand = new CustomCommand(StartCommand_Execute, StartCommand_CanExecute);
            StopCommand = new CustomCommand(StopCommand_Execute, StopCommand_CanExecute);
            ConnectToBand();
        }

        public SensorStreamModel GyroscopeSensorStream
        {
            get { return _gyroscopeSensorStream; }
            set
            {
                if (value.Equals(_gyroscopeSensorStream))
                {
                    return;
                }
                _gyroscopeSensorStream = value;
                OnPropertyChanged();
            }
        }

        public SensorStreamModel AccelerometerSensorStream
        {
            get { return _accelerometerSensorStream; }
            set
            {
                if (Equals(value, _accelerometerSensorStream)) return;
                _accelerometerSensorStream = value;
                OnPropertyChanged();
            }
        }

        public CustomCommand StartCommand { get; set; }
        public CustomCommand StopCommand { get; set; }
        public IBandService BandService { get; set; }
        public IBandClient BandClient { get; set; }
        public bool IsGettingData { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async void ConnectToBand()
        {
            BandService = new BandService();
            BandClient = await BandService.InitTask();

            BandClient.SensorManager.Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            BandClient.SensorManager.Accelerometer.ReportingInterval =
                AccelerometerSensorStream.ReportingInterval(SensorStreamModel.SupportedValues.High);

            BandClient.SensorManager.Gyroscope.ReadingChanged += Gyroscope_ReadingChanged;
            BandClient.SensorManager.Gyroscope.ReportingInterval =
                AccelerometerSensorStream.ReportingInterval(SensorStreamModel.SupportedValues.High);

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

            AccelerometerSensorStream.SerializeJsonToFile(AccelerometerSensorStreamCollection,
                nameof(AccelerometerSensorStream) + ".json");
            GyroscopeSensorStream.SerializeJsonToFile(GyroscopeSensorStreamCollection,
                nameof(GyroscopeSensorStream) + ".json");
        }

        private bool StartCommand_CanExecute(object o)
        {
            return BandService.IsConnected && !IsGettingData;
        }

        private async void StartCommand_Execute(object o)
        {
            await BandClient.SensorManager.Accelerometer.StartReadingsAsync(new CancellationToken());
            await BandClient.SensorManager.Gyroscope.StartReadingsAsync(new CancellationToken());

            IsGettingData = true;
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            //await Task.Delay(TimeSpan.FromSeconds(5));
        }

        private async void Gyroscope_ReadingChanged(object sender, BandSensorReadingEventArgs<IBandGyroscopeReading> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                GyroscopeSensorStream.X = e.SensorReading.AngularVelocityX;
                GyroscopeSensorStream.Y = e.SensorReading.AngularVelocityY;
                GyroscopeSensorStream.Z = e.SensorReading.AngularVelocityZ;
                GyroscopeSensorStream.Time = e.SensorReading.Timestamp.ToUnixTimeMilliseconds();

                GyroscopeSensorStreamCollection.Add(GyroscopeSensorStream.ShallowCopy());
            });
        }

        private async void Accelerometer_ReadingChanged(object sender,
            BandSensorReadingEventArgs<IBandAccelerometerReading> e)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AccelerometerSensorStream.X = e.SensorReading.AccelerationX;
                AccelerometerSensorStream.Y = e.SensorReading.AccelerationY;
                AccelerometerSensorStream.Z = e.SensorReading.AccelerationZ;
                AccelerometerSensorStream.Time = e.SensorReading.Timestamp.ToUnixTimeMilliseconds();

                AccelerometerSensorStreamCollection.Add(AccelerometerSensorStream.ShallowCopy());
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}