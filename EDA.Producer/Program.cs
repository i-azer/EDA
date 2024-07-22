using Confluent.Kafka;
using EDA.Producer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Kafka Topic
var producerTopic = builder.Configuration.GetValue<string>("Topic");
builder.Services.AddSingleton(producerTopic!);

var producerConfig = new ProducerConfig();
builder.Configuration.Bind("ProducerConfig", producerConfig);
builder.Services.AddSingleton<ProducerConfig>(producerConfig);


builder.Services.AddScoped<IProducerService, ProducerService>();


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

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();


app.MapPost("/send", async (httpCtx) => {
    var message = "";
    using (StreamReader sr = new StreamReader(httpCtx.Request.Body))
    {
        message = await sr.ReadToEndAsync();
    }

    var producerSvc = httpCtx.RequestServices.GetRequiredService<IProducerService>();
    await producerSvc.SetTopic(producerTopic!);
    try
    {
        if (message == null)
        {
            throw new InvalidDataException("Must have a message body");
        }
        await producerSvc.Send(message?.ToString());
    }
    catch (Exception ex)
    {
        await httpCtx.Response.BodyWriter.WriteAsync(System.Text.Encoding.ASCII.GetBytes(ex.Message));
        httpCtx.Response.StatusCode = 500;
    }
    httpCtx.Response.StatusCode = 200;
});


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
