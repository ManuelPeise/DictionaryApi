namespace Logic.Shared.Interfaces
{
    public interface ILibreTranslateClient
    {
        Task<string?> TranslateWord(string word, string sourceLanguage, string targetLanguage);
        Task<Dictionary<string, string?>> TranslateWords(List<string> words, string sourceLanguage, string targetLanguage);
    }
}
