using Shared.Enums;

namespace Logic.Words.Interfaces
{
    public interface IWordService
    {
        Task ExecuteWordService(WordServiceType type);
    }
}
