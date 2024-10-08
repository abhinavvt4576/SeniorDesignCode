var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register IHttpClientFactory
builder.Services.AddHttpClient(); // Add this line to register IHttpClientFactory

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS to allow communication from React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // React app runs on this port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

// Apply CORS globally in the middleware pipeline
app.UseCors("AllowReactApp");

app.UseRouting(); // Add routing middleware

// Map controllers to endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // This maps the controllers, including the GenerateController
});

app.Run();
