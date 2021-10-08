using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NavigusWebApp.Shared.Models;

namespace NavigusWebApi.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        public IHttpContextAccessor Accessor { get; }
        public FirestoreDb Db { get; }

        public const string ListCollectionName = "COURSES";


        public QuestionController(IHttpContextAccessor accessor, FirestoreDb db)
        {
            Accessor = accessor;
            Db = db;
        }

        [Authorize(Roles = "Teacher,Student")]
        [HttpGet("list/{courseId}")]
        public async Task<IActionResult> List(string courseId)
        {
            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace((courseId)))
                return BadRequest("Course Id cant be null, please specify existing course id in route");

            try
            {
                //checking if course already exists in db
                var rec = await Db.Collection(ListCollectionName).Document(courseId).GetSnapshotAsync();

                //if not exist
                if (!rec.Exists)
                    return BadRequest($"Course : {courseId} not exists");

                var prev = rec.ConvertTo<CourseModel>();


                return Ok(prev.Quiz.Questions);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("add/{courseId}")]
        public async Task<IActionResult> Add(string courseId,[FromBody]QuestionModel question)
        {
            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace((courseId)))
                return BadRequest("Course Id cant be null, please specify existing course id in route");

            if(question.Question==null || question.Points>10 
                || question.Options==null || question.Options.Length<=1 || question.CorrectOptionIndexs is null)
            {
                return BadRequest("Please specify valid question or points (0-10) and more than 1 option");
            }

            if (question.CorrectOptionIndexs.Count(x => x >= 0 && x < question.Options.Length) != question.CorrectOptionIndexs.Length)
            {
                return BadRequest("Correct options are 0 based indexs make sure you specify within range [0 - sizeof(options))");
            }
            

            try
            {
                //checking if course already exists in db
                var rec = await Db.Collection(ListCollectionName).Document(courseId).GetSnapshotAsync();

                //if not exist
                if (!rec.Exists)
                    return BadRequest($"Course : {courseId} not exists");

                var prev = rec.ConvertTo<CourseModel>();

                if (prev.Quiz == null)
                    return BadRequest("Quiz is null , please add quiz duration and passing marks first");

                var newQues= new List<QuestionModel>();
                if (prev.Quiz.Questions != null)
                    newQues.AddRange(prev.Quiz.Questions);
                newQues.Add(question);
                prev.Quiz.Questions = newQues.ToArray();

                await Db.Collection(ListCollectionName).Document(courseId).SetAsync(prev);
                return Ok($"Added successfully , total count of questions : {newQues.Count}");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [Authorize(Roles = "Teacher")]
        [HttpGet("delete/{courseId}/{questionIndex}")]
        public async Task<IActionResult> Delete(string courseId,string questionIndex)
        {
            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace((courseId)))
                return BadRequest("Course Id cant be null, please specify existing course id in route");

            if (string.IsNullOrWhiteSpace((questionIndex)) || ! int.TryParse(questionIndex,out _))
                return BadRequest("QuestionIndex can't be null or any thing other than int");

            try
            {
                //checking if course already exists in db
                var rec = await Db.Collection(ListCollectionName).Document(courseId).GetSnapshotAsync();

                //if not exist
                if (!rec.Exists)
                    return BadRequest($"Course : {courseId} not exists");

                var prev = rec.ConvertTo<CourseModel>();

                if (prev.Quiz == null)
                    return BadRequest("Quiz is null , no point in deleting anything");

                var newQues = new List<QuestionModel>();
                if (prev.Quiz.Questions != null)
                    newQues.AddRange(prev.Quiz.Questions);

                var ind=int.Parse(questionIndex);
                if((uint)ind>=newQues.Count())
                {
                    return BadRequest($"Index out of bound {ind} for length {newQues.Count}");
                }
                newQues.RemoveAt(ind);

                prev.Quiz.Questions = newQues.ToArray();

                await Db.Collection(ListCollectionName).Document(courseId).SetAsync(prev);

                return Ok($"Deleted question index {questionIndex} from {courseId}");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
