using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppJobSearchOnline.Data;

namespace WebAppJobSearchOnline.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class JobApplicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public JobApplicationsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: JobApplications
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                // If the user is an admin, show all job applications
                return View(await _context.JobApplications.ToListAsync());
            }
            else if (User.IsInRole("User"))
            {
                var jobApplications = _context.JobApplications
                    .Where(P => P.UserId.Equals(User.Identity.Name)).ToList();

                return View(jobApplications);
            } else
            {
                var jobApplications = _context.JobApplications
                    .Where(P => P.UserId.Equals("")).ToList();

                return View(jobApplications);
            }
        }

        // GET: JobApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // GET: JobApplications/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobPostingId,UserId,Status,AppliedDate")] JobApplication jobApplication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobApplication);
        }

        // GET: JobApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication == null)
            {
                return NotFound();
            }
            return View(jobApplication);
        }

        // POST: JobApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobPostingId,UserId,Status,AppliedDate,CVFilePath,CVFileName,CVFileType")] JobApplication jobApplication)
        {
            if (id != jobApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobApplication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobApplicationExists(jobApplication.Id))
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
            return View(jobApplication);
        }

        // GET: JobApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobApplication = await _context.JobApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobApplication == null)
            {
                return NotFound();
            }

            return View(jobApplication);
        }

        // POST: JobApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobApplication = await _context.JobApplications.FindAsync(id);
            if (jobApplication != null)
            {
                _context.JobApplications.Remove(jobApplication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobApplicationExists(int id)
        {
            return _context.JobApplications.Any(e => e.Id == id);
        }

        // GET: JobApplications/Apply/5
        [Authorize(Roles = "User")]
        public IActionResult Apply(int id)
        {
            var jobPosting = _context.JobPostings.Find(id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            ViewBag.JobPostingId = id;
            ViewBag.JobTitle = jobPosting.Title;
            return View();
        }

        // POST: JobApplications/Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Apply(int JobPostingId, IFormFile cvFile)
        {
            if (cvFile == null || cvFile.Length == 0)
            {
                ModelState.AddModelError("cvFile", "Please upload your CV.");
                ViewBag.JobPostingId = JobPostingId;
                return View();
            }

            // Check file extension
            var extension = Path.GetExtension(cvFile.FileName).ToLower();
            if (extension != ".pdf" && extension != ".doc" && extension != ".docx")
            {
                ModelState.AddModelError("cvFile", "Only PDF and Word documents are allowed.");
                ViewBag.JobPostingId = JobPostingId;
                return View();
            }

            // Check file size (limit to 5MB)
            if (cvFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("cvFile", "File size must be less than 5MB.");
                ViewBag.JobPostingId = JobPostingId;
                return View();
            }

            try
            {
                // Create uploads directory if it doesn't exist
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", "cvs");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique file name to prevent overwriting
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + cvFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save the file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await cvFile.CopyToAsync(fileStream);
                }

                // Create new job application
                var jobApplication = new JobApplication
                {
                    JobPostingId = JobPostingId,
                    UserId = User.Identity.Name,
                    Status = "APPLIED",
                    AppliedDate = DateTime.Now,
                    CVFilePath = filePath,
                    CVFileName = cvFile.FileName,
                    CVFileType = extension
                };

                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                return RedirectToAction("Index", "JobApplications");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while uploading your CV. Please try again.");
                ViewBag.JobPostingId = JobPostingId;
                return View();
            }
        }

        // GET: Download CV file
        [Authorize(Roles = "Admin, User")]
        public IActionResult DownloadCV(int id)
        {
            var application = _context.JobApplications.Find(id);
            if (application == null || string.IsNullOrEmpty(application.CVFilePath))
            {
                return NotFound();
            }

            // Check if user has permission to download (admin or owner of application)
            if (!User.IsInRole("Admin") && application.UserId != User.Identity.Name)
            {
                return Unauthorized();
            }

            var fileBytes = System.IO.File.ReadAllBytes(application.CVFilePath);
            return File(fileBytes, GetContentType(application.CVFileType), application.CVFileName);
        }

        private string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
