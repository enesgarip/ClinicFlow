using ClinicFlow.Infrastructure.Persistence;
using ClinicFlow.Infrastructure.Services.Patients;
using Microsoft.EntityFrameworkCore;
using ClinicFlow.API.Infrastructure.Tenancy;
using ClinicFlow.Application.Abstractions;
using ClinicFlow.Application.Patients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ClinicFlowDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, HeaderTenantProvider>();
builder.Services.AddScoped<IPatientService, PatientService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
