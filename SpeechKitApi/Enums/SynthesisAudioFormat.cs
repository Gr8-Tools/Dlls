// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using SpeechKitApi.Attributes;

namespace SpeechKitApi.Enums
{
    /// <summary>
    /// Format of synthesized audio.
    /// </summary>
    public enum SynthesisAudioFormat
    {
        /// <summary>
        /// Raw data
        /// </summary>
        [EnumValueString("lpcm")]
        Lpcm,

        /// <summary>
        /// Audio in Opus format, using OGG as a container.
        /// </summary>
        [EnumValueString("oggopus")]
        Opus
    }
}