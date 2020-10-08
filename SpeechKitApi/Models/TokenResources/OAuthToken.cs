using System;
using Newtonsoft.Json;

namespace SpeechKitApi.Models.TokenResources
{
    [Serializable]
    public class OAuthToken: IToken
    {
        [JsonProperty("yandexPassportOauthToken")]
        public string Key { get; set; }
    }
}