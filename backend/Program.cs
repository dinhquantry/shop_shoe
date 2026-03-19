using backend.Extensions;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();
builder.Services.AddCoreApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowNextJsApp");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.MapControllers();

app.Run();