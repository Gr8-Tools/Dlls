using System;
using Newtonsoft.Json;

namespace SpeechKitApi.Models.TokenResources
{
    [Serializable]
    public class JwtToken: IToken
    {
        [JsonProperty("jwt")] 
        public string Key { get; set; }
    }
}