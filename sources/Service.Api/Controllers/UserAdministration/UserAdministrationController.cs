using Logic.UserService.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models.Administration;

namespace Service.Api.Controllers.UserAdministration
{
    public class UserAdministrationController : ApiControllerBase
    {
        private readonly IUserAdministration _userAdministration;
       

        public UserAdministrationController(IUserAdministration userAdministration)
        {
            _userAdministration = userAdministration;
        }

        [HttpPost(Name = "RegisterUser")]
        public async Task<UserRegistrationResult?> RegisterUser([FromBody] UserRegistrationRequestModel requestModel)
        {
            var result = await _userAdministration.RegisterUser(requestModel);

            return result;
        }
    }
}
