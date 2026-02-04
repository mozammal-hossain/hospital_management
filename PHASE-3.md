# Phase-3: API এন্ডপয়েন্ট ও কন্ট্রোলার

**সময়:** প্রায় ২ ঘণ্টা  
**স্টাইল:** গল্প বলার ভঙ্গিতে, টিউটর মোড — বোঝা, তারপর কোড।

---

## একটু পটভূমি

Phase-1 এ আমরা মডেল, DTO আর `IPatientRepository` ইন্টারফেস দিয়ে প্রজেক্টের হাড়গোড় গড়ে তুলেছি। Phase-2 এ আমরা `PatientRepository` দিয়ে ইন-মেমোরি স্টোর ও CRUD অপারেশন ইমপ্লিমেন্ট করেছি। এখন পর্যন্ত ক্লায়েন্ট (ব্রাউজার, Postman, Flutter অ্যাপ) থেকে ডেটা নিয়ে আসা বা পাঠানোর কোনো উপায় নেই। Phase-3 এ আমরা **API এন্ডপয়েন্ট** যোগ করব — মানে HTTP রিকোয়েস্ট (GET, POST, PUT, DELETE) রিসিভ করে রিপোজিটরি দিয়ে ডেটা যোগ/আপডেট/মুছে দেব এবং রেসপন্স পাঠাব। শেষে `GET /patients`, `POST /patients`, `PUT /patients/{id}`, `DELETE /patients/{id}` সব কাজ করবে।

এ Phase এ **কন্ট্রোলার** বানাব — ASP.NET Core এর `ApiController` দিয়ে RESTful রাউট সাজিয়ে, রিপোজিটরি ইনজেক্ট করে DTO থেকে Patient বানিয়ে Add/Update কল করব। ডেটাবেস বা অথেনটিকেশন এখনো Phase-3 এর স্কোপে নয়।

---

## Phase-3 এ কী কী করতে হবে (সংক্ষেপে)

১. **Controllers ফোল্ডার** — প্রজেক্ট রুটে `Controllers/` ফোল্ডার তৈরি করা।  
২. **PatientsController** — `PatientsController` ক্লাস বানিয়ে `[ApiController]` ও `[Route("api/[controller]")]` অ্যাট্রিবিউট দেয়া।  
৩. **GET /api/patients** — সব রোগী রিটার্ন; রিপোজিটরির `GetAll()` কল।  
৪. **GET /api/patients/{id}** — Id দিয়ে একটা রোগী রিটার্ন; না পেলে 404।  
৫. **POST /api/patients** — বডি থেকে `CreatePatientRequest` নিয়ে নতুন Patient বানিয়ে `Add()` কল; 201 Created ও লোকেশন হেডার।  
৬. **PUT /api/patients/{id}** — বডি থেকে `UpdatePatientRequest` নিয়ে থাকা Patient আপডেট; না পেলে 404।  
৭. **DELETE /api/patients/{id}** — Id দিয়ে রোগী মুছে দেয়া; না পেলে 404।  
৮. **Program.cs** — `AddControllers()` ও `MapControllers()` যোগ করা।

নিচে প্রতিটা ধাপ Flutter এর সাথে মিলিয়ে বর্ণনা করা হলো।

---

## ১. Controllers ফোল্ডার ও কন্ট্রোলার সেটআপ

**কী করবে:**  
প্রজেক্ট রুটে `Controllers/` ফোল্ডার বানাবে। ভেতরে `PatientsController.cs` ফাইলে একটা ক্লাস লিখবে যেটা `ControllerBase` ইনহেরিট করবে। ক্লাসের উপরে দেবে:

- `[ApiController]` — ASP.NET কে বলবে এটা API কন্ট্রোলার; অটো মডেল ভ্যালিডেশন, 400 বেড রিকোয়েস্ট ইত্যাদি।
- `[Route("api/[controller]")]` — রাউট হবে `api/Patients` (controller নাম থেকে; চাইলে `api/patients` করার জন্য `[controller]` এর জায়গায় ছোট হাতের নাম দিতে পারো)।

কন্ট্রোলার কনস্ট্রাক্টরে `IPatientRepository` ইনজেক্ট করবে (constructor injection)।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা যেমন `Provider` বা `GetIt` দিয়ে রিপোজিটরি ইনজেক্ট করে সেটা স্ক্রিন/ব্লক থেকে ব্যবহার করি — .NET এ কন্ট্রোলার কনস্ট্রাক্টরে `IPatientRepository` চাইলে DI কন্টেইনার অটোমেটিক দেবে। রাউটিংটা Flutter এর `go_router` বা named routes এর মতো — URL প্যাটার্ন নির্ধারণ করে দেয়।

---

## ২. GET /api/patients — সব রোগী তালিকা

**কী করবে:**  
একটা অ্যাকশন মেথড লিখবে `GetAll()` বা `Get()` নামে। উপরে `[HttpGet]` অ্যাট্রিবিউট দেবে। ভেতরে `_repository.GetAll()` কল করে ফলাফল রিটার্ন করবে। ASP.NET অটোমেটিক JSON এ সিরিয়ালাইজ করে রেসপন্স পাঠাবে। খালি থাকলে খালি অ্যারে `[]` যাবে।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা `http.get(uri)` বা `dio.get()` দিয়ে API কল করি — এখানে আমরা সেই API টা ইমপ্লিমেন্ট করছি। ক্লায়েন্ট যখন `GET /api/patients` করবে, কন্ট্রোলার এই মেথড রান করবে।

---

## ৩. GET /api/patients/{id} — Id দিয়ে একটা রোগী

**কী করবে:**  
অ্যাকশন মেথড `GetById(int id)` — রাউটে `{id}` থাকবে তাই `[HttpGet("{id}")]` দেবে। `_repository.GetById(id)` কল করবে। পেলে সেই `Patient` রিটার্ন; না পেলে `NotFound()` রিটার্ন করবে (যেটা 404 স্ট্যাটাস কোড দেয়)।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা `Navigator.push` দিয়ে ডিটেইল পেজে `id` পাস করি; এখানে URL ই `api/patients/5` — id সরাসরি URL এ। ক্লায়েন্ট 404 পেলে "রোগী খুঁজে পাওয়া যায়নি" দেখাতে পারবে।

---

## ৪. POST /api/patients — নতুন রোগী যোগ

**কী করবে:**  
অ্যাকশন মেথড `Create([FromBody] CreatePatientRequest request)`। `[HttpPost]` ও `[FromBody]` দিয়ে বডির JSON কে `CreatePatientRequest` এ বাইন্ড করবে। তারপর:

1. `request` থেকে নতুন `Patient` অবজেক্ট বানাবে — Id দেবে না (রিপোজিটরি Add এর সময় বা আগে জেনারেট হবে যদি রিপোজিটরি না করে); অথবা রিপোজিটরি Add কল করার আগে `Id = 0` রেখে দিলে রিপোজিটরি নিজে Id সেট করবে।
2. `_repository.Add(patient)` কল করবে।
3. `CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient)` রিটার্ন করবে — এটা 201 Created স্ট্যাটাস দেয় এবং `Location` হেডারে নতুন রিসোর্সের URL পাঠায়।

**মনে রাখো:**  
`CreatePatientRequest` এ Id নেই; `AdmittedAt`, `IsDischarged` থাকলে সেগুলো optional — চাইলে Create এ `AdmittedAt = DateTime.UtcNow` দিয়ে ভর্তির সময় সেট করতে পারো।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা `await repository.createPatient(patient)` করি; এখানে কন্ট্রোলার সেই repository এর `Add` কল করছে। Request বডি থেকে DTO নিয়ে Entity বানানোর কাজটা কন্ট্রোলারেই করছি — Flutter এও আমরা অনেক সময় আলাদা সেরিভিস/ম্যাপার ব্যবহার করি।

---

## ৫. PUT /api/patients/{id} — রোগী আপডেট

**কী করবে:**  
অ্যাকশন মেথড `Update(int id, [FromBody] UpdatePatientRequest request)`। `[HttpPut("{id}")]` দেবে। লজিক:

1. `_repository.GetById(id)` দিয়ে থাকা রোগী খুঁজো; না পেলে `NotFound()`।
2. পেলে `request` এর যে ফিল্ডগুলো null নয় শুধু সেগুলো দিয়ে existing Patient আপডেট করো (partial update)। যেমন: `if (request.FullName != null) existing.FullName = request.FullName;` — অথবা একটা হেল্পার মেথড/ম্যাপার লিখে পরিষ্কার রাখো।
3. `_repository.Update(existing)` কল করো।
4. পেলে `Ok(updated)` বা শুধু `Ok()` রিটার্ন।

**Flutter এর সাথে তুলনা:**  
Update request এ শুধু বদলানোর ফিল্ড পাঠানো — Flutter এ `copyWith` দিয়ে আমরা আংশিক আপডেট করি; এখানে DTO এর nullable ফিল্ড দিয়ে সেটা করছি।

---

## ৬. DELETE /api/patients/{id} — রোগী মুছে ফেলা

**কী করবে:**  
অ্যাকশন মেথড `Delete(int id)`। `[HttpDelete("{id}")]` দেবে। `_repository.Delete(id)` কল করবে। সফল হলে `NoContent()` (204) রিটার্ন; না পেলে `NotFound()`।

**Flutter এর সাথে তুলনা:**  
REST কনভেনশন অনুযায়ী ডিলিট 成功后 204 No Content দেয়া সাধারণ — বডি ছাড়া স্ট্যাটাসই যথেষ্ট।

---

## ৭. Program.cs — কন্ট্রোলার রেজিস্ট্রেশন ও ম্যাপিং

**কী করবে:**  
`Program.cs` এ দুটো জিনিস যোগ করবে:

1. **রেজিস্ট্রেশন:** `builder.Services.AddControllers();` — কন্ট্রোলার সার্ভিস কন্টেইনারে যোগ।
2. **ম্যাপিং:** `app.MapControllers();` — রাউটগুলো অ্যাকটিভ করবে।

নিশ্চিত করো `AddControllers()` এর পর `AddOpenApi()` ইত্যাদি যেমন আছে তেমন আছে; `MapControllers()` টা `app.Run()` এর আগে, অন্য `Map*` calls এর পাশে দিতে হবে।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা `MaterialApp` এ `routes` বা `go_router` কনফিগার করি — এখানে `MapControllers()` সেই রাউটগুলো সক্রিয় করে।

---

## ৮. (ঐচ্ছিক) ভ্যালিডেশন

Phase-3 এর মূল ফোকাস এন্ডপয়েন্ট; কিন্তু চাইলে DTO তে ডেটা অ্যানোটেশন যোগ করতে পারো:

- `[Required]` — FullName, Email ইত্যাদির জন্য
- `[EmailAddress]` — Email ফিল্ডের জন্য
- `[StringLength(...)]` — দৈর্ঘ্য সীমা

`[ApiController]` থাকলে মডেল স্টেট ইনভ্যালিড হলে অটোমেটিক 400 Bad Request ফিরবে। এই অংশটা ঐচ্ছিক — আগে এন্ডপয়েন্ট ঠিকঠাক চালু করো, তারপর ভ্যালিডেশন যোগ করতে পারো।

---

## Phase-3 শেষে কী থাকবে (চোখে দেখার মতো)

- **প্রজেক্ট রুটে:**

  - `Controllers/` — ভেতরে `PatientsController.cs`
  - `Models/` — আগের মতো
  - `Services/` — আগের মতো

- **PatientsController:**

  - GET `api/patients` — সব রোগী
  - GET `api/patients/{id}` — Id দিয়ে একটা
  - POST `api/patients` — নতুন যোগ
  - PUT `api/patients/{id}` — আপডেট
  - DELETE `api/patients/{id}` — মুছে ফেলা

- **Program.cs:**

  - `AddControllers()` ও `MapControllers()` যোগ

- **চলবে:**
  - `dotnet run` দিয়ে অ্যাপ রান করলে Postman বা ব্রাউজার থেকে `/api/patients` কল করে ডেটা দেখা/যোগ/আপডেট/ডিলিট করা যাবে। OpenAPI (Swagger) চালু থাকলে `/swagger` বা `/openapi/v1.json` দিয়ে এন্ডপয়েন্ট টেস্ট করা যাবে।

---

## Flutter vs .NET — Phase-3 এক নজরে

| ধারণা              | Flutter / Dart              | .NET / C# (Phase-3)                                |
| ------------------ | --------------------------- | -------------------------------------------------- |
| API এন্ডপয়েন্ট    | Backend (Node/Go/Dart) রাউট | `[HttpGet]`, `[HttpPost]` — কন্ট্রোলার অ্যাকশন     |
| রাউটিং             | go_router / named routes    | `[Route("api/[controller]")]`, `[HttpGet("{id}")]` |
| রিপোজিটরি ইনজেকশন  | Provider / GetIt            | কন্ট্রাক্টরে `IPatientRepository`                  |
| Request বডি বাইন্ড | JSON.decode → Model         | `[FromBody] CreatePatientRequest request`          |
| 404 / 201          | Response status manually    | `NotFound()`, `CreatedAtAction()`, `NoContent()`   |
| DTO → Entity       | manual mapping              | কন্ট্রোলারে request দিয়ে Patient বানানো           |

---

## ছোট উপদেশ (টিউটর থেকে)

- একসাথে সব এন্ডপয়েন্ট না লিখে একটার পর একটা টেস্ট করো — প্রথমে GET All, তারপর GET by Id, তারপর POST, তারপর PUT, শেষে DELETE।
- DTO আর Entity আলাদা রাখার কারণে কন্ট্রোলারে একটু ম্যাপিং লজিক লাগবে — এটাই সঠিক প্যাটার্ন; Entity কে সরাসরি request বডিতে বাইন্ড করো না।
- রাউট `api/patients` না `api/Patients` — ASP.NET ডিফল্টভাবে PascalCase; ক্লায়েন্টের জন্য lowercase চাইলে `[Route("api/patients")]` স্পষ্টভাবে লিখে দিতে পারো।
- OpenAPI/Swagger যোগ থাকলে ব্রাউজার থেকে সহজে টেস্ট করতে পারবে।

Phase-4 এ আমরা **ডেটাবেস** (যেমন SQLite, PostgreSQL বা Supabase) যোগ করতে পারব — তখন ইন-মেমোরি রিপোজিটরির বদলে ডাটাবেস রিপোজিটরি ব্যবহার করব; কন্ট্রোলার ও এন্ডপয়েন্ট একই থাকবে।  
শুভকামনা।
