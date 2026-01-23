using Logic.Words.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;

namespace Service.Api
{
    public class WordController : ApiControllerBase
    {
        private readonly IWordService _wordService;
        public WordController(IWordService wordService)
        {
            _wordService = wordService;
        }

        [HttpGet(Name = "ExecuteWordService")]
        public async Task<IActionResult> ExecuteWordService()
        {
            await _wordService.ExecuteWordService(WordServiceType.Kaikki);

            return Ok("Word service executed successfully.");
        }
    }
}
