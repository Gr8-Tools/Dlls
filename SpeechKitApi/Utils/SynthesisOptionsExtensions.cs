using System.Collections.Generic;
using System.Linq;
using SpeechKitApi.Enums;
using SpeechKitApi.Models;

namespace SpeechKitApi.Utils
{
    public static class SynthesisOptionsExtensions
    {
        public static Dictionary<string, string> GetDictionary(this SynthesisOptions options)
        {
            var queryParams = new Dictionary<string, string>
            {
                ["text"] = options.Text,
                ["lang"] = options.Language.GetEnumString() ?? "",
                ["voice"] = options.Speaker.GetEnumString() ?? "",
                ["emotion"] = options.Emotion.GetEnumString() ?? "",
                ["speed"] = options.Speed.ToString("F1").Replace(',', '.'),
                ["format"] = options.AudioFormat.GetEnumString() ?? "",
                ["sampleRateHertz"] = options.Quality.GetEnumString() ?? "",
                ["folderId"] = options.FolderId
            };

            if (options.AudioFormat == SynthesisAudioFormat.Opus)
                queryParams.Remove("sampleRateHertz");
            
            var removableParams = queryParams
                .Where(kvp => string.IsNullOrEmpty(kvp.Value)).ToArray();

            if (removableParams.Length != 0)
            {
                foreach (var key in removableParams.Select(kvp => kvp.Key))
                    queryParams.Remove(key);
            }

            return queryParams;
        }
    }
}