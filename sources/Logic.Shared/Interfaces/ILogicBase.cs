using Logic.Shared.Models;

namespace Logic.Shared.Interfaces
{
    public interface ILogicBase
    {
        CurrentUser GetCurrentUser(bool? includeDetails = false);
        string GetPasswordHash(string password, string salt);
    }
}
