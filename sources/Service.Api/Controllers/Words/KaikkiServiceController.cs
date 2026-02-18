using Logic.Import.Interfaces;
using Logic.Parsing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Service.Api.Controllers.Words
{
    public class KaikkiServiceController : ApiControllerBase
    {
        private readonly IKaikkiParser _kaikkiParser;
        private readonly IKaikkiDumpFileParser _kaikkiDumpFileParser;
        public KaikkiServiceController(IKaikkiParser kaikkiParser, IKaikkiDumpFileParser kaikkiDumpFileParser)
        {
            _kaikkiParser = kaikkiParser;
            _kaikkiDumpFileParser = kaikkiDumpFileParser;
        }

       // [UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
        [HttpPost(Name = "Execute")]
        public async Task Execute()
        {
            await _kaikkiParser.ParseFileStream(new List<KaikkiExtractionTypeEnum>
            {
               KaikkiExtractionTypeEnum.GermanExtractions,
               KaikkiExtractionTypeEnum.EnglishExtractions,
               KaikkiExtractionTypeEnum.DanishExtractions
            });
        }

        // [UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
        [HttpPost(Name = "ExecuteDumpFileParser")]
        public async Task ExecuteDumpFileParser()
        {
            await _kaikkiDumpFileParser.GetKaikkiWordDictionary(new List<TranslationEnum>
            {
               TranslationEnum.De,
               TranslationEnum.En,
               TranslationEnum.Da
            });
        }
    }
}
