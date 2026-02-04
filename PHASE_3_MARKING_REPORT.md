# Phase-3 Marking Report — Hospital Management API

**Reviewer perspective:** Flutter & .NET specialist  
**Your context:** .NET beginner, 3 years of Flutter experience  
**Phase assessed:** Phase-3 — API Endpoints & Controller

**Updated report is shown first.**

---

# Current report (latest assessment)

**Phase assessed:** Phase-3 — API Endpoints & Controller (current state)

---

## Summary

| Criterion                 | Expected (Phase-3)                                        | Status  | Notes                                                                  |
| ------------------------- | --------------------------------------------------------- | ------- | ---------------------------------------------------------------------- |
| Controllers folder        | `Controllers/` at project root                            | ✅ Done | `Controllers/PatientsController.cs` present                            |
| PatientsController        | `[ApiController]`, `[Route("api/...")]`, `ControllerBase` | ✅ Done | `[Route("api/patients")]` (explicit lowercase)                         |
| Constructor injection     | `IPatientRepository` injected via constructor             | ✅ Done | `public PatientsController(IPatientRepository repository)` (fixed)     |
| GET /api/patients         | GetAll; return all patients; empty array if none          | ✅ Done | `[HttpGet]` → `_repository.GetAll()`                                   |
| GET /api/patients/{id}    | GetById; 404 if not found                                 | ✅ Done | `[HttpGet("{id}")]` → `GetById(id)`; `NotFound()` when null            |
| POST /api/patients        | CreatePatientRequest → Patient → Add; 201 + Location      | ✅ Done | `[FromBody]`, map to Patient, `Add()`, `CreatedAtAction(GetById, ...)` |
| PUT /api/patients/{id}    | UpdatePatientRequest; partial update; 404 if not found    | ✅ Done | GetById → null coalesce from request → `Update()` → `Ok(updated)`      |
| DELETE /api/patients/{id} | Delete by id; 204 No Content or 404                       | ✅ Done | `_repository.Delete(id)` → `NoContent()` or `NotFound()`               |
| Program.cs                | `AddControllers()` and `MapControllers()`                 | ✅ Done | Both added (fixed during review)                                       |
| Runnable / endpoints work | `dotnet run` and routes active                            | ✅ Done | Build: **0 errors, 0 warnings**; endpoints registered                  |

---

## Detailed Feedback

### What you did well

1. **Controller structure** — `PatientsController` inherits `ControllerBase`, has `[ApiController]` and `[Route("api/patients")]`. You used explicit lowercase route as suggested in the spec. Repository is injected and stored in a readonly field.

2. **All five endpoints** — GET all returns `_repository.GetAll()`. GET by id uses `GetById(id)` and returns `NotFound()` when null. POST maps `CreatePatientRequest` to `Patient` (with `AdmittedAt = DateTime.UtcNow`, `IsDischarged = false`), calls `Add()`, and returns `CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient)` for 201 and Location header. PUT loads existing patient, applies partial update with null-coalescing for each field, calls `Update()`, returns `Ok(updatedPatient)`. DELETE returns `NoContent()` on success and `NotFound()` otherwise. Behaviour matches the Phase-3 spec.

3. **DTO usage** — Create uses `CreatePatientRequest` with no Id; Update uses `UpdatePatientRequest` with nullable fields for partial update. Manual null check in Create for required fields; optional validation (e.g. `[Required]`) can be added later.

4. **Defensive handling** — POST and PUT check for repository returning null and return 500; repository currently always returns a value when the entity exists, but the pattern is correct.

### What was fixed during this review

1. **Program.cs** — `AddControllers()` and `MapControllers()` were missing, so controller routes were never registered. Added `builder.Services.AddControllers();` and `app.MapControllers();` so `/api/patients` endpoints are active at runtime.

2. **Controller constructor** — The constructor had no access modifier (defaulting to `private`), which would prevent the DI container from creating the controller. Changed to `public PatientsController(IPatientRepository repository)`.

### Optional notes

- **POST response body** — You use `patient` in `CreatedAtAction(..., patient)`; after `Add()`, the repository sets `patient.Id` on the same reference, so the response body has the correct Id. Using `createdPatient` (the return value of `Add()`) would be equivalent and slightly clearer.
- **Validation** — Phase-3 says validation is optional. You can add `[Required]`, `[EmailAddress]`, etc. on DTOs later; `[ApiController]` will then return 400 for invalid model state.

---

## Flutter ↔ .NET mapping (current state)

| Phase-3 idea         | Flutter / Dart           | .NET / C# (your project)                                     |
| -------------------- | ------------------------ | ------------------------------------------------------------ |
| API endpoints        | Backend routes           | `[HttpGet]`, `[HttpPost]`, `[HttpPut]`, `[HttpDelete]` ✅    |
| Routing              | go_router / named routes | `[Route("api/patients")]`, `[HttpGet("{id}")]` ✅            |
| Repository injection | Provider / GetIt         | Constructor `IPatientRepository` ✅                          |
| Request body binding | JSON.decode → model      | `[FromBody] CreatePatientRequest request` ✅                 |
| 404 / 201 / 204      | Manual status codes      | `NotFound()`, `CreatedAtAction()`, `NoContent()` ✅          |
| DTO → Entity         | Manual mapping           | Controller builds `Patient` from request, then Add/Update ✅ |

---

## Rating: **10 / 10**

- **10** — All Phase-3 objectives are met: Controllers folder, PatientsController with correct attributes and injection, GET all, GET by id (with 404), POST (201 + Location), PUT (partial update, 404), DELETE (204/404), and Program.cs registration with `AddControllers()` and `MapControllers()`. After the two fixes applied during review (Program.cs and public constructor), the app runs and the endpoints are active. Build: 0 errors, 0 warnings.

---

## What to do next

Phase-3 is **complete**. Move on to **Phase-4: Database** — replace the in-memory repository with a real store (e.g. SQLite, PostgreSQL, or Supabase); controller and endpoints stay the same.

---

## Encouragement

Your controller logic and endpoint behaviour were already correct; the missing wiring in Program.cs and the constructor visibility were the only gaps. Same idea as in Flutter — define routes, inject the repository, map DTOs to entities, and return the right status codes. You’re ready for Phase-4.

---

---

## Previous assessment (history)

_Below: Initial judgement before fixes._

---

# Initial judgement (before fixes)

**Phase assessed:** Phase-3 — API Endpoints & Controller (initial submission)

Summary: Controller implementation (all five endpoints, DTO mapping, 404/201/204 behaviour) was correct and matched the spec. Two issues were found and fixed during review:

1. **Program.cs** — `AddControllers()` and `MapControllers()` were not present, so the PatientsController was never registered and its routes were not mapped. Without these, `GET/POST/PUT/DELETE /api/patients` would not be available at runtime.
2. **Controller constructor** — The constructor lacked the `public` modifier (defaulting to private), which would prevent the DI container from instantiating the controller.

**Rating before fixes: 7.5 / 10** — All controller code and behaviour were correct; the deduction was for the missing registration (endpoints not active) and the constructor visibility (controller activation would fail). **Current state after fixes: 10/10.**

---

_(Table below was initial assessment; all criteria met after fixes.)_

| Criterion                  | Expected (Phase-3)                           | Status (initial) | Notes               |
| -------------------------- | -------------------------------------------- | ---------------- | ------------------- |
| Controllers folder         | `Controllers/`                               | ✅ Done          |                     |
| PatientsController + route | [ApiController], Route api/patients          | ✅ Done          |                     |
| Constructor injection      | IPatientRepository                           | ⚠️ Private ctor  | Fixed to public     |
| GET all / GET by id        | 404 when not found                           | ✅ Done          |                     |
| POST 201 + Location        | CreatePatientRequest → Add → CreatedAtAction | ✅ Done          |                     |
| PUT partial update + 404   | GetById → update → Ok                        | ✅ Done          |                     |
| DELETE 204 / 404           | NoContent or NotFound                        | ✅ Done          |                     |
| Program.cs AddControllers  | AddControllers(); MapControllers();          | ❌ Missing       | Added during review |
