using Logic.Parsing.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Service.Api.Controllers.Words
{
    public class KaikkiServiceController : ApiControllerBase
    {
        private readonly IKaikkiDumpFileParser _kaikkiDumpFileParser;
        public KaikkiServiceController(IKaikkiDumpFileParser kaikkiDumpFileParser)
        {
            
            _kaikkiDumpFileParser = kaikkiDumpFileParser;
        }

        [UserRoleAuthentication(RequiredRole = UserRoleEnum.MaintenanceUser)]
        [HttpPost(Name = "ExecuteDumpFileParser")]
        public async Task ExecuteDumpFileParser()
        {
            //await _kaikkiDumpFileParser.ParseFileStream(new List<TranslationEnum>
            //{
            //   TranslationEnum.De,
            //   TranslationEnum.En,
            //   TranslationEnum.Da
            //});
        }
    }
}
