namespace SpeechKitApi.Wav.Utils
{
    public static class Params
    {
        public const int HEADER_SIZE = 44;
        
        public const string RIFF = "RIFF";
        public const string WAVE = "WAVE";
        public const string FORMAT = "fmt ";
        public const int SUB_CHUNK_1 = 16;
        public const ushort AUDIO_FORMAT = 1;
        public const ushort BPS = 16;
        public const ushort BLOCK_ALIGN = (SpeechKitApi.Utils.Params.LPCM_DEFAULT_CHANNELS * 2);
        public const string DATA = "data";

        public const int BYTES_TO_SAMPLES_RESIZE = 4;
    }
}