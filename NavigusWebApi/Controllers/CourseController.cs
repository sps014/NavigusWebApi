using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class CourseController : ControllerBase
    {
        public IHttpContextAccessor Accessor { get; }
        public FirestoreDb Db { get; }

        public const string ListCollectionName = "COURSES";
        

        public CourseController(IHttpContextAccessor accessor,FirestoreDb db)
        {
            Accessor = accessor;
            Db = db;
        }

        [Authorize(Roles ="Student,Teacher")]
        [HttpGet("list")]
        public async Task<IActionResult> List()
        {
            try
            {
                var records= Db.Collection(ListCollectionName).ListDocumentsAsync();
                var courses = new List<CourseModel>();
            
                await foreach (var r in records)
                {
                    var snap = await r.GetSnapshotAsync();
                    if (snap.Exists)
                    {
                        courses.Add(snap.ConvertTo<CourseModel>());
                    }
                }
                return Ok(courses);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            
        }
        
        [Authorize(Roles ="Teacher")]
        [HttpPost("add")]
        public async Task<IActionResult> Add(CourseModel courseModel)
        {
            //get creator GUID
            courseModel.Creator = Accessor.GetUid();
            
            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace((courseModel.CourseId)))
                return BadRequest("Course Id cant be null");

            try
            {
                //checking if course already exists in db
                var rec = await Db.Collection(ListCollectionName).Document(courseModel.CourseId).GetSnapshotAsync();
                
                //cant add if already exist
                if (rec.Exists)
                    return BadRequest($"Course : {courseModel.CourseId} already exists");
                
                //add to db
                await Db.Collection(ListCollectionName).Document(courseModel.CourseId).SetAsync(courseModel);
                
                return Ok($"Added course : {courseModel.CourseId}");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Roles ="Teacher")]
        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(string id,[FromBody]CourseModel courseModel)
        {
            //get creator GUID
            courseModel.Creator = Accessor.GetUid();
            
            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace((id)))
                return BadRequest("Course Id cant be null, please specify existing course id in route");
            
            //checking if course id is non empty
            if (string.IsNullOrWhiteSpace((courseModel.CourseId)))
                return BadRequest("new Course Id cant be null");

            try
            {
                //checking if course already exists in db
                var rec = await Db.Collection(ListCollectionName).Document(id).GetSnapshotAsync();
                
                // if not already exist
                if (!rec.Exists)
                    return BadRequest($"Course : {id} not exists, please add new");

                var prev = rec.ConvertTo<CourseModel>();
                
                //if new course does not have new quiz add previous quiz
                courseModel.Quiz ??= prev.Quiz;
                
                //delete previous and add new 
                await Db.Collection(ListCollectionName).Document(id).DeleteAsync();
                
                //add to db
                await Db.Collection(ListCollectionName).Document(courseModel.CourseId).SetAsync(courseModel);
                
                return Ok($"Edited course : {courseModel.CourseId} from {id}");

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [Authorize(Roles = "Teacher,Student")]
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

                //if not  exist
                if (!rec.Exists)
                    return BadRequest($"Course : {id} not exists, please add new");

                var prev = rec.ConvertTo<CourseModel>();


                return Ok(prev);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
