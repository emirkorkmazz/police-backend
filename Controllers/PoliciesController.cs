using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Police.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Police.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoliciesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PoliciesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPoliciesWithUserId(int userId, string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            // Fetch policies and join with customers, products, and companies
            var policies = await _context.Policies
                .Where(p => p.CreatedBy == userId)
                .Select(p => new
                {
                    p.PolicyId,
                    p.CustomerId,
                    CustomerName = _context.Customers
                        .Where(c => c.CustomerId == p.CustomerId)
                        .Select(c => c.IsPerson ? c.NameSurname : c.CompanyName)
                        .FirstOrDefault(),
                    p.ProductId,
                    ProductName = _context.Products
                        .Where(pr => pr.ProductId == p.ProductId)
                        .Select(pr => pr.ProductName)
                        .FirstOrDefault(),
                    p.CompanyId,
                    CompanyName = _context.Companies
                        .Where(co => co.CompanyId == p.CompanyId)
                        .Select(co => co.FullName)
                        .FirstOrDefault(),
                    p.PolicyNumber,
                    p.PolicyStartDate,
                    p.PolicyEndDate,
                    p.LicenseNumber,
                    p.PlateNumber,
                    p.ShasiNumber,
                    p.PolicyAmount,
                    p.PolicyRate,
                    p.ReminderDays,
                    p.CreatedBy,
                    p.CreatedDate,
                    p.UpdatedDate
                })
                .ToListAsync();

            return Ok(policies);
        }


        [HttpPost]
        public async Task<ActionResult<PoliciesModel>> PostPolicy([FromQuery] int userId, [FromQuery] string token, [FromBody] PoliciesModel policy)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            policy.CreatedBy = userId;
            policy.CreatedDate = DateTime.UtcNow;
            policy.UpdatedDate = DateTime.UtcNow;

            // Mevcut en yüksek PolicyId'yi bul
            var maxPolicyId = await _context.Policies.MaxAsync(p => (int?)p.PolicyId) ?? 0;

            // Yeni PolicyId'yi belirle (mevcut en yüksek PolicyId + 1)
            policy.PolicyId = maxPolicyId + 1;

            _context.Policies.Add(policy);

            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            var existingIncome = await _context.Incomes
                .SingleOrDefaultAsync(i => i.CompanyId == policy.CompanyId
                                        && i.ProductId == policy.ProductId
                                        && i.Month == currentMonth
                                        && i.Year == currentYear);

            if (existingIncome != null)
            {
                existingIncome.Amount += policy.PolicyAmount;
                _context.Incomes.Update(existingIncome);
            }
            else
            {
                var newIncome = new IncomeModel
                {
                    CompanyId = policy.CompanyId,
                    ProductId = policy.ProductId,
                    Month = currentMonth,
                    Year = currentYear,
                    Amount = policy.PolicyAmount
                };
                _context.Incomes.Add(newIncome);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
            {
                return Conflict("Duplicate key value violates unique constraint. The income record already exists for the specified ProductId, CompanyId, Month, and Year.");
            }

            return CreatedAtAction(nameof(GetPolicy), new { id = policy.PolicyId }, policy);
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<PoliciesModel>> GetPolicy(int id, [FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            var policy = await _context.Policies.SingleOrDefaultAsync(p => p.PolicyId == id && p.CreatedBy == userId);

            if (policy == null)
            {
                return NotFound("Poliçe bulunamadı.");
            }

            return Ok(policy);
        }

        [HttpGet("expiring-soon")]
        public async Task<ActionResult<IEnumerable<object>>> GetPoliciesExpiringSoon(int userId, string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            var today = DateTime.UtcNow;
            var upcomingDate = today.AddDays(10);

            var expiringPolicies = await _context.Policies
                .Where(p => p.CreatedBy == userId && p.PolicyEndDate <= upcomingDate && p.PolicyEndDate >= today)
                .Select(p => new
                {
                    p.PolicyId,
                    p.CustomerId,
                    CustomerName = _context.Customers
                        .Where(c => c.CustomerId == p.CustomerId)
                        .Select(c => c.IsPerson ? c.NameSurname : c.CompanyName)
                        .FirstOrDefault(),
                    Telephone = _context.Customers
                .Where(c => c.CustomerId == p.CustomerId)
                .Select(c => c.Telephone)
                .FirstOrDefault(),
                    p.ProductId,
                    ProductName = _context.Products
                        .Where(pr => pr.ProductId == p.ProductId)
                        .Select(pr => pr.ProductName)
                        .FirstOrDefault(),
                    p.CompanyId,
                    CompanyName = _context.Companies
                        .Where(co => co.CompanyId == p.CompanyId)
                        .Select(co => co.FullName)
                        .FirstOrDefault(),
                    p.PolicyNumber,
                    p.PolicyStartDate,
                    p.PolicyEndDate,
                    p.LicenseNumber,
                    p.PlateNumber,
                    p.ShasiNumber,
                    p.PolicyAmount,
                    p.PolicyRate,
                    p.ReminderDays,
                    p.CreatedBy,
                    p.CreatedDate,
                    p.UpdatedDate
                })
                .ToListAsync();

            return Ok(expiringPolicies);
        }


    }
}
