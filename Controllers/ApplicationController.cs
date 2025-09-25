using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAIS.Models;
using SAIS.Models.Data;
using SAIS.Models.DTOs;
using SAIS.Services;

namespace SAIS.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly SAISDbContext _context;

        public ApplicationController(SAISDbContext context)
        {
            _context = context;
        }

        // GET: Application
        public async Task<IActionResult> Index(string searchString, string officerFilter, string programFilter, DateTime? startDate, DateTime? endDate)
        {
            var applications = _context.Applications
                .Include(a => a.Applicant)
                .Include(a => a.Officer)
                .Include(a => a.AppliedPrograms)
                    .ThenInclude(ap => ap.SocialAssistanceProgram)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                applications = applications.Where(a =>
                    a.Applicant.FirstName.Contains(searchString) ||
                    a.Applicant.LastName.Contains(searchString) ||
                    a.Applicant.IdNumber.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(officerFilter))
            {
                applications = applications.Where(a =>
                    a.Officer.OfficerName.Contains(officerFilter));
            }

            if (!string.IsNullOrEmpty(programFilter))
            {
                applications = applications.Where(a =>
                    a.AppliedPrograms.Any(ap => ap.SocialAssistanceProgram.ProgramName.Contains(programFilter)));
            }

            if (startDate.HasValue)
            {
                applications = applications.Where(a => a.ApplicationDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                applications = applications.Where(a => a.ApplicationDate <= endDate.Value);
            }

            var applicationList = await applications.OrderByDescending(a => a.ApplicationDate).ToListAsync();
            var applicationDtos = applicationList.Select(MappingService.ToApplicationListDto).ToList();

            return View(applicationDtos);
        }

        // GET: Application/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.Applicant)
                    .ThenInclude(ap => ap.GenderCategory)
                .Include(a => a.Applicant)
                    .ThenInclude(ap => ap.MaritalStatus)
                .Include(a => a.Applicant)
                    .ThenInclude(ap => ap.Village)
                        .ThenInclude(v => v.SubLocation)
                            .ThenInclude(sl => sl.Location)
                                .ThenInclude(l => l.SubCounty)
                                    .ThenInclude(sc => sc.County)
                .Include(a => a.Applicant)
                    .ThenInclude(ap => ap.PhoneNumbers)
                .Include(a => a.Officer)
                .Include(a => a.AppliedPrograms)
                    .ThenInclude(ap => ap.SocialAssistanceProgram)
                .FirstOrDefaultAsync(m => m.ApplicationId == id);

            if (application == null)
            {
                return NotFound();
            }

            var applicationDto = MappingService.ToApplicationDetailsDto(application);
            return View(applicationDto);
        }

        // GET: Application/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ApplicantId"] = await GetApplicantsAsync();
            ViewData["OfficerId"] = await GetOfficersAsync();
            ViewData["Programs"] = await GetProgramsAsync();
            return View(new ApplicationCreateDto());
        }

        // POST: Application/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationCreateDto dto)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"=== CREATE APPLICATION DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"ProgramIds: {string.Join(", ", dto.ProgramIds ?? new List<int>())}");
            System.Diagnostics.Debug.WriteLine($"ProgramIds Count: {dto.ProgramIds?.Count ?? 0}");
            System.Diagnostics.Debug.WriteLine($"ApplicantId: {dto.ApplicantId}");
            System.Diagnostics.Debug.WriteLine($"OfficerId: {dto.OfficerId}");
            System.Diagnostics.Debug.WriteLine($"ApplicationDate: {dto.ApplicationDate}");
            System.Diagnostics.Debug.WriteLine($"ApplicantSignedDate: {dto.ApplicantSignedDate}");
            System.Diagnostics.Debug.WriteLine($"OfficerSignedDate: {dto.OfficerSignedDate}");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
            foreach (var key in ModelState.Keys)
            {
                var value = ModelState[key];
                if (value != null)
                {
                    System.Diagnostics.Debug.WriteLine($"  {key}: {value.AttemptedValue ?? "null"} (Valid: {value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid})");
                    if (value.ValidationState != Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid)
                    {
                        foreach (var error in value.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"    Error: {error.ErrorMessage}");
                        }
                    }
                }
            }
            System.Diagnostics.Debug.WriteLine($"=== END DEBUG ===");

            // Custom validation
            if (dto.ProgramIds == null || dto.ProgramIds.Count == 0 || dto.ProgramIds.All(id => id == 0))
            {
                ModelState.AddModelError("ProgramIds", "At least one program must be selected.");
            }

            if (dto.ApplicantId == null || dto.ApplicantId == 0)
            {
                ModelState.AddModelError("ApplicantId", "Please select an applicant.");
            }

            if (dto.OfficerId == null || dto.OfficerId == 0)
            {
                ModelState.AddModelError("OfficerId", "Please select an officer.");
            }

            // Check for duplicate program applications
            if (dto.ApplicantId.HasValue && dto.ProgramIds != null && dto.ProgramIds.Count > 0)
            {
                var existingApplications = await _context.AppliedPrograms
                    .Where(ap => ap.Application.ApplicantId == dto.ApplicantId.Value)
                    .Where(ap => dto.ProgramIds.Contains(ap.ProgramId))
                    .Select(ap => new
                    {
                        ProgramName = ap.SocialAssistanceProgram.ProgramName,
                        ApplicationId = ap.ApplicationId,
                        ApplicationDate = ap.Application.ApplicationDate
                    })
                    .ToListAsync();

                if (existingApplications.Any())
                {
                    var duplicatePrograms = existingApplications
                        .Select(ap => $"{ap.ProgramName} (Application #{ap.ApplicationId} - {ap.ApplicationDate:dd/MM/yyyy})")
                        .ToList();

                    ModelState.AddModelError("ProgramIds",
                        $"This applicant has already applied for the following programs: {string.Join(", ", duplicatePrograms)}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var application = MappingService.ToApplication(dto);
                    _context.Add(application);
                    await _context.SaveChangesAsync();

                    // Add selected programs
                    if (dto.ProgramIds != null && dto.ProgramIds.Count > 0)
                    {
                        foreach (var programId in dto.ProgramIds)
                        {
                            _context.AppliedPrograms.Add(new AppliedProgram
                            {   
                                ApplicationId = application.ApplicationId,
                                ProgramId = programId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Application created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547) // Foreign key constraint violation
                        {
                            ModelState.AddModelError("", "Invalid selection. Please ensure all dropdown values are valid.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A database error occurred. Please try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the application. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            ViewData["ApplicantId"] = await GetApplicantsAsync();
            ViewData["OfficerId"] = await GetOfficersAsync();
            ViewData["Programs"] = await GetProgramsAsync();
            return View(dto);
        }

        // GET: Application/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.AppliedPrograms)
                .FirstOrDefaultAsync(a => a.ApplicationId == id);

            if (application == null)
            {
                return NotFound();
            }

            var applicationDto = new ApplicationEditDto
            {
                ApplicationId = application.ApplicationId,
                ApplicationDate = application.ApplicationDate,
                ApplicantSignedDate = application.ApplicantSignedDate,
                OfficerSignedDate = application.OfficerSignedDate,
                OfficerId = application.OfficerId,
                ProgramIds = application.AppliedPrograms.Select(ap => ap.ProgramId).ToList()
            };

            ViewData["ApplicantId"] = await GetApplicantsAsync();
            ViewData["OfficerId"] = await GetOfficersAsync();
            ViewData["Programs"] = await GetProgramsAsync();
            return View(applicationDto);
        }

        // POST: Application/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplicationEditDto dto)
        {
            if (id != dto.ApplicationId)
            {
                return NotFound();
            }

            // Custom validation
            if (dto.ProgramIds == null || dto.ProgramIds.Count == 0)
            {
                ModelState.AddModelError("ProgramIds", "At least one program must be selected.");
            }

            // Check for duplicate program applications (excluding current application)
            if (dto.ProgramIds != null && dto.ProgramIds.Count > 0)
            {
                var application = await _context.Applications.FindAsync(dto.ApplicationId);
                if (application != null)
                {
                    var existingApplications = await _context.AppliedPrograms
                        .Where(ap => ap.Application.ApplicantId == application.ApplicantId)
                        .Where(ap => ap.ApplicationId != dto.ApplicationId) // Exclude current application
                        .Where(ap => dto.ProgramIds.Contains(ap.ProgramId))
                        .Select(ap => new
                        {
                            ProgramName = ap.SocialAssistanceProgram.ProgramName,
                            ApplicationId = ap.ApplicationId,
                            ApplicationDate = ap.Application.ApplicationDate
                        })
                        .ToListAsync();

                    if (existingApplications.Any())
                    {
                        var duplicatePrograms = existingApplications
                            .Select(ap => $"{ap.ProgramName} (Application #{ap.ApplicationId} - {ap.ApplicationDate:dd/MM/yyyy})")
                            .ToList();

                        ModelState.AddModelError("ProgramIds",
                            $"This applicant has already applied for the following programs: {string.Join(", ", duplicatePrograms)}");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var application = await _context.Applications.FindAsync(dto.ApplicationId);
                    if (application == null)
                    {
                        return NotFound();
                    }

                    MappingService.UpdateApplication(application, dto);
                    _context.Update(application);

                    // Update selected programs
                    var existingPrograms = await _context.AppliedPrograms
                        .Where(ap => ap.ApplicationId == application.ApplicationId)
                        .ToListAsync();

                    _context.AppliedPrograms.RemoveRange(existingPrograms);

                    if (dto.ProgramIds != null && dto.ProgramIds.Count > 0)
                    {
                        foreach (var programId in dto.ProgramIds)
                        {
                            _context.AppliedPrograms.Add(new AppliedProgram
                            {
                                ApplicationId = application.ApplicationId,
                                ProgramId = programId
                            });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Application updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationExists(dto.ApplicationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "The application was modified by another user. Please refresh and try again.");
                    }
                }
                catch (DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547) // Foreign key constraint violation
                        {
                            ModelState.AddModelError("", "Invalid selection. Please ensure all dropdown values are valid.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A database error occurred. Please try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the application. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            ViewData["ApplicantId"] = await GetApplicantsAsync();
            ViewData["OfficerId"] = await GetOfficersAsync();
            ViewData["Programs"] = await GetProgramsAsync();
            return View(dto);
        }

        // GET: Application/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var application = await _context.Applications
                .Include(a => a.Applicant)
                .Include(a => a.Officer)
                .Include(a => a.AppliedPrograms)
                    .ThenInclude(ap => ap.SocialAssistanceProgram)
                .FirstOrDefaultAsync(m => m.ApplicationId == id);

            if (application == null)
            {
                return NotFound();
            }

            var applicationDto = MappingService.ToApplicationDetailsDto(application);
            return View(applicationDto);
        }

        // POST: Application/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var application = await _context.Applications.FindAsync(id);
                if (application != null)
                {
                    _context.Applications.Remove(application);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Application deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Application not found.";
                }
            }
            catch (DbUpdateException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database error during delete: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while deleting the application. Please try again.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error during delete: {ex.Message}");
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the application. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Application/NewApplication
        public async Task<IActionResult> NewApplication()
        {
            ViewData["OfficerId"] = await GetOfficersAsync();
            ViewData["Programs"] = await GetProgramsAsync();
            return View(new ApplicationCreateDto());
        }

        // POST: Application/NewApplication
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewApplication(string idNumber, ApplicationCreateDto dto)
        {
            if (string.IsNullOrEmpty(idNumber))
            {
                ModelState.AddModelError("", "Please enter an ID number.");
                ViewData["OfficerId"] = await GetOfficersAsync();
                ViewData["Programs"] = await GetProgramsAsync();
                return View(dto);
            }

            // Look up applicant by ID number
            var applicant = await _context.Applicants
                .FirstOrDefaultAsync(a => a.IdNumber == idNumber);

            if (applicant == null)
            {
                ModelState.AddModelError("", "No applicant found with this ID number. Please create the applicant first.");
                ViewData["OfficerId"] = await GetOfficersAsync();
                ViewData["Programs"] = await GetProgramsAsync();
                return View(dto);
            }

            dto.ApplicantId = applicant.ApplicantId;

            // Custom validation
            if (dto.ProgramIds == null || dto.ProgramIds.Count == 0)
            {
                ModelState.AddModelError("ProgramIds", "At least one program must be selected.");
            }

            // Check for duplicate program applications
            if (dto.ProgramIds != null && dto.ProgramIds.Count > 0)
            {
                var existingApplications = await _context.AppliedPrograms
                    .Where(ap => ap.Application.ApplicantId == applicant.ApplicantId)
                    .Where(ap => dto.ProgramIds.Contains(ap.ProgramId))
                    .Select(ap => new
                    {
                        ProgramName = ap.SocialAssistanceProgram.ProgramName,
                        ApplicationId = ap.ApplicationId,
                        ApplicationDate = ap.Application.ApplicationDate
                    })
                    .ToListAsync();

                if (existingApplications.Any())
                {
                    var duplicatePrograms = existingApplications
                        .Select(ap => $"{ap.ProgramName} (Application #{ap.ApplicationId} - {ap.ApplicationDate:dd/MM/yyyy})")
                        .ToList();

                    ModelState.AddModelError("ProgramIds",
                        $"This applicant has already applied for the following programs: {string.Join(", ", duplicatePrograms)}");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var application = MappingService.ToApplication(dto);
                    _context.Add(application);
                    await _context.SaveChangesAsync();

                    // Add selected programs
                    if (dto.ProgramIds != null && dto.ProgramIds.Count > 0)
                    {
                        foreach (var programId in dto.ProgramIds)
                        {
                            _context.AppliedPrograms.Add(new AppliedProgram
                            {
                                ApplicationId = application.ApplicationId,
                                ProgramId = programId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Application created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547) // Foreign key constraint violation
                        {
                            ModelState.AddModelError("", "Invalid selection. Please ensure all dropdown values are valid.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A database error occurred. Please try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the application. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            ViewData["OfficerId"] = await GetOfficersAsync();
            ViewData["Programs"] = await GetProgramsAsync();
            return View(dto);
        }

        // AJAX endpoint to search applicants
        public async Task<IActionResult> SearchApplicants(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return Json(new List<object>());
            }

            var applicants = await _context.Applicants
                .Where(a => a.FirstName.Contains(term) ||
                           a.LastName.Contains(term) ||
                           a.IdNumber.Contains(term))
                .Select(a => new
                {
                    value = a.ApplicantId,
                    label = $"{a.FirstName} {a.LastName} ({a.IdNumber})",
                    idNumber = a.IdNumber
                })
                .Take(10)
                .ToListAsync();

            return Json(applicants);
        }

        // AJAX endpoint to get existing applications for an applicant
        public async Task<IActionResult> GetExistingApplications(int applicantId)
        {
            var existingApplications = await _context.AppliedPrograms
                .Where(ap => ap.Application.ApplicantId == applicantId)
                .Select(ap => new
                {
                    programName = ap.SocialAssistanceProgram.ProgramName,
                    applicationId = ap.ApplicationId,
                    applicationDate = ap.Application.ApplicationDate.ToString("dd/MM/yyyy")
                })
                .OrderBy(ap => ap.applicationDate)
                .ToListAsync();

            return Json(existingApplications);
        }

        private bool ApplicationExists(int id)
        {
            return _context.Applications.Any(e => e.ApplicationId == id);
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetApplicantsAsync()
        {
            return await _context.Applicants
                .Select(a => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = a.ApplicantId.ToString(),
                    Text = $"{a.FirstName} {a.LastName} ({a.IdNumber})"
                })
                .ToListAsync();
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetOfficersAsync()
        {
            return await _context.Officers
                .Select(o => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = o.OfficerId.ToString(),
                    Text = $"{o.OfficerName} - {o.Designation}"
                })
                .ToListAsync();
        }

        private async Task<List<SocialAssistanceProgram>> GetProgramsAsync()
        {
            return await _context.SocialAssistancePrograms
                .OrderBy(p => p.ProgramName)
                .ToListAsync();
        }
    }
}
