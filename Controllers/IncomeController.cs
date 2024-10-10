using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Police.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoliceS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public IncomeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("searchwithmonthyear")]
        public async Task<ActionResult<IEnumerable<IncomeModel>>> GetIncomeByMonthYear(int userId, string token, int month, int year)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);
            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var incomes = await _context.Incomes
                .Where(i => i.CompanyId == userId && i.Month == month && i.Year == year)
                .ToListAsync();

            if (incomes == null || incomes.Count == 0)
            {
                return NotFound("Belirtilen ay ve yıl için gelir bulunamadı.");
            }

            return Ok(incomes);
        }

        [HttpGet("getincomes")]
        public async Task<ActionResult<IEnumerable<object>>> GetIncome(int userId, string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);
            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var incomes = await _context.Incomes
                .Where(i => i.CompanyId == userId)
                .Select(i => new
                {
                    i.IncomeId,
                    i.CompanyId,
                    i.ProductId,
                    i.Month,
                    i.Year,
                    i.Amount,
                    ProductName = _context.Products
                                  .Where(p => p.ProductId == i.ProductId)
                                  .Select(p => p.ProductName)
                                  .FirstOrDefault()
                })
                .ToListAsync();

            if (incomes == null || incomes.Count == 0)
            {
                return NotFound("Belirtilen yıl için gelir bulunamadı.");
            }

            return Ok(incomes);
        }

        [HttpGet("searchwithyear")]
        public async Task<ActionResult<IEnumerable<object>>> GetIncomeByYear(int userId, string token, int year)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);
            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var incomes = await _context.Incomes
                .Where(i => i.CompanyId == userId && i.Year == year)
                .Select(i => new
                {
                    i.IncomeId,
                    i.CompanyId,
                    i.ProductId,
                    i.Month,
                    i.Year,
                    i.Amount,
                    ProductName = _context.Products
                                  .Where(p => p.ProductId == i.ProductId)
                                  .Select(p => p.ProductName)
                                  .FirstOrDefault()
                })
                .ToListAsync();

            if (incomes == null || incomes.Count == 0)
            {
                return NotFound("Belirtilen yıl için gelir bulunamadı.");
            }

            return Ok(incomes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IncomeModel>> GetIncomeById(int id, [FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);
            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var income = await _context.Incomes.FindAsync(id);

            if (income == null)
            {
                return NotFound("Gelir kaydı bulunamadı.");
            }

            if (income.CompanyId != userId)
            {
                return Unauthorized("Bu gelir kaydını görüntüleme yetkiniz yok.");
            }

            return Ok(income);
        }


        [HttpPost]
        public async Task<ActionResult<IncomeModel>> PostIncome([FromQuery] int userId, [FromQuery] string token, [FromBody] IncomeModel income)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            income.CompanyId = userId;

            _context.Incomes.Add(income);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIncome), new { id = income.IncomeId }, income);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncome(int id, [FromQuery] int userId, [FromQuery] string token, [FromBody] IncomeModel income)
        {
            if (id != income.IncomeId)
            {
                return BadRequest();
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            if (income.CompanyId != userId)
            {
                return Unauthorized("Bu gelir kaydını düzenleme yetkiniz yok.");
            }

            _context.Entry(income).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IncomeExist(id))
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
        public async Task<IActionResult> DeleteIncome(int id, [FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            var income = await _context.Incomes.FindAsync(id);
            if (income == null)
            {
                return NotFound();
            }

            if (income.CompanyId != userId)
            {
                return Unauthorized("Bu gelir kaydını silme yetkiniz yok.");
            }

            _context.Incomes.Remove(income);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool IncomeExist(int id)
        {
            return _context.Incomes.Any(e => e.IncomeId == id);
        }
    }
}
