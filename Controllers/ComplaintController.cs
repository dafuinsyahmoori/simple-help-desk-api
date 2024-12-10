using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleHelpDeskAPI.DbContexts;
using SimpleHelpDeskAPI.Models;
using SimpleHelpDeskAPI.Utilities;

namespace SimpleHelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    [Authorize]
    public class ComplaintController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ComplaintUtility _complaintUtility;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ComplaintController(ApplicationDbContext applicationDbContext, ComplaintUtility complaintUtility, IWebHostEnvironment webHostEnvironment)
        {
            _applicationDbContext = applicationDbContext;
            _complaintUtility = complaintUtility;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("requests")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllComplaintRequestsAsync(bool includeResponse = false)
        {
            if (includeResponse)
            {
                return Ok(await _applicationDbContext.ComplaintRequests
                    .Include(c => c.ComplaintResponse)
                    .AsNoTracking()
                    .ToListAsync());
            }

            return Ok(await _applicationDbContext.ComplaintRequests
                .AsNoTracking()
                .ToListAsync());
        }

        [HttpGet("requests/{id:int:min(1):required}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetComplaintRequestByIdAsync(int id, bool includeResponse = false)
        {
            var queryableComplaintRequests = _applicationDbContext.ComplaintRequests.Where(c => c.Id == id);

            if (includeResponse)
                queryableComplaintRequests = queryableComplaintRequests.Include(c => c.ComplaintResponse);

            var complaintRequest = await queryableComplaintRequests
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (complaintRequest is null)
                return BadRequest(new { Message = "complaint request is not found" });

            return Ok(complaintRequest);
        }

        [HttpGet("requests/{id:int:min(1):required}/image-or-video")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetComplaintRequestImageOrVideoByIdAsync(int id)
        {
            var complaintRequest = await _applicationDbContext.ComplaintRequests
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.ImageOrVideoUrl,
                    UserId = EF.Property<Guid>(c, "UserId")
                })
                .FirstOrDefaultAsync();

            if (complaintRequest is null)
                return BadRequest(new {Message = "complaint request is not found"});

            string? contentType = null;

            switch (Path.GetExtension(complaintRequest.ImageOrVideoUrl))
            {
                case ".png":
                    contentType = "image/png";
                    break;

                case ".jpg":
                    contentType = "image/jpg";
                    break;

                case ".jpeg":
                    contentType = "image/jpeg";
                    break;

                case ".mp4":
                    contentType = "video/mp4";
                    break;

                case ".mkv":
                    contentType = "video/mkv";
                    break;

                default:
                    break;
            }

            if (contentType is null)
                return NoContent();

            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads", complaintRequest.UserId.ToString(), complaintRequest.ImageOrVideoUrl!);

            return PhysicalFile(filePath, contentType);
        }

        [HttpPost("requests/{id:int:min(1):required}/do/respond")]
        [Authorize(Policy = "AdministratorOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RespondToComplaintAsync(int id, AdministratorComplaintResponse administratorComplaintResponse)
        {
            var administratorIdClaim = HttpContext.User.Claims.First(cl => cl.Type == "ID");
            var administratorId = Guid.Parse(administratorIdClaim.Value);

            var administrator = await _applicationDbContext.Administrators
                .Where(a => a.Id == administratorId)
                .FirstOrDefaultAsync();

            if (administrator is null)
                return BadRequest(new { Message = "administrator is not found" });

            var complaintRequest = await _applicationDbContext.ComplaintRequests
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();

            if (complaintRequest is null)
                return BadRequest(new { Message = "complaint request is not found" });

            administrator.ComplaintResponses.Add(new()
            {
                Title = administratorComplaintResponse.Title,
                Content = administratorComplaintResponse.Content,
                ComplaintRequest = complaintRequest
            });

            await _applicationDbContext.SaveChangesAsync();

            return Created("/api/administrators/me/complaint-responses", null);
        }

        [HttpPost("requests/do/create")]
        [Authorize(Policy = "UserOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateComplaintAsync([FromForm] UserComplaintRequest userComplaintRequest)
        {
            var userIdClaim = HttpContext.User.Claims.First(cl => cl.Type == "ID");
            var userId = Guid.Parse(userIdClaim.Value);

            var user = await _applicationDbContext.Users
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync();

            if (user is null)
                return BadRequest(new { Message = "user is not found" });

            string? newImageOrVideoFileName = null;

            if (userComplaintRequest.ImageOrVideo is not null)
            {
                var savedImageOrVideoFileName = await _complaintUtility.SaveComplaintImageOrVideoAsync(userComplaintRequest.ImageOrVideo, user.Id.ToString());

                if (savedImageOrVideoFileName is null)
                    return BadRequest(new { Message = "cannot save image or video" });

                newImageOrVideoFileName = savedImageOrVideoFileName;
            }

            user.ComplaintRequests.Add(new()
            {
                Title = userComplaintRequest.Title,
                ImageOrVideoUrl = newImageOrVideoFileName,
                Content = userComplaintRequest.Content
            });

            await _applicationDbContext.SaveChangesAsync();

            return Created("/api/users/me/complaint-requests", null);
        }

        [HttpGet("responses")]
        [Authorize(Policy = "AdministratorOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllComplaintResponsesAsync(bool includeRequest = false)
        {
            if (includeRequest)
            {
                return Ok(await _applicationDbContext.ComplaintResponses
                    .Include(c => c.ComplaintRequest)
                    .AsNoTracking()
                    .ToListAsync());
            }

            return Ok(await _applicationDbContext.ComplaintResponses
                .AsNoTracking()
                .ToListAsync());
        }

        [HttpGet("responses/{id:int:min(1):required}")]
        [Authorize(Policy = "AdministratorOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetComplaintResponseByIdAsync(int id, bool includeRequest = false)
        {
            var queryableComplaintResponses = _applicationDbContext.ComplaintResponses.Where(c => c.Id == id);

            if (includeRequest)
                queryableComplaintResponses = queryableComplaintResponses.Include(c => c.ComplaintRequest);

            var complaintResponse = await queryableComplaintResponses
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (complaintResponse is null)
                return BadRequest(new { Message = "complaint response is not found" });

            return Ok(complaintResponse);
        }
    }
}