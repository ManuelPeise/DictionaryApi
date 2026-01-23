using Data.Database.Entities.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Data.Database.Seeds
{
    public class UserCredentialsSeed : IEntityTypeConfiguration<UserCredentialsEntity>
    {
        public void Configure(EntityTypeBuilder<UserCredentialsEntity> builder)
        {
            var salt = new Guid("956abc96-ef50-4848-a3e1-776c1060d2f6").ToString();
            var timeStamp = DateTime.Parse("2026.01.01", CultureInfo.InvariantCulture);

            builder.HasData(new UserCredentialsEntity
            {
                Id = 1,
                Salt = salt,
                PasswordHash = GetPasswordHash("Pass@word", salt),
                CreatedAt = timeStamp,
                CreatedBy = "System"
            });
        }

        private string GetPasswordHash(string password, string salt)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password).ToList();
            passwordBytes.AddRange(Encoding.UTF8.GetBytes(salt));

            return Convert.ToBase64String(passwordBytes.ToArray());

        }
    }
}
