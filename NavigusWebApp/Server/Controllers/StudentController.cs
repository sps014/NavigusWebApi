using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NavigusWebApi.Extensions;
using NavigusWebApp.Shared.Models;

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
                    EnrolledTimeStamp = DateTime.UtcNow.Ticks.ToString(),
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
        [Authorize(Roles = "Student")]
        [HttpGet("startquiz/{courseId}")]
        public async Task<IActionResult> StartQuiz(string courseId)
        {
            //if course exists or created by teacher
            var snap = await Db.Collection(CourseController.ListCollectionName).Document(courseId).GetSnapshotAsync();
            if (!snap.Exists)
                return BadRequest($"No Course with id : {courseId} found");
            var duration = snap.ConvertTo<CourseModel>().Quiz.Duration;

            var uid = Accessor.GetUid();
            try
            {
                var student = await CreateOrGetProfileAsync(uid);

                var courses = student.EnrolledCourses;
                var course=courses.FirstOrDefault(x=>x.CourseId==courseId);
                if(course is null)
                {
                    return BadRequest($"not registered for {courseId}");
                }
                if(course.QuizStarted)
                {
                    return BadRequest($"Quiz Already started for you");
                }

                course.QuizStarted = true;
                course.QuizExpireTimeStamp = DateTime.UtcNow.AddMinutes(duration).Ticks.ToString();

                await Db.Collection(ListCollectionName).Document(uid).SetAsync(student);

                return Ok($"Successfully started quiz for {courseId}");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Student")]
        [HttpPost("submitanswer")]
        public async Task<IActionResult> SubmitAnswer(AnswerModel ans)
        {
            var courseId = ans.CourseId;
            //if course exists or created by teacher
            var snap = await Db.Collection(CourseController.ListCollectionName).Document(courseId).GetSnapshotAsync();
            if (!snap.Exists)
                return BadRequest($"No Course with id : {courseId} found");
            var courseInfo=snap.ConvertTo<CourseModel>();

            int len = courseInfo.Quiz.Questions.Length;

            var uid = Accessor.GetUid();
            try
            {
                var student = await CreateOrGetProfileAsync(uid);

                var courses = student.EnrolledCourses;

                var course = courses.FirstOrDefault(x => x.CourseId == courseId);
               
                //validations

                if (course is null)
                {
                    return BadRequest($"not registered for {courseId}");
                }
                else if (!course.QuizStarted)
                {
                    return BadRequest($"Quiz not started, first call startquiz end point");
                }
                else if(course.QuizExpireTime<DateTime.UtcNow.Ticks)
                {
                    return BadRequest($"Can't submit quiz already expired diff");
                }
                else if(len <= ans.QuestionIndex)
                {
                    return BadRequest($"(out of bound) {len} questions are there got {ans.QuestionIndex} index");
                }
                else if(course.Attempted.Contains(ans.QuestionIndex))
                {
                    return BadRequest($"already submitted answer for {ans.QuestionIndex}");
                }

                var curQues = courseInfo.Quiz.Questions[ans.QuestionIndex];

                if (ans.Answers.Count(x=>x>=0 && x<curQues.Options.Length)!=ans.Answers.Length)
                {
                    return BadRequest($"Invalid answer option was submitted for {ans.QuestionIndex}");
                }
                
                course.Attempted.Add(ans.QuestionIndex);

                bool isCorrect = EqualAns(curQues.CorrectOptionIndexs.ToList(),ans.Answers.ToList());

                //gamification for scoreboard
                if(isCorrect)
                {
                    course.CorrectAnswersIndex.Add(ans.QuestionIndex);
                    course.PointsObtained += (int)curQues.Points;
                    course.XpObtained += 10;
                }

                //update value
                await Db.Collection(ListCollectionName).Document(uid).SetAsync(student);
                await UpdateLeaderboard(uid, courseId, course);

                return Ok(new
                {
                    PassStatus= course.PointsObtained >= courseInfo.Quiz.PassingMarks,
                    Points=course.PointsObtained,
                    XP=course.XpObtained,
                    NextQuestionSmartSuggestionIndex=SmartSuggestionForNextQuestion(
                        courseInfo.Quiz,course,ans.QuestionIndex,isCorrect)
                });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private int SmartSuggestionForNextQuestion(QuizModel quiz,StudentCourseDetailsModel stud
            ,int lastInd,bool lastcorrect)
        {
            if(quiz.Questions.Length==stud.Attempted.Count)
                return -1;

            var remaining = new List<QuestionModel>();
            for(int i=0;i<quiz.Questions.Length;i++)
            {
                if (stud.Attempted.Contains(i))
                    continue;
                remaining.Add(quiz.Questions[i]);
            }

            QuestionModel q;
            if(lastcorrect)
                q=remaining.Where(x=>x.Difficulty>=quiz.Questions[lastInd].Difficulty).First();
            else
                q = remaining.Where(x => x.Difficulty <= quiz.Questions[lastInd].Difficulty).First();

            int ind = -1;
            if(q is null)
                q= remaining[0];
            ind = Array.IndexOf(quiz.Questions, q);
            return ind;
        }
        private async Task UpdateLeaderboard(string uid,string courseId,StudentCourseDetailsModel data)
        {
            await Db.Collection(courseId+"_Scores").Document(uid).SetAsync(
                new ScoreModel { Points = data.PointsObtained, Xp = data.XpObtained, Uid = uid });
        }
        private bool EqualAns(List<int> A,List<int> B)
        {
            var firstNotSecond = A.Except(B).ToList();
            var secondNotFirst = B.Except(A).ToList();
            return !firstNotSecond.Any() && !secondNotFirst.Any();

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
