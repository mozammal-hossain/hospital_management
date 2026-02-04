# Phase-4 Marking Report — Hospital Management API

**Reviewer perspective:** Flutter & .NET specialist  
**Your context:** .NET beginner, 3 years of Flutter experience  
**Phase assessed:** Phase-4 — Database (SQLite + EF Core)

**Updated report is shown first.**

---

# Current report (latest assessment — re-judgement)

**Phase assessed:** Phase-4 — Database connection & persistent storage (current state)

---

## Summary

| Criterion                     | Expected (Phase-4)                                        | Status  | Notes                                                                                                   |
| ----------------------------- | --------------------------------------------------------- | ------- | ------------------------------------------------------------------------------------------------------- |
| NuGet packages                | EF Core Sqlite + Design                                   | ✅ Done | `Microsoft.EntityFrameworkCore.Sqlite` 10.0.2, `Design` 10.0.2                                          |
| DbContext                     | DbSet&lt;Patient&gt;, constructor with options, config    | ✅ Done | `Data/PatientDbContext.cs`; ToTable, HasKey, column config in OnModelCreating                           |
| Patient model & table mapping | Id/key, columns mapped                                    | ✅ Done | Patient unchanged; OnModelCreating configures table & column types                                      |
| Migrations                    | InitialCreate (or first migration) + database update      | ✅ Done | `Migrations/` present: `InitialCreate` creates Patients table; `hospital.db` has schema applied         |
| Database repository           | Class implementing IPatientRepository with DbContext CRUD | ✅ Done | `PatientRepository` uses `PatientDbContext`; GetAll/GetById/Add/Update/Delete                           |
| Connection string             | appsettings.json DefaultConnection                        | ✅ Done | `"DefaultConnection": "Data Source=hospital.db"`                                                        |
| Program.cs                    | AddDbContext + AddScoped&lt;IPatientRepository, …&gt;     | ✅ Done | AddDbContext&lt;PatientDbContext&gt;(UseSqlite); AddScoped&lt;IPatientRepository, PatientRepository&gt; |
| DI switch                     | Use DB repository instead of in-memory                    | ✅ Done | Single repository is now DB-backed                                                                      |
| Controller unchanged          | No change to endpoints                                    | ✅ Done | PatientsController unchanged; uses IPatientRepository                                                   |
| Runnable app                  | dotnet run succeeds                                       | ✅ Done | Build: **0 errors, 0 warnings**                                                                         |

---

## Detailed Feedback

### What you did well

1. **DbContext** — `PatientDbContext` in `Data/PatientDbContext.cs` correctly inherits `DbContext`, exposes `DbSet<Patient> Patients`, and takes `DbContextOptions<PatientDbContext>` in the constructor. `OnModelCreating` configures the table name, key, and column types (e.g. `HasMaxLength`, `HasColumnType`) — good for schema control.

2. **Database repository implementation** — You repurposed `PatientRepository` to use `PatientDbContext` instead of adding a separate `PatientDbRepository`. All five methods are correctly implemented: **GetAll** → `_context.Patients.ToList()`; **GetById** → `FirstOrDefault(p => p.Id == id)`; **Add** → `Add(patient)`, `SaveChanges()`, return patient (EF Core will set `Id` after save for SQLite); **Update** → find existing, copy properties, `Update(existingPatient)`, `SaveChanges()`; **Delete** → find, `Remove(patient)`, `SaveChanges()`. Behaviour matches the Phase-4 spec.

3. **Configuration** — Connection string in `appsettings.json` and `Program.cs` registration with `AddDbContext<PatientDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")))` and `AddScoped<IPatientRepository, PatientRepository>()` are correct. Controller is unchanged and still uses `IPatientRepository`; DI now resolves to the DB-backed implementation.

4. **Packages** — EF Core Sqlite and Design are present in the project file; build is clean (0 errors, 0 warnings).

### What's missing or needs improvement

- **Nothing blocking.** Migrations are now in place: `InitialCreate` creates the `Patients` table with Id (autoincrement), FullName, DateOfBirth, Gender, PhoneNumber, Email, AdmittedAt, IsDischarged; `hospital.db` has the schema applied. Phase-4 schema requirement is met.

- **Optional (spec suggestion)** — The spec suggested a **new** class `PatientDbRepository` and keeping the in-memory `PatientRepository` for future unit tests. You replaced the in-memory implementation in place. That is a valid choice and keeps the codebase simpler; only note that if you later want an in-memory mock for tests, you would need to reintroduce it (e.g. a separate class and switch via configuration).

### Optional notes

- **SQLite column types** — In `OnModelCreating` you used `HasColumnType("date")` for `AdmittedAt` and `HasColumnType("bit")` for `IsDischarged`. SQLite does not have native `date` or `bit` types (it uses INTEGER and TEXT). The EF Core SQLite provider usually maps these; if you see odd behaviour, you can rely on default mapping or use SQLite-friendly types.
- **Id on Add** — The controller creates a `Patient` without setting `Id`; after `Add` + `SaveChanges()`, EF Core sets the generated Id on the same instance, so `CreatedAtAction(..., patient)` correctly returns the new id. Good.

---

## Flutter ↔ .NET mapping (current state)

| Phase-4 idea        | Flutter / Dart               | .NET / C# (your project)                                        |
| ------------------- | ---------------------------- | --------------------------------------------------------------- |
| Database package    | sqflite / drift              | EF Core Sqlite + Design ✅                                      |
| Database connection | openDatabase(path)           | `DbContext` + `UseSqlite(connectionString)` ✅                  |
| Table ↔ model       | Table map / Drift table      | `DbSet<Patient>` + OnModelCreating ✅                           |
| Schema update       | version / migration          | Migrations ✅ (InitialCreate + database update)                 |
| Repository storage  | List → DB helper             | `PatientRepository` → DbContext (replaced in place) ✅          |
| DI switch           | Change Provider registration | `AddScoped<IPatientRepository, PatientRepository>` (DB impl) ✅ |
| Endpoints           | Unchanged                    | Controller unchanged ✅                                         |

---

## Rating: **9.5 / 10**

- **9.5** — All Phase-4 deliverables are in place: NuGet packages, DbContext, Patient mapping, **migrations (InitialCreate created and applied)**, DB-backed repository, connection string, Program.cs registration, controller unchanged. Build: 0 errors, 0 warnings. The `Patients` table exists in `hospital.db` and the app will persist data across restarts.
- **Why not 10:** Small deduction for not keeping a separate in-memory repository as suggested in the spec for future unit tests; otherwise Phase-4 is complete.
- **Why not lower:** Every required criterion is met; migrations gap from the previous assessment has been closed.

---

## What to do next

Phase-4 is **complete**. Move on to **Phase-5** (validation & polish). Optionally, reintroduce an in-memory `IPatientRepository` implementation for unit testing and switch via configuration if desired.

---

## Encouragement

You’ve wired the database layer correctly and closed the migrations gap: DbContext, repository methods, connection string, DI, and schema (InitialCreate + applied) all match the spec. Same idea as in Flutter — swap the backing store (list → database) without changing the API. Phase-4 is complete.

Good luck with Phase-5 (validation & polish).

---

---

## Previous assessment (history)

_Below: Latest assessment before re-judgement (when migrations were still missing)._

---

### Summary (previous)

| Criterion            | Status     | Notes                                                      |
| -------------------- | ---------- | ---------------------------------------------------------- |
| Migrations           | ❌ Missing | No `Migrations/` folder; schema not created via migrations |
| (All other criteria) | ✅ Done    | Same as current summary above                              |

---

### Rating (previous): **7.5 / 10**

- **7.5** — All Phase-4 code deliverables were in place except migrations: no `Migrations/` folder and no evidence that the database schema had been created via `dotnet ef migrations add` and `dotnet ef database update`, so the app was likely to fail at runtime when accessing the database.
- After adding and applying the first migration, Phase-4 became complete; rating updated to **9.5/10**.

---

### What to do next (previous)

1. **Add and apply migrations** — `dotnet ef migrations add InitialCreate` then `dotnet ef database update`. **Done** in current state.
2. **(Optional)** Keep or reintroduce an in-memory `IPatientRepository` implementation for unit testing.
