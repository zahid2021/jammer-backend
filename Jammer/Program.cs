using Jammer.DBContext;
using Jammer.Utills;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Jammer.UserModule.Repositories;
using Jammer.UserModule.Repositories.InterFace;
using Jammer.ProductModule.Repositories;
using Jammer.ProductModule.Repositories.InterFace;
using Jammer.CouponModule.Repositories.InterFace;
using Jammer.CouponModule.Repositories;
using Jammer.CartModule.Repositories.InterFace;
using Jammer.CartModule.Repositories;
using Jammer.OrderModule.Repositories.InterFace;
using Jammer.OrderModule.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on all IPs and port 55048
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(55048); 
});



builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews();
builder.Services.AddControllers().AddNewtonsoftJson(option =>
option.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});
builder.Services.AddControllers();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IDapperContext, DapperContext>();
builder.Services.AddSingleton<ITokenHelper, TokenHelper>();
builder.Services.AddSingleton<IDeliveryBoyRepositories, DeliveryBoyRepositories>();
builder.Services.AddSingleton<IProductRepository, ProductRepositories>();
builder.Services.AddSingleton<ICategoryRepository, CategoryRepository>();
builder.Services.AddSingleton<IReviewRepository, ReviewRepository>();
builder.Services.AddSingleton<ICartRepositories, CartRepositories>();
builder.Services.AddSingleton<IBannerRepository, BannerRepository>();
builder.Services.AddSingleton<IWishListRepositories, WishListRepositories>();
builder.Services.AddSingleton<IOrderRepositories, OrderRepositories>();
 
builder.Services.AddSingleton<IAddressRepositories, AddressRepositories>();

builder.Services.AddSingleton<ICouponRepositories, CouponRepositories>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var jwtIssuer = builder.Configuration.GetSection("Jwt:Issuer").Get<string>();
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Get<string>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = false,
         ValidateIssuerSigningKey = true,
         ValidIssuer = jwtIssuer,
         ValidAudience = jwtIssuer,
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
     };
 });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});
builder.Services.AddSingleton<TokenHelper>();
builder.Services.AddSingleton<DapperContext>();
var app = builder.Build();
// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAllOrigins");

app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.Run();
