using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NavigusWebApi.Manager;
using System.Text;

const string Key = "Navigus Secure Key For JWT";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "NavigusWebApi", Version = "v1" });
});

//add firebase database
var db = ConfigureFirebase();
builder.Services.AddSingleton(d => db);
builder.Services.AddSingleton(a => FirebaseAuth.DefaultInstance);


//add jwt middleware
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x=>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey=true,
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key)),
        ValidateIssuer=false,
        ValidateAudience=false
    };
});

builder.Services.AddSingleton<IJwtAuthManager>(new JwtAuthManager(Key));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "NavigusWebApi v1"));
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();



static FirestoreDb ConfigureFirebase()
{
    var path = $"{AppDomain.CurrentDomain.BaseDirectory}firebase.json";

    var credentials = new AppOptions()
    {
        Credential = GoogleCredential.FromFile(path)
    };
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

    FirebaseApp.Create(credentials);
    return FirestoreDb.Create("naviguscloud");

}