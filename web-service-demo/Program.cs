using Microsoft.OpenApi.Models;

// WebApplication - это преднастроенная версия IHost для веб-приложений ASP.NET
var builder = WebApplication.CreateBuilder(args);

// Вывод на консоль текущего окружения, которое было указано в конфигурации приложения.
Console.WriteLine($"ASPNETCORE_ENVIRONMENT:   {builder.Configuration["ASPNETCORE_ENVIRONMENT"]}");

// Настройка Kestrel - HTTP-инфраструктуры, которая будет использована в качестве сервера.
builder.WebHost.ConfigureKestrel((context, options) =>
{
    // Получение настроек Kestrel из конфигурации приложения.
    var kestrelSettings = context.Configuration.GetSection("Kestrel");
    
    // Конфигурирование Kestrel по полученным настройкам.
    options.Configure(kestrelSettings);
});

// Добавление в сервисы приложения экземпляров, которые предоставляют функции API и Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Настройка заголовка Swagger
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Generic Host - Minimal API Demo", Version = "v1" });
});

// Создание веб-приложения по конфигурации, которая была построена ранее.
var app = builder.Build();

// Включение Swagger и UI при старте приложения.
app.UseSwagger();
app.UseSwaggerUI();

// Создание списка с сообщениями для демонстрации функции API.
List<string> messages = new List<string>() { "Привет, Generic Host!" };

// Настройка роутинга веб-приложения
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("api/", () => messages);
app.MapPost("api/", (string message) =>
{
    messages.Add(message);
    return messages;
});
app.MapDelete("api/{id}", (int id) =>
{
    messages.RemoveAt(id);
    return messages;
});
app.MapPut("api/{id}", (int id, string message) =>
{
    messages[id] = message;
    return messages;
});

// Запуск веб-приложения.
app.Run();