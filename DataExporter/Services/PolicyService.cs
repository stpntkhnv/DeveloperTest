using DataExporter.Dtos;
using DataExporter.Model;
using Microsoft.EntityFrameworkCore;


namespace DataExporter.Services
{
    public class PolicyService
    {
        private ExporterDbContext _dbContext;

        public PolicyService(ExporterDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Creates a new policy from the DTO.
        /// </summary>
        /// <param name="policy"></param>
        /// <returns>Returns a ReadPolicyDto representing the new policy, if succeded. Returns null, otherwise.</returns>
        public async Task<ReadPolicyDto?> CreatePolicyAsync(CreatePolicyDto createPolicyDto)
        {
            var policy = new Policy
            {
                PolicyNumber = createPolicyDto.PolicyNumber,
                Premium = createPolicyDto.Premium,
                StartDate = createPolicyDto.StartDate
            };

            _dbContext.Policies.Add(policy);
            await _dbContext.SaveChangesAsync();

            return new ReadPolicyDto
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };
        }

        /// <summary>
        /// Retrives all policies.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a list of ReadPoliciesDto.</returns>
        public async Task<IList<ReadPolicyDto>> ReadPoliciesAsync()
        {
            return await _dbContext.Policies.
                Select(p => new ReadPolicyDto
            {
                Id = p.Id,
                PolicyNumber = p.PolicyNumber,
                Premium = p.Premium,
                StartDate = p.StartDate
            }).ToListAsync();
        }

        /// <summary>
        /// Retrieves a policy by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Returns a ReadPolicyDto.</returns>
        public async Task<ReadPolicyDto?> ReadPolicyAsync(int id)
        {
            var policy = await _dbContext.Policies.SingleOrDefaultAsync(x => x.Id == id);

            if (policy == null)
            {
                return null;
            }

            var policyDto = new ReadPolicyDto()
            {
                Id = policy.Id,
                PolicyNumber = policy.PolicyNumber,
                Premium = policy.Premium,
                StartDate = policy.StartDate
            };

            return policyDto;
        }

        public async Task<List<Policy>> GetPoliciesWithNotesAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.Policies
                .Include(p => p.Notes)
                .Where(p => p.StartDate >= startDate && p.StartDate <= endDate)
                .ToListAsync();
        }
    }
}
