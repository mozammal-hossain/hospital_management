# Phase-4: ডেটাবেস সংযোগ ও স্থায়ী স্টোরেজ

**সময়:** প্রায় ২–৩ ঘণ্টা  
**স্টাইল:** গল্প বলার ভঙ্গিতে, টিউটর মোড — বোঝা, তারপর কোড।

---

## একটু পটভূমি

Phase-1 থেকে Phase-3 এ আমরা মডেল, DTO, ইন-মেমোরি রিপোজিটরি আর পূর্ণ CRUD API বানিয়েছি। এখন `GET /api/patients`, `POST`, `PUT`, `DELETE` সব কাজ করে — কিন্তু ডেটা শুধু মেমোরিতে থাকে। অ্যাপ রিস্টার্ট করলেই সব রোগীর তথ্য চলে যাবে। Phase-4 এ আমরা **ডেটাবেস** যোগ করব — মানে একটা আসল স্টোর (যেমন SQLite বা PostgreSQL) এ ডেটা রাখব। রিপোজিটরি ইন্টারফেস (`IPatientRepository`) আর কন্ট্রোলার একই থাকবে; শুধু রিপোজিটরি ইমপ্লিমেন্টেশন বদলাবে — ইন-মেমোরি লিস্টের বদলে Entity Framework Core (EF Core) দিয়ে ডেটাবেসে read/write করব।

এ Phase এ **SQLite** দিয়ে শুরু করা ভালো — একটাই ফাইল, কোনো আলাদা ডেটাবেস সার্ভার লাগে না, ডেভেলপমেন্টে দ্রুত চালু করা যায়। পরে চাইলে একই প্যাটার্নে PostgreSQL বা Supabase এ সুইচ করা যাবে।

---

## Phase-4 এ কী কী করতে হবে (সংক্ষেপে)

১. **NuGet প্যাকেজ** — Entity Framework Core (SQLite + Design) প্যাকেজ যোগ করা।  
২. **DbContext** — `PatientDbContext` বা `HospitalDbContext` বানিয়ে `DbSet<Patient>` রাখা।  
৩. **Patient মডেল ও টেবিল ম্যাপিং** — Id, FullName ইত্যাদি ফিল্ড ডেটাবেস কলামের সাথে ম্যাপ করা; প্রয়োজন হলে কনফিগারেশন।  
৪. **মাইগ্রেশন** — প্রথম মাইগ্রেশন তৈরি করে ডেটাবেস স্কিমা (টেবিল) জেনারেট করা।  
৫. **ডেটাবেস রিপোজিটরি** — নতুন ক্লাস (যেমন `PatientDbRepository`) যেটা `IPatientRepository` ইমপ্লিমেন্ট করবে এবং DbContext দিয়ে CRUD করবে।  
৬. **কানেকশন স্ট্রিং** — `appsettings.json` এ SQLite কানেকশন স্ট্রিং রাখা; Program.cs এ DbContext ও নতুন রিপোজিটরি রেজিস্ট্রেশন।  
৭. **DI স্যুইচ** — `AddScoped<IPatientRepository, PatientDbRepository>()` দিয়ে ইন-মেমোরি রিপোজিটরির বদলে ডেটাবেস রিপোজিটরি ব্যবহার করা।  
৮. **কন্ট্রোলার** — কোনো পরিবর্তন না; একই এন্ডপয়েন্ট, শুধু পিছনে ডেটা এবার ডেটাবেস থেকে আসবে/যাবে।

নিচে প্রতিটা ধাপ Flutter এর সাথে মিলিয়ে বর্ণনা করা হলো।

---

## ১. NuGet প্যাকেজ যোগ করা

**কী করবে:**  
প্রজেক্টে নিচের প্যাকেজগুলো যোগ করবে:

- `Microsoft.EntityFrameworkCore.Sqlite` — SQLite এর সাথে EF Core ব্যবহারের জন্য।
- `Microsoft.EntityFrameworkCore.Design` — মাইগ্রেশন ও স্ক্যাফোল্ডিং এর জন্য (যেমন `dotnet ef migrations add`)。

কমান্ড:  
`dotnet add package Microsoft.EntityFrameworkCore.Sqlite`  
`dotnet add package Microsoft.EntityFrameworkCore.Design`

অথবা `.csproj` ফাইলে `PackageReference` এড করে।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা `pubspec.yaml` এ `sqflite` বা `drift` যোগ করি — .NET এ NuGet প্যাকেজ একই ধারণা। EF Core দিয়ে আমরা সরাসরি C# অবজেক্ট দিয়ে কাজ করি, SQL কোয়েরি নিজে লিখতে হয় না অনেকক্ষেত্রে।

---

## ২. DbContext বানানো

**কী করবে:**  
একটা ক্লাস বানাবে (যেমন `Data/PatientDbContext.cs` বা `Services/HospitalDbContext.cs`) যেটা `DbContext` ইনহেরিট করবে। ভেতরে `DbSet<Patient> Patients { get; set; }` রাখবে। কনস্ট্রাক্টরে `DbContextOptions<PatientDbContext>` নিয়ে `base(options)` কল করবে। চাইলে `OnConfiguring` বা `OnModelCreating` এ টেবিল নাম, কলাম টাইপ ইত্যাদি কনফিগার করতে পারো।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা ডেটাবেস ওপেন করে টেবিল define করি; এখানে `DbContext` হলো সেই “ডেটাবেস কানেকশন + টেবিল সেট” এর প্রতিনিধি। `DbSet<Patient>` মানে “Patients টেবিল”।

---

## ৩. Patient মডেল ও টেবিল ম্যাপিং

**কী করবে:**  
Phase-1 এর `Patient` ক্লাসই ব্যবহার করবে। EF Core সাধারণত প্রপার্টি নাম দিয়েই কলাম বানায়। যদি Id টা `int` হয় এবং অটো ইনক্রিমেন্ট চাই, তাহলে `[Key]` ও ডেটাবেস জেনারেটেড Id এর জন্য কনফিগারেশন দিতে পারো। `DateTime?` (AdmittedAt), `bool` (IsDischarged) — EF Core নিজে ম্যাপ করে। টেবিল নাম আলাদা দিতে চাইলে `OnModelCreating` এ `modelBuilder.Entity<Patient>().ToTable("Patients");` লিখতে পারো।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা টেবিল স্কিমা ও মডেল ক্লাস আলাদা বা একসাথে রাখি; এখানে একই `Patient` ক্লাস এনটিটি ও ডেটাবেস টেবিলের সাথে ম্যাপ হয়।

---

## ৪. মাইগ্রেশন তৈরি ও অ্যাপ্লাই

**কী করবে:**  
প্রথমে EF Core টুলস ইনস্টল (যদি না থাকে):  
`dotnet tool install --global dotnet-ef`  
তারপর প্রজেক্ট ফোল্ডারে:  
`dotnet ef migrations add InitialCreate`  
এটা একটা মাইগ্রেশন ফোল্ডার ও ফাইল তৈরি করবে। তারপর:  
`dotnet ef database update`  
এটা SQLite ডেটাবেস ফাইল (যেমন `hospital.db`) বানিয়ে টেবিল তৈরি করবে।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা অনেক সময় ভার্সন নম্বর দিয়ে ডেটাবেস স্কিমা আপডেট করি; এখানে মাইগ্রেশনই সেই “ভার্সন” — প্রতিবার মডেল বদলালে নতুন মাইগ্রেশন এড করে `database update` দিলে ডেটাবেস আপডেট হয়।

---

## ৫. ডেটাবেস রিপোজিটরি (PatientDbRepository)

**কী করবে:**  
নতুন ক্লাস (যেমন `Services/PatientDbRepository.cs`) বানাবে যেটা `IPatientRepository` ইমপ্লিমেন্ট করবে। কনস্ট্রাক্টরে `PatientDbContext` (বা তোমার DbContext) ইনজেক্ট করবে। প্রতিটা মেথড:

- **GetAll** — `_context.Patients.ToList()` (বা `AsNoTracking()` দিয়ে read-only)
- **GetById** — `_context.Patients.Find(id)` বা `FirstOrDefaultAsync(p => p.Id == id)`
- **Add** — `_context.Patients.Add(patient); _context.SaveChanges();` — Id জেনারেট DB/EF করলে আগে 0 রেখে দিলে SQLite identity দিয়ে অটো হবে
- **Update** — খুঁজে বের করে প্রপার্টি আপডেট, তারপর `SaveChanges()`
- **Delete** — খুঁজে বের করে `Remove(patient)` তারপর `SaveChanges()`

মনে রাখো: ইন্টারফেসে সিনক্রোনাস মেথড থাকলে সিনক্রোনাসই লিখবে; অথবা ইন্টারফেসটা Async করে `*Async` মেথড ব্যবহার করতে পারো (পরবর্তী উন্নতির জন্য)।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা `DatabaseHelper` বা রিপোজিটরি ক্লাসে `sqflite` দিয়ে insert/update/query করি — এখানে EF Core সেই কাজটা সহজ করে দেয়; আমরা শুধু `DbSet` এ Add/Update/Remove করে `SaveChanges` দেই।

---

## ৬. কানেকশন স্ট্রিং ও Program.cs

**কী করবে:**  
`appsettings.json` এ একটা কানেকশন স্ট্রিং যোগ করবে, যেমন:  
`"ConnectionStrings": { "DefaultConnection": "Data Source=hospital.db" }`  
`Program.cs` এ:

- `builder.Services.AddDbContext<PatientDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));`
- `builder.Services.AddScoped<IPatientRepository, PatientDbRepository>();` — আগের ইন-মেমোরি রিপোজিটরি এর বদলে।

ডেটাবেস রিপোজিটরি Scoped রাখা জরুরি — DbContext ও Scoped হয়, একটা রিকোয়েস্টে একই কনটেক্সট ব্যবহার করতে হবে।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা ডেটাবেস পাথ বা কানেকশন স্ট্রিং env/config থেকে নিই — এখানে `appsettings.json` ও `Configuration` দিয়ে একই কাজ।

---

## ৭. DI স্যুইচ — শুধু রিপোজিটরি বদলানো

**কী করবে:**  
`Program.cs` এ যে লাইনে `AddScoped<IPatientRepository, PatientRepository>` (ইন-মেমোরি) আছে, সেটা বদলে `AddScoped<IPatientRepository, PatientDbRepository>` দেবে। কন্ট্রোলার কোড একটাও বদলাতে হবে না — সে শুধু `IPatientRepository` চায়, কন্টেইনার এখন `PatientDbRepository` দেবে। অ্যাপ স্টার্ট করলে মাইগ্রেশন থেকে ডেটাবেস তৈরি হবে (প্রয়োজনে `app.Run()` এর আগে `using var scope = app.Services.CreateScope(); scope.ServiceProvider.GetRequiredService<PatientDbContext>().Database.Migrate();` দিয়ে অটো মাইগ্রেশন চালাতে পারো)।

**Flutter এর সাথে তুলনা:**  
Provider বা GetIt এ আমরা একই ইন্টারফেসের পিছনে এক দিন মক রিপোজিটরি, আরেক দিন আসল API রিপোজিটরি রেজিস্টার করি — এখানে একই ধারণা; শুধু ইমপ্লিমেন্টেশন বদলেছে।

---

## ৮. কন্ট্রোলার — কোনো পরিবর্তন নয়

**কী করবে:**  
কিছু করবে না। `PatientsController` আগের মতোই `IPatientRepository` ব্যবহার করবে। রিকোয়েস্ট আসলে DI এখন `PatientDbRepository` ইনজেক্ট করবে, তাই ডেটা ডেটাবেস থেকে আসবে/যাবে। API এন্ডপয়েন্ট ও রেসপন্স ফরম্যাট একই থাকবে।

**Flutter এর সাথে তুলনা:**  
UI/ব্লক লেয়ার একই রিপোজিটরি ইন্টারফেস ব্যবহার করলে পিছনে লোকাল স্টোর নাকি ক্লাউড — সেটা বদলালে স্ক্রিন কোড বদলাতে হয় না। এখানেও তাই।

---

## Phase-4 শেষে কী থাকবে (চোখে দেখার মতো)

- **প্রজেক্টে:**

  - `Data/` বা `Services/` — `PatientDbContext.cs` (অথবা যে নাম দিয়েছ)
  - `Services/PatientDbRepository.cs` — `IPatientRepository` এর ডেটাবেস ইমপ্লিমেন্টেশন
  - `Migrations/` — অন্তত একটা মাইগ্রেশন (যেমন `InitialCreate`)
  - রুটে বা বাইনারী পাশে `hospital.db` (SQLite ফাইল) — অ্যাপ রান ও মাইগ্রেশন অ্যাপ্লাই এর পর

- **Program.cs:**

  - `AddDbContext<PatientDbContext>(...)` ও `AddScoped<IPatientRepository, PatientDbRepository>()`
  - কানেকশন স্ট্রিং `appsettings.json` থেকে

- **কন্ট্রোলার:**

  - আগের মতোই; কোনো পরিবর্তন নেই

- **চলবে:**
  - `dotnet run` করলে অ্যাপ রান হবে; POST দিয়ে রোগী যোগ করলে SQLite ফাইলে সেভ হবে; রিস্টার্ট করলেও ডেটা থাকবে। GET/PUT/DELETE সব ডেটাবেসের সাথে কাজ করবে।

---

## Flutter vs .NET — Phase-4 এক নজরে

| ধারণা           | Flutter / Dart             | .NET / C# (Phase-4)                                     |
| --------------- | -------------------------- | ------------------------------------------------------- |
| ডেটাবেস প্যাকেজ | sqflite / drift            | EF Core SQLite + Design                                 |
| ডেটাবেস কানেকশন | openDatabase(path)         | `DbContext` + `UseSqlite(connectionString)`             |
| টেবিল ↔ মডেল    | টেবিল ম্যাপ / ড্রিফট টেবিল | `DbSet<Patient>` + মাইগ্রেশন                            |
| স্কিমা আপডেট    | version / migration        | `dotnet ef migrations add` + `database update`          |
| রিপোজিটরি স্টোর | List → DB helper           | `PatientRepository` → `PatientDbRepository` (DbContext) |
| DI স্যুইচ       | Provider রেজিস্ট্রেশন বদল  | `AddScoped<IPatientRepository, PatientDbRepository>`    |
| এন্ডপয়েন্ট     | অপরিবর্তিত                 | কন্ট্রোলার অপরিবর্তিত; শুধু পিছনের স্টোর ডেটাবেস        |

---

## ছোট উপদেশ (টিউটর থেকে)

- প্রথমে প্যাকেজ ও DbContext সেট করো; তারপর একটা ছোট মাইগ্রেশন বানিয়ে টেবিল দেখে নাও। তারপর রিপোজিটরি ইমপ্লিমেন্ট করে DI বদলাও — একসাথে সব করলে গোলমাল হতে পারে।
- SQLite এ Id অটো ইনক্রিমেন্টের জন্য `Patient` এর Id কে `int` রাখ고 EF তে key হিসেবে কনফিগার করলেই হয়; SQLite নিজে মান সেট করে দেয়।
- ইন-মেমোরি `PatientRepository` ফাইলটা মুছে ফেলো না — পরবর্তীতে ইউনিট টেস্টে মক হিসেবে ব্যবহার করতে পারো; শুধু DI তে ডেটাবেস রিপোজিটরি রেজিস্টার করো।
- চাইলে পরবর্তী ফেজে ভ্যালিডেশন (Required, Email ইত্যাদি) ও সুন্দর 400 এরর মেসেজ যোগ করা যাবে।

Phase-5 এ আমরা **ভ্যালিডেশন ও পলিশ** নিয়ে কাজ করতে পারব — DTO তে ডেটা অ্যানোটেশন, 400 বেড রিকোয়েস্টে ভ্যালিডেশন এরর তালিকা, আর প্রয়োজন হলে লগিং বা হেলথ চেক যোগ করা।  
শুভকামনা।
