using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SimpleHelpDeskAPI.DbContexts;

namespace SimpleHelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/administrators")]
    [Authorize(Policy = "AdministratorOnly")]
    public class AdministratorController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMemoryCache _memoryCache;

        public AdministratorController(ApplicationDbContext applicationDbContext, IMemoryCache memoryCache)
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

            if (_memoryCache.TryGetValue(idClaim.Value, out var cachedAdministrator))
                return Ok(cachedAdministrator);

            var id = Guid.Parse(idClaim.Value);

            var administrator = await _applicationDbContext.Administrators
                .Where(a => a.Id == id)
                .Select(a => new
                {
                    a.Id,
                    a.FirstName,
                    a.LastName,
                    a.Email
                })
                .FirstOrDefaultAsync();

            if (administrator is null)
                return BadRequest(new { Message = "administrator is not found" });

            _memoryCache.Set(idClaim.Value, administrator, TimeSpan.FromMinutes(2));

            return Ok(administrator);
        }

        [HttpGet("me/complaint-responses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyComplaintResponsesAsync(bool includeRequest = false)
        {
            var idClaim = HttpContext.User.Claims.First(cl => cl.Type == "ID");
            var id = Guid.Parse(idClaim.Value);

            var queryableComplaintResponses = _applicationDbContext.ComplaintResponses.Where(c => EF.Property<Guid>(c, "AdministratorId") == id);

            if (includeRequest)
                queryableComplaintResponses = queryableComplaintResponses.Include(c => c.ComplaintRequest);

            var complaintResponses = await queryableComplaintResponses
                .AsNoTracking()
                .ToListAsync();

            return Ok(complaintResponses);
        }
    }
}