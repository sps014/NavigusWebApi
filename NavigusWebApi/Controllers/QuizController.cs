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
    public class QuizController : ControllerBase
    {
        public IHttpContextAccessor Accessor { get; }
        public FirestoreDb Db { get; }

        public const string ListCollectionName = "COURSES";


        public QuizController(IHttpContextAccessor accessor, FirestoreDb db)
        {
            Accessor = accessor;
            Db = db;
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("set/{courseId}")]
        public async Task<IActionResult> Set(string courseId, QuizModel quizModel)
        {

            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace(courseId))
                return BadRequest("Course Id cant be null");

            if (quizModel.PassingMarks < 0 || quizModel.Duration <= 0)
                return BadRequest("please specify valid duration and passing mark for quiz");

            try
            {
                //checking if course already exists in db
                var rec = await Db.Collection(ListCollectionName).Document(courseId).GetSnapshotAsync();

                //cant add if already exist
                if (!rec.Exists)
                    return BadRequest($"Course : {courseId} does not exists");

                var p=rec.ConvertTo<CourseModel>();

                //retain old questions if empty questions are pushed
                var old = p.Quiz.Questions;
                p.Quiz = quizModel;
                p.Quiz.Questions ??= old;


                //add to db
                await Db.Collection(ListCollectionName).Document(p.CourseId).SetAsync(p);

                return Ok($"Successfuly overrriden  Quiz for course : {p.CourseId}");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles ="Teacher,Student")]
        [HttpGet("get/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace((id)))
                return BadRequest("Course Id cant be null, please specify existing course id in route");

            try
            {
                //checking if course already exists in db
                var rec = await Db.Collection(ListCollectionName).Document(id).GetSnapshotAsync();

                //if not exist
                if (!rec.Exists)
                    return BadRequest($"Course : {id} not exists");

                var prev = rec.ConvertTo<CourseModel>();


                return Ok(prev.Quiz);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
