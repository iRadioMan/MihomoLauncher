using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MihomoLauncher
{
    public class CpuChecker
    {
        [DllImport("kernel32.dll")]
        private static extern bool IsProcessorFeaturePresent(uint processorFeature);

        private const uint PF_SSE3_INSTRUCTIONS_AVAILABLE = 13;
        private const uint PF_SSSE3_INSTRUCTIONS_AVAILABLE = 36;
        private const uint PF_SSE4_1_INSTRUCTIONS_AVAILABLE = 37;
        private const uint PF_SSE4_2_INSTRUCTIONS_AVAILABLE = 38;
        private const uint PF_AVX_INSTRUCTIONS_AVAILABLE = 39;
        private const uint PF_AVX2_INSTRUCTIONS_AVAILABLE = 40;

        public static int GetCpuLevel()
        {
            int level = 1;

            bool hasV2 = IsProcessorFeaturePresent(PF_SSE4_2_INSTRUCTIONS_AVAILABLE) &&
                         IsProcessorFeaturePresent(PF_SSSE3_INSTRUCTIONS_AVAILABLE);

            if (hasV2) level = 2;

            bool hasV3 = hasV2 && IsProcessorFeaturePresent(PF_AVX2_INSTRUCTIONS_AVAILABLE);

            if (hasV3) level = 3;

            return level;
        }
    }
}
