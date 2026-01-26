using Shared.Models.User;

namespace Logic.Shared.Interfaces
{
    public interface ILogicBase
    {
        UserModel GetCurrentUser(bool? includeDetails = false);
    }
}
