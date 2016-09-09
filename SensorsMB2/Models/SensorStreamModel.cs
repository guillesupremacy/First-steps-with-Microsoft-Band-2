using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Windows.Storage;
using Newtonsoft.Json;
using SensorsMB2.Properties;

namespace SensorsMB2.Models
{
    [DataContract]
    public class SensorStreamModel : INotifyPropertyChanged
    {
        public enum SupportedValues
        {
            High,
            Mid,
            Low
        }

        private long _time;
        private double _x;
        private double _y;
        private double _z;

        public SensorStreamModel()
        {
        }

        [DataMember]
        public double X
        {
            get { return _x; }
            set
            {
                if (value.Equals(_x)) return;
                _x = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public double Y
        {
            get { return _y; }
            set
            {
                if (value.Equals(_y)) return;
                _y = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public double Z
        {
            get { return _z; }
            set
            {
                if (value.Equals(_z)) return;
                _z = value;
                OnPropertyChanged();
            }
        }

        [DataMember]
        public long Time
        {
            get { return _time; }
            set
            {
                if (value.Equals(_time)) return;
                _time = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public SensorStreamModel ShallowCopy()
        {
            return (SensorStreamModel) MemberwiseClone();
        }

        public async void SerializeJsonToFile(Collection<SensorStreamModel> sensorStreamDataCollection, string fileName)
        {
            var file = await DownloadsFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(sensorStreamDataCollection, Formatting.Indented));
        }

        public TimeSpan ReportingInterval(SupportedValues supportedValues)
        {
            switch (supportedValues)
            {
                case SupportedValues.High:
                    return TimeSpan.FromMilliseconds(16.0);
                case SupportedValues.Mid:
                    return TimeSpan.FromMilliseconds(32.0);
                case SupportedValues.Low:
                    return TimeSpan.FromMilliseconds(128.0);
                default:
                    return TimeSpan.FromMilliseconds(16.0);
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}