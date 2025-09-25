using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAIS.Models;
using SAIS.Models.Data;

namespace SAIS.Controllers
{
    public class SocialAssistanceProgramController : Controller
    {
        private readonly SAISDbContext _context;

        public SocialAssistanceProgramController(SAISDbContext context)
        {
            _context = context;
        }

        // GET: SocialAssistanceProgram
        public async Task<IActionResult> Index()
        {
            return View(await _context.SocialAssistancePrograms.ToListAsync());
        }

        // GET: SocialAssistanceProgram/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socialAssistanceProgram = await _context.SocialAssistancePrograms
                .Include(sap => sap.AppliedPrograms)
                    .ThenInclude(ap => ap.Application)
                        .ThenInclude(a => a.Applicant)
                .FirstOrDefaultAsync(m => m.ProgramId == id);

            if (socialAssistanceProgram == null)
            {
                return NotFound();
            }

            return View(socialAssistanceProgram);
        }

        // GET: SocialAssistanceProgram/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SocialAssistanceProgram/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProgramName")] SocialAssistanceProgram socialAssistanceProgram)
        {
            if (ModelState.IsValid)
            {
                _context.Add(socialAssistanceProgram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(socialAssistanceProgram);
        }

        // GET: SocialAssistanceProgram/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socialAssistanceProgram = await _context.SocialAssistancePrograms.FindAsync(id);
            if (socialAssistanceProgram == null)
            {
                return NotFound();
            }
            return View(socialAssistanceProgram);
        }

        // POST: SocialAssistanceProgram/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProgramId,ProgramName")] SocialAssistanceProgram socialAssistanceProgram)
        {
            if (id != socialAssistanceProgram.ProgramId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(socialAssistanceProgram);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SocialAssistanceProgramExists(socialAssistanceProgram.ProgramId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(socialAssistanceProgram);
        }

        // GET: SocialAssistanceProgram/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socialAssistanceProgram = await _context.SocialAssistancePrograms
                .FirstOrDefaultAsync(m => m.ProgramId == id);
            if (socialAssistanceProgram == null)
            {
                return NotFound();
            }

            return View(socialAssistanceProgram);
        }

        // POST: SocialAssistanceProgram/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var socialAssistanceProgram = await _context.SocialAssistancePrograms.FindAsync(id);
            if (socialAssistanceProgram != null)
            {
                _context.SocialAssistancePrograms.Remove(socialAssistanceProgram);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SocialAssistanceProgramExists(int id)
        {
            return _context.SocialAssistancePrograms.Any(e => e.ProgramId == id);
        }
    }
}
