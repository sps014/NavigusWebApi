using System;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using NavigusWebApi.Manager;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


const string Key = "Navigus Secure Key For JWT";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(x => x.SuppressModelStateInvalidFilter = true);

//add firebase database
var db = ConfigureFirebase();
builder.Services.AddSingleton(d => db);
builder.Services.AddSingleton(a => FirebaseAuth.DefaultInstance);
builder.Services.AddRazorPages();

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

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();


app.MapControllers();
app.MapRazorPages();
app.MapFallbackToFile("index.html");


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