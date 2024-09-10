using Commons;
using Infrastructure;
using MTS.Infrastructure;
using MTS.WebApi;
using FluentValidation.AspNetCore;
using FluentValidation;
using MTS.WebApi.Requset_Validator;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var assemblies = ReflectionHelper.GetAllReferencedAssemblies();

builder.Services.AddDbContext<BaseDbContext>();
//add MediatR service
builder.Services.AddMediatR(assemblies);
//add custom services
builder.Services.AddBaseServies();
// add FluentValidation
//builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidation(assemblies);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
