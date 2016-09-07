﻿using System.Threading.Tasks;
using Microsoft.Band;

namespace SensorsMB2.Services
{
    public interface IBandService
    {
        Task FindBand();
        Task ConnectToBand();
        Task<IBandClient> InitTask();
    }
}
