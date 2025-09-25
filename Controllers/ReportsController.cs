using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SAIS.Models;
using SAIS.Models.Data;

namespace SAIS.Controllers
{
    public class ReportsController : Controller
    {
        private readonly SAISDbContext _context;

        public ReportsController(SAISDbContext context)
        {
            _context = context;
        }

        // GET: Reports
        public async Task<IActionResult> Index()
        {
            var reportData = await GetReportData();
            return View(reportData);
        }

        // GET: Reports/Applications
        public async Task<IActionResult> Applications(string officerFilter, string programFilter, DateTime? startDate, DateTime? endDate)
        {
            var applications = _context.Applications
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
                .Include(a => a.Officer)
                .Include(a => a.AppliedPrograms)
                    .ThenInclude(ap => ap.SocialAssistanceProgram)
                .AsQueryable();

            if (!string.IsNullOrEmpty(officerFilter))
            {
                applications = applications.Where(a => a.Officer.OfficerName.Contains(officerFilter));
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

            var reportData = await applications.OrderByDescending(a => a.ApplicationDate).ToListAsync();

            ViewData["OfficerFilter"] = officerFilter;
            ViewData["ProgramFilter"] = programFilter;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(reportData);
        }

        // GET: Reports/Export
        public async Task<IActionResult> Export(string format, string officerFilter, string programFilter, DateTime? startDate, DateTime? endDate)
        {
            var applications = _context.Applications
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
                .Include(a => a.Officer)
                .Include(a => a.AppliedPrograms)
                    .ThenInclude(ap => ap.SocialAssistanceProgram)
                .AsQueryable();

            if (!string.IsNullOrEmpty(officerFilter))
            {
                applications = applications.Where(a => a.Officer.OfficerName.Contains(officerFilter));
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

            var reportData = await applications.OrderByDescending(a => a.ApplicationDate).ToListAsync();

            if (format?.ToLower() == "excel")
            {
                return await ExportToExcel(reportData);
            }
            else if (format?.ToLower() == "pdf")
            {
                return await ExportToPdf(reportData);
            }

            return BadRequest("Invalid export format");
        }

        private Task<IActionResult> ExportToExcel(List<Application> applications)
        {
            // Simple CSV export for now - in production, use a proper Excel library like EPPlus
            var csv = "Application Date,Applicant Name,ID Number,Gender,Marital Status,County,Officer,Programs,Status\n";

            foreach (var app in applications)
            {
                var programs = string.Join("; ", app.AppliedPrograms.Select(ap => ap.SocialAssistanceProgram.ProgramName));
                var status = app.OfficerSignedDate != default(DateTime) ? "Completed" : "Pending";
                var county = app.Applicant.Village.SubLocation.Location.SubCounty.County.CountyName;

                csv += $"{app.ApplicationDate:yyyy-MM-dd}," +
                       $"\"{app.Applicant.FirstName} {app.Applicant.LastName}\"," +
                       $"\"{app.Applicant.IdNumber}\"," +
                       $"\"{app.Applicant.GenderCategory.GenderCategoryName}\"," +
                       $"\"{app.Applicant.MaritalStatus.StatusName}\"," +
                       $"\"{county}\"," +
                       $"\"{app.Officer.OfficerName}\"," +
                       $"\"{programs}\"," +
                       $"\"{status}\"\n";
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(csv);
            return Task.FromResult<IActionResult>(File(bytes, "text/csv", $"applications_report_{DateTime.Now:yyyyMMdd}.csv"));
        }

        private Task<IActionResult> ExportToPdf(List<Application> applications)
        {
            // Simple HTML export for now - in production, use a proper PDF library like iTextSharp
            var html = GenerateHtmlReport(applications);
            var bytes = System.Text.Encoding.UTF8.GetBytes(html);
            return Task.FromResult<IActionResult>(File(bytes, "text/html", $"applications_report_{DateTime.Now:yyyyMMdd}.html"));
        }

        private string GenerateHtmlReport(List<Application> applications)
        {
            var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Applications Report - {DateTime.Now:yyyy-MM-dd}</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        table {{ border-collapse: collapse; width: 100%; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: left; }}
        th {{ background-color: #f2f2f2; }}
        .header {{ text-align: center; margin-bottom: 20px; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>Social Assistance Applications Report</h1>
        <p>Generated on: {DateTime.Now:yyyy-MM-dd HH:mm}</p>
        <p>Total Applications: {applications.Count}</p>
    </div>
    <table>
        <thead>
            <tr>
                <th>Application Date</th>
                <th>Applicant Name</th>
                <th>ID Number</th>
                <th>Gender</th>
                <th>Marital Status</th>
                <th>County</th>
                <th>Officer</th>
                <th>Programs</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>";

            foreach (var app in applications)
            {
                var programs = string.Join(", ", app.AppliedPrograms.Select(ap => ap.SocialAssistanceProgram.ProgramName));
                var status = app.OfficerSignedDate != default(DateTime) ? "Completed" : "Pending";
                var county = app.Applicant.Village.SubLocation.Location.SubCounty.County.CountyName;

                html += $@"
            <tr>
                <td>{app.ApplicationDate:yyyy-MM-dd}</td>
                <td>{app.Applicant.FirstName} {app.Applicant.LastName}</td>
                <td>{app.Applicant.IdNumber}</td>
                <td>{app.Applicant.GenderCategory.GenderCategoryName}</td>
                <td>{app.Applicant.MaritalStatus.StatusName}</td>
                <td>{county}</td>
                <td>{app.Officer.OfficerName}</td>
                <td>{programs}</td>
                <td>{status}</td>
            </tr>";
            }

            html += @"
        </tbody>
    </table>
</body>
</html>";

            return html;
        }

        private async Task<object> GetReportData()
        {
            var totalApplications = await _context.Applications.CountAsync();
            var completedApplications = await _context.Applications.CountAsync(a => a.OfficerSignedDate != default(DateTime));
            var pendingApplications = totalApplications - completedApplications;

            var applicationsByProgram = await _context.AppliedPrograms
                .GroupBy(ap => ap.SocialAssistanceProgram.ProgramName)
                .Select(g => new { Program = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var applicationsByOfficer = await _context.Applications
                .GroupBy(a => a.Officer.OfficerName)
                .Select(g => new { Officer = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return new
            {
                TotalApplications = totalApplications,
                CompletedApplications = completedApplications,
                PendingApplications = pendingApplications,
                ApplicationsByProgram = applicationsByProgram,
                ApplicationsByOfficer = applicationsByOfficer
            };
        }
    }
}
