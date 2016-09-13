using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Windows.Storage;
using Newtonsoft.Json;

namespace SensorsMB2.Models
{
    [DataContract]
    public class SensorStreamModel
    {
        public enum SupportedValues
        {
            High,
            Mid,
            Low
        }

        public SensorStreamModel()
        {
        }

        public SensorStreamModel(double time, double x, double y, double z)
        {
            Time = time;
            X = x;
            Y = y;
            Z = z;
        }

        [DataMember]
        public double Time { get; set; }

        [DataMember]
        public double X { get; set; }

        [DataMember]
        public double Y { get; set; }

        [DataMember]
        public double Z { get; set; }

        public SensorStreamModel ShallowCopy()
        {
            return (SensorStreamModel) MemberwiseClone();
        }

        public static TimeSpan ReportingInterval(SupportedValues supportedValues)
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

        public static async void SerializeJsonToFile(Collection<SensorStreamModel> collection, string fileName)
        {
            var file = await DownloadsFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);
            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(collection, Formatting.Indented));
        }
    }
}