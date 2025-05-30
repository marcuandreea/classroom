using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using mvc.Models;
using mvc.Services;

namespace mvc.Controllers
{
    public class ClassroomStudentsController : BaseController
    {
        private readonly IClassroomStudentsService _classroomStudentsService;

        public ClassroomStudentsController(UserManager<UserAccount> userManager, IClassroomStudentsService classroomStudentsService) : base(userManager)
        {
            _classroomStudentsService = classroomStudentsService;
        }

        // GET: ClassroomStudents
        public async Task<IActionResult> Index()
        {
            var classroomStudents = await _classroomStudentsService.GetAllClassroomStudentsAsync();
            return View(classroomStudents);
        }

        // GET: ClassroomStudents/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var classroomStudent = await _classroomStudentsService.GetClassroomStudentByIdAsync(id);
            if (classroomStudent == null)
            {
                return NotFound();
            }
            return View(classroomStudent);
        }

        // GET: ClassroomStudents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClassroomStudents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("userId,classroomId")] ClassroomStudents classroomStudent)
        {
            if (ModelState.IsValid)
            {
                await _classroomStudentsService.AddClassroomStudentAsync(classroomStudent);
                return RedirectToAction(nameof(Index));
            }
            return View(classroomStudent);
        }

        // GET: ClassroomStudents/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var classroomStudent = await _classroomStudentsService.GetClassroomStudentByIdAsync(id);
            if (classroomStudent == null)
            {
                return NotFound();
            }
            return View(classroomStudent);
        }

        // POST: ClassroomStudents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,userId,classroomId")] ClassroomStudents classroomStudent)
        {
            if (id != classroomStudent.id)
            {
                return BadRequest("ClassroomStudent ID mismatch.");
            }

            if (ModelState.IsValid)
            {
                await _classroomStudentsService.UpdateClassroomStudentAsync(classroomStudent);
                return RedirectToAction(nameof(Index));
            }
            return View(classroomStudent);
        }

        // GET: ClassroomStudents/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var classroomStudent = await _classroomStudentsService.GetClassroomStudentByIdAsync(id);
            if (classroomStudent == null)
            {
                return NotFound();
            }
            return View(classroomStudent);
        }

        // POST: ClassroomStudents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _classroomStudentsService.DeleteClassroomStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveClassroom(int id)
        {
            // Retrieve the current user's ID from the session
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID cannot be null or empty.");
            }

            // Check if the user is part of the classroom
            var classroomStudent = await _classroomStudentsService.GetClassroomStudentByIdAsync(id, userId);
            if (classroomStudent == null)
            {
                return NotFound("User is not part of this classroom.");
            }

            // Remove the user from the classroom
            await _classroomStudentsService.DeleteClassroomStudentAsync(classroomStudent.id);

            // Redirect back to the home page or classroom list
            return RedirectToAction("Home", "Home");
        }
    }
}
