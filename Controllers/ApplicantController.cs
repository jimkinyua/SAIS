using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAIS.Models;
using SAIS.Models.Data;
using SAIS.Models.DTOs;
using SAIS.Services;

namespace SAIS.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly SAISDbContext _context;

        public ApplicantController(SAISDbContext context)
        {
            _context = context;
        }

        // GET: Applicant
        public async Task<IActionResult> Index(string searchString, string countyFilter)
        {
            var applicants = _context.Applicants
                .Include(a => a.GenderCategory)
                .Include(a => a.MaritalStatus)
                .Include(a => a.Village)
                    .ThenInclude(v => v.SubLocation)
                        .ThenInclude(sl => sl.Location)
                            .ThenInclude(l => l.SubCounty)
                                .ThenInclude(sc => sc.County)
                .Include(a => a.PhoneNumbers)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                applicants = applicants.Where(a =>
                    a.FirstName.Contains(searchString) ||
                    a.LastName.Contains(searchString) ||
                    a.IdNumber.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(countyFilter))
            {
                applicants = applicants.Where(a =>
                    a.Village.SubLocation.Location.SubCounty.County.CountyName.Contains(countyFilter));
            }

            var applicantList = await applicants.ToListAsync();
            var applicantDtos = applicantList.Select(MappingService.ToApplicantListDto).ToList();

            return View(applicantDtos);
        }

        // GET: Applicant/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants
                .Include(a => a.GenderCategory)
                .Include(a => a.MaritalStatus)
                .Include(a => a.Village)
                    .ThenInclude(v => v.SubLocation)
                        .ThenInclude(sl => sl.Location)
                            .ThenInclude(l => l.SubCounty)
                                .ThenInclude(sc => sc.County)
                .Include(a => a.PhoneNumbers)
                .Include(a => a.Applications)
                    .ThenInclude(app => app.Officer)
                .FirstOrDefaultAsync(m => m.ApplicantId == id);

            if (applicant == null)
            {
                return NotFound();
            }

            var applicantDto = MappingService.ToApplicantDto(applicant);
            return View(applicantDto);
        }

        // GET: Applicant/Create
        public async Task<IActionResult> Create()
        {
            var genderCategories = await GetGenderCategoriesAsync();
            var maritalStatuses = await GetMaritalStatusesAsync();
            var counties = await GetCountiesAsync();

            // Debug logging
            System.Diagnostics.Debug.WriteLine($"=== CREATE GET DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"GenderCategories count: {genderCategories.Count}");
            foreach (var g in genderCategories)
            {
                System.Diagnostics.Debug.WriteLine($"  Gender: {g.Value} - {g.Text}");
            }
            System.Diagnostics.Debug.WriteLine($"MaritalStatuses count: {maritalStatuses.Count}");
            foreach (var m in maritalStatuses)
            {
                System.Diagnostics.Debug.WriteLine($"  MaritalStatus: {m.Value} - {m.Text}");
            }
            System.Diagnostics.Debug.WriteLine($"Counties count: {counties.Count}");
            foreach (var c in counties)
            {
                System.Diagnostics.Debug.WriteLine($"  County: {c.Value} - {c.Text}");
            }
            System.Diagnostics.Debug.WriteLine($"=== END CREATE GET DEBUG ===");

            ViewData["GenderId"] = genderCategories;
            ViewData["MaritalStatusId"] = maritalStatuses;
            ViewData["CountyId"] = counties;
            return View(new ApplicantCreateDto());
        }

        // POST: Applicant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicantCreateDto dto)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"=== CREATE APPLICANT DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"GenderId: {dto.GenderId}");
            System.Diagnostics.Debug.WriteLine($"MaritalStatusId: {dto.MaritalStatusId}");
            System.Diagnostics.Debug.WriteLine($"VillageId: {dto.VillageId}");
            System.Diagnostics.Debug.WriteLine($"IdNumber: {dto.IdNumber}");
            System.Diagnostics.Debug.WriteLine($"FirstName: {dto.FirstName}");
            System.Diagnostics.Debug.WriteLine($"LastName: {dto.LastName}");

            // Log ModelState
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

            // Check what's actually in the database
            var genderExists = await _context.GenderCategories.AnyAsync(g => g.GenderCategoryId == dto.GenderId);
            var maritalStatusExists = await _context.MaritalStatuses.AnyAsync(m => m.MaritalStatusId == dto.MaritalStatusId);
            var villageExists = await _context.Villages.AnyAsync(v => v.VillageId == dto.VillageId);

            System.Diagnostics.Debug.WriteLine($"Gender exists: {genderExists}");
            System.Diagnostics.Debug.WriteLine($"MaritalStatus exists: {maritalStatusExists}");
            System.Diagnostics.Debug.WriteLine($"Village exists: {villageExists}");

            // Log all GenderCategories in database
            var allGenders = await _context.GenderCategories.ToListAsync();
            System.Diagnostics.Debug.WriteLine($"All Genders in DB: {string.Join(", ", allGenders.Select(g => $"{g.GenderCategoryId}:{g.GenderCategoryName}"))}");

            // Log all MaritalStatuses in database
            var allMaritalStatuses = await _context.MaritalStatuses.ToListAsync();
            System.Diagnostics.Debug.WriteLine($"All MaritalStatuses in DB: {string.Join(", ", allMaritalStatuses.Select(m => $"{m.MaritalStatusId}:{m.StatusName}"))}");

            // Log all Villages in database
            var allVillages = await _context.Villages.ToListAsync();
            System.Diagnostics.Debug.WriteLine($"All Villages in DB: {string.Join(", ", allVillages.Select(v => $"{v.VillageId}:{v.VillageName}"))}");

            System.Diagnostics.Debug.WriteLine($"=== END DEBUG ===");

            // Custom validation
            if (await _context.Applicants.AnyAsync(a => a.IdNumber == dto.IdNumber))
            {
                ModelState.AddModelError("IdNumber", "An applicant with this ID number already exists.");
            }

            if (dto.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
            }

            // Validate phone numbers
            if (dto.PhoneNumbers == null || dto.PhoneNumbers.Count == 0 || !dto.PhoneNumbers.Any(p => !string.IsNullOrWhiteSpace(p)))
            {
                ModelState.AddModelError("PhoneNumbers", "At least one phone number is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var applicant = MappingService.ToApplicant(dto);
                    _context.Add(applicant);
                    await _context.SaveChangesAsync();

                    // Add phone numbers
                    if (dto.PhoneNumbers != null && dto.PhoneNumbers.Count > 0)
                    {
                        foreach (var phoneNumber in dto.PhoneNumbers)
                        {
                            if (!string.IsNullOrWhiteSpace(phoneNumber))
                            {
                                _context.PhoneNumbers.Add(new PhoneNumber
                                {
                                    ApplicantId = applicant.ApplicantId,
                                    Number = phoneNumber.Trim()
                                });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Applicant created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception for debugging
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");

                    // Check for specific constraint violations
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547) // Foreign key constraint violation
                        {
                            ModelState.AddModelError("", "Invalid selection. Please ensure all dropdown values are valid.");
                        }
                        else if (sqlEx.Number == 2627) // Unique constraint violation
                        {
                            ModelState.AddModelError("IdNumber", "An applicant with this ID number already exists.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A database error occurred. Please try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the applicant. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging
                    System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            ViewData["GenderId"] = await GetGenderCategoriesAsync();
            ViewData["MaritalStatusId"] = await GetMaritalStatusesAsync();
            ViewData["CountyId"] = await GetCountiesAsync();
            return View(dto);
        }

        // GET: Applicant/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants
                .Include(a => a.PhoneNumbers)
                .FirstOrDefaultAsync(a => a.ApplicantId == id);

            if (applicant == null)
            {
                return NotFound();
            }

            var applicantDto = new ApplicantEditDto
            {
                ApplicantId = applicant.ApplicantId,
                FirstName = applicant.FirstName,
                MiddleName = applicant.MiddleName,
                LastName = applicant.LastName,
                IdNumber = applicant.IdNumber,
                Age = applicant.Age,
                GenderId = applicant.GenderId,
                MaritalStatusId = applicant.MaritalStatusId,
                VillageId = applicant.VillageId,
                PostalAddress = applicant.PostalAddress,
                PhysicalAddress = applicant.PhysicalAddress,
                DateOfBirth = applicant.DateOfBirth,
                PhoneNumbers = applicant.PhoneNumbers?.Select(p => p.Number).ToList() ?? new List<string>()
            };

            // Load the full applicant with geographic hierarchy for the view
            var fullApplicant = await _context.Applicants
                .Include(a => a.Village)
                    .ThenInclude(v => v.SubLocation)
                        .ThenInclude(sl => sl.Location)
                            .ThenInclude(l => l.SubCounty)
                                .ThenInclude(sc => sc.County)
                .FirstOrDefaultAsync(a => a.ApplicantId == id);

            ViewBag.CurrentCountyId = fullApplicant?.Village?.SubLocation?.Location?.SubCounty?.CountyId ?? 0;
            ViewBag.CurrentSubCountyId = fullApplicant?.Village?.SubLocation?.Location?.SubCountyId ?? 0;
            ViewBag.CurrentLocationId = fullApplicant?.Village?.SubLocation?.LocationId ?? 0;
            ViewBag.CurrentSubLocationId = fullApplicant?.Village?.SubLocationId ?? 0;

            ViewData["GenderId"] = await GetGenderCategoriesAsync();
            ViewData["MaritalStatusId"] = await GetMaritalStatusesAsync();
            ViewData["CountyId"] = await GetCountiesAsync();
            return View(applicantDto);
        }

        // POST: Applicant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ApplicantEditDto dto)
        {
            if (id != dto.ApplicantId)
            {
                return NotFound();
            }

            // Custom validation
            if (await _context.Applicants.AnyAsync(a => a.IdNumber == dto.IdNumber && a.ApplicantId != dto.ApplicantId))
            {
                ModelState.AddModelError("IdNumber", "An applicant with this ID number already exists.");
            }

            if (dto.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
            }

            // Validate phone numbers
            if (dto.PhoneNumbers == null || dto.PhoneNumbers.Count == 0 || !dto.PhoneNumbers.Any(p => !string.IsNullOrWhiteSpace(p)))
            {
                ModelState.AddModelError("PhoneNumbers", "At least one phone number is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var applicant = await _context.Applicants.FindAsync(dto.ApplicantId);
                    if (applicant == null)
                    {
                        return NotFound();
                    }

                    MappingService.UpdateApplicant(applicant, dto);
                    _context.Update(applicant);

                    // Update phone numbers
                    var existingPhones = await _context.PhoneNumbers
                        .Where(p => p.ApplicantId == applicant.ApplicantId)
                        .ToListAsync();

                    _context.PhoneNumbers.RemoveRange(existingPhones);

                    if (dto.PhoneNumbers != null && dto.PhoneNumbers.Count > 0)
                    {
                        foreach (var phoneNumber in dto.PhoneNumbers)
                        {
                            if (!string.IsNullOrWhiteSpace(phoneNumber))
                            {
                                _context.PhoneNumbers.Add(new PhoneNumber
                                {
                                    ApplicantId = applicant.ApplicantId,
                                    Number = phoneNumber.Trim()
                                });
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Applicant updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicantExists(dto.ApplicantId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", "The applicant was modified by another user. Please refresh and try again.");
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception for debugging
                    System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");

                    // Check for specific constraint violations
                    if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
                    {
                        if (sqlEx.Number == 547) // Foreign key constraint violation
                        {
                            ModelState.AddModelError("", "Invalid selection. Please ensure all dropdown values are valid.");
                        }
                        else if (sqlEx.Number == 2627) // Unique constraint violation
                        {
                            ModelState.AddModelError("IdNumber", "An applicant with this ID number already exists.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A database error occurred. Please try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the applicant. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging
                    System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            ViewData["GenderId"] = await GetGenderCategoriesAsync();
            ViewData["MaritalStatusId"] = await GetMaritalStatusesAsync();
            ViewData["CountyId"] = await GetCountiesAsync();
            return View(dto);
        }

        // GET: Applicant/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicant = await _context.Applicants
                .Include(a => a.GenderCategory)
                .Include(a => a.MaritalStatus)
                .Include(a => a.Village)
                    .ThenInclude(v => v.SubLocation)
                        .ThenInclude(sl => sl.Location)
                            .ThenInclude(l => l.SubCounty)
                                .ThenInclude(sc => sc.County)
                .Include(a => a.PhoneNumbers)
                .FirstOrDefaultAsync(m => m.ApplicantId == id);

            if (applicant == null)
            {
                return NotFound();
            }

            var applicantDto = MappingService.ToApplicantDto(applicant);
            return View(applicantDto);
        }

        // POST: Applicant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var applicant = await _context.Applicants.FindAsync(id);
                if (applicant != null)
                {
                    _context.Applicants.Remove(applicant);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Applicant deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Applicant not found.";
                }
            }
            catch (DbUpdateException ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Database error during delete: {ex.Message}");

                // Check if it's a foreign key constraint violation
                if (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx && sqlEx.Number == 547)
                {
                    TempData["ErrorMessage"] = "Cannot delete this applicant because they have associated applications. Please delete the applications first.";
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while deleting the applicant. Please try again.";
                }
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                System.Diagnostics.Debug.WriteLine($"Unexpected error during delete: {ex.Message}");
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the applicant. Please try again.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Applicant/Lookup
        public async Task<IActionResult> Lookup(string idNumber)
        {
            if (string.IsNullOrEmpty(idNumber))
            {
                return Json(new { found = false });
            }

            var applicant = await _context.Applicants
                .Include(a => a.GenderCategory)
                .Include(a => a.MaritalStatus)
                .Include(a => a.Village)
                    .ThenInclude(v => v.SubLocation)
                        .ThenInclude(sl => sl.Location)
                            .ThenInclude(l => l.SubCounty)
                                .ThenInclude(sc => sc.County)
                .Include(a => a.PhoneNumbers)
                .FirstOrDefaultAsync(a => a.IdNumber == idNumber);

            if (applicant == null)
            {
                return Json(new { found = false });
            }

            return Json(new
            {
                found = true,
                applicantId = applicant.ApplicantId,
                firstName = applicant.FirstName,
                middleName = applicant.MiddleName,
                lastName = applicant.LastName,
                idNumber = applicant.IdNumber,
                age = applicant.Age,
                genderId = applicant.GenderId,
                genderName = applicant.GenderCategory.GenderCategoryName,
                maritalStatusId = applicant.MaritalStatusId,
                maritalStatusName = applicant.MaritalStatus.StatusName,
                villageId = applicant.VillageId,
                villageName = applicant.Village.VillageName,
                countyId = applicant.Village.SubLocation.Location.SubCounty.CountyId,
                subCountyId = applicant.Village.SubLocation.Location.SubCountyId,
                locationId = applicant.Village.SubLocation.LocationId,
                subLocationId = applicant.Village.SubLocationId,
                postalAddress = applicant.PostalAddress,
                physicalAddress = applicant.PhysicalAddress,
                dateOfBirth = applicant.DateOfBirth?.ToString("yyyy-MM-dd"),
                phoneNumbers = applicant.PhoneNumbers.Select(p => p.Number).ToArray()
            });
        }

        // AJAX endpoints for cascading dropdowns
        public async Task<IActionResult> GetCounties()
        {
            var counties = await _context.Counties
                .Select(c => new { c.CountyId, c.CountyName })
                .ToListAsync();

            return Json(counties);
        }

        public async Task<IActionResult> GetSubCounties(int countyId)
        {
            var subCounties = await _context.SubCounties
                .Where(sc => sc.CountyId == countyId)
                .Select(sc => new { sc.SubCountyId, sc.SubCountyName })
                .ToListAsync();

            return Json(subCounties);
        }

        public async Task<IActionResult> GetLocations(int subCountyId)
        {
            var locations = await _context.Locations
                .Where(l => l.SubCountyId == subCountyId)
                .Select(l => new { l.LocationId, l.LocationName })
                .ToListAsync();

            return Json(locations);
        }

        public async Task<IActionResult> GetSubLocations(int locationId)
        {
            var subLocations = await _context.SubLocations
                .Where(sl => sl.LocationId == locationId)
                .Select(sl => new { sl.SubLocationId, sl.SubLocationName })
                .ToListAsync();

            return Json(subLocations);
        }

        public async Task<IActionResult> GetVillages(int subLocationId)
        {
            var villages = await _context.Villages
                .Where(v => v.SubLocationId == subLocationId)
                .Select(v => new { v.VillageId, v.VillageName })
                .ToListAsync();

            return Json(villages);
        }

        private bool ApplicantExists(int id)
        {
            return _context.Applicants.Any(e => e.ApplicantId == id);
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetGenderCategoriesAsync()
        {
            return await _context.GenderCategories
                .Select(g => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = g.GenderCategoryId.ToString(),
                    Text = g.GenderCategoryName
                })
                .ToListAsync();
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetMaritalStatusesAsync()
        {
            return await _context.MaritalStatuses
                .Select(m => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = m.MaritalStatusId.ToString(),
                    Text = m.StatusName
                })
                .ToListAsync();
        }

        private async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetCountiesAsync()
        {
            return await _context.Counties
                .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = c.CountyId.ToString(),
                    Text = c.CountyName
                })
                .ToListAsync();
        }

        // GET: Applicant/RegisterAndApply
        public async Task<IActionResult> RegisterAndApply()
        {
            var dto = new CombinedApplicantApplicationDto
            {
                GenderCategories = await GetGenderCategoriesAsync(),
                MaritalStatuses = await GetMaritalStatusesAsync(),
                Counties = await GetCountiesAsync(),
                Officers = await GetOfficersAsync(),
                Programs = await GetProgramsAsync()
            };

            return View(dto);
        }

        // POST: Applicant/RegisterAndApply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAndApply(CombinedApplicantApplicationDto dto)
        {
            // Debug logging
            System.Diagnostics.Debug.WriteLine($"=== REGISTER AND APPLY DEBUG ===");
            System.Diagnostics.Debug.WriteLine($"FirstName: {dto.FirstName}");
            System.Diagnostics.Debug.WriteLine($"LastName: {dto.LastName}");
            System.Diagnostics.Debug.WriteLine($"IdNumber: {dto.IdNumber}");
            System.Diagnostics.Debug.WriteLine($"ProgramIds: {string.Join(", ", dto.ProgramIds ?? new List<int>())}");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            // Custom validation
            if (await _context.Applicants.AnyAsync(a => a.IdNumber == dto.IdNumber))
            {
                ModelState.AddModelError("IdNumber", "An applicant with this ID number already exists.");
            }

            // Age and DateOfBirth validation and calculation
            if (dto.DateOfBirth.HasValue && dto.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
            }

            // Calculate age from date of birth if provided
            if (dto.DateOfBirth.HasValue && !dto.Age.HasValue)
            {
                dto.Age = DateTime.Today.Year - dto.DateOfBirth.Value.Year;
                if (dto.DateOfBirth.Value.Date > DateTime.Today.AddYears(-dto.Age.Value))
                {
                    dto.Age--;
                }
            }

            // Calculate date of birth from age if provided
            if (dto.Age.HasValue && !dto.DateOfBirth.HasValue)
            {
                dto.DateOfBirth = DateTime.Today.AddYears(-dto.Age.Value);
            }

            // Ensure at least one of Age or DateOfBirth is provided
            if (!dto.DateOfBirth.HasValue && !dto.Age.HasValue)
            {
                ModelState.AddModelError("DateOfBirth", "Either Date of Birth or Age must be provided.");
                ModelState.AddModelError("Age", "Either Date of Birth or Age must be provided.");
            }

            if (dto.PhoneNumbers == null || dto.PhoneNumbers.Count == 0 || !dto.PhoneNumbers.Any(p => !string.IsNullOrWhiteSpace(p)))
            {
                ModelState.AddModelError("PhoneNumbers", "At least one phone number is required.");
            }

            if (dto.ProgramIds == null || dto.ProgramIds.Count == 0)
            {
                ModelState.AddModelError("ProgramIds", "At least one program must be selected.");
            }

            if (dto.OfficerId == 0)
            {
                ModelState.AddModelError("OfficerId", "Please select an officer.");
            }

            if (dto.VillageId == 0)
            {
                ModelState.AddModelError("VillageId", "Please select a village.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create applicant
                    var applicant = new Applicant
                    {
                        FirstName = dto.FirstName,
                        MiddleName = dto.MiddleName,
                        LastName = dto.LastName,
                        IdNumber = dto.IdNumber,
                        DateOfBirth = dto.DateOfBirth,
                        Age = dto.Age ?? (dto.DateOfBirth.HasValue ? DateTime.Today.Year - dto.DateOfBirth.Value.Year : 0),
                        GenderId = dto.GenderId,
                        MaritalStatusId = dto.MaritalStatusId,
                        VillageId = dto.VillageId,
                        PostalAddress = dto.PostalAddress ?? string.Empty,
                        PhysicalAddress = dto.PhysicalAddress ?? string.Empty
                    };

                    _context.Add(applicant);
                    await _context.SaveChangesAsync();

                    // Add phone numbers
                    if (dto.PhoneNumbers != null && dto.PhoneNumbers.Count > 0)
                    {
                        foreach (var phoneNumber in dto.PhoneNumbers)
                        {
                            if (!string.IsNullOrWhiteSpace(phoneNumber))
                            {
                                _context.PhoneNumbers.Add(new PhoneNumber
                                {
                                    ApplicantId = applicant.ApplicantId,
                                    Number = phoneNumber.Trim()
                                });
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    // Create application
                    var application = new Application
                    {
                        ApplicantId = applicant.ApplicantId,
                        ApplicationDate = dto.ApplicationDate,
                        ApplicantSignedDate = dto.ApplicantSignedDate,
                        OfficerSignedDate = dto.OfficerSignedDate,
                        OfficerId = dto.OfficerId
                    };

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

                    TempData["SuccessMessage"] = $"Applicant registered and application submitted successfully! Application ID: {application.ApplicationId}";
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
                        else if (sqlEx.Number == 2627) // Unique constraint violation
                        {
                            ModelState.AddModelError("IdNumber", "An applicant with this ID number already exists.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "A database error occurred. Please try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while saving the data. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                }
            }

            // Reload dropdown data for the view
            dto.GenderCategories = await GetGenderCategoriesAsync();
            dto.MaritalStatuses = await GetMaritalStatusesAsync();
            dto.Counties = await GetCountiesAsync();
            dto.Officers = await GetOfficersAsync();
            dto.Programs = await GetProgramsAsync();

            return View(dto);
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
