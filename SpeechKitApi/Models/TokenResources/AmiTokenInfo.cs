using System;
using Newtonsoft.Json;

namespace SpeechKitApi.Models.TokenResources
{
    [Serializable]
    public class AmiTokenInfo: IToken
    {
        [JsonProperty("iamToken")]
        public string Key { get; set; }
            
        [JsonProperty("expiresAt")]
        public string ExpiresAt;
    }
}