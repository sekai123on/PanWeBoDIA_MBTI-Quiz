# MBTI API — C# / .NET 8 Backend

## Project structure

```
MbtiApi/
├── Domain/
│   ├── Entities/          Question, TestSession, Answer, TestResult
│   └── Enums/             Dimension, DimensionChoice, QuestionLanguage
│
├── Application/
│   ├── Interfaces/        IQuestionRepository, ISessionRepository,
│   │                      IResultRepository, IQuizService, IResultService
│   ├── Services/          QuizService, ResultService, MbtiDescriptions
│   └── DTOs/
│       ├── Request/       StartSessionRequest, SubmitAllAnswersRequest
│       └── Response/      QuestionResponse, SessionResponse, ResultResponse
│
├── Infrastructure/
│   ├── Data/              AppDbContext  (EF Core + PostgreSQL)
│   └── Repositories/      QuestionRepository, SessionRepository, ResultRepository
│
└── API/
    ├── Controllers/       QuizController
    ├── Middleware/        ExceptionMiddleware
    ├── Program.cs
    └── appsettings.json
```

## Prerequisites

- .NET 8 SDK
- PostgreSQL 14+
- (Optional) Docker for local PostgreSQL

## Quick start

```bash
# 1. Clone / open the project
cd MbtiApi

# 2. Set your PostgreSQL connection string in appsettings.json
#    or via environment variable:
export ConnectionStrings__Postgres="Host=localhost;Port=5432;Database=mbti_db;Username=postgres;Password=yourpassword"

# 3. Restore packages
dotnet restore

# 4. Apply EF Core migrations
dotnet ef migrations add InitialCreate --project MbtiApi.csproj
dotnet ef database update

# 5. Run
dotnet run --project MbtiApi.csproj
```

Swagger UI: https://localhost:5001/swagger

## REST Endpoints (v1)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET    | /api/v1/quiz/questions?lang=en | Get all questions (en or my) |
| POST   | /api/v1/quiz/sessions | Start a new test session |
| POST   | /api/v1/quiz/sessions/{id}/submit | Submit answers → receive MBTI type |
| GET    | /api/v1/quiz/sessions/{id}/result | Fetch result for a session |

## Dependency injection wiring

```
IQuestionRepository → QuestionRepository
ISessionRepository  → SessionRepository
IResultRepository   → ResultRepository
IQuizService        → QuizService
IResultService      → ResultService
```

## MBTI scoring logic

Each question belongs to one of four `Dimension` axes:

| Axis | A-pole | B-pole |
|------|--------|--------|
| EI   | E (Extraversion) | I (Introversion) |
| SN   | S (Sensing) | N (iNtuition) |
| TF   | T (Thinking) | F (Feeling) |
| JP   | J (Judging) | P (Perceiving) |

Score = % of answers choosing A-pole per axis.
≥ 50% → A-pole letter, < 50% → B-pole letter.

## Languages supported

- `en` — English
- `my` — Myanmar (Burmese)

Each question stores bilingual text fields (`TextEn`, `TextMy`, `OptionAEn`, `OptionAMy`, etc.).
The API localises at query time based on the `lang` query param.

## Next steps

- [ ] Seed question bank (60 questions, 15 per dimension, bilingual)
- [ ] Add JWT authentication for user sessions
- [ ] React Native mobile app
- [ ] Next.js website frontend
