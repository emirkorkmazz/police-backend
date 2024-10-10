using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Police.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Police.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompaniesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompaniesModel>>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompaniesModel>> GetCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        [HttpPost]
        public async Task<ActionResult<CompaniesModel>> PostCompany(CompaniesModel company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompany), new { id = company.CompanyId }, company);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany(int id, CompaniesModel company)
        {
            if (id != company.CompanyId)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompanyExists(int id)
        {
            return _context.Companies.Any(e => e.CompanyId == id);
        }
    }
}
