using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolNexus.Application.Audit;
using ToolNexus.Application.Audit.DTOs;

namespace ToolNexus.WebUI.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditController : ControllerBase
    {
        private readonly IAuditService _auditService;

        public AuditController(IAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<AuditTrailDto>>> GetAll()
        {
            var audits = await _auditService.GetAllAsync();
            return Ok(audits);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<AuditTrailDto>> GetById(Guid id)
        {
            var audit = await _auditService.GetByIdAsync(id);
            if (audit == null)
                return NotFound();

            return Ok(audit);
        }

        [HttpGet("entity/{entityType}/{entityId}")]
        [Authorize(Roles = "ADMIN,SUPERVISOR")]
        public async Task<ActionResult<IEnumerable<AuditTrailDto>>> GetByEntity(string entityType, string entityId)
        {
            var audits = await _auditService.GetByEntityAsync(entityType, entityId);
            return Ok(audits);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<IEnumerable<AuditTrailDto>>> GetByUser(Guid userId)
        {
            var audits = await _auditService.GetByUserAsync(userId);
            return Ok(audits);
        }

        [HttpPost("filter")]
        [Authorize(Roles = "ADMIN,SUPERVISOR")]
        public async Task<ActionResult<IEnumerable<AuditTrailDto>>> GetByFilter([FromBody] AuditTrailFilterDto filter)
        {
            var audits = await _auditService.GetByFilterAsync(filter);
            return Ok(audits);
        }

        [HttpGet("statistics")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<Dictionary<string, int>>> GetStatistics([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var statistics = await _auditService.GetAuditStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }

        [HttpGet("my-activity")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AuditTrailDto>>> GetMyActivity()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid user ID");

            var audits = await _auditService.GetByUserAsync(userId);
            return Ok(audits);
        }
    }
}