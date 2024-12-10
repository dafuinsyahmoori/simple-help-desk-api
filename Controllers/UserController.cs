using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimpleHelpDeskAPI.DbContexts;

namespace SimpleHelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Policy = "UserOnly")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMemoryCache _memoryCache;

        public UserController(ApplicationDbContext applicationDbContext, IMemoryCache memoryCache)
        {
            _applicationDbContext = applicationDbContext;
            _memoryCache = memoryCache;
        }

        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMeAsync()
        {
            var idClaim = HttpContext.User.Claims.First(cl => cl.Type == "ID");

            if (_memoryCache.TryGetValue(idClaim.Value, out var cachedUser))
                return Ok(cachedUser);

            var id = Guid.Parse(idClaim.Value);

            var user = await _applicationDbContext.Users
                .Where(u => u.Id == id)
                .Select(u => new
                {
                    u.Id,
                    u.FirstName,
                    u.MiddleName,
                    u.LastName,
                    Age = DateTime.UtcNow.Year - u.BirthDate.Year,
                    u.PhoneNumber,
                    u.Email
                })
                .FirstOrDefaultAsync();

            if (user is null)
                return BadRequest(new { Message = "user is not found" });

            _memoryCache.Set(idClaim.Value, user, TimeSpan.FromMinutes(2));

            return Ok(user);
        }

        [HttpGet("me/complaint-requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyComplaintRequestsAsync(bool includeResponse = false)
        {
            var idClaim = HttpContext.User.Claims.First(cl => cl.Type == "ID");
            var id = Guid.Parse(idClaim.Value);

            var queryableComplaintRequests = _applicationDbContext.ComplaintRequests.Where(c => EF.Property<Guid>(c, "UserId") == id);

            if (includeResponse)
                queryableComplaintRequests = queryableComplaintRequests.Include(c => c.ComplaintResponse);

            var complaintRequests = await queryableComplaintRequests
                .AsNoTracking()
                .ToListAsync();

            return Ok(complaintRequests);
        }
    }
}