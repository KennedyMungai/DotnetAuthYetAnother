using System.Text;
using DotnetAuthYetAnother.Api.Configuration;
using DotnetAuthYetAnother.Api.Data;
using DotnetAuthYetAnother.Api.Repositories.FormulaOneRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IFormulaOneCRUDService, FormulaOneCRUDService>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

var key = builder.Configuration.GetSection("JwtConfig").GetSection("Secret").Value;
var tokenValidationParameters = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
    ValidateIssuer = false, //TODO: Change to true on deployment
    ValidateAudience = false, //TODO Change to true on deployment
    RequireExpirationTime = false, //TODO Change to true on deployment
    ValidateLifetime = true
};

builder.Services.AddDbContext<FormulaOneDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnString"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnString"))
    );
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(jwt =>
{
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = tokenValidationParameters;
});

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedEmail = false;
}).AddEntityFrameworkStores<FormulaOneDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
