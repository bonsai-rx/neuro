using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bonsai.PulsePal
{
    public enum ParameterCode : byte
    {
        Biphasic = 1,
        Phase1Voltage = 2,
        Phase2Voltage = 3,
        Phase1Duration = 4,
        InterPhaseInterval = 5,
        Phase2Duration = 6,
        InterPulseInterval = 7,
        BurstDuration = 8,
        InterBurstInterval = 9,
        PulseTrainDuration = 10,
        PulseTrainDelay = 11,
        TriggerOnChannel1 = 12,
        TriggerOnChannel2 = 13,
        CustomTrainIdentity = 14,
        CustomTrainTarget = 15,
        CustomTrainLoop = 16,
        RestingVoltage = 17,
        TriggerMode = 128
    }
}
