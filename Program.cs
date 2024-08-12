using SistemaVigilanciaBCPApi.Hubs;
//using SistemaVigilanciaBCPApi.SubscribeTableDependencies;
using Microsoft.EntityFrameworkCore;
using APIBCPSistemaVigilancia.Models.BDBCPSistemaVigilancia;
using SistemaVigilanciaBCPApi.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using VideovigilanciaDB.Models.VideovigilanciaBCP;

var builder = WebApplication.CreateBuilder(args);
var key = builder.Configuration.GetValue<string>("JwtSettings:key");
var keyBytes = Encoding.UTF8.GetBytes(key);
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("BCPSistemaVigilancia");
builder.Services.AddHostedService<TareaValidaUsuarioActivo>();
builder.Services.AddControllers();
builder.Services.AddDbContext<BCPSistemaVigilanciaContext>(options => options
                            .UseSqlServer(builder.Configuration.GetConnectionString("BCPSistemaVigilancia")));
builder.Services.AddDbContext<videovigilanciaBCPContext>(options => options
                            .UseSqlServer(builder.Configuration.GetConnectionString("VideovigilanciaBCP")));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ControlUsuarioHub>();
//builder.Services.AddSingleton<SubscribeProductTableDependency>();
builder.Services.AddScoped<ISeguridadService, SeguridadService>();
builder.Services.AddScoped<IAuditoriaIngresosService, AuditoriaIngresosService>();
builder.Services.AddScoped<IAuditoriaActividadesService, AuditoriaActividadesService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ICriptografia, Criptografia>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IAlarma, Alarma>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.MapHub<ControlUsuarioHub>("/UsuarioHub");

//app.UseSqlTableDependency<SubscribeProductTableDependency>(connectionString);

app.Run();
