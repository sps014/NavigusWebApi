using System;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NavigusWebApi.Manager;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http.Headers;
using Microsoft.OpenApi.Models;

const string Key = "Navigus Secure Key For JWT";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name ="Authorization",
        Type=SecuritySchemeType.ApiKey,
        Scheme="Bearer",
        In= ParameterLocation.Header,
        BearerFormat="JWT",
        Description="JWT Auth"
    });
    c.SwaggerDoc("v1", new() { Title = "NavigusWebApi", Version = "v1" });
});

//add firebase database
var db = ConfigureFirebase();
builder.Services.AddSingleton(d => db);
builder.Services.AddSingleton(a => FirebaseAuth.DefaultInstance);


//add jwt middleware
builder.Services.AddHttpContextAccessor();
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

app.MapGet("/",SetupIndexPage);

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

IResult SetupIndexPage()
{
    return Results.Content(@"<h2>Navigus Web Api is running.<h2><br/>
<h4>refer <b> <a href='https://github.com/sps014/NavigusWebApi/blob/main/README.md'>github readme</a> </b>
for more information</h4><br>","text/html");
}