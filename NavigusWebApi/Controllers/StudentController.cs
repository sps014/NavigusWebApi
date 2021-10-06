using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NavigusWebApi.Extensions;
using NavigusWebApi.Models;

namespace NavigusWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        public IHttpContextAccessor Accessor { get; }
        public FirestoreDb Db { get; }

        public const string ListCollectionName = "STUDENTS";


        public StudentController(IHttpContextAccessor accessor, FirestoreDb db)
        {
            Accessor = accessor;
            Db = db;
        }

        [Authorize(Roles="Student")]
        [HttpGet("enrolledlist")]
        public async Task<IActionResult> CourseList()
        {
            var uid = Accessor.GetUid();
            try
            {
                var student =await CreateOrGetProfileAsync(uid);
                return Ok(student);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Student")]
        [HttpGet("enroll/{courseId}")]
        public async Task<IActionResult> Enroll(string courseId)
        {
            //if course exists or created by teacher
            var snap = await Db.Collection(CourseController.ListCollectionName).Document(courseId).GetSnapshotAsync();
            if (!snap.Exists)
                return BadRequest($"No Course with id : {courseId} found");

            var uid = Accessor.GetUid();
            try
            {
                var student = await CreateOrGetProfileAsync(uid);

                if(student.EnrolledCourses.Count(x=>x.CourseId == courseId)>=1)
                {
                    return BadRequest($"Already enrolled in {courseId}");
                }
                var courses=student.EnrolledCourses.ToList();
                courses.Add(new StudentCourseDetailsModel
                {
                    CourseId = courseId,
                    EnrolledTimeStamp = DateTime.UtcNow.Ticks,
                    Attempted = new List<int>(),
                    CorrectAnswersIndex = new List<int>()
                });

                student.EnrolledCourses = courses.ToArray();
               
               await  Db.Collection(ListCollectionName).Document(uid).SetAsync(student);

               return Ok($"Successfully enrolled in {courseId}");
               
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<StudentModel> CreateOrGetProfileAsync(string uid)
        {
            var rec = await Db.Collection(ListCollectionName).Document(uid).GetSnapshotAsync();
            StudentModel student = new StudentModel
            {
                EnrolledCourses = new StudentCourseDetailsModel[] { },
                Uid = uid
            };
            if (!rec.Exists)
            {
                await Db.Collection(ListCollectionName).Document(uid).SetAsync(
                    student);
            }
            student = rec.ConvertTo<StudentModel>();

            return student;
        }
    }
}
