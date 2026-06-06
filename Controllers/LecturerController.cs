using Microsoft.AspNetCore.Mvc;
using UniManage.Models;
using System.Linq;

namespace UniManage.Controllers
{
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LecturerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LECTURER DASHBOARD
        public IActionResult Dashboard()
        {
            // GET LOGGED-IN USER ID

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // GET LECTURER

            var lecturer = (from l in _context.Lecturers
                            join u in _context.Users
                            on l.UserID equals u.UserID
                            where l.UserID == lecturerUserId.Value
                            select new
                            {
                                l.LecturerID,
                                l.Department,
                                l.Specialization,
                                u.FullName
                            }).FirstOrDefault();

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // TOTAL MODULES

            int totalModules = _context.Modules
                .Count(m => m.LecturerID == lecturer.LecturerID);

            // TOTAL STUDENTS

            int totalStudents = _context.Students.Count();

            // VIEWBAG

            ViewBag.LecturerName = lecturer.FullName;
            ViewBag.Department = lecturer.Department;
            ViewBag.Specialization = lecturer.Specialization;

            int totalAssignments = _context.Assignments
                .Count(a => a.LecturerID == lecturer.LecturerID);

            int pendingReviews = _context.AssignmentSubmissions
                .Count(s => s.Status == "Submitted");

            ViewBag.TotalModules = totalModules;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalAssignments = totalAssignments;
            ViewBag.PendingReviews = pendingReviews;
            // LOAD MODULES

            var modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .ToList();

            return View(modules);
        }

        public IActionResult MyModules()
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var modules = _context.Modules
                .Where(m => m.LecturerID == lecturer.LecturerID)
                .ToList();

            return View(modules);
        }

        public IActionResult ModuleDetails(int id)
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var module = _context.Modules
                .FirstOrDefault(m => m.ModuleID == id);

            if (module == null)
            {
                return NotFound();
            }

            ViewBag.Materials = _context.Materials
                .Where(m => m.ModuleID == id)
                .OrderByDescending(m => m.UploadedAt)
                .ToList();

            return View(module);
        }

        [HttpPost]
        public async Task<IActionResult> UploadMaterial(AddMaterialViewModel model)
        {
            if (model == null)
            {
                return Content("ERROR: Model is NULL");
            }

            if (model.File == null)
            {
                return Content("ERROR: No file was uploaded");
            }

            if (!ModelState.IsValid)
            {
                return Content("ERROR: ModelState is Invalid");
            }

            string uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads/materials");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid().ToString() +
                              Path.GetExtension(model.File.FileName);

            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            Material material = new Material
            {
                ModuleID = model.ModuleID,
                Title = model.Title,
                MaterialType = model.MaterialType,
                FilePath = "/uploads/materials/" + fileName,
                UploadedAt = DateTime.Now
            };

            _context.Materials.Add(material);

            await _context.SaveChangesAsync();

            return RedirectToAction("ModuleDetails",
                new { id = model.ModuleID });
        }

        [HttpGet]
        public IActionResult CreateAssignment()
        {
            ViewBag.Courses = _context.Courses.ToList();

            ViewBag.Modules = _context.Modules.ToList();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> CreateAssignment(
    AddAssignmentViewModel model)
        {
            string filePath = "";

            if (model.AttachmentFile != null)
            {
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/assignments");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string fileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(model.AttachmentFile.FileName);

                string fullPath =
                    Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.AttachmentFile.CopyToAsync(stream);
                }

                filePath = "/uploads/assignments/" + fileName;
            }

            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            Assignment assignment = new Assignment()
            {
                CourseID = model.CourseID,
                ModuleID = model.ModuleID,
                BatchID = model.BatchID,
                Title = model.Title,
                Description = model.Description,
                Deadline = model.Deadline,
                LecturerID = lecturer?.LecturerID,
                AttachmentPath = filePath,
                CreatedAt = DateTime.Now
            };

            _context.Assignments.Add(assignment);

            await _context.SaveChangesAsync();

            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public JsonResult GetModulesByCourse(int courseId)
        {
            var modules = (from cm in _context.CourseModules
                           join m in _context.Modules
                           on cm.ModuleID equals m.ModuleID
                           where cm.CourseID == courseId
                           select new
                           {
                               m.ModuleID,
                               m.ModuleName,
                               m.ModuleCode
                           }).ToList();

            return Json(modules);
        }

        [HttpGet]
        public JsonResult GetBatchesByCourse(int courseId)
        {
            var batches = _context.Batches
                .Where(b => b.CourseID == courseId &&
                            b.Status == "Active")
                .Select(b => new
                {
                    b.BatchID,
                    b.BatchName
                })
                .ToList();

            return Json(batches);
        }

        public IActionResult Assignments()
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var assignments = _context.Assignments
                .Where(a => a.LecturerID == lecturer.LecturerID)
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            return View(assignments);
        }

        [HttpGet]
        public IActionResult EditAssignment(int id)
        {
            var assignment = _context.Assignments
                .FirstOrDefault(a => a.AssignmentID == id);

            if (assignment == null)
            {
                return NotFound();
            }

            ViewBag.Courses = _context.Courses.ToList();

            ViewBag.Modules = (from cm in _context.CourseModules
                               join m in _context.Modules
                               on cm.ModuleID equals m.ModuleID
                               where cm.CourseID == assignment.CourseID
                               select m).ToList();

            ViewBag.Batches = _context.Batches
                .Where(b => b.CourseID == assignment.CourseID)
                .ToList();

            return View(assignment);
        }

        [HttpPost]
        public IActionResult EditAssignment(Assignment model)
        {
            var assignment = _context.Assignments
                .FirstOrDefault(a => a.AssignmentID == model.AssignmentID);

            if (assignment == null)
            {
                return NotFound();
            }

            assignment.Title = model.Title;
            assignment.Description = model.Description;
            assignment.CourseID = model.CourseID;
            assignment.ModuleID = model.ModuleID;
            assignment.BatchID = model.BatchID;
            assignment.Deadline = model.Deadline;

            _context.SaveChanges();

            return RedirectToAction("Assignments");
        }

        public IActionResult Messages(int? groupId)
        {
            int? lecturerUserId =
                HttpContext.Session.GetInt32("UserID");

            if (lecturerUserId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var lecturer = _context.Lecturers
                .FirstOrDefault(l => l.UserID == lecturerUserId);

            if (lecturer == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var groups =
            (
                from gm in _context.GroupMembers

                join mg in _context.MessageGroups
                    on gm.GroupID equals mg.GroupID

                join b in _context.Batches
                    on mg.BatchID equals b.BatchID

                join c in _context.Courses
                    on mg.CourseID equals c.CourseID

                where gm.UserID == lecturerUserId.Value

                select new LecturerMessageGroupViewModel
                {
                    GroupID = mg.GroupID,

                    GroupName = mg.GroupName,

                    BatchID = b.BatchID,

                    BatchName = b.BatchName,

                    CourseName = c.CourseName,

                    StudentCount =
                        _context.Students.Count(s =>
                            s.BatchID == b.BatchID)
                }
            ).Distinct().ToList();

            ViewBag.SelectedGroupId = groupId;

            if (groupId != null)
            {
                var selectedGroup = _context.MessageGroups
                    .FirstOrDefault(g => g.GroupID == groupId);

                ViewBag.GroupName = selectedGroup?.GroupName;

                ViewBag.Messages = _context.GroupMessages
                    .Where(m => m.GroupID == groupId)
                    .OrderBy(m => m.SentAt)
                    .ToList();
            }

            return View(groups);
        }

        [HttpPost]
        public async Task<IActionResult> SendGroupMessage(
    int groupId,
    string? messageText,
    IFormFile? file)
        {
            int? userId =
                HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string? filePath = null;
            string? fileName = null;

            if (file != null && file.Length > 0)
            {
                string uploadFolder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/chatfiles");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                string savedFileName =
                    Guid.NewGuid().ToString() +
                    Path.GetExtension(file.FileName);

                string fullPath =
                    Path.Combine(uploadFolder, savedFileName);

                using (var stream =
                       new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                filePath = "/uploads/chatfiles/" + savedFileName;
                fileName = file.FileName;
            }

            if (!string.IsNullOrWhiteSpace(messageText) || file != null)
            {
                GroupMessage message = new GroupMessage
                {
                    GroupID = groupId,
                    SenderUserID = userId.Value,
                    MessageText = messageText,
                    FilePath = filePath,
                    FileName = fileName,
                    SentAt = DateTime.Now
                };

                _context.GroupMessages.Add(message);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Messages",
                new { groupId });
        }

    }
}