using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Police.Data;
using Police.Migrations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PoliceS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReferenceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReferenceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("user-reference")]
        public async Task<ActionResult<IEnumerable<ReferencesModel>>> GetUserReference(int userId, string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya UserId.");
            }

            var references = await _context.References
                                          .Where(c => c.CreatedBy == userId)
                                          .ToListAsync();

            return Ok(references);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<ReferencesModel>> GetReference(int id, [FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            var reference = await _context.References.FindAsync(id);

            if (reference == null)
            {
                return NotFound("Referans bulunamadı.");
            }

            if (reference.CreatedBy != userId)
            {
                return Unauthorized("Bu referansı görüntüleme yetkiniz yok.");
            }

            return Ok(reference);
        }


        [HttpPost]
        public async Task<ActionResult<ReferencesModel>> PostReference([FromQuery] int userId, [FromQuery] string token, [FromBody] ReferencesModel reference)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            reference.CreatedBy = userId;
            reference.CreatedDate = DateTime.UtcNow;
            reference.UpdatedDate = DateTime.UtcNow;

            _context.References.Add(reference);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReference), new { id = reference.ReferenceId, userId = userId, token = token }, reference);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutReference(int id, [FromQuery] int userId, [FromQuery] string token, [FromBody] ReferencesModel reference)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            if (id != reference.ReferenceId)
            {
                return BadRequest("Referans ID uyuşmazlığı.");
            }

            var existingReference = await _context.References.FindAsync(id);
            if (existingReference == null)
            {
                return NotFound("Referans bulunamadı.");
            }

            if (existingReference.CreatedBy != userId)
            {
                return Unauthorized("Bu referansı güncelleme yetkiniz yok.");
            }

            // Sadece gönderilen alanları güncelle
            if (!string.IsNullOrEmpty(reference.NameSurname))
            {
                existingReference.NameSurname = reference.NameSurname;
            }

            if (!string.IsNullOrEmpty(reference.City))
            {
                existingReference.City = reference.City;
            }

            if (!string.IsNullOrEmpty(reference.District))
            {
                existingReference.District = reference.District;
            }

            if (!string.IsNullOrEmpty(reference.Telephone))
            {
                existingReference.Telephone = reference.Telephone;
            }

            if (!string.IsNullOrEmpty(reference.Email))
            {
                existingReference.Email = reference.Email;
            }

            existingReference.UpdatedDate = DateTime.UtcNow;

            _context.Entry(existingReference).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReferenceExist(id))
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
        public async Task<IActionResult> DeleteReference(int id, [FromQuery] int userId, [FromQuery] string token)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId && u.Token == token);

            if (user == null)
            {
                return Unauthorized("Geçersiz token veya kullanıcı.");
            }

            var reference = await _context.References.FindAsync(id);
            if (reference == null)
            {
                return NotFound("Referans bulunamadı.");
            }

            if (reference.CreatedBy != userId)
            {
                return Unauthorized("Bu referansı silme yetkiniz yok.");
            }

            _context.References.Remove(reference);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReferenceExist(int id)
        {
            return _context.References.Any(e => e.ReferenceId == id);
        }
    }
}
