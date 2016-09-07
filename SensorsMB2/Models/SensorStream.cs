using System.ComponentModel;
using System.Runtime.CompilerServices;
using SensorsMB2.Properties;

namespace SensorsMB2.Models
{
    public class SensorStream : INotifyPropertyChanged
    {
        private Accelerometer _accelerometer;
        private Gyroscope _gyroscope;

        public Accelerometer Accelerometer
        {
            get { return _accelerometer; }
            set
            {
                if (Equals(value, _accelerometer)) return;
                _accelerometer = value;
                OnPropertyChanged();
            }
        }

        public Gyroscope Gyroscope  
        {
            get { return _gyroscope; }
            set
            {
                if (Equals(value, _gyroscope)) return;
                _gyroscope = value;
                OnPropertyChanged();
            }
        }

        public SensorStream()
        {
            Accelerometer = new Accelerometer();
            Gyroscope = new Gyroscope();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
