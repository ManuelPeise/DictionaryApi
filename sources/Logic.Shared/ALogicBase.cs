using Data.Database;
using System.Text;

namespace Logic.Shared
{
    public abstract class ALogicBase
    {
        private readonly DatabaseContext _dbContext;
        public ALogicBase(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected string GetPasswordHash(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password).ToList();
            passwordBytes.AddRange(Encoding.UTF8.GetBytes(salt));

            return Convert.ToBase64String(passwordBytes.ToArray());
        }

    }
}
