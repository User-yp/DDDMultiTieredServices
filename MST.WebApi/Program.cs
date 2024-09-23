using CommonInitializer;
using Commons.Options;
using MTS.WebApi.Requset_Validator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//ApplicationBuilder
builder.ConfigureDbConfiguration();
//WebApplicationBuilder
builder.ConfigureExtraServices(new InitializerOptions
{
    EventBusQueueName = "MST.WebApi",
    LogFilePath = "e:/Log/MTS-Service.log"
});

builder.Services.AddControllers();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MST.WebApi", Version = "v1" });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddScoped<ValidatorControl>();
//add MediatR service
//builder.Services.AddMediatR(assemblies);
// add FluentValidation
//builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "OrderService.WebAPI v1"));
}

app.UseDefault();

app.MapControllers();

app.Run();
