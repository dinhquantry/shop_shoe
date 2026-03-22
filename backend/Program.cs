using backend.Extensions; 
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddApiInfrastructure(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCoreApplicationServices();

var app = builder.Build();

await backend.Data.AppDbSeeder.SeedDefaultDataAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseCors("AllowNextJsApp");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 

app.Run();
