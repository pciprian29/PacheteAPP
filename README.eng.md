# PacheteAPP

**PacheteAPP** is an internal web application for a courier/parcel-tracking company, built with ASP.NET Core Razor Pages. It manages the full lifecycle of a parcel — creation, damage reports, missing-information notices, photo evidence, and a full audit trail of every change — and exposes a REST API that powers a companion Android app (**PacheteScan**) used by warehouse and delivery staff in the field.

## What it does

- **Package management** — create and track parcels (AWB, sender/recipient, weight, status), with an auto-generated tracking code and status history.
- **Damage reports** — log a damage report against a parcel, with photo evidence attached.
- **Missing-information notices** — log an issue with a parcel's label or details (including the case where the parcel's own tracking code can't be read), with a photo of the issue.
- **Resolution workflow** — when a report can't be tied to a parcel automatically (e.g. an unreadable label), operators can search existing parcels by partial match across multiple fields (email, address, description, weight, AWB fragment) and manually link the report to the correct one.
- **Photo evidence** — images are uploaded for both damage reports and missing-information notices, stored outside the web root, served through an authorized endpoint, and automatically thumbnailed at upload time so listings load a small preview instead of the full-resolution original.
- **Full audit history** — every change to a package, damage report, or missing-information record is captured automatically, with no extra code needed per feature: a custom `SaveChangesAsync` override inspects every tracked entity change before it's saved and logs the field name, old value, new value, timestamp, and acting user. The history is browsable in a dedicated page with AWB search, sortable columns (by AWB or date), and pagination.
- **Dual authentication** — cookie-based sign-in for the web UI (ASP.NET Core Identity), and JWT bearer tokens for the Android app, both governed by the same claim-based permission system.
- **REST API for the companion app** — endpoints for login, creating packages/damage reports/missing-info notices, uploading and retrieving photos (including thumbnails), and a single aggregated "package details" endpoint that returns a parcel with all its related reports and photos in one call, built for barcode-scan lookups from the field.

## Architecture

Consistent layering across every feature:

```
Page / Controller  →  Service (interface + implementation)  →  ApplicationDbContext (EF Core)  →  SQL Server
```

- **Pages** (`Pages/*Pages/`) — Razor Pages for the web UI; each feature area has its own folder (`PachetPages`, `DeteriorariPages`, `InfoLipsaPages`, `IstoricPages`, `UsersPages`). Page models are declared directly in the `.cshtml.cs` code-behind, not as separate classes.
- **Controllers** (`Controllers/`) — Web API controllers consumed by the Android app, under `api/android/...`, secured with JWT and the same claim-based policies as the web UI.
- **Services** (`Services/`) — one interface + implementation per feature (e.g. `IPachetService`/`PachetService`), constructor-injected with `ApplicationDbContext`. Every write operation returns a typed result object (`Success`/`Fail`/`Confirmare` via static factory methods) instead of throwing for expected validation failures.
- **DTOs** (`DTOs/`) — sealed records for API requests/responses, kept separate from the EF Core entities so the wire format can evolve independently of the database schema.
- **Models** (`Models/`) — EF Core entities, matched 1:1 to existing SQL Server tables (`ExcludeFromMigrations`, since the schema is managed directly in SQL); plus `Models/Views/` for read-only mappings onto SQL views used for denormalized listing pages, and `Models/Helper/` for small stateless generators (AWB codes, image filenames).
- **Authorization** — claim-based policies (`CanView`, `CanCreate`, `CanEdit`, `CanDelete`, `CanUseMobileApp`), each checked via `RequireClaim("Permisiune", "...")`, applied consistently across both Razor Pages and API controllers.

## Data model highlights

- A package (`Pachet`) doesn't link directly to damage reports or missing-info notices — both go through `Inregistrare` (a generic "registration" record carrying the user, timestamp, and type), which is what the audit history and photo-entity linking are built around.
- Images (`Imagine`) use a lightweight polymorphic association: a type column (`id_tip_imagine`) plus a generic entity id (`id_entitate`), rather than separate foreign keys per feature — kept deliberately uncoupled from the database's referential-integrity constraints, since a photo can belong to either a damage report or a missing-info notice.
- Generated image filenames encode context for readability on disk (parcel AWB or a `FARAAWB-<id>` fallback when the AWB itself is the missing piece, capture date, a random suffix, and a type letter), while the actual entity association is resolved through the database columns, not by parsing the filename.

## Tech stack

ASP.NET Core (Razor Pages + Web API), Entity Framework Core, SQL Server, ASP.NET Core Identity (cookie auth) + JWT Bearer (API auth), SixLabors.ImageSharp (server-side thumbnail generation).

## Companion app

The Android app that consumes this API — barcode scanning via Honeywell AIDC hardware (with manual-entry fallback on standard Android devices), field-based report creation with photo capture, and offline-tolerant upload with retry — lives in a separate repository (**PacheteScan**).

## Status

Actively developed. Planned work includes generating and serving reduced-resolution thumbnails more broadly, and an offline photo-queue mechanism (WorkManager-based, on the Android side) for unreliable field connectivity.
