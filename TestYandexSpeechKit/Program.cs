using System.Linq;
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
            Execute("C:\\tmp");
        }

        static void Execute(params object[] args)
        {
            var path = (string) args[0];
            var client = SpeechKitClient.Create(new OAuthToken {Key = ClientParams.OAuthKey});

            var externalOptions = new SynthesisExternalOptions
            {
                Emotion = Emotion.Evil,
                Language = SynthesisLanguage.Russian,
                Quality = SynthesisQuality.High,
                Speaker = Speaker.Oksana,
                AudioFormat = SynthesisAudioFormat.Lpcm
            };

            var optionsArray = SynthesisOptions.Create(
                new string[] {"Соси жопу", "А можешь и не жопу"},
                externalOptions,
                ClientParams.YandexCloudFolderId
            ).ToArray();
            
            var dataArray = client.GetMultipleSpeech(optionsArray).GetAwaiter().GetResult();
            for(var i = 0; i < dataArray.Length; i++)
                WavConverter.Convert(in dataArray[i], in optionsArray[i], path);
            
            client.Dispose();
        } 
    }
}