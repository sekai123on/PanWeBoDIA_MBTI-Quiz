using Microsoft.EntityFrameworkCore;
using MbtiApi.API.Middleware;
using MbtiApi.Application.Interfaces;
using MbtiApi.Application.Services;
using MbtiApi.Infrastructure.Data;
using MbtiApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ── Database (PostgreSQL via Npgsql) ─────────────────────────────────────────
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseNpgsql(
       "Host=localhost;Port=5432;Database=mbti_db;Username=postgres;Password=sasa#123",
        b => b.MigrationsAssembly("MbtiApi")
    ));

// ── Repositories (Interface → Implementation) ────────────────────────────────
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<ISessionRepository,  SessionRepository>();
builder.Services.AddScoped<IResultRepository,   ResultRepository>();

// ── Services ─────────────────────────────────────────────────────────────────
builder.Services.AddScoped<IQuizService,   QuizService>();
builder.Services.AddScoped<IResultService, ResultService>();
builder.Services.AddScoped<IAuthService,   AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
// ── API ───────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "MBTI API", Version = "v1" });
});

// ── CORS (allow mobile + web clients) ────────────────────────────────────────
builder.Services.AddCors(opts => opts.AddDefaultPolicy(p =>
    p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Auto-apply migrations on startup ─────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
