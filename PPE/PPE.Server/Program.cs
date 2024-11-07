using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PPE.BLL;
using PPE.Core;
using PPE.DataModel;
using PPE.Model.Shared;
using PPE.WebCore;

var builder = WebApplication.CreateBuilder(args);
ConfigManager.Builder = builder;
builder.Logging.AddLog4netExt();
builder.Services.AddHttpContextAccessor();
builder.Host.AddAutofacRegister();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<CommonDbContext>(options =>
    options.UseMySQL(connectionString));

builder.Services.AddDbContext<CompleteDbContext>(options =>
options.UseMySQL(connectionString));

builder.Services.AddDbContext<LearnDbContext>(options =>
options.UseMySQL(connectionString));

builder.Services.AddDbContext<SystemLogDbContext>(options => options.UseMySQL(ConfigManager.Instance().GetConnectionString("SystemLogConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection")!;
    options.InstanceName = "RedisInstance";
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod();
    });
});

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password = new PasswordOptions
    {
        RequireDigit = false,
        RequiredLength = 5,
        RequiredUniqueChars = 1,
        RequireLowercase = false,
        RequireNonAlphanumeric = false,
        RequireUppercase = false,
    };
    options.Stores.MaxLengthForKeys = 128;
});

builder.Services.Configure<PasswordHasherOptions>(options =>
{
    options.IterationCount = 150_000;
});

// builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
//     .AddIdentityCookies()
//     .ApplicationCookie!.Configure(opt => opt.Events = new CookieAuthenticationEvents()
//     {
//         OnRedirectToLogin = ctx =>
//         {
//             ctx.Response.StatusCode = 401;
//             return Task.CompletedTask;
//         }
//     })
//     .AddJwtBearer();

builder.Services.AddAuthorizationBuilder();

//builder.Services.AddIdentityCore<Base_User>()
//    .AddEntityFrameworkStores<CommonDbContext>()
//    .AddRoles<Base_Role>()
//    .AddUserManager<UserManager>()
//    .AddRoleManager<RoleManager>()
//    .AddSignInManager<SignInManager>()
//    .AddErrorDescriber<OperationErrorDescriber>()
//    .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
//    .AddApiEndpoints();

builder.Services.AddIdentityApiEndpoints<Base_User>()
    .AddEntityFrameworkStores<CommonDbContext>()
    .AddRoles<Base_Role>()
    .AddUserManager<UserManager>()
    .AddRoleManager<RoleManager>()
    .AddSignInManager<SignInManager>()
    .AddErrorDescriber<OperationErrorDescriber>()
    .AddClaimsPrincipalFactory<UserClaimsPrincipalFactory>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<DefaultValueParameterFileter>();
    options.AddSecurityDefinition("JWTBearer", new OpenApiSecurityScheme()
    {
        Description = "������Token����ʽΪ Bearer xxxxxxxx ��ע���м�����пո�",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {scheme,Array.Empty<string>()}
                });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();

app.MapSwagger().RequireAuthorization();
app.UseAuthorization();

//app.MapIdentityApiExt();

app.MapControllers();

app.Run();
