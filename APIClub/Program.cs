using APIClub.Domain.AlquilerArticulos;
using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.ReservasSalones;
using APIClub.Domain.ReservasSalones.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using APIClub.Domain.PaymentsOnline;
using APIClub.Domain.PaymentsOnline.Repository;
using APIClub.Domain.GestionSocios.Validations;
using APIClub.Infrastructure;
using APIClub.Infrastructure.Persistence.Repositorio;
using APIClub.Infrastructure.Persistence.Data;
using APIClub.Application.Services;
using APIClub.Application.Common;
using APIClub.Application.Validators;

using APIClub.Domain.Auth;
using APIClub.Domain.Auth.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using APIClub.Domain.Notificaciones.Infra;
using APIClub.Domain.Notificaciones.Models;
using APIClub.Domain.Notificaciones.Services;

var builder = WebApplication.CreateBuilder(args);

// Hardcoded for safety against environment variables override
var connectionString = "Data Source=club.db";

builder.Services.AddDbContext<AppDbcontext>(options =>
    options.UseSqlite(connectionString));

// Configurar WhatsApp
builder.Services.Configure<WhatsAppConfig>(
    builder.Configuration.GetSection("WhatsApp"));

// Registrar HttpClients
builder.Services.AddHttpClient<IWhatsappService,WhatsapService>((sp,client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri("https://graph.facebook.com/");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue("Bearer", config["WhatsApp:AccessToken"]);
    client.DefaultRequestHeaders.Accept.Add(
        new MediaTypeWithQualityHeaderValue("application/json"));
});

builder.Services.AddHttpClient<IMercadoPagoService, MPService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    client.BaseAddress = new Uri("https://api.mercadopago.com");
    client.DefaultRequestHeaders.Authorization =
        new AuthenticationHeaderValue(
            "Bearer",
            config["MercadoPago:AccessToken"]
        );
});

// registrar servicios
builder.Services.AddScoped<ISociosManagmentService,SociosManagmentService>();
builder.Services.AddScoped<ICuotasService,CuotasService>();
builder.Services.AddScoped<IReservasServices,ReservasServices>();
builder.Services.AddScoped<ICobranzasServices,CobranzasService>();
builder.Services.AddScoped<IManagmentArticulosService,ManagmentArticulosService>();
builder.Services.AddScoped<IAlquilerArticulosService ,AlquilerArticulosService>();
builder.Services.AddScoped<IPaymentService,PaymentService>();
builder.Services.AddScoped<IPaymentTokenService,PaymentTokenService>();
builder.Services.AddScoped<IMercadoPagoService,MPService>();
builder.Services.AddScoped<INotificationsService, NotificacionsService>();

// AUTENTICACIÓN
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IUsuariosRepository, UsuariosRepository>();

//OTROS
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<ISocioIntegrityValidator, SocioIntegrityValidator>();

// registrar repositorios
builder.Services.AddScoped<ISocioRepository,SociosRepository>();
builder.Services.AddScoped<ICuotaRepository,CuotaRepository>();
builder.Services.AddScoped<IReservasRepository,ReservasRepository>();
builder.Services.AddScoped<IArticuloRepository,ArticuloRepository>();
builder.Services.AddScoped<IAlquilerRepository,AlquilerRepository>();
builder.Services.AddScoped<IitemAlquilerRepository, ItemsAlquilerRepository>();
builder.Services.AddScoped<IPaymentTokenRepository,PaymentTokenRepository>();


// Configuración de JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"]; 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
        
        // Evento para leer el token desde la cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["auth_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });


// confiugracion cors.
builder.Services.AddCors(options =>
{        
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Ajustar según puerto frontend
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials(); // Permitir cookies
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Usar la política CORS que permite credenciales
app.UseCors("AllowFrontend");

app.UseAuthentication(); // AGREGAR ESTO ANTES DE AUTHORIZATION
app.UseAuthorization();

app.MapControllers();

app.Run();
