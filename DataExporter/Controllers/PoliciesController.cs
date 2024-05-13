using DataExporter.Dtos;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PoliciesController : ControllerBase
    {
        private PolicyService _policyService;

        public PoliciesController(PolicyService policyService) 
        { 
            _policyService = policyService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPolicies([FromBody]CreatePolicyDto createPolicyDto)
        {
            if (createPolicyDto == null)
            {
                return BadRequest("Invalid policy data.");
            }

            var policy = await _policyService.CreatePolicyAsync(createPolicyDto);

            if (policy == null)
            {
                return BadRequest("Failed to create the policy.");
            }

            return CreatedAtAction(nameof(GetPolicy), new { policyId = policy.Id }, policy);
        }

        [HttpGet]
        public async Task<IActionResult> GetPolicies()
        {
            var policies = await _policyService.ReadPoliciesAsync();

            return Ok(policies);
        }

        [HttpGet("{policyId}")]
        public async Task<IActionResult> GetPolicy(int policyId)
        {
            var policy = await _policyService.ReadPolicyAsync(policyId);

            if (policy == null)
            {
                return NotFound($"Policy with ID {policyId} not found.");
            }

            return Ok(policy);
        }

        [HttpPost("export")]
        public async Task<IActionResult> ExportData([FromQuery]DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("The start date must be before the end date.");
            }

            var policies = await _policyService.GetPoliciesWithNotesAsync(startDate, endDate);
            var exportDtos = policies.Select(p => new ExportDto
            {
                PolicyNumber = p.PolicyNumber,
                Premium = p.Premium,
                StartDate = p.StartDate,
                Notes = p.Notes.Select(n => n.Text).ToList()
            }).ToList();

            return Ok(exportDtos);
        }
    }
}
