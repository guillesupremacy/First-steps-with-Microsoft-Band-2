using System.Linq;
using System.Threading.Tasks;
using Microsoft.Band;
using Microsoft.Band.Notifications;

namespace SensorsMB2.Services
{
    public class BandService : IBandService
    {
        private IBandInfo Band { get; set; }
        private IBandClient BandClient { get; set; }
        public bool IsConnected => BandClient != null;

        public async Task FindBand()
        {
            var bands = await BandClientManager.Instance.GetBandsAsync();
            Band = bands.FirstOrDefault();
        }

        public async Task ConnectToBand()
        {
            BandClient = await BandClientManager.Instance.ConnectAsync(Band);
            await BandClient.NotificationManager.VibrateAsync(VibrationType.NotificationOneTone);
        }

        public async Task<IBandClient> InitTask()
        {
            if (IsConnected)
                return BandClient;

            await FindBand();
            await ConnectToBand();
            return BandClient;
        }
    }
}