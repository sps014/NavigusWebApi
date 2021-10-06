using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NavigusWebApi.Models;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;
using NavigusWebApi.Manager;
using Microsoft.AspNetCore.Authorization;

namespace NavigusWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        //Firebase Auth instance (via dependency injection)
        public FirebaseAuth Auth { get; set; }

        //Firebase DB instance (via dependency injection)
        public FirestoreDb Db { get; set; }

        //Get Singlton JWT manager instance
        public IJwtAuthManager JwtManager { get; set; }

        //Name of Collection in firebase Database
        public const string CollectionName = "UIDs";

        //Dependecy Injection for Firebase Auth and Db
        public AuthController(FirebaseAuth auth,FirestoreDb db,IJwtAuthManager jwtAuth)
        {
            Auth = auth;
            Db = db;
            JwtManager = jwtAuth;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserModel user)
        {
            //check is username and password is null or not
            if (user.Email is null || user.Password is null)
                return BadRequest("User name and password can't be null");

            //procceed to fetch user from firestore DB
            try
            {
                //get user by email
                var u = await Auth.GetUserByEmailAsync(user.Email);

                //get userinfo in database from uid 
                var dbdata= await Db.Collection(CollectionName)
                    .Document(u.Uid).GetSnapshotAsync();

                //if user exist get userinfo
                if(dbdata.Exists)
                {
                    var val = dbdata.ConvertTo<UserInfo>();

                    //check if password matches
                    if (val.Password != user.Password)
                        return BadRequest("Invalid Password");

                    //generate token with uid
                    var token = JwtManager.Authenticate(u.Uid,val.Role.ToString());

                    return Ok($"{user.Email} logged in Successfully  with role {val.Role}\ntoken={token}");
                }
                else
                {
                    return BadRequest($"Failed to login, user does not exists");
                }


            }
            catch (Exception e)
            {
                return BadRequest($"Login Failed \n{e.Message}");
            }
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] NewUserModel user)
        {
            //if username and password is not null continue
            if (user.Email is null || user.Password is null)
                return BadRequest("User name and password can't be null");

            //if role is Teacher with value 1 or student with value 0 continue
            else if (user.Role != Roles.Student && user.Role != Roles.Teacher)
                return BadRequest("Invalid User Role, use value 1 for Teacher , 0 for student");

            try
            {
                //create user in firebase auth portal
                var u=await Auth.CreateUserAsync(
                    new UserRecordArgs { Email = user.Email, Password = user.Password });

                //write user info to database
                await Db.Collection(CollectionName).Document(u.Uid)
                    .SetAsync(new UserInfo{Role=user.Role,Password=user.Password,UserName=user.UserName,Email=user.Email});

                return Ok($"created user {user.Email} with role {user.Role}");

            }
            catch(Exception e)
            {
                return BadRequest($"Signup Failed \n{e.Message}");
            }
        }

    }
}
