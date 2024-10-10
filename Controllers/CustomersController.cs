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
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("user-customers")]
        public async Task<ActionResult<IEnumerable<CustomersModel>>> GetUserCustomers([FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var customers = await _context.Customers
                                          .Where(c => c.CreatedBy == userId)
                                          .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomersModel>> GetCustomer(int id, [FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);
            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound("Müşteri bulunamadı.");
            }

            if (customer.CreatedBy != userId)
            {
                return Unauthorized("Bu müşteri kaydını görüntüleme yetkiniz yok.");
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<CustomersModel>> PostCustomer([FromQuery] int userId, [FromQuery] string token, [FromBody] CustomersModel customer)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            // Tarih ve kullanıcı bilgilerini ayarlama
            customer.CreatedBy = userId;
            customer.CreatedDate = DateTime.UtcNow;
            customer.UpdatedBy = userId;
            customer.UpdatedDate = DateTime.UtcNow;

            _context.Customers.Add(customer);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Hata yönetimi ve loglama yapılabilir
                return StatusCode(500, "Veritabanına kaydedilirken bir hata oluştu.");
            }

            return Ok(customer);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, [FromQuery] int userId, [FromQuery] string token, [FromBody] CustomersModel customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            var existingCustomer = await _context.Customers.AsNoTracking().SingleOrDefaultAsync(c => c.CustomerId == id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            customer.CreatedDate = existingCustomer.CreatedDate;
            customer.UpdatedDate = DateTime.UtcNow; 
            customer.UpdatedBy = userId;
            customer.CreatedBy = existingCustomer.CreatedBy;
            customer.CustomerId = id;

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExist(id))
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
        public async Task<IActionResult> DeleteCustomer(int id, [FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Müşteri kaydı bulunamadı.");
            }

            if (customer.CreatedBy != userId)
            {
                return Unauthorized("Bu müşteri kaydını silme yetkiniz yok.");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("customer-names")]
        public async Task<ActionResult<IEnumerable<object>>> GetCustomerNames([FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var customerData = await _context.Customers
                .Where(c => c.CreatedBy == userId)
                .Select(c => new
                {
                    CustomerId = c.CustomerId,
                    CustomerName = c.IsPerson ? c.NameSurname : c.CompanyName
                })
                .ToListAsync();

            return Ok(customerData);
        }


        private bool CustomerExist(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
