using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Data.Database.Entities.User
{
    public class UserEntity : AEntityBase
    {
        public Guid UserIdExternal { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public byte[] ProfileImage { get; set; } = Array.Empty<byte>();
        public DateTime DateOfBirth { get; set; }
        public UserRoleEnum UserRole { get; set; }
        public int UserCredentialsId { get; set; }
        [ForeignKey(nameof(UserCredentialsId))]
        public UserCredentialsEntity UserCredentials { get; set; }
    }
}
