using System;
using SpeechKitApi;
using SpeechKitApi.Enums;
using SpeechKitApi.Models;
using SpeechKitApi.Models.TokenResources;
using SpeechKitApi.Wav;

namespace TestYandexSpeechKit
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Execute();
        }

        static void Execute()
        {
            var client = SpeechKitClient.Create(new OAuthToken {Key = ClientParams.OAuthKey});
            
            var options = new SynthesisOptions("Привет!", 0.75f, ClientParams.YandexCloudFolderId)
            {
                Emotion = Emotion.Good,
                Language = SynthesisLanguage.Russian,
                Quality = SynthesisQuality.High,
                Speaker = Speaker.Oksana,
                AudioFormat = SynthesisAudioFormat.Lpcm
            };
            //client.SaveSpeech(options, $"C:\\tmp").GetAwaiter().GetResult();
            var data = client.GetSpeech(options).GetAwaiter().GetResult();
            WavConverter.Convert(in data, in options, "C:\\tmp");
            
            client.Dispose();
            Console.Write("Completed!");
        } 
    }
}