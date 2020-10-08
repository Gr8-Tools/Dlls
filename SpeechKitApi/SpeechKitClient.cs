using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Jose;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using SpeechKitApi.Enums;
using SpeechKitApi.Models;
using SpeechKitApi.Models.TokenResources;
using SpeechKitApi.Utils;

namespace SpeechKitApi
{
    public class SpeechKitClient: IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly AmiTokenInfo _amiTokenInfo;

        #region CONSTRUCTORS
        private SpeechKitClient(string amiToken)
        {
            _httpClient = new HttpClient();

            _amiTokenInfo = new AmiTokenInfo {Key = amiToken};
        }
        
        private SpeechKitClient(IToken tokenPayLoad)
        {
            _httpClient = new HttpClient();

            _amiTokenInfo = _httpClient.GetAmiTokenInfo(tokenPayLoad).GetAwaiter().GetResult();
        }
        
        /// <summary>
        /// Создает клтента по существующему AMI-токену
        /// </summary>
        public static SpeechKitClient Create(string amiToken)
        {
            return new SpeechKitClient(amiToken);
        }
        
        /// <summary>
        /// Создает клиента по его JWT/OAuth токену
        /// </summary>
        public static SpeechKitClient Create(IToken tokenPayLoad)
        {
            return new SpeechKitClient(tokenPayLoad);
        }

        /// <summary>
        /// Создает клиента из JWT токена, предварительно сформировав его по
        /// идентификаторам сервисного аккаунта и севрисного авторизованного ключа
        /// и закрытому сервисному авториванному ключу
        /// </summary>
        public static SpeechKitClient CreateFromJwtToken(string accId, string authKeyId, string privateKeyFileName)
        {
            var tokenPayLoad = GetJwtToken(accId, authKeyId, privateKeyFileName);
            return new SpeechKitClient(tokenPayLoad);
        }
        #endregion

        /// <summary>
        /// Возвращает массив байтов, из которых формируется аудио, по опции
        /// </summary>
        public async Task<byte[]> GetSpeech(SynthesisOptions options, bool setHeader = true)
        {
            if(setHeader)
                _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _amiTokenInfo.Key);
            
            var queryParams = options.GetDictionary();
            var content = new FormUrlEncodedContent(queryParams);
            var response = await _httpClient.PostAsync(Params.TEXT_TO_SPEECH_URI, content);
            
            if(setHeader)
                _httpClient.DefaultRequestHeaders.Clear();

            return await response.Content.ReadAsByteArrayAsync();
        }
        
        /// <summary>
        /// Формирует аудио-файл по переданным опциям 
        /// </summary>
        public async Task SaveSpeech(SynthesisOptions options, string directoryName, bool setHeader = true)//TestBinnaryFileName
        {
            var responseBytes = await GetSpeech(options, setHeader);
             
            var fileName = $"{directoryName}\\{options.Text.GetValidPathString()}";
            if (options.ExteranlOptions.AudioFormat == SynthesisAudioFormat.Opus)
                fileName += ".ogg";
            
            File.WriteAllBytes(fileName, responseBytes);
        }
        
        /// <summary>
        /// Формирует аудио-файлы по переданным опциям 
        /// </summary>
        public async Task<byte[][]> GetMultipleSpeech(IEnumerable<SynthesisOptions> optionsEnumerable)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _amiTokenInfo.Key);
            var optionsArray = optionsEnumerable.ToArray();

            var dataList = new List<byte[]> { Capacity = optionsArray.Length };
            foreach (var option in optionsArray)
                dataList.Add(await GetSpeech(option, false));

            _httpClient.DefaultRequestHeaders.Clear();
            return dataList.ToArray();
        }
        
        /// <summary>
        /// Формирует аудио-файлы по переданным опциям 
        /// </summary>
        public async Task SaveMultipleSpeech(IEnumerable<SynthesisOptions> optionsArray, string directoryName)
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _amiTokenInfo.Key);
            foreach (var options in optionsArray)
                await SaveSpeech(options, directoryName, false);
            
            _httpClient.DefaultRequestHeaders.Clear();
        }

        /// <summary>
        /// Возвращает JWT-токен
        /// </summary>
        private static JwtToken GetJwtToken(string accId, string authKeyId, string privateKeyFileName)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var headers = new Dictionary<string, object> { {"kid", authKeyId } };
            var payload = new Dictionary<string, object>
            {
                { "aud", Params.JWT_TOKEN_URI },
                { "iss", accId },
                { "iat", now },
                { "exp", now + 3600 }
            };
            
            RsaPrivateCrtKeyParameters privateKeyParams;
            using (var pemStream = File.OpenText(privateKeyFileName))
                privateKeyParams = new PemReader(pemStream).ReadObject() as RsaPrivateCrtKeyParameters;

            var encodedToken = "";
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(DotNetUtilities.ToRSAParameters(privateKeyParams));
                encodedToken = JWT.Encode(payload, rsa, JwsAlgorithm.PS256, headers);
            }
            return new JwtToken { Key = encodedToken };
        }
        
        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
                return;

            _httpClient?.Dispose();
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SpeechKitClient));
        }

        #endregion
    }
}