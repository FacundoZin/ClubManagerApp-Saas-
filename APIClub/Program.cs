using APIClub.Common;
using APIClub.Data;
using APIClub.Domain.AlquilerArticulos;
using APIClub.Domain.AlquilerArticulos.Repositories;
using APIClub.Domain.Background;
using APIClub.Domain.GestionSocios;
using APIClub.Domain.GestionSocios.Repositories;
using APIClub.Domain.ReservasSalones;
using APIClub.Domain.ReservasSalones.Repositories;
using APIClub.Domain.Notificaciones;
using APIClub.Repositorio;
using APIClub.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using APIClub.Domain.PaymentsOnline;
using APIClub.Domain.PaymentsOnline.Repository;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=club.db";


builder.Services.AddDbContext<AppDbcontext>(options =>
    options.UseSqlite(connectionString));

// Configurar WhatsApp
builder.Services.Configure<WhatsAppConfig>(
    builder.Configuration.GetSection("WhatsApp"));


// Registrar HttpClients
builder.Services.AddHttpClient<NotifyService>((sp,client) =>
{
    var config = sp.GetRequiredService<IOptions<IConfiguration>>().Value;

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

//registrar servicios
builder.Services.AddScoped<ISociosManagmentService,SociosManagmentService>();
builder.Services.AddScoped<ICuotasService,CuotasService>();
builder.Services.AddScoped<IReservasServices,ReservasServices>();
builder.Services.AddScoped<ICobranzasServices,CobranzasService>();
builder.Services.AddScoped<IManagmentArticulosService,ManagmentArticulosService>();
builder.Services.AddScoped<IAlquilerArticulosService ,AlquilerArticulosService>();
builder.Services.AddScoped<INotifyService,NotifyService>(); 
builder.Services.AddScoped<IPaymentService,PaymentService>();
builder.Services.AddScoped<IPaymentTokenService,PaymentTokenService>();
builder.Services.AddScoped<IMercadoPagoService,MPService>();

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


// confiugracion cors.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
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

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
