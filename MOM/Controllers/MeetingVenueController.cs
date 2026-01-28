using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingVenueController : Controller
    {
        private readonly DataService _dataService;

        public MeetingVenueController()
        {
            _dataService = DataService.Instance;
        }

        public IActionResult MeetingVenueList(string searchTerm = "", string sortBy = "MeetingVenueName", string sortOrder = "asc")
        {
            try
            {
                var venues = _dataService.GetFilteredData(_dataService.MeetingVenues, searchTerm, null);
                
                // Apply sorting
                var sortedRows = venues.AsEnumerable();
                switch (sortBy.ToLower())
                {
                    case "meetingvenuename":
                        sortedRows = sortOrder == "desc" 
                            ? sortedRows.OrderByDescending(r => r.Field<string>("MeetingVenueName"))
                            : sortedRows.OrderBy(r => r.Field<string>("MeetingVenueName"));
                        break;
                    case "created":
                        sortedRows = sortOrder == "desc"
                            ? sortedRows.OrderByDescending(r => r.Field<DateTime>("Created"))
                            : sortedRows.OrderBy(r => r.Field<DateTime>("Created"));
                        break;
                    default:
                        sortedRows = sortedRows.OrderBy(r => r.Field<string>("MeetingVenueName"));
                        break;
                }

                var sortedTable = venues.Clone();
                foreach (var row in sortedRows)
                {
                    sortedTable.ImportRow(row);
                }

                ViewBag.SearchTerm = searchTerm;
                ViewBag.SortBy = sortBy;
                ViewBag.SortOrder = sortOrder;
                ViewBag.TotalRecords = _dataService.MeetingVenues.Rows.Count;
                ViewBag.FilteredRecords = sortedTable.Rows.Count;

                return View(sortedTable);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meeting venues: {ex.Message}";
                return View(new DataTable());
            }
        }

        public IActionResult MeetingVenueAddEdit(int? id)
        {
            try
            {
                var model = new MeetingVenueModel();

                if (id.HasValue && id.Value > 0)
                {
                    var row = _dataService.MeetingVenues.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("MeetingVenueID") == id.Value);

                    if (row == null)
                    {
                        TempData["ErrorMessage"] = "Meeting venue not found.";
                        return RedirectToAction("MeetingVenueList");
                    }

                    model.MeetingVenueID = row.Field<int>("MeetingVenueID");
                    model.MeetingVenueName = row.Field<string>("MeetingVenueName") ?? "";
                    model.Created = row.Field<DateTime>("Created");
                    model.Modified = row.Field<DateTime>("Modified");

                    // Get meeting count for this venue
                    model.MeetingCount = _dataService.Meetings.AsEnumerable()
                        .Count(r => r.Field<int>("MeetingVenueID") == id.Value);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meeting venue: {ex.Message}";
                return RedirectToAction("MeetingVenueList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MeetingVenueAddEdit(MeetingVenueModel model)
        {
            try
            {
                // Custom validation for duplicate names
                var existingVenue = _dataService.MeetingVenues.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("MeetingVenueName")?.Equals(model.MeetingVenueName, StringComparison.OrdinalIgnoreCase) == true
                                    && r.Field<int>("MeetingVenueID") != model.MeetingVenueID);

                if (existingVenue != null)
                {
                    ModelState.AddModelError("MeetingVenueName", "A meeting venue with this name already exists.");
                }

                if (!ModelState.IsValid)
                {
                    // Reload counts for edit mode
                    if (model.MeetingVenueID > 0)
                    {
                        model.MeetingCount = _dataService.Meetings.AsEnumerable()
                            .Count(r => r.Field<int>("MeetingVenueID") == model.MeetingVenueID);
                    }
                    return View(model);
                }

                if (model.MeetingVenueID == 0)
                {
                    // Add new venue
                    var newId = _dataService.GetNextId(_dataService.MeetingVenues, "MeetingVenueID");
                    _dataService.MeetingVenues.Rows.Add(
                        newId,
                        model.MeetingVenueName.Trim(),
                        DateTime.Now,
                        DateTime.Now
                    );
                    TempData["SuccessMessage"] = "Meeting venue added successfully.";
                }
                else
                {
                    // Update existing venue
                    var row = _dataService.MeetingVenues.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("MeetingVenueID") == model.MeetingVenueID);

                    if (row != null)
                    {
                        row["MeetingVenueName"] = model.MeetingVenueName.Trim();
                        row["Modified"] = DateTime.Now;
                        TempData["SuccessMessage"] = "Meeting venue updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Meeting venue not found for update.";
                    }
                }

                return RedirectToAction("MeetingVenueList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving meeting venue: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                // Check if venue has meetings
                var hasMeetings = _dataService.Meetings.AsEnumerable()
                    .Any(r => r.Field<int>("MeetingVenueID") == id);

                if (hasMeetings)
                {
                    return Json(new { success = false, message = "Cannot delete meeting venue. It has associated meetings." });
                }

                var row = _dataService.MeetingVenues.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("MeetingVenueID") == id);

                if (row != null)
                {
                    _dataService.MeetingVenues.Rows.Remove(row);
                    return Json(new { success = true, message = "Meeting venue deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Meeting venue not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting meeting venue: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetMeetingVenueDetails(int id)
        {
            try
            {
                var row = _dataService.MeetingVenues.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("MeetingVenueID") == id);

                if (row == null)
                {
                    return Json(new { success = false, message = "Meeting venue not found." });
                }

                var meetingCount = _dataService.Meetings.AsEnumerable()
                    .Count(r => r.Field<int>("MeetingVenueID") == id);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        MeetingVenueID = row.Field<int>("MeetingVenueID"),
                        MeetingVenueName = row.Field<string>("MeetingVenueName"),
                        Created = row.Field<DateTime>("Created").ToString("dd/MM/yyyy hh:mm tt"),
                        Modified = row.Field<DateTime>("Modified").ToString("dd/MM/yyyy hh:mm tt"),
                        MeetingCount = meetingCount
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error getting meeting venue details: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult GetMeetingVenues()
        {
            try
            {
                var venues = _dataService.MeetingVenues.AsEnumerable()
                    .Select(row => new
                    {
                        Value = row.Field<int>("MeetingVenueID"),
                        Text = row.Field<string>("MeetingVenueName")
                    })
                    .OrderBy(v => v.Text)
                    .ToList();

                return Json(venues);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
