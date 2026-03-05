using Logic.Shared.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Enums;
using Shared.Models.Settings;
using Shared.Models.Words;
using System.Text.Json;

namespace Logic.Shared
{
    public class LibreTranslateClient : ILibreTranslateClient
    {
        private readonly HttpClient _httpClient;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger<LibreTranslateClient> _logger;
        public LibreTranslateClient(IOptions<ApiSettings> options, ILogger<LibreTranslateClient> logger)
        {
            _apiSettings = options.Value;
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task<string?> TranslateWord(string word, string sourceLanguage, string targetLanguage)
        {
            try
            {
                var body = JsonSerializer.Serialize(new LibreTranslateRequestBody
                {
                    Word = word,
                    SourceLanguage = sourceLanguage.ToString(),
                    TargetLanguage = targetLanguage.ToString(),
                    Format = "text"
                });

                var requestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri($"{_apiSettings.LibreTranslateUrl}translate"),
                    Content = new StringContent(body, System.Text.Encoding.UTF8, "application/json")
                };

                var response = await _httpClient.SendAsync(requestMessage);

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                var translationResult = System.Text.Json.JsonSerializer.Deserialize<LibreTranslation>(responseContent);

                return translationResult?.TranslatedText ?? string.Empty;

            }
            catch (Exception exception)
            {
                await _logger.LogMessageAsync($"Error translating word.", LogMessageTypeEnum.Error, exception.Message, exception.StackTrace);

                return exception.Message;
            }
        }

        public async Task<Dictionary<string, string?>> TranslateWords(List<string> words, string sourceLanguage, string targetLanguage)
        {
            var dictionary = new Dictionary<string, string?>();

            foreach (var word in words)
            {
                var translaredWord = await TranslateWord(word, sourceLanguage, targetLanguage);

                if (!string.IsNullOrEmpty(translaredWord) && !dictionary.TryGetValue(word, out _))
                {
                    dictionary[word] = translaredWord;
                }
            }

            return dictionary;
        }
    }
}
