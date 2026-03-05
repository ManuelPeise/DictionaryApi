using Logic.Parsing.Interfaces;
using Logic.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Models.Words;

namespace Service.Api.Controllers.Words
{
    public class KaikkiServiceController : ApiControllerBase
    {
        private readonly IKaikkiDumpFileParser _kaikkiDumpFileParser;
        private readonly ILibreTranslateClient _libreTranslateClient;

        public KaikkiServiceController(IKaikkiDumpFileParser kaikkiDumpFileParser, ILibreTranslateClient libreTranslateClient)
        {

            _kaikkiDumpFileParser = kaikkiDumpFileParser;
            _libreTranslateClient = libreTranslateClient;
        }

#if DEBUG
        //[UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
        [HttpPost(Name = "ExecuteDumpFileParser")]
        public async Task ExecuteDumpFileParser()
        {
            await _kaikkiDumpFileParser.ParseFileStream(new List<TranslationEnum>
            {
               TranslationEnum.De,
               TranslationEnum.En,
               TranslationEnum.Da
            });
        }
#endif

        [HttpGet(Name = "GetKaikkiWord")]
        public async Task<TranslationJsonModel?> GetKaikkiWord(string word)
        {
            return await _kaikkiDumpFileParser.LoadWord(word);

        }

        [HttpGet(Name = "Translate")]
        public async Task<string?> Translate(string word)
        {
            try
            {
                var translation = await _libreTranslateClient.TranslateWord(word, TranslationEnum.En.ToString().ToLower(), TranslationEnum.De.ToString().ToLower());

                if (translation == null) 
                { 
                    return "Translation not found.";
                }
                return translation;
            }
            catch(Exception exception)
            {
                return exception.Message;
            }

        }
    }
}
