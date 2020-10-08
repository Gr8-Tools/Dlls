using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpeechKitApi.Models.TokenResources;

namespace SpeechKitApi.Utils
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Возвращает информацию, каксаемую AMI-token'а 
        /// </summary>
        public static async Task<AmiTokenInfo> GetAmiTokenInfo(this HttpClient httpClient, IToken tokenPayLoad)
        {
            var stringPayLoad = await Task.Run(() => JsonConvert.SerializeObject(tokenPayLoad));
            var httpContent = new StringContent(stringPayLoad, Encoding.UTF8, "application/json");

            AmiTokenInfo amiTokenInfo = null;
            var httpResponse =
                await httpClient.PostAsync("https://iam.api.cloud.yandex.net/iam/v1/tokens", httpContent);
            if(!httpResponse.IsSuccessStatusCode)
                throw new Exception($"Error {httpResponse.StatusCode}: {httpResponse}");

            if (httpResponse.Content == null)
                throw new Exception($"AMI response content is empty!");
            
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            amiTokenInfo = JsonConvert.DeserializeObject<AmiTokenInfo>(responseContent);

            return amiTokenInfo;
        }
    }
}