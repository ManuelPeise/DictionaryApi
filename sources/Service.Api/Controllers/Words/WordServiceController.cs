using Logic.Import.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Service.Api.Controllers.Words
{
    public class WordServiceController : ApiControllerBase
    {
        private readonly IKaikkiParser _kaikkiParser;

        public WordServiceController(IKaikkiParser kaikkiParser)
        {
            _kaikkiParser = kaikkiParser;
        }

        [UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
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
    }
}
