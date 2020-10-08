// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using SpeechKitApi.Attributes;

namespace SpeechKitApi.Enums
{
    /// <summary>
    /// Quality of generated audio.
    /// </summary>
    public enum SynthesisQuality
    {
        /// <summary>
        /// Sampling rate of 48 kHz and bit rate of 768 kbit/s.
        /// </summary>
        [EnumValueString("48000")]
        High,
        
        /// <summary>
        /// Sampling rate of 48 kHz and bit rate of 768 kbit/s.
        /// </summary>
        [EnumValueString("16000")]
        Medium,

        /// <summary>
        ///  Sampling rate of 8 kHz and bit rate of 128 kbit/s.
        /// </summary>
        [EnumValueString("8000")]
        Low
    }
}