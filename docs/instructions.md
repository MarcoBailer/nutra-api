# Nutra API - Technical Documentation

## 1. Project Overview

ASP.NET Core 9.0 REST API for nutritional management. Provides food database search, meal planning, food diary, nutritional assessments, and professional nutritionist features.

- **ORM**: Entity Framework Core 9 with PostgreSQL (Npgsql)
- **Authentication**: JWT Bearer via shark-lock (OpenID Connect)
- **Food Database**: Brazilian TBCA + Manufacturers + Fast Food + Generic foods (seeded from SQLite)
- **API Docs**: Swagger/OpenAPI via Swashbuckle 6.6.2

---

## 2. Authentication & Authorization

### JWT Bearer Setup (`Program.cs`)
- **Authority**: shark-lock server (configurable via `Authentication:Authority`)
- **Audience**: `nutra-api`
- **Claims mapping**: `NameClaimType = "name"`, `RoleClaimType = "role"`
- **Development mode**: Accepts self-signed certificates (`BackchannelHttpHandler` with bypass)
- **Clock skew**: 5 minutes

### Local User Projection
On first JWT validation, if no local `ApplicationUser` exists, one is created:
- `Id` = JWT `sub` claim (string)
- Syncs `email` and `name` from JWT claims
- Updates on subsequent logins if claims change

### Authorization
- **Default fallback policy**: ALL endpoints require authentication (Bearer scheme) unless explicitly `[AllowAnonymous]`
- **Role-based**: Some endpoints require `Nutricionista` or `Admin` roles via `[Authorize(Roles = "Nutricionista,Admin")]`

### CORS
- Configured for frontend URL from `AppSettings:BaseUrlFront` (default: `http://localhost:3000`)
- Allows any header and method

---

## 3. API Endpoints

### AccountsController (`/api/accounts`) — [Authorize]

| Method | Path | Description |
|--------|------|-------------|
| GET | `/me` | Get authenticated user's complete profile |
| PUT | `/update-profile` | Update personal data (name, CPF, address, phone) |
| POST | `/desativar` | Soft delete user account |
| POST | `/reativar` | Reactivate deactivated account |
| POST | `/vinculos/{vinculoId}/responder` | Patient responds to nutritionist link invitation |
| GET | `/meus-nutricionistas` | List nutritionists linked to patient |

### UserController (`/api/user`) — [Authorize]

| Method | Path | Description |
|--------|------|-------------|
| POST | `/perfil-nutricional` | Create nutritional profile |
| PUT | `/perfil-nutricional` | Update nutritional profile |
| GET | `/buscar-perfil-nutricional` | Retrieve nutritional profile |
| POST | `/preferencia-alimentar/{id}/{tabela}/{afinidade}` | Add food preference |
| DELETE | `/preferencia-alimentar/{preferenciaId}` | Remove food preference |
| POST | `/registro-biometrico` | Record biometric data |
| GET | `/historico-biometrico` | List biometric history |
| GET | `/historico-clinico` | List clinical history |
| POST | `/historico-clinico` | Add clinical condition |
| PUT | `/historico-clinico/{id}` | Update clinical condition |
| DELETE | `/historico-clinico/{id}` | Remove clinical condition |
| POST | `/anamnese-alimentar` | Save food anamnesis |
| GET | `/anamnese-alimentar/ultima` | Get latest anamnesis |
| GET | `/anamnese-alimentar/historico` | List all anamnesis records |

### AlimentosController (`/api/alimentos`) — [No Auth, Public]

| Method | Path | Description |
|--------|------|-------------|
| GET | `/fabricante/alimento/{nome}` | Search manufacturer foods (paginated) |
| GET | `/fastfood/alimento/{nome}` | Search fast food items (paginated) |
| GET | `/tbca/alimento/{nome}` | Search TBCA foods (paginated) |
| GET | `/genericos/alimento/{nome}` | Search generic foods (paginated) |

### BuscaController (`/api/busca`) — [Authorize]

| Method | Path | Description |
|--------|------|-------------|
| GET | `/BuscarTudo/{termo}` | Unified search across all 4 food databases (min 3 chars) |
| GET | `/BuscarPorId/{id}/{tabela}` | Search food by ID and table type |

### AvaliacaoNutricionalController (`/api/avaliacaonutricional`) — [Authorize]

| Method | Path | Roles | Description |
|--------|------|-------|-------------|
| POST | `/registrar` | Any | Register assessment with auto-calculations |
| GET | `/{avaliacaoId}` | Any | Get assessment by ID |
| GET | `/minhas-avaliacoes` | Any | List all patient assessments |
| GET | `/comparar/{anteriorId}/{atualId}` | Any | Compare two assessments (evolution) |
| DELETE | `/{avaliacaoId}` | Any | Delete assessment |
| POST | `/paciente/{userId}/registrar` | Nutricionista, Admin | Professional registers for patient |
| GET | `/paciente/{userId}/avaliacoes` | Nutricionista, Admin | List patient assessments |
| POST | `/{avaliacaoId}/fotos` | Any | Add progress photos |
| DELETE | `/fotos/{fotoId}` | Any | Remove progress photo |

### DiarioAlimentarController (`/api/diarioalimentar`) — [Authorize]

| Method | Path | Description |
|--------|------|-------------|
| POST | `/consumo` | Register single food consumption |
| POST | `/consumo/lote` | Register batch consumption |
| DELETE | `/consumo/{registroId}` | Delete consumption record |
| POST | `/fotos` | Add meal photo |
| DELETE | `/fotos/{fotoId}` | Remove meal photo |
| GET | `/fotos` | List photos for specific day |
| GET | `/dia` | Get complete diary for a day (planned vs consumed) |
| GET | `/periodo` | Get diary for date range (max 90 days) |
| GET | `/relatorio-adesao` | Generate own adherence report |
| GET | `/relatorio-adesao/paciente/{userId}` | Generate patient adherence report [Professional] |

### PlanoAlimentarController (`/api/planoalimentar`) — [Authorize]

| Method | Path | Description |
|--------|------|-------------|
| POST | `/` | Create meal plan |
| POST | `/profissional` | Professional creates plan for patient |
| GET | `/{planoId}` | Get specific plan |
| GET | `/ativo` | Get active plan |
| GET | `/` | List all plans |
| PUT | `/{planoId}` | Update plan info |
| DELETE | `/{planoId}` | Delete plan |
| POST | `/{planoId}/ativar` | Activate plan (deactivates previous) |
| POST | `/{planoId}/refeicoes` | Add meal to plan |
| DELETE | `/refeicoes/{refeicaoId}` | Remove meal |
| POST | `/refeicoes/{refeicaoId}/itens` | Add item to meal |
| DELETE | `/itens/{itemId}` | Remove item |
| POST | `/itens/{itemId}/substituicoes` | Add equivalent substitution |
| DELETE | `/substituicoes/{substituicaoId}` | Remove substitution |
| POST | `/modelos` | Create diet template |
| GET | `/modelos` | List available templates |
| GET | `/modelos/{modeloId}` | Get template details |
| DELETE | `/modelos/{modeloId}` | Delete template (soft) |
| POST | `/modelos/{modeloId}/criar-plano` | Create plan from template (auto-scaled) |

### NutricionistaController (`/api/nutricionista`) — [Nutricionista, Admin]

| Method | Path | Roles | Description |
|--------|------|-------|-------------|
| POST | `/cadastro` | AllowAnonymous | Register as professional |
| GET | `/perfil` | Nutricionista, Admin | Get professional profile |
| PUT | `/perfil` | Nutricionista, Admin | Update professional profile |
| GET | `/clinicas` | Nutricionista, Admin | List clinics |
| POST | `/clinicas` | Nutricionista, Admin | Create clinic |
| PUT | `/clinicas/{clinicaId}` | Nutricionista, Admin | Update clinic |
| DELETE | `/clinicas/{clinicaId}` | Nutricionista, Admin | Remove clinic (soft) |
| GET | `/pacientes` | Nutricionista, Admin | List linked patients |
| POST | `/pacientes/convite` | Nutricionista, Admin | Send patient invitation |
| DELETE | `/pacientes/vinculo/{vinculoId}` | Nutricionista, Admin | End patient link |
| PUT | `/assinatura/{novoPlano}` | Nutricionista, Admin | Update subscription |

### RefeicaoController (`/api/refeicao`) — [Authorize] (Legacy)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/cadastrar-refeicao` | Register consumption (use DiarioAlimentar instead) |
| GET | `/status-diario` | Get daily nutritional status |

---

## 4. Database Schema

### Core Entities

**ApplicationUser** — Local projection of JWT user
- `Id` (string, = JWT sub), personal data, contact info, address
- 1:1 → `PerfilNutricional` (optional), 1:1 → `PerfilProfissional` (optional)

**PerfilNutricional** — Patient nutritional profile
- Height, weight, body fat %, activity level, occupation
- Goals, dietary preferences, food restrictions
- 1:1 → `MetaNutricional` (auto-generated daily targets)
- 1:many → `RegistroBiometrico`, `PreferenciaAlimentar`, `RestricaoAlimentar`, `HistoricoClinico`, `AnamneseAlimentar`, `AvaliacaoAntropometrica`, `PlanoAlimentar`

**AvaliacaoAntropometrica** — Anthropometric snapshot
- All measurements: circumferences, skinfolds, bioimpedance
- Auto-calculated: IMC, RCQ, %body fat, TMB (3 formulas), GET, ideal weight, macros
- 1:many → `FotoProgresso`

**PlanoAlimentar** — Meal plan
- Status: Rascunho → Ativo → Pausado → Finalizado
- Daily targets: calories, macros, water, fiber
- 1:many → `RefeicaoPlano` → `ItemRefeicao` → `SubstituicaoEquivalente`

**RegistroAlimentar** — Food diary entry
- Food ID, table type, quantity, date/time, meal type
- JSON snapshot of complete nutritional data at consumption time
- 1:many → `FotoRefeicao`

**VinculoPacienteProfissional** — Patient-Professional link
- Status: Pendente → Ativo → Inativo → Recusado

### Food Tables
- **Tbca** — Brazilian Food Composition Table (100g reference, 50+ nutritional fields)
- **Fabricantes** — Manufacturer nutrition labels
- **FastFoods** — Fast food chain items
- **Genericos** — Generic/standard foods

---

## 5. Nutritional Calculations (`CalculadoraNutricionalService`)

### Auto-Calculated on Assessment Registration

| Metric | Formula | Classification |
|--------|---------|----------------|
| **IMC** (BMI) | weight(kg) / height(m)² | WHO: Magreza I–III, Eutrófico, Sobrepeso, Obesidade I–III |
| **RCQ** (Waist-Hip) | waist / hip circumference | Gender-specific risk: low, moderate, high |
| **Body Fat %** | Jackson-Pollock 3 or 7 site → Siri formula | Or direct bioimpedance reading |
| **TMB** (BMR) | Mifflin-St Jeor (default), Harris-Benedict, Katch-McArdle | kcal/day |
| **GET** (TEE) | TMB × Activity Factor (1.2–1.9) | kcal/day |
| **Macros** | Protein: 1.8–2.4 g/kg (by goal), Fat: 0.9 g/kg, Carbs: remainder | g/day |
| **Fiber** | 14g per 1000 kcal | g/day |
| **Water** | 35 ml/kg body weight | L/day |
| **Ideal Weight** | Devine formula or IMC-based (22 kg/m² target) | kg |

---

## 6. Food Database (TBCA)

**Tabela Brasileira de Composição de Alimentos** — Brazilian reference food composition data.

- **Reference portion**: 100g (standardized)
- **Seeding**: Loaded from `Data/alimentos.db` (SQLite) at startup via `DatabaseSeeder` (runs if tables are empty)
- **Fields** (50+): Energy (kcal, kJ), protein, available/total carbs, fiber, lipids, added sugar, alcohol, water, ash. Minerals: Ca, P, Fe, Mg, Mn, Zn, Cu, Se, Na, K. Vitamins: A (RAE, RE), B6, B12, C, D, E, niacin, riboflavin, thiamine, folate. Fats: saturated, trans, mono, poly, cholesterol.
- **Search** (`BuscaService`): Parallel `Task.WhenAll` across all 4 tables, multi-word matching (all words must match, case-insensitive), results sorted by name length (closer matches first). TBCA returns up to 20 results, other tables up to 10 each.

---

## 7. Key Business Logic

- **Diet model auto-scaling**: Templates at a base calorie level are proportionally scaled to the user's actual macro targets when creating a plan
- **Nutritional snapshots**: Each food diary entry stores complete nutritional JSON at consumption time — ensures historical accuracy even if food database changes later
- **One active plan**: Only one `PlanoAlimentar` can be Ativo per user. Activating a new plan auto-deactivates the previous one.
- **Assessment comparison**: Compare two assessments side-by-side to show evolution metrics
- **Adherence reporting**: Analyzes a date range (max 90 days) for macro consistency, meal frequency, average adherence percentage
- **Soft deletes**: Status flags rather than hard deletes for audit trail (plans, clinics, accounts)

---

## 8. Configuration & Environment

| Variable | Description |
|----------|-------------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Authentication__Authority` | shark-lock server URL |
| `Authentication__Audience` | `nutra-api` |
| `Authentication__ClientId` | OAuth client ID (for token introspection) |
| `Authentication__ClientSecret` | OAuth client secret |
| `AppSettings__BaseUrlFront` | Frontend URL (CORS origin) |
| `AppSettings__BaseUrlBack` | Backend URL |
| `ASPNETCORE_ENVIRONMENT` | `Development` or `Production` |
| `DOCKER_CONTAINER` | If set, activates `/nutra-api` PathBase for nginx routing |

### Docker Behavior
When `DOCKER_CONTAINER=true`:
- Uses `UsePathBase("/nutra-api")` so all routes are prefixed
- Forwarded headers accepted from any proxy (X-Forwarded-For, X-Forwarded-Proto)
- Data Protection keys stored in `/app/keys` volume

### Auto-Migration
On startup, `Program.cs` checks for pending EF Core migrations and applies them automatically.

### Swagger
Enabled in ALL environments (development and production) at `/swagger`. Bearer JWT authentication scheme displayed in Swagger UI.

---

## 9. Key Services

| Service | Responsibility |
|---------|---------------|
| `BuscaService` | Parallel food search across 4 tables |
| `CalculadoraNutricionalService` | All anthropometric calculations (IMC, RCQ, TMB, GET, macros) |
| `AvaliacaoNutricionalService` | Assessment CRUD, comparison, photo management |
| `UserProfileService` | Nutritional profile, biometric records, clinical history, anamnesis |
| `PlanoAlimentarService` | Meal plan CRUD, template management, auto-scaling |
| `DiarioAlimentarService` | Food diary, daily summaries, adherence reports |
| `NutricionistaService` | Professional profiles, clinics, patient linking, subscriptions |
| `AccountsService` | Account lifecycle (update, deactivate, reactivate) |
| `ApplicationUserService` | Local user CRUD, JWT projection management |
| `RefeicaoService` | Legacy meal consumption (deprecated in favor of DiarioAlimentar) |

---

## 10. Error Handling

**Standard response format**:
```json
{ "sucesso": true/false, "mensagem": "User-friendly message" }
```

- `InvalidOperationException` for business logic errors → 400 Bad Request
- Auth failures logged via `AuthLogger` helper
- Transactions with rollback for atomic multi-entity operations
- Null checks and authorization before data access
