using System;
using System.Dynamic;
using System.IO;
using System.Text;
using SpeechKitApi.Models;
using SpeechKitApi.Utils;

//using SpeechKitApi.Utils;

namespace SpeechKitApi.Wav
{
    public static class WavConverter
    {
        private static bool _isConstantHeaderInitialized;
        private static byte[] _constantHeaders;
        
        /// <summary>
        /// Инициализирует стандартные WAV-заголовки
        /// </summary>
        private static void ConstantHeaderInitialize()
        {
            if(_isConstantHeaderInitialized)
                return;
            
            const int offset = 0;
            const int bufferSize = 4;
            var emptyData = new byte[bufferSize];
            using (var stream = new MemoryStream())
            {
                stream.Write(Utils.Params.RIFF);//4
                stream.Write(emptyData, offset, bufferSize);//4
                stream.Write(Utils.Params.WAVE);//4
                stream.Write(Utils.Params.FORMAT, 4);//4
                stream.Write(Utils.Params.SUB_CHUNK_1);//4
                stream.Write(Utils.Params.AUDIO_FORMAT);//2
                stream.Write(Params.LPCM_DEFAULT_CHANNELS, 2);//2
                stream.Write(emptyData, offset, bufferSize);//4
                stream.Write(emptyData, offset, bufferSize);//4
                stream.Write(Utils.Params.BLOCK_ALIGN);//2
                stream.Write(Utils.Params.BPS);//2
                stream.Write(Utils.Params.DATA);//4
                stream.Write(emptyData, offset, bufferSize);//4

                _constantHeaders = stream.ToArray();
            }

            _isConstantHeaderInitialized = true;
        }
        
        /// <summary>
        /// Конвертирует сырые данные в wav-данные 
        /// </summary>
        public static byte[] Convert(in byte[] rawData, in SynthesisOptions options)
        {
            if(rawData.Length % 4 != 0)
                throw new Exception("Audio data has incorrect length!");

            return GetWavData(in rawData, in options);
        }

        /// <summary>
        /// Сохраняет сырые данные в wav-файл 
        /// </summary>
        public static void Convert(in byte[] rawData, in SynthesisOptions options, string filePath)
        {
            var wavData = Convert(in rawData, in options);

            var fileName = $"{options.Text.GetValidPathString()}.wav";
            filePath = Path.Combine(filePath, fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                fileStream.Write(wavData, 0, wavData.Length);
            }
        }

        /// <summary>
        /// Формирует wav-данные из сырых данных 
        /// </summary>
        private static byte[] GetWavData(in byte[] data, in SynthesisOptions options)
        {
            byte[] wavData;
            using (var stream = new MemoryStream())
            {
                ReserveHeadersSpace(stream);
                WriteData(stream, in data);
                WriteHeader(stream, in options, data.Length / Utils.Params.BYTES_TO_SAMPLES_RESIZE);

                wavData = stream.ToArray();
            }

            return wavData;
        }
        
        /// <summary>
        /// Резервирует место под заголовок
        /// </summary>
        private static void ReserveHeadersSpace(Stream stream)
        {
            ConstantHeaderInitialize();
            stream.Write(_constantHeaders, 0, _constantHeaders.Length);
        }
        
        /// <summary>
        /// Записывает данные в поток 
        /// </summary>
        private static void WriteData(Stream stream, in byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
        
        /// <summary>
        /// Заплняет заголовки файла 
        /// </summary>
        private static void WriteHeader(Stream stream, in SynthesisOptions options, int samplesCount)
        {
            const int chunkSizeOffset = 4;
            const int sampleRateOffset = 24;
            const int byteRateOffset = 28;
            const int subChunk2Offset = 40;

            const int requiredSize = 4;
            
            var chunkSize = stream.Length - (chunkSizeOffset * 2);
            var sampleRate = int.Parse(options.Quality.GetEnumString()) / Params.LPCM_DEFAULT_CHANNELS;
            var byteRate = sampleRate * Params.LPCM_DEFAULT_CHANNELS * 2;
            var subChunk2 = samplesCount * Params.LPCM_DEFAULT_CHANNELS * 2;
            
            stream.Seek(chunkSizeOffset, SeekOrigin.Begin);
            stream.Write(chunkSize, requiredSize);

            stream.Seek(sampleRateOffset - chunkSizeOffset - requiredSize, SeekOrigin.Current);
            stream.Write(sampleRate);

            stream.Seek(byteRateOffset - sampleRateOffset - requiredSize, SeekOrigin.Current);
            stream.Write(byteRate);
            
            stream.Seek(subChunk2Offset - byteRateOffset - requiredSize, SeekOrigin.Current);
            stream.Write(subChunk2);
        }

        /// <summary>
        /// Записывает в поток значение типа Int64 
        /// </summary>
        private static void Write(this Stream stream, long value, int size = 0, int offset = 0)
        {
            var data = BitConverter.GetBytes(value);
            if (size == 0)
                size = data.Length;
            stream.Write(data, offset, size);
        }
        
        /// <summary>
        /// Записывает в поток значение типа Int32 
        /// </summary>
        private static void Write(this Stream stream, int value, int size = 0, int offset = 0)
        {
            var data = BitConverter.GetBytes(value);
            if (size == 0)
                size = sizeof(Int32);
            stream.Write(data, offset, size);
        }
        
        /// <summary>
        /// Записывает в поток значение типа Int16 
        /// </summary>
        private static void Write(this Stream stream, ushort value, int size = 0, int offset = 0)
        {
            var data = BitConverter.GetBytes(value);
            if (size == 0)
                size = sizeof(ushort);
            stream.Write(data, offset, size);
        }

        /// <summary>
        /// Записывает в поток значение типа String. Если не задана длина массива байтов (=0), то берется длинна сообщения 
        /// </summary>
        private static void Write(this Stream stream, string value, int size = 0, int offset = 0)
        {
            var data = Encoding.UTF8.GetBytes(value);
            if (size == 0)
                size = value.Length;
            stream.Write(data, offset, size);
        }
    }
}