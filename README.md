# PacheteAPP

**PacheteAPP** este o aplicatie web interna pentru o firma de curierat/tracking colete, construita cu ASP.NET Core Razor Pages. Gestioneaza intregul ciclu de viata al unui colet — creare, rapoarte de deteriorare, sesizari de informatii lipsa, dovezi foto, si un istoric complet al fiecarei modificari — si expune un API REST care alimenteaza o aplicatie Android insotitoare (**PacheteScan**), folosita de personalul din depozit si de curieri in teren.

## Ce face

- **Gestionarea pachetelor** — creare si urmarire colete (AWB, expeditor/destinatar, greutate, status), cu cod de tracking generat automat si istoric de status.
- **Rapoarte de deteriorare** — inregistrarea unei deteriorari pentru un colet, cu dovezi foto atasate.
- **Sesizari de informatii lipsa** — inregistrarea unei probleme cu eticheta sau detaliile unui colet (inclusiv cazul in care chiar codul de tracking al coletului nu poate fi citit), cu o poza a problemei.
- **Flux de solutionare** — cand o sesizare nu poate fi legata automat de un colet (ex. eticheta ilizibila), operatorii pot cauta printre coletele existente prin potrivire partiala pe mai multe campuri (email, adresa, descriere, greutate, fragment de AWB) si pot lega manual sesizarea de coletul corect.
- **Dovezi foto** — imaginile sunt incarcate atat pentru rapoartele de deteriorare cat si pentru sesizarile de informatii lipsa, stocate in afara wwwroot, servite printr-un endpoint autorizat, si redimensionate automat la incarcare, astfel incat listele sa se incarce cu o previzualizare mica in loc de originalul la rezolutie completa.
- **Istoric complet de modificari** — fiecare schimbare la un pachet, raport de deteriorare, sau sesizare de informatii lipsa este surprinsa automat, fara cod suplimentar per functionalitate: o suprascriere custom a `SaveChangesAsync` inspecteaza fiecare modificare de entitate urmarita inainte de salvare si inregistreaza numele campului, valoarea veche, valoarea noua, data si userul care a facut modificarea. Istoricul e vizibil intr-o pagina dedicata, cu cautare dupa AWB, coloane sortabile (dupa AWB sau data), si paginare.
- **Autentificare duala** — login pe cookie pentru interfata web (ASP.NET Core Identity), si tokenuri JWT pentru aplicatia Android, ambele guvernate de acelasi sistem de permisiuni bazat pe claim-uri.
- **API REST pentru aplicatia insotitoare** — endpoint-uri pentru login, creare pachete/rapoarte de deteriorare/sesizari informatii lipsa, incarcare si citire poze (inclusiv thumbnail-uri), si un endpoint agregat de "detalii pachet" care intoarce un colet cu toate rapoartele si pozele asociate intr-un singur apel, construit pentru cautari prin scanare de cod de bare din teren.

## Arhitectura

Structura consecventa pe fiecare functionalitate:

```
Page / Controller  →  Service (interfata + implementare)  →  ApplicationDbContext (EF Core)  →  SQL Server
```

- **Pages** (`Pages/*Pages/`) — Razor Pages pentru interfata web; fiecare zona functionala are propriul folder (`PachetPages`, `DeteriorariPages`, `InfoLipsaPages`, `IstoricPages`, `UsersPages`). Modelele de pagina sunt declarate direct in code-behind-ul `.cshtml.cs`, nu ca fisiere separate.
- **Controllers** (`Controllers/`) — controllere Web API consumate de aplicatia Android, sub `api/android/...`, securizate cu JWT si aceleasi policy-uri bazate pe claim-uri ca interfata web.
- **Services** (`Services/`) — cate o interfata + implementare per functionalitate (ex. `IPachetService`/`PachetService`), cu `ApplicationDbContext` injectat prin constructor. Fiecare operatie de scriere intoarce un obiect de rezultat tipizat (`Success`/`Fail`/`Confirmare` prin metode statice de fabricare) in loc sa arunce exceptii pentru esecuri de validare asteptate.
- **DTOs** (`DTOs/`) — records sealed pentru request-urile/raspunsurile API-ului, tinute separat de entitatile EF Core, ca formatul de comunicare sa poata evolua independent de schema bazei de date.
- **Models** (`Models/`) — entitati EF Core, mapate 1:1 pe tabelele SQL Server existente (`ExcludeFromMigrations`, fiindca schema e gestionata direct in SQL); plus `Models/Views/` pentru maparile read-only pe view-uri SQL folosite in paginile de listare denormalizate, si `Models/Helper/` pentru generatoare mici, fara stare (coduri AWB, nume de fisiere imagine).
- **Autorizare** — policy-uri bazate pe claim-uri (`CanView`, `CanCreate`, `CanEdit`, `CanDelete`, `CanUseMobileApp`), fiecare verificat prin `RequireClaim("Permisiune", "...")`, aplicate consecvent atat pe Razor Pages cat si pe controllerele API.

## Aspecte notabile ale modelului de date

- Un pachet (`Pachet`) nu se leaga direct de rapoartele de deteriorare sau sesizarile de informatii lipsa — ambele trec prin `Inregistrare` (o inregistrare generica ce poarta userul, data si tipul), in jurul careia sunt construite istoricul de audit si legarea pozelor de entitati.
- Imaginile (`Imagine`) folosesc o asociere polimorfica usoara: o coloana de tip (`id_tip_imagine`) plus un id generic de entitate (`id_entitate`), in loc de chei straine separate per functionalitate — lasata deliberat neconstransa de integritatea referentiala a bazei de date, fiindca o poza poate apartine fie unui raport de deteriorare, fie unei sesizari de informatii lipsa.
- Numele fisierelor de imagine generate encodeaza contextul pentru lizibilitate pe disc (AWB-ul coletului sau un fallback `FARAAWB-<id>` cand chiar AWB-ul e piesa lipsa, data capturii, un sufix random, si o litera de tip), in timp ce asocierea reala cu entitatea se rezolva prin coloanele bazei de date, nu prin parsarea numelui fisierului.

## Stack tehnic

ASP.NET Core (Razor Pages + Web API), Entity Framework Core, SQL Server, ASP.NET Core Identity (autentificare pe cookie) + JWT Bearer (autentificare API), SixLabors.ImageSharp (generare thumbnail-uri pe server).

## Aplicatia insotitoare

Aplicatia Android care consuma acest API — scanare de cod de bare prin hardware Honeywell AIDC (cu fallback pe introducere manuala pe dispozitive Android standard), creare de rapoarte din teren cu captura de poze, si incarcare tolerata la intreruperi de retea cu reincercare automata — se afla intr-un repository separat (**PacheteScan**).

## Status

In dezvoltare activa. Munca planificata include extinderea generarii si servirii thumbnail-urilor la mai multe locuri, si un mecanism de coada offline pentru poze (bazat pe WorkManager, pe partea de Android) pentru conectivitate nesigura in teren.
