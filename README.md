<div align="center">

# nutra

**A nutrition API for Brazilian food.** ~70,000 foods, the science behind the numbers, and the tooling a dietitian actually needs.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-4169E1?logo=postgresql&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-9-512BD4)
![OpenID Connect](https://img.shields.io/badge/OpenID%20Connect-F78C40?logo=openid&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)

</div>

---

## The problem

Every calorie-tracking app suffers from the same two flaws in Brazil.

The first is the food database. International apps know what a Big Mac is but have never heard of *pão de queijo*, *tapioca*, or the specific yogurt brand on the shelf at the local market. What you get is a "close enough" match — and nutrition built on approximations isn't nutrition.

The second is the math. Most apps hide a single hardcoded formula behind a friendly progress ring. Basal metabolism isn't one formula: there are several, each validated for a different population, and picking the wrong one skews every number downstream.

**nutra** is the data and calculation layer that fixes both. It serves Brazilian food composition data, computes nutritional targets using formulas from the actual scientific literature, and exposes all of it as a REST API any client can consume.

## What it does

🔍 **Search across ~70,000 Brazilian foods**
Four separate sources — the official TBCA/TACO composition tables, packaged goods by manufacturer, fast food chains, and generic items — queried in parallel and merged into a single result.

🧮 **Nutritional targets with citations, not guesses**
Basal metabolic rate via **Mifflin-St Jeor**, **Harris-Benedict** (Roza & Shizgal revision), or **Katch-McArdle** when lean mass is available. Total expenditure using WHO/FAO/UNU activity factors, then a calorie deficit or surplus based on the stated goal. BMI and waist-to-hip ratio classified by WHO criteria. Every formula in the code carries its paper reference.

🍽️ **Meal planning**
Plans broken into meals, meals broken into items, with per-meal macro targets. Reusable diet templates and equivalent-substitution tables, so a patient can swap rice for pasta without leaving the plan.

📓 **Food diary**
Daily intake logging with photos, measured against the plan and the target — so progress is a number rather than a feeling.

📋 **Clinical assessment**
Food anamnesis, anthropometric measurements, clinical history, dietary restrictions and preferences, and progress photos over time.

👩‍⚕️ **Built for professionals too**
Dietitians register with CRN verification, manage their patient roster through an explicit patient–professional link, and run assessments and plans on their patients' behalf.

🔐 **No passwords stored here**
Authentication is delegated entirely to an external identity provider over OpenID Connect. This API only validates tokens and projects a local user from the claims it trusts.

## How it fits together

```
 ┌───────────────┐                        ┌──────────────────┐
 │  nutra-app    │ ── signed token ─────▶ │                  │
 │  (web client) │                        │    nutra API     │
 └───────────────┘                        │                  │
         │                                │  search engine   │
         │  login                         │  calculators     │
         ▼                                │  plans & diary   │
 ┌───────────────┐   validates tokens     │  assessments     │
 │   sharklock   │ ◀────── OIDC ───────── │                  │
 │   (identity)  │                        └────────┬─────────┘
 └───────────────┘                                 │
                                                   ▼
                                            PostgreSQL, on a
                                            private network
                                            the API alone can reach
```

## Stack

| | |
|---|---|
| **Language** | C# / .NET 9 |
| **API** | ASP.NET Core 9 + Swagger |
| **Database** | PostgreSQL 16 + Entity Framework Core 9 (SQLite for local dev) |
| **Authentication** | JWT Bearer validated against an OIDC provider |
| **Infra** | Docker Compose, GitHub Actions → GHCR → automated deploy |

Layered architecture: controllers stay thin, services hold the rules, EF Core owns persistence. Around 30 entities across three domains — food composition, users and profiles, and nutritional rules.

## Running it

```bash
dotnet restore
dotnet ef database update
dotnet run                     # https://localhost:7287/swagger
```

Or the whole stack, database included:

```bash
cp .env.example .env      # fill in the secrets
docker network create app-network
docker compose up -d
```

The database sits on an internal Docker network — only the API can reach it. Neither the web client nor the reverse proxy has a route to it.

## The interesting challenges

- **Searching four tables as if they were one.** The sources have different schemas, different levels of detail, and different trustworthiness. Queries run concurrently against each and are merged into a single normalized result, so response time is the slowest source rather than the sum of all of them.
- **Choosing the right formula automatically.** Katch-McArdle is the most accurate — when body composition data exists. Mifflin-St Jeor is the best general estimate. The service picks based on the data actually available for that patient, instead of pretending one formula fits everyone.
- **Users owned by another service.** With identity delegated over OIDC, the local user is a projection built from token claims on first contact — no password, no duplicated profile, no drift between the two databases.
- **Two audiences, one data model.** A patient sees their own diary; a dietitian sees several patients' data. The patient–professional link makes that boundary explicit, instead of scattering it across query filters.

## Status

Working, in production, consumed by the **nutra-app** web client. A personal project, still evolving.

**In progress:** broader food coverage, richer substitution logic, and test coverage.
