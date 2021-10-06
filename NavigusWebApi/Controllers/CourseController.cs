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
    }
}
