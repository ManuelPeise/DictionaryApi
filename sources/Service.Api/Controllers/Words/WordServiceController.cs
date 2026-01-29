using Logic.Words.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace Service.Api.Controllers.Words
{
    public class WordServiceController
    {
        private readonly IWordService _wordService;

        public WordServiceController(IWordService wordService)
        {
            _wordService = wordService;
        }

        [HttpGet(Name = "LoadWords")]
        public async Task<IActionResult> LoadWords()
        {
            try
            {
                await _wordService.ExecuteWordService(Shared.Enums.WordServiceType.Kaikki);
                return new OkResult();
            }
            catch (Exception ex)
            {
                // Log the exception (not shown here for brevity)
                return new StatusCodeResult(500); // Internal Server Error
            }
        }
    }
}
