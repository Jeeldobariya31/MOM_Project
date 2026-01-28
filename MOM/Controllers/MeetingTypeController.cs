using Microsoft.AspNetCore.Mvc;
using MOM.Models;
using MOM.Services;
using System.Data;

namespace MOM.Controllers
{
    public class MeetingTypeController : Controller
    {
        private readonly DataService _dataService;

        public MeetingTypeController()
        {
            _dataService = DataService.Instance;
        }

        public IActionResult MeetingTypeList(string searchTerm = "", string sortBy = "MeetingTypeName", string sortOrder = "asc")
        {
            try
            {
                var meetingTypes = _dataService.GetFilteredData(_dataService.MeetingTypes, searchTerm, null);
                
                var sortedRows = meetingTypes.AsEnumerable();
                switch (sortBy.ToLower())
                {
                    case "meetingtypename":
                        sortedRows = sortOrder == "desc" 
                            ? sortedRows.OrderByDescending(r => r.Field<string>("MeetingTypeName"))
                            : sortedRows.OrderBy(r => r.Field<string>("MeetingTypeName"));
                        break;
                    case "created":
                        sortedRows = sortOrder == "desc"
                            ? sortedRows.OrderByDescending(r => r.Field<DateTime>("Created"))
                            : sortedRows.OrderBy(r => r.Field<DateTime>("Created"));
                        break;
                    default:
                        sortedRows = sortedRows.OrderBy(r => r.Field<string>("MeetingTypeName"));
                        break;
                }

                var sortedTable = meetingTypes.Clone();
                foreach (var row in sortedRows)
                {
                    sortedTable.ImportRow(row);
                }

                ViewBag.SearchTerm = searchTerm;
                ViewBag.SortBy = sortBy;
                ViewBag.SortOrder = sortOrder;
                ViewBag.TotalRecords = _dataService.MeetingTypes.Rows.Count;
                ViewBag.FilteredRecords = sortedTable.Rows.Count;

                return View(sortedTable);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meeting types: {ex.Message}";
                return View(new DataTable());
            }
        }

        public IActionResult MeetingTypeAddEdit(int? id)
        {
            try
            {
                var model = new MeetingTypeModel();

                if (id.HasValue && id.Value > 0)
                {
                    var row = _dataService.MeetingTypes.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("MeetingTypeID") == id.Value);

                    if (row == null)
                    {
                        TempData["ErrorMessage"] = "Meeting type not found.";
                        return RedirectToAction("MeetingTypeList");
                    }

                    model.MeetingTypeID = row.Field<int>("MeetingTypeID");
                    model.MeetingTypeName = row.Field<string>("MeetingTypeName") ?? "";
                    model.Remarks = row.Field<string>("Remarks") ?? "";
                    model.Created = row.Field<DateTime>("Created");
                    model.Modified = row.Field<DateTime>("Modified");

                    // Get meeting count for this type
                    model.MeetingCount = _dataService.Meetings.AsEnumerable()
                        .Count(r => r.Field<int>("MeetingTypeID") == id.Value);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading meeting type: {ex.Message}";
                return RedirectToAction("MeetingTypeList");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MeetingTypeAddEdit(MeetingTypeModel model)
        {
            try
            {
                // Custom validation for duplicate names
                var existingType = _dataService.MeetingTypes.AsEnumerable()
                    .FirstOrDefault(r => r.Field<string>("MeetingTypeName")?.Equals(model.MeetingTypeName, StringComparison.OrdinalIgnoreCase) == true
                                    && r.Field<int>("MeetingTypeID") != model.MeetingTypeID);

                if (existingType != null)
                {
                    ModelState.AddModelError("MeetingTypeName", "A meeting type with this name already exists.");
                }

                if (!ModelState.IsValid)
                {
                    // Reload counts for edit mode
                    if (model.MeetingTypeID > 0)
                    {
                        model.MeetingCount = _dataService.Meetings.AsEnumerable()
                            .Count(r => r.Field<int>("MeetingTypeID") == model.MeetingTypeID);
                    }
                    return View(model);
                }

                if (model.MeetingTypeID == 0)
                {
                    // Add new meeting type
                    var newId = _dataService.GetNextId(_dataService.MeetingTypes, "MeetingTypeID");
                    _dataService.MeetingTypes.Rows.Add(
                        newId,
                        model.MeetingTypeName.Trim(),
                        model.Remarks.Trim(),
                        DateTime.Now,
                        DateTime.Now
                    );
                    TempData["SuccessMessage"] = "Meeting type added successfully.";
                }
                else
                {
                    // Update existing meeting type
                    var row = _dataService.MeetingTypes.AsEnumerable()
                                .FirstOrDefault(r => r.Field<int>("MeetingTypeID") == model.MeetingTypeID);

                    if (row != null)
                    {
                        row["MeetingTypeName"] = model.MeetingTypeName.Trim();
                        row["Remarks"] = model.Remarks.Trim();
                        row["Modified"] = DateTime.Now;
                        TempData["SuccessMessage"] = "Meeting type updated successfully.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Meeting type not found for update.";
                    }
                }

                return RedirectToAction("MeetingTypeList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error saving meeting type: {ex.Message}";
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                // Check if meeting type has meetings
                var hasMeetings = _dataService.Meetings.AsEnumerable()
                    .Any(r => r.Field<int>("MeetingTypeID") == id);

                if (hasMeetings)
                {
                    return Json(new { success = false, message = "Cannot delete meeting type. It has associated meetings." });
                }

                var row = _dataService.MeetingTypes.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("MeetingTypeID") == id);

                if (row != null)
                {
                    _dataService.MeetingTypes.Rows.Remove(row);
                    return Json(new { success = true, message = "Meeting type deleted successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Meeting type not found." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error deleting meeting type: {ex.Message}" });
            }
        }

        [HttpGet]
        public IActionResult GetMeetingTypeDetails(int id)
        {
            try
            {
                var row = _dataService.MeetingTypes.AsEnumerable()
                            .FirstOrDefault(r => r.Field<int>("MeetingTypeID") == id);

                if (row == null)
                {
                    return Json(new { success = false, message = "Meeting type not found." });
                }

                var meetingCount = _dataService.Meetings.AsEnumerable()
                    .Count(r => r.Field<int>("MeetingTypeID") == id);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        MeetingTypeID = row.Field<int>("MeetingTypeID"),
                        MeetingTypeName = row.Field<string>("MeetingTypeName"),
                        Remarks = row.Field<string>("Remarks"),
                        Created = row.Field<DateTime>("Created").ToString("dd/MM/yyyy hh:mm tt"),
                        Modified = row.Field<DateTime>("Modified").ToString("dd/MM/yyyy hh:mm tt"),
                        MeetingCount = meetingCount
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error getting meeting type details: {ex.Message}" });
            }
        }

        [HttpGet]
        public JsonResult GetMeetingTypes()
        {
            try
            {
                var meetingTypes = _dataService.MeetingTypes.AsEnumerable()
                    .Select(row => new
                    {
                        Value = row.Field<int>("MeetingTypeID"),
                        Text = row.Field<string>("MeetingTypeName")
                    })
                    .OrderBy(mt => mt.Text)
                    .ToList();

                return Json(meetingTypes);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
    }
}
