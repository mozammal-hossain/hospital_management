# Phase-2: ইন-মেমোরি রিপোজিটরি ইমপ্লিমেন্টেশন

**সময়:** প্রায় ২ ঘণ্টা  
**স্টাইল:** গল্প বলার ভঙ্গিতে, টিউটর মোড — শুধু বোঝা, তারপর কোড।

---

## একটু পটভূমি

Phase-1 এ আমরা প্রজেক্টের হাড়গোড় গড়ে তুলেছি: মডেল, DTO, আর `IPatientRepository` ইন্টারফেস। এখনো কোনো এন্ডপয়েন্ট নেই, ডেটাবেসও নেই। Phase-2 এ আমরা **ইন-মেমোরি রিপোজিটরি** বানাব — মানে `PatientRepository` ক্লাসে `GetAll`, `GetById`, `Add`, `Update`, `Delete` এর ভেতরে আসল লজিক লিখব; ডেটা একটা লিস্ট বা ডিকশনারিতে রেখে দেব। পরে Phase-3/৪ এ আমরা এই রিপোজিটরি কন্ট্রোলার/এন্ডপয়েন্টের সাথে যুক্ত করব; ডেটাবেস (যেমন SQL/Supabase) আরও পরের ফেজে।

এ Phase এ শুধু **রিপোজিটরি ইমপ্লিমেন্টেশন** — কোনো API রাউট বা কন্ট্রোলার বানাব না।

---

## Phase-2 এ কী কী করতে হবে (সংক্ষেপে)

১. **PatientRepository ক্লাস** — `IPatientRepository` ইমপ্লিমেন্ট করে একটা কনক্রিট ক্লাস বানানো (যদি Phase-1 এ স্টাব রাখে থাকো, সেটাকে পূর্ণ করা)।  
২. **ইন-মেমোরি স্টোর** — ভেতরে একটা `List<Patient>` বা `Dictionary<int, Patient>` রেখে ডেটা রাখা।  
৩. **GetAll** — লিস্ট/ডিকশনারি থেকে সব রোগী রিটার্ন করা।  
৪. **GetById** — দেওয়া Id দিয়ে একটা রোগী খুঁজে রিটার্ন করা; না পেলে `null`।  
৫. **Add** — নতুন রোগী যোগ করা: Id অটো জেনারেট (যেমন পরবর্তী নম্বর), লিস্টে এড, সেই অবজেক্ট রিটার্ন।  
৬. **Update** — Id মিলে এমন রোগী খুঁজে তার ফিল্ড আপডেট করা; না পেলে `null`।  
৭. **Delete** — Id মিলে রোগী খুঁজে লিস্ট থেকে সরানো; সফল হলে `true`, না পেলে `false`।  
৮. **DI নিশ্চিত করা** — `Program.cs` এ `AddScoped<IPatientRepository, PatientRepository>()` থাকলে ঠিক আছে; না থাকলে যোগ করা।

নিচে প্রতিটা ধাপ Flutter এর সাথে মিলিয়ে বর্ণনা করা হলো।

---

## ১. PatientRepository ক্লাস ও ইন্টারফেস রেফারেন্স

**কী করবে:**  
`Services/` এ `PatientRepository` ক্লাস থাকবে যেটা `IPatientRepository` ইমপ্লিমেন্ট করবে। Phase-1 এ যদি শুধু ইন্টারফেস আর স্টাব থাকে, তাহলে সেই স্টাব ক্লাসকেই পূর্ণ করবে; নয়তো নতুন করে ক্লাস বানাবে। ইন্টারফেসে যে মেথডগুলো আছে (`GetAll`, `GetById`, `Add`, `Update`, `Delete`) সেগুলোর সাইনেচার যেন মিলে।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা `class ApiPatientRepository implements PatientRepository` লিখে প্রতিটা মেথডে আসল লজিক দিই। এখানেও একই: `class PatientRepository : IPatientRepository` — ভেতরে আমরা ইন-মেমোরি লজিক লিখব।

---

## ২. ইন-মেমোরি স্টোর (List বা Dictionary)

**কী করবে:**  
রিপোজিটরি ক্লাসের ভেতরে প্রাইভেট ফিল্ড হিসেবে ডেটা রাখার জায়গা বানাবে। দুটো সাধারণ উপায়:

- **`List<Patient>`** — সহজ; Add করতে `list.Add(patient)`, GetById তে `list.FirstOrDefault(p => p.Id == id)`, Delete তে `list.Remove(...)`।
- **`Dictionary<int, Patient>`** — Id দিয়ে দ্রুত খোঁজার জন্য; Key = Id, Value = Patient।

Id জেনারেট করার জন্য একটা কাউন্টার বা `nextId` রাখতে পারো (প্রতি Add এ বাড়াবে)। মনে রাখো: একই রিকোয়েস্টের ভেতরে Scoped রিপোজিটরি একই ইনস্ট্যান্স, তাই লিস্ট/ডিকশনারি রিকোয়েস্টের সময় পর্যন্ত থাকবে; রিকোয়েস্ট শেষে ডেটা থাকবে না (ইন-মেমোরি)।

**Flutter এর সাথে তুলনা:**  
Flutter এ আমরা অনেক সময় `List<Patient> _patients = [];` বা `final _cache = <int, Patient>{};` রেখে লোকাল ডেটা সিমুলেট করি। .NET এও একই ধারণা — এখনো নেটওয়ার্ক বা ডেটাবেস নয়, শুধু মেমোরিতে।

---

## ৩. GetAll

**কী করবে:**  
লিস্ট বা ডিকশনারির সব `Patient` রিটার্ন করবে। খালি থাকলে খালি কালেকশন রিটার্ন (যেমন `Enumerable.Empty<Patient>()` বা খালি লিস্ট)। `IEnumerable<Patient>` রিটার্ন টাইপ ইন্টারফেসে যেমন আছে তেমনই রাখবে।

**Flutter এর সাথে তুলনা:**  
Flutter এ `Future<List<Patient>> getAll()` এ আমরা লিস্ট রিটার্ন করি। এখানে সিনক্রোনাসভাবে লিস্ট/ডিকশনারি থেকে রিটার্ন করলেই হয়।

---

## ৪. GetById(int id)

**কী করবে:**  
দেওয়া `id` দিয়ে রোগী খুঁজবে। পেলে সেই `Patient` রিটার্ন, না পেলে `null`। ইন্টারফেসে `Patient? GetById(int id)` থাকলে সেটাই মানবে।

**Flutter এর সাথে তুলনা:**  
`findById(id)` বা `firstWhereOrNull` দিয়ে খুঁজে নেওয়ার মতো। .NET এ `list.FirstOrDefault(p => p.Id == id)` বা ডিকশনারিতে `TryGetValue(id, out var patient)`।

---

## ৫. Add(Patient patient)

**কী করবে:**  
নতুন রোগী যোগ করবে। যেহেতু ক্লায়েন্ট Id পাঠাবে না (Create রিকোয়েস্টে Id নেই), রিপোজিটরিতে ঢোকার আগে বা ভেতরে **Id অ্যাসাইন** করতে হবে। সাধারণ নিয়ম: একটা `_nextId` বা কাউন্টার রেখে প্রতিবার Add এ `patient.Id = _nextId++;` (অথবা লিস্টের সর্বোচ্চ Id + ১) দিয়ে তারপর লিস্ট/ডিকশনারিতে যোগ করবে। যোগ করার পর সেই অবজেক্ট (Id সহ) রিটার্ন করবে। ভ্যালিডেশন (যেমন নাম খালি কিনা) Phase-2 এর স্কোপের বাইরে রাখতে পারো — সেটা Phase-3/৪ এ কন্ট্রোলার বা ফ্লুয়েন্ট ভ্যালিডেশনে।

**Flutter এর সাথে তুলনা:**  
লোকাল লিস্টে `_patients.add(patient)` আর Id জেনারেট করে দেয়া — একই ধারণা।

---

## ৬. Update(Patient patient)

**কী করবে:**  
`patient.Id` দিয়ে খুঁজে সেই এন্ট্রি আপডেট করবে। পেলে ওই রোগীর ফিল্ডগুলো (FullName, DateOfBirth, ইত্যাদি) `patient` অবজেক্ট দিয়ে প্রতিস্থাপন করবে, না পেলে `null` রিটার্ন। লিস্ট ব্যবহার করলে খুঁজে বের করে সেই আইটেমের প্রপার্টি আপডেট; ডিকশনারি ব্যবহার করলে `dictionary[id] = patient` (যদি Id পরিবর্তন না করি)।

**Flutter এর সাথে তুলনা:**  
লিস্টে index পেয়ে `_patients[index] = patient` বা ম্যাপে key দিয়ে আপডেট — একই লজিক।

---

## ৭. Delete(int id)

**কী করবে:**  
দেওয়া `id` দিয়ে রোগী খুঁজে স্টোর থেকে সরাবে। সরাতে পারলে `true`, না পেলে `false` রিটার্ন। ইন্টারফেসে `bool Delete(int id)` থাকলে সেটাই।

**Flutter এর সাথে তুলনা:**  
`_patients.removeWhere((p) => p.id == id)` বা ম্যাপে `_cache.remove(id)` — রিটার্নটা সফল কিনা সেটা বলে দিলেই হয়।

---

## ৮. Program.cs এ DI নিশ্চিত করা

**কী করবে:**  
`Program.cs` এ নিশ্চিত করবে: `builder.Services.AddScoped<IPatientRepository, PatientRepository>();` আছে। Phase-1 এ রাখা থাকলে কিছু করার দরকার নেই; না থাকলে এই লাইন যোগ করবে যাতে কন্ট্রোলার/এন্ডপয়েন্টে `IPatientRepository` ইনজেক্ট করা যায়।

**Flutter এর সাথে তুলনা:**  
আগের ফেজের মতোই — `get_it` বা Provider এ রিপোজিটরি রেজিস্ট্রেশন। এখানে Scoped মানে প্রতি HTTP রিকোয়েস্টে একই রিপোজিটরি ইনস্ট্যান্স; তাই একটা রিকোয়েস্টের ভেতরে ইন-মেমোরি লিস্ট একই থাকবে।

---

## Phase-2 শেষে কী থাকবে (চোখে দেখার মতো)

- **Services/**

  - `IPatientRepository.cs` — আগের মতো
  - `PatientRepository.cs` — পূর্ণ ইমপ্লিমেন্টেশন: ইন-মেমোরি স্টোর + GetAll, GetById, Add, Update, Delete

- **Program.cs**

  - `AddScoped<IPatientRepository, PatientRepository>()` দিয়ে রিপোজিটরি রেজিস্টার্ড

- **চলবে:**
  - `dotnet run` দিয়ে অ্যাপ রান হবে। এখনো `/patients` এন্ডপয়েন্ট নেই, তাই ব্রাউজার থেকে ডেটা দেখতে পারবে না — কিন্তু ইউনিট টেস্ট বা পরের ফেজে কন্ট্রোলার লিখে এই রিপোজিটরি ব্যবহার করলে ডেটা ইন-মেমোরিতে ঠিকভাবে Add/Get/Update/Delete হবে।

---

## Flutter vs .NET — Phase-2 এক নজরে

| ধারণা                 | Flutter / Dart                    | .NET / C# (Phase-2)                                |
| --------------------- | --------------------------------- | -------------------------------------------------- |
| রিপোজিটরি ইমপ্লিমেন্ট | `implements PatientRepository`    | `class PatientRepository : IPatientRepository`     |
| লোকাল স্টোর           | `List<Patient> _patients`         | `private readonly List<Patient> _patients`         |
| Id জেনারেট            | নিজে কাউন্টার/ইনক্রিমেন্ট         | `_nextId++` বা `list.Max(p => p.Id) + 1`           |
| খুঁজে বের করা         | `firstWhere` / `firstWhereOrNull` | `FirstOrDefault(p => p.Id == id)`                  |
| আপডেট/ডিলিট           | লিস্ট/ম্যাপ ম্যানিপুলেশন          | লিস্ট/ডিকশনারি ম্যানিপুলেশন                        |
| DI                    | get_it / Provider                 | `AddScoped<IPatientRepository, PatientRepository>` |

---

## ছোট উপদেশ (টিউটর থেকে)

- প্রথমে শুধু স্টোর (লিস্ট/ডিকশনারি) আর Id জেনারেট লজিক সেট করো; তারপর একটার পর একটা মেথড (GetAll → GetById → Add → Update → Delete) ইমপ্লিমেন্ট করলে গোলমাল কম হবে।
- Add/Update এ যেটা রিপোজিটরি পাচ্ছে সেটা **এনটিটি (Patient)**; DTO থেকে Patient বানানোটা কন্ট্রোলার/এন্ডপয়েন্টে করবে Phase-3 এ।
- ফাইলের নাম ঠিক রাখো: `PatientRepository.cs` (অর্থাৎ বানান চেক করো, যেমন `PatIent` না হয়ে `Patient`)।

Phase-3 এ আমরা **API এন্ডপয়েন্ট** যোগ করব — GET/POST/PUT/DELETE `/patients` এবং রিপোজিটরি ইনজেক্ট করে Create/Update রিকোয়েস্ট বডি থেকে DTO নিয়ে Patient বানিয়ে Add/Update কল করব।  
শুভকামনা।
