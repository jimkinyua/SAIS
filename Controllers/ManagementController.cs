using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SAIS.Models.Data;
using SAIS.Models;
using SAIS.Models.DTOs;

namespace SAIS.Controllers
{
    public class ManagementController : Controller
    {
        private readonly SAISDbContext _context;

        public ManagementController(SAISDbContext context)
        {
            _context = context;
        }

        // GET: Management
        public IActionResult Index()
        {
            return View();
        }

        // GET: Management/Counties
        public async Task<IActionResult> Counties()
        {
            var counties = await _context.Counties
                .Include(c => c.SubCounties)
                .Select(c => new CountyDto
                {
                    CountyId = c.CountyId,
                    CountyName = c.CountyName,
                    SubCountiesCount = c.SubCounties.Count
                })
                .ToListAsync();

            return View(counties);
        }

        // GET: Management/Counties/Manage/5 - Hierarchical management for a county
        public async Task<IActionResult> ManageCounty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var county = await _context.Counties
                .Include(c => c.SubCounties)
                .ThenInclude(sc => sc.Locations)
                .ThenInclude(l => l.SubLocations)
                .ThenInclude(sl => sl.Villages)
                .FirstOrDefaultAsync(c => c.CountyId == id);

            if (county == null)
            {
                return NotFound();
            }

            var countyDto = new CountyDto
            {
                CountyId = county.CountyId,
                CountyName = county.CountyName,
                SubCountiesCount = county.SubCounties.Count
            };

            ViewBag.County = countyDto;
            ViewBag.SubCounties = county.SubCounties.Select(sc => new SubCountyDto
            {
                SubCountyId = sc.SubCountyId,
                SubCountyName = sc.SubCountyName,
                CountyName = county.CountyName,
                LocationsCount = sc.Locations.Count
            }).ToList();

            return View();
        }

        // GET: Management/Counties/Create
        public IActionResult CreateCounty()
        {
            return View(new CountyCreateDto());
        }

        // POST: Management/Counties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCounty(CountyCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var county = new County
                    {
                        CountyName = dto.CountyName
                    };

                    _context.Add(county);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "County created successfully!";
                    return RedirectToAction(nameof(Counties));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating county: {ex.Message}";
                }
            }

            return View(dto);
        }

        // GET: Management/Counties/Edit/5
        public async Task<IActionResult> EditCounty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var county = await _context.Counties.FindAsync(id);
            if (county == null)
            {
                return NotFound();
            }

            var dto = new CountyEditDto
            {
                CountyId = county.CountyId,
                CountyName = county.CountyName
            };

            return View(dto);
        }

        // POST: Management/Counties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCounty(int id, CountyEditDto dto)
        {
            if (id != dto.CountyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var county = await _context.Counties.FindAsync(id);
                    if (county == null)
                    {
                        return NotFound();
                    }

                    county.CountyName = dto.CountyName;
                    _context.Update(county);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "County updated successfully!";
                    return RedirectToAction(nameof(Counties));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating county: {ex.Message}";
                }
            }

            return View(dto);
        }

        // GET: Management/Counties/Delete/5
        public async Task<IActionResult> DeleteCounty(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var county = await _context.Counties
                .Include(c => c.SubCounties)
                .FirstOrDefaultAsync(c => c.CountyId == id);

            if (county == null)
            {
                return NotFound();
            }

            var dto = new CountyDto
            {
                CountyId = county.CountyId,
                CountyName = county.CountyName,
                SubCountiesCount = county.SubCounties.Count
            };

            return View(dto);
        }

        // POST: Management/Counties/Delete/5
        [HttpPost, ActionName("DeleteCounty")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCountyConfirmed(int id)
        {
            try
            {
                var county = await _context.Counties.FindAsync(id);
                if (county != null)
                {
                    _context.Counties.Remove(county);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "County deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting county: {ex.Message}";
            }

            return RedirectToAction(nameof(Counties));
        }

        // GET: Management/Villages
        public async Task<IActionResult> Villages()
        {
            var villages = await _context.Villages
                .Include(v => v.SubLocation)
                .ThenInclude(sl => sl.Location)
                .ThenInclude(l => l.SubCounty)
                .ThenInclude(sc => sc.County)
                .Select(v => new VillageDto
                {
                    VillageId = v.VillageId,
                    VillageName = v.VillageName,
                    SubCountyName = v.SubLocation.Location.SubCounty.SubCountyName,
                    CountyName = v.SubLocation.Location.SubCounty.County.CountyName
                })
                .ToListAsync();

            return View(villages);
        }

        // GET: Management/Villages/Create
        public async Task<IActionResult> CreateVillage()
        {
            ViewData["SubCountyId"] = await GetSubCountiesAsync();
            return View(new VillageCreateDto());
        }

        // POST: Management/Villages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVillage(VillageCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var village = new Village
                    {
                        VillageName = dto.VillageName,
                        SubLocationId = dto.SubCountyId // Using SubCountyId as SubLocationId for now
                    };

                    _context.Add(village);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Village created successfully!";
                    return RedirectToAction(nameof(Villages));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating village: {ex.Message}";
                }
            }

            ViewData["SubCountyId"] = await GetSubCountiesAsync();
            return View(dto);
        }

        // GET: Management/Villages/Edit/5
        public async Task<IActionResult> EditVillage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var village = await _context.Villages.FindAsync(id);
            if (village == null)
            {
                return NotFound();
            }

            var dto = new VillageEditDto
            {
                VillageId = village.VillageId,
                VillageName = village.VillageName,
                SubCountyId = village.SubLocationId // Using SubLocationId as SubCountyId for now
            };

            ViewData["SubCountyId"] = await GetSubCountiesAsync();
            return View(dto);
        }

        // POST: Management/Villages/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVillage(int id, VillageEditDto dto)
        {
            if (id != dto.VillageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var village = await _context.Villages.FindAsync(id);
                    if (village == null)
                    {
                        return NotFound();
                    }

                    village.VillageName = dto.VillageName;
                    village.SubLocationId = dto.SubCountyId; // Using SubCountyId as SubLocationId for now
                    _context.Update(village);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Village updated successfully!";
                    return RedirectToAction(nameof(Villages));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating village: {ex.Message}";
                }
            }

            ViewData["SubCountyId"] = await GetSubCountiesAsync();
            return View(dto);
        }

        // GET: Management/Villages/Delete/5
        public async Task<IActionResult> DeleteVillage(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var village = await _context.Villages
                .Include(v => v.SubLocation)
                .ThenInclude(sl => sl.Location)
                .ThenInclude(l => l.SubCounty)
                .ThenInclude(sc => sc.County)
                .FirstOrDefaultAsync(v => v.VillageId == id);

            if (village == null)
            {
                return NotFound();
            }

            var dto = new VillageDto
            {
                VillageId = village.VillageId,
                VillageName = village.VillageName,
                SubCountyName = village.SubLocation.Location.SubCounty.SubCountyName,
                CountyName = village.SubLocation.Location.SubCounty.County.CountyName
            };

            return View(dto);
        }

        // POST: Management/Villages/Delete/5
        [HttpPost, ActionName("DeleteVillage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVillageConfirmed(int id)
        {
            try
            {
                var village = await _context.Villages.FindAsync(id);
                if (village != null)
                {
                    _context.Villages.Remove(village);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Village deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting village: {ex.Message}";
            }

            return RedirectToAction(nameof(Villages));
        }

        // GET: Management/Genders
        public async Task<IActionResult> Genders()
        {
            var genders = await _context.GenderCategories
                .Select(g => new GenderCategoryDto
                {
                    GenderCategoryId = g.GenderCategoryId,
                    GenderName = g.GenderCategoryName
                })
                .ToListAsync();

            return View(genders);
        }

        // GET: Management/Genders/Create
        public IActionResult CreateGender()
        {
            return View(new GenderCategoryCreateDto());
        }

        // POST: Management/Genders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGender(GenderCategoryCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var gender = new GenderCategory
                    {
                        GenderCategoryName = dto.GenderName
                    };

                    _context.Add(gender);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Gender category created successfully!";
                    return RedirectToAction(nameof(Genders));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating gender category: {ex.Message}";
                }
            }

            return View(dto);
        }

        // GET: Management/Genders/Edit/5
        public async Task<IActionResult> EditGender(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gender = await _context.GenderCategories.FindAsync(id);
            if (gender == null)
            {
                return NotFound();
            }

            var dto = new GenderCategoryEditDto
            {
                GenderCategoryId = gender.GenderCategoryId,
                GenderName = gender.GenderCategoryName
            };

            return View(dto);
        }

        // POST: Management/Genders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGender(int id, GenderCategoryEditDto dto)
        {
            if (id != dto.GenderCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var gender = await _context.GenderCategories.FindAsync(id);
                    if (gender == null)
                    {
                        return NotFound();
                    }

                    gender.GenderCategoryName = dto.GenderName;
                    _context.Update(gender);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Gender category updated successfully!";
                    return RedirectToAction(nameof(Genders));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating gender category: {ex.Message}";
                }
            }

            return View(dto);
        }

        // GET: Management/Genders/Delete/5
        public async Task<IActionResult> DeleteGender(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gender = await _context.GenderCategories.FindAsync(id);
            if (gender == null)
            {
                return NotFound();
            }

            var dto = new GenderCategoryDto
            {
                GenderCategoryId = gender.GenderCategoryId,
                GenderName = gender.GenderCategoryName
            };

            return View(dto);
        }

        // POST: Management/Genders/Delete/5
        [HttpPost, ActionName("DeleteGender")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGenderConfirmed(int id)
        {
            try
            {
                var gender = await _context.GenderCategories.FindAsync(id);
                if (gender != null)
                {
                    _context.GenderCategories.Remove(gender);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Gender category deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting gender category: {ex.Message}";
            }

            return RedirectToAction(nameof(Genders));
        }

        // GET: Management/MaritalStatuses
        public async Task<IActionResult> MaritalStatuses()
        {
            var maritalStatuses = await _context.MaritalStatuses
                .Select(ms => new MaritalStatusDto
                {
                    MaritalStatusId = ms.MaritalStatusId,
                    MaritalStatusName = ms.StatusName
                })
                .ToListAsync();

            return View(maritalStatuses);
        }

        // GET: Management/MaritalStatuses/Create
        public IActionResult CreateMaritalStatus()
        {
            return View(new MaritalStatusCreateDto());
        }

        // POST: Management/MaritalStatuses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMaritalStatus(MaritalStatusCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var maritalStatus = new MaritalStatus
                    {
                        StatusName = dto.MaritalStatusName
                    };

                    _context.Add(maritalStatus);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Marital status created successfully!";
                    return RedirectToAction(nameof(MaritalStatuses));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating marital status: {ex.Message}";
                }
            }

            return View(dto);
        }

        // GET: Management/MaritalStatuses/Edit/5
        public async Task<IActionResult> EditMaritalStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maritalStatus = await _context.MaritalStatuses.FindAsync(id);
            if (maritalStatus == null)
            {
                return NotFound();
            }

            var dto = new MaritalStatusEditDto
            {
                MaritalStatusId = maritalStatus.MaritalStatusId,
                MaritalStatusName = maritalStatus.StatusName
            };

            return View(dto);
        }

        // POST: Management/MaritalStatuses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaritalStatus(int id, MaritalStatusEditDto dto)
        {
            if (id != dto.MaritalStatusId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var maritalStatus = await _context.MaritalStatuses.FindAsync(id);
                    if (maritalStatus == null)
                    {
                        return NotFound();
                    }

                    maritalStatus.StatusName = dto.MaritalStatusName;
                    _context.Update(maritalStatus);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Marital status updated successfully!";
                    return RedirectToAction(nameof(MaritalStatuses));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating marital status: {ex.Message}";
                }
            }

            return View(dto);
        }

        // GET: Management/MaritalStatuses/Delete/5
        public async Task<IActionResult> DeleteMaritalStatus(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maritalStatus = await _context.MaritalStatuses.FindAsync(id);
            if (maritalStatus == null)
            {
                return NotFound();
            }

            var dto = new MaritalStatusDto
            {
                MaritalStatusId = maritalStatus.MaritalStatusId,
                MaritalStatusName = maritalStatus.StatusName
            };

            return View(dto);
        }

        // POST: Management/MaritalStatuses/Delete/5
        [HttpPost, ActionName("DeleteMaritalStatus")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMaritalStatusConfirmed(int id)
        {
            try
            {
                var maritalStatus = await _context.MaritalStatuses.FindAsync(id);
                if (maritalStatus != null)
                {
                    _context.MaritalStatuses.Remove(maritalStatus);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Marital status deleted successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting marital status: {ex.Message}";
            }

            return RedirectToAction(nameof(MaritalStatuses));
        }

        private async Task<List<SelectListItem>> GetSubCountiesAsync()
        {
            return await _context.SubCounties
                .Include(sc => sc.County)
                .Select(sc => new SelectListItem
                {
                    Value = sc.SubCountyId.ToString(),
                    Text = $"{sc.SubCountyName} ({sc.County.CountyName})"
                })
                .ToListAsync();
        }

        // AJAX endpoints for quick stats
        public async Task<IActionResult> GetCountiesCount()
        {
            var count = await _context.Counties.CountAsync();
            return Json(count);
        }

        public async Task<IActionResult> GetVillagesCount()
        {
            var count = await _context.Villages.CountAsync();
            return Json(count);
        }

        public async Task<IActionResult> GetGendersCount()
        {
            var count = await _context.GenderCategories.CountAsync();
            return Json(count);
        }

        public async Task<IActionResult> GetMaritalStatusesCount()
        {
            var count = await _context.MaritalStatuses.CountAsync();
            return Json(count);
        }

        // AJAX endpoints for hierarchical management
        public async Task<IActionResult> GetSubCountiesForCounty(int countyId)
        {
            var subCounties = await _context.SubCounties
                .Where(sc => sc.CountyId == countyId)
                .Select(sc => new
                {
                    subCountyId = sc.SubCountyId,
                    subCountyName = sc.SubCountyName,
                    locationsCount = sc.Locations.Count
                })
                .ToListAsync();

            return Json(subCounties);
        }

        public async Task<IActionResult> GetLocationsForCounty(int countyId)
        {
            var locations = await _context.Locations
                .Include(l => l.SubCounty)
                .Where(l => l.SubCounty.CountyId == countyId)
                .Select(l => new
                {
                    locationId = l.LocationId,
                    locationName = l.LocationName,
                    subCountyName = l.SubCounty.SubCountyName,
                    subLocationsCount = l.SubLocations.Count
                })
                .ToListAsync();

            return Json(locations);
        }

        public async Task<IActionResult> GetSubLocationsForCounty(int countyId)
        {
            var subLocations = await _context.SubLocations
                .Include(sl => sl.Location)
                .ThenInclude(l => l.SubCounty)
                .Where(sl => sl.Location.SubCounty.CountyId == countyId)
                .Select(sl => new
                {
                    subLocationId = sl.SubLocationId,
                    subLocationName = sl.SubLocationName,
                    locationName = sl.Location.LocationName,
                    subCountyName = sl.Location.SubCounty.SubCountyName,
                    villagesCount = sl.Villages.Count
                })
                .ToListAsync();

            return Json(subLocations);
        }

        public async Task<IActionResult> GetVillagesForCounty(int countyId)
        {
            var villages = await _context.Villages
                .Include(v => v.SubLocation)
                .ThenInclude(sl => sl.Location)
                .ThenInclude(l => l.SubCounty)
                .Where(v => v.SubLocation.Location.SubCounty.CountyId == countyId)
                .Select(v => new
                {
                    villageId = v.VillageId,
                    villageName = v.VillageName,
                    subLocationName = v.SubLocation.SubLocationName,
                    locationName = v.SubLocation.Location.LocationName,
                    subCountyName = v.SubLocation.Location.SubCounty.SubCountyName
                })
                .ToListAsync();

            return Json(villages);
        }

        // AJAX endpoints for creating entities
        [HttpPost]
        [Route("Management/CreateSubCounty")]
        public async Task<IActionResult> CreateSubCounty([FromBody] SubCountyCreateDto dto)
        {
            try
            {
                var subCounty = new SubCounty
                {
                    SubCountyName = dto.SubCountyName,
                    CountyId = dto.CountyId
                };

                _context.Add(subCounty);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Sub-county created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Management/CreateLocation")]
        public async Task<IActionResult> CreateLocation([FromBody] LocationCreateDto dto)
        {
            try
            {
                var location = new Location
                {
                    LocationName = dto.LocationName,
                    SubCountyId = dto.SubCountyId
                };

                _context.Add(location);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Location created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Management/CreateSubLocation")]
        public async Task<IActionResult> CreateSubLocation([FromBody] SubLocationCreateDto dto)
        {
            try
            {
                var subLocation = new SubLocation
                {
                    SubLocationName = dto.SubLocationName,
                    LocationId = dto.LocationId
                };

                _context.Add(subLocation);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Sub-location created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Management/CreateVillageAjax")]
        public async Task<IActionResult> CreateVillageAjax([FromBody] VillageCreateDto dto)
        {
            try
            {
                var village = new Village
                {
                    VillageName = dto.VillageName,
                    SubLocationId = dto.SubCountyId // Using SubCountyId as SubLocationId for now
                };

                _context.Add(village);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Village created successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
