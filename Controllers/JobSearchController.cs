using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebAppJobSearchOnline.Data;
using WebAppJobSearchOnline.Models;

namespace WebAppJobSearchOnline.Controllers
{
    [Authorize(Roles = "Admin, User")]
    public class JobSearchController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public JobSearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JobSearch
        public async Task<IActionResult> Index(string titleSearch, string locationSearch
            , string jobTypeSearch, string statusSearch)
        {
            // Create a queryable for JobPostings
            IQueryable<JobPosting> query = _context.JobPostings;

            // Apply search filter for Title if provided
            if (!string.IsNullOrEmpty(titleSearch))
            {
                query = query.Where(jp => jp.Title.Contains(titleSearch));
            }

            // Apply search filter for Location if provided
            if (!string.IsNullOrEmpty(locationSearch))
            {
                query = query.Where(jp => jp.Location.Contains(locationSearch));
            }

            // Apply search filter for Job Type if provided
            if (!string.IsNullOrEmpty(jobTypeSearch))
            {
                query = query.Where(jp => jp.JobType.Contains(jobTypeSearch));
            }

            // Apply search filter for Status if provided
            if (!string.IsNullOrEmpty(statusSearch))
            {
                query = query.Where(jp => jp.Status.Contains(statusSearch));
            }

            // Execute the query and pass the results to the view
            var jobPostings = await query.ToListAsync();
            ViewBag.TitleSearch = titleSearch;
            ViewBag.LocationSearch = locationSearch;

            return View(jobPostings);
        }

        // GET: JobSearch/Details/5
        public async Task<IActionResult> Details(int? id, JobSearchViewModel jobSearchView)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // GET: JobSearch/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: JobSearch/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Requirements,Location,SalaryMin,SalaryMax,JobType,Status,StartPosting,EndPosting")] JobPosting jobPosting)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobPosting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobPosting);
        }

        // GET: JobSearch/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return View(jobPosting);
        }

        // POST: JobSearch/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Requirements,Location,SalaryMin,SalaryMax,JobType,Status,StartPosting,EndPosting")] JobPosting jobPosting)
        {
            if (id != jobPosting.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobPosting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobPostingExists(jobPosting.Id))
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
            return View(jobPosting);
        }

        // GET: JobSearch/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting);
        }

        // POST: JobSearch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting != null)
            {
                _context.JobPostings.Remove(jobPosting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: JobSearch/Apply/5
        public async Task<IActionResult> Apply(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobPosting = await _context.JobPostings.FindAsync(id);
            if (jobPosting == null)
            {
                return NotFound();
            }
            return View(jobPosting);
        }

        // POST: JobSearch/Apply/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Apply(int id, [Bind("Id,Title,Requirements,Location,SalaryMin,SalaryMax,JobType,Status,StartPosting,EndPosting")] JobPosting jobPosting)
        public async Task<IActionResult> Apply(int id)
        {
            try
            {
                JobApplication jobApplication = new JobApplication();
                jobApplication.JobPostingId = id;
                jobApplication.AppliedDate = DateTime.Now;
                jobApplication.Status = "APPLIED";
                jobApplication.UserId = User.Identity.Name;

                _context.Add(jobApplication);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!JobPostingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "JobApplications");
        }

        private bool JobPostingExists(int id)
        {
            return _context.JobPostings.Any(e => e.Id == id);
        }
    }
}
