using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NavigusWebApp.Shared.Models;

namespace NavigusWebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        public IHttpContextAccessor Accessor { get; }
        public FirestoreDb Db { get; }

        public const string ListCollectionName = "_Scores";

        public LeaderboardController(IHttpContextAccessor accessor, FirestoreDb db)
        {
            Accessor = accessor;
            Db = db;
        }
        [HttpGet("get/{courseId}")]
        public async Task<IActionResult> Get(string courseId)
        {
            if (string.IsNullOrWhiteSpace(courseId))
                return BadRequest($"{courseId} is null");

            var snap = await Db.Collection(CourseController.ListCollectionName).Document(courseId).GetSnapshotAsync();
            if (!snap.Exists)
                return BadRequest($"No Course with id : {courseId} found");

            try
            {
                var query = await Db.Collection(courseId + ListCollectionName).OrderByDescending("Xp")
                    .Limit(50).GetSnapshotAsync();

                var list=query.ToArray();
                var res = new List<ScoreModel>();
                foreach (var item in list)
                {
                    if(item.Exists)
                    {
                        var r = item.ConvertTo<ScoreModel>();
                        res.Add(r);
                    }
                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
