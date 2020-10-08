using SpeechKitApi.Enums;

namespace SpeechKitApi.Models
{
    /// <summary>
    /// Дополнительные опции построения речи
    /// </summary>
    public class SynthesisExternalOptions
    {
        /// <summary>
        /// File extension (format) of the synthesized file.
        /// </summary>
        public SynthesisAudioFormat AudioFormat { get; set; } = SynthesisAudioFormat.Lpcm;

        /// <summary>
        /// Sampling rate and bit rate of the synthesized PCM audio. Note that the quality parameter only affects the audio characteristics for Wav.
        /// </summary>
        public SynthesisQuality Quality { get; set; } = SynthesisQuality.High;

        /// <summary>
        /// Language
        /// </summary>
        public SynthesisLanguage Language { get; set; } = SynthesisLanguage.Russian;

        /// <summary>
        /// The voice for the synthesized speech.
        /// </summary>
        public Speaker Speaker { get; set; } = Speaker.Zahar;

        /// <summary>
        /// The emotional connotation of the voice.
        /// </summary>
        public Emotion Emotion { get; set; } = Emotion.Neutral;
    }
}