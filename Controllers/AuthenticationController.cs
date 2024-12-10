using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimpleHelpDeskAPI.DbContexts;
using SimpleHelpDeskAPI.Models;
using SimpleHelpDeskAPI.Utilities;

namespace SimpleHelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IPasswordHasher<object> _passwordHasher;
        private readonly AuthenticationUtility _authenticationUtility;
        private readonly IMemoryCache _memoryCache;

        public AuthenticationController(ApplicationDbContext applicationDbContext, IPasswordHasher<object> passwordHasher, AuthenticationUtility authenticationUtility, IMemoryCache memoryCache)
        {
            _applicationDbContext = applicationDbContext;
            _passwordHasher = passwordHasher;
            _authenticationUtility = authenticationUtility;
            _memoryCache = memoryCache;
        }

        [HttpPost("user/sign-up")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUpUserAsync(UserForm userForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _applicationDbContext.Users.AnyAsync(u => u.Email == userForm.Email))
                return BadRequest(new { Message = "email has already been used" });

            var newUserId = Guid.NewGuid();
            var newUserHashedPassword = _passwordHasher.HashPassword(null!, userForm.Password);

            await _applicationDbContext.Users.AddAsync(new()
            {
                Id = newUserId,
                FirstName = userForm.FirstName,
                MiddleName = userForm.MiddleName,
                LastName = userForm.LastName,
                BirthDate = userForm.BirthDate,
                PhoneNumber = userForm.PhoneNumber,
                Email = userForm.Email,
                Password = newUserHashedPassword
            });

            await _applicationDbContext.SaveChangesAsync();

            await _authenticationUtility.SignInAsync("User", newUserId.ToString());

            return Created("/api/users/me", null);
        }

        [HttpPost("administrator/sign-up")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignUpAdministratorAsync(AdministratorForm administratorForm)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _applicationDbContext.Administrators.AnyAsync(a => a.Email == administratorForm.Email))
                return BadRequest(new { Message = "email has already been used" });

            var newAdministratorId = Guid.NewGuid();
            var newAdministratorHashedPassword = _passwordHasher.HashPassword(null!, administratorForm.Password);

            await _applicationDbContext.Administrators.AddAsync(new()
            {
                Id = newAdministratorId,
                FirstName = administratorForm.FirstName,
                LastName = administratorForm.LastName,
                Email = administratorForm.Email,
                Password = newAdministratorHashedPassword
            });

            await _applicationDbContext.SaveChangesAsync();

            await _authenticationUtility.SignInAsync("Administrator", newAdministratorId.ToString());

            return Created("/api/administrators/me", null);
        }

        [HttpPost("user/sign-in")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignInUserAsync(Credential credential)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _applicationDbContext.Users
                .Where(u => u.Email == credential.Email)
                .Select(u => new { u.Id, u.Password })
                .FirstOrDefaultAsync();

            if (user is null)
                return BadRequest(new { Message = "user is not registered" });

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(null!, user.Password, credential.Password);

            if (passwordVerificationResult is PasswordVerificationResult.Failed)
                return BadRequest(new { Message = "wrong password" });

            await _authenticationUtility.SignInAsync("User", user.Id.ToString());

            return Ok();
        }

        [HttpPost("administrator/sign-in")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SignInAdministratorAsync(Credential credential)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var administrator = await _applicationDbContext.Administrators
                .Where(a => a.Email == credential.Email)
                .Select(a => new { a.Id, a.Password })
                .FirstOrDefaultAsync();

            if (administrator is null)
                return BadRequest(new { Message = "administrator is not registered" });

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(null!, administrator.Password, credential.Password);

            if (passwordVerificationResult is PasswordVerificationResult.Failed)
                return BadRequest(new { Message = "wrong password" });

            await _authenticationUtility.SignInAsync("Administrator", administrator.Id.ToString());

            return Ok();
        }

        [HttpPost("sign-out")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> SignOutAsync()
        {
            var idClaim = HttpContext.User.Claims.First(cl => cl.Type == "ID");

            _memoryCache.Remove(idClaim.Value);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return NoContent();
        }
    }
}