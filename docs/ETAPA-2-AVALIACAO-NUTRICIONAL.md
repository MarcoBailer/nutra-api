# Etapa 2 — Avaliação Nutricional

## Visão Geral

A Etapa 2 implementa o módulo de **Avaliação Nutricional** completo, incluindo:

- **Antropometria** (medidas corporais)
- **Cálculos automáticos** (TMB, GET, %gordura, peso ideal, macronutrientes)
- **Composição corporal** (bioimpedância + dobras cutâneas)
- **Fotos de progresso** (acompanhamento visual)
- **Comparação evolutiva** (entre avaliações)

---

## Arquivos Criados

### Enums
| Arquivo | Descrição |
|---------|-----------|
| `Enum/EFormulaCalculo.cs` | Fórmulas de TMB: MifflinStJeor, HarrisBenedict, KatchMcArdle |
| `Enum/EProtocoloDobrasCutaneas.cs` | Protocolos: JacksonPollock3, JacksonPollock7, Guedes3, Petroski |
| `Enum/ETipoFotoProgresso.cs` | Tipos de foto: Frontal, LateralEsquerdo, LateralDireito, Costas |
| `Enum/EMetodoPesoIdeal.cs` | Métodos de peso ideal: Devine, Hamwi, Robinson, Miller, IMC |

### Models
| Arquivo | Descrição |
|---------|-----------|
| `Models/RegraNutricional/AvaliacaoAntropometrica.cs` | Entidade principal com todas medidas + cálculos persistidos |
| `Models/RegraNutricional/FotoProgresso.cs` | Foto de progresso vinculada a uma avaliação |

### DTOs
| Arquivo | Descrição |
|---------|-----------|
| `Models/Dtos/AvaliacaoAntropometricaDto.cs` | DTO de entrada (input) com validação |
| `Models/Dtos/AvaliacaoAntropometricaResultadoDto.cs` | DTOs de saída com resultado completo, resumo, comparação e evolução |
| `Models/Dtos/FotoProgressoDto.cs` | DTO para foto de progresso |

### Interface
| Arquivo | Descrição |
|---------|-----------|
| `Interfaces/IAvaliacaoNutricional.cs` | Contrato do serviço de avaliação |

### Serviço
| Arquivo | Descrição |
|---------|-----------|
| `Services/AvaliacaoNutricionalService.cs` | Implementação completa (registrar, listar, comparar, excluir, fotos) |

### Controller
| Arquivo | Descrição |
|---------|-----------|
| `Controllers/AvaliacaoNutricionalController.cs` | API REST com endpoints completos |

---

## Arquivos Modificados

| Arquivo | Alteração |
|---------|-----------|
| `Interfaces/ICalculadoraNutricional.cs` | Expandida com métodos para IMC, RCQ, Harris-Benedict, Katch-McArdle, peso ideal, macros |
| `Services/CalculadoraNutricionalService.cs` | Reescrita com todas fórmulas e referências científicas |
| `Models/Usuario/PerfilNutricional.cs` | Adicionada navegação `AvaliacoesAntropometricas` |
| `Data/AlimentosContext.cs` | Adicionados `DbSet<AvaliacaoAntropometrica>` e `DbSet<FotoProgresso>` com relacionamentos |
| `Program.cs` | Registradas `IAvaliacaoNutricional` e `IRefeicao` (corrigido bug de DI faltante) |

---

## Endpoints da API

### Paciente (auto-avaliação)

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/api/AvaliacaoNutricional/registrar` | Registrar avaliação completa |
| `GET` | `/api/AvaliacaoNutricional/{avaliacaoId}` | Obter avaliação por Id |
| `GET` | `/api/AvaliacaoNutricional/minhas-avaliacoes` | Listar avaliações (resumo) |
| `GET` | `/api/AvaliacaoNutricional/comparar/{anteriorId}/{atualId}` | Comparar duas avaliações |
| `DELETE` | `/api/AvaliacaoNutricional/{avaliacaoId}` | Excluir avaliação |

### Nutricionista (em nome do paciente)

| Método | Rota | Descrição | Roles |
|--------|------|-----------|-------|
| `POST` | `/api/AvaliacaoNutricional/paciente/{pacienteUserId}/registrar` | Registrar avaliação | Nutricionista, Admin |
| `GET` | `/api/AvaliacaoNutricional/paciente/{pacienteUserId}/avaliacoes` | Listar avaliações do paciente | Nutricionista, Admin |

### Fotos de Progresso

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/api/AvaliacaoNutricional/{avaliacaoId}/fotos` | Adicionar fotos a avaliação |
| `DELETE` | `/api/AvaliacaoNutricional/fotos/{fotoId}` | Remover foto |

---

## Fórmulas Implementadas

### TMB — Taxa Metabólica Basal

#### 1. Mifflin-St Jeor (1990) ⭐ Padrão
```
Homens:   TMB = (10 × peso) + (6.25 × altura) - (5 × idade) + 5
Mulheres: TMB = (10 × peso) + (6.25 × altura) - (5 × idade) - 161
```
> Referência: Mifflin MD, St Jeor ST, et al. _Am J Clin Nutr._ 1990;51(2):241-7.

#### 2. Harris-Benedict Revisada (1984)
```
Homens:   TMB = 88.362 + (13.397 × peso) + (4.799 × altura) - (5.677 × idade)
Mulheres: TMB = 447.593 + (9.247 × peso) + (3.098 × altura) - (4.330 × idade)
```
> Referência: Roza AM, Shizgal HM. _Am J Clin Nutr._ 1984;40(1):168-82.

#### 3. Katch-McArdle (1996)
```
TMB = 370 + (21.6 × massa magra em kg)
```
> Só calculada quando há dados de massa magra (bioimpedância ou dobras cutâneas).

### GET — Gasto Energético Total

```
GET = TMB × Fator de Atividade
```

| Nível | Fator |
|-------|-------|
| Sedentário | 1.200 |
| Levemente Ativo | 1.375 |
| Moderadamente Ativo | 1.550 |
| Muito Ativo | 1.725 |
| Extremamente Ativo | 1.900 |

### Taxa Metabólica Ajustada ao Objetivo

| Objetivo | Multiplicador |
|----------|--------------|
| Perda de Gordura | 0.80 (déficit 20%) |
| Hipertrofia | 1.10 (superávit 10%) |
| Recomposição Corporal | 0.95 (déficit 5%) |
| Saúde Metabólica | 1.00 |
| Performance Esportiva | 1.15 |
| Ganho de Energia | 1.05 |

### IMC — Classificação OMS

```
IMC = Peso (kg) / Altura² (m)
```

| Faixa | Classificação |
|-------|---------------|
| < 16.0 | Magreza grau III (grave) |
| 16.0 – 16.9 | Magreza grau II (moderada) |
| 17.0 – 18.4 | Magreza grau I (leve) |
| 18.5 – 24.9 | Eutrófico (normal) |
| 25.0 – 29.9 | Sobrepeso (pré-obeso) |
| 30.0 – 34.9 | Obesidade grau I |
| 35.0 – 39.9 | Obesidade grau II |
| ≥ 40.0 | Obesidade grau III (mórbida) |

### RCQ — Relação Cintura/Quadril

```
RCQ = Cintura (cm) / Quadril (cm)
```

| Gênero | Baixo Risco | Moderado | Alto Risco |
|--------|-------------|----------|------------|
| Masculino | ≤ 0.90 | 0.91 – 0.99 | ≥ 1.00 |
| Feminino | ≤ 0.80 | 0.81 – 0.84 | ≥ 0.85 |

### Gordura Corporal por Dobras Cutâneas

#### Jackson & Pollock 3 Dobras (1985)
```
Homens (peitoral, abdominal, coxa):
  Dc = 1.10938 – 0.0008267(S) + 0.0000016(S²) – 0.0002574(idade)

Mulheres (tríceps, suprailíaca, coxa):
  Dc = 1.0994921 – 0.0009929(S) + 0.0000023(S²) – 0.0001392(idade)

%Gordura (Siri): (4.95 / Dc - 4.50) × 100
```

#### Jackson & Pollock 7 Dobras (1978)
```
Dobras: peitoral, axilar média, tríceps, subescapular, abdominal, suprailíaca, coxa

Homens:
  Dc = 1.112 – 0.00043499(S) + 0.00000055(S²) – 0.00028826(idade)

Mulheres:
  Dc = 1.097 – 0.00046971(S) + 0.00000056(S²) – 0.00012828(idade)
```

### Peso Ideal

#### Devine (1974)
```
Homens:   50.0 + 2.3 × (altura_polegadas - 60)
Mulheres: 45.5 + 2.3 × (altura_polegadas - 60)
```

#### IMC Ideal (22 kg/m²)
```
PI = 22 × altura(m)²
```

### Macronutrientes

| Nutriente | Estratégia |
|-----------|-----------|
| Proteína | g/kg de peso (1.8 a 2.4 conforme objetivo) |
| Gordura | 0.9 g/kg (regulação hormonal) |
| Carboidrato | Restante calórico ÷ 4 |
| Fibra | 14g / 1000 kcal |
| Água | 35ml / kg |

---

## Exemplo de Requisição

### POST `/api/AvaliacaoNutricional/registrar`

```json
{
  "pesoKg": 85.5,
  "alturaCm": 178.0,
  "circunferenciaCinturaCm": 88.0,
  "circunferenciaQuadrilCm": 102.0,
  "circunferenciaBracoDireitoCm": 35.0,
  "circunferenciaCoxaDireitaCm": 58.0,
  "protocoloDobrasCutaneas": 1,
  "dobraPeitoralMm": 12.0,
  "dobraAbdominalMm": 25.0,
  "dobraCoxaMm": 18.0,
  "possuiBioimpedancia": false,
  "fotosProgresso": [
    {
      "url": "https://storage.example.com/fotos/frente.jpg",
      "tipo": 1,
      "descricao": "Foto frontal antes do início"
    }
  ],
  "observacoes": "Avaliação inicial"
}
```

### Resposta Esperada (simplificada)

```json
{
  "id": 1,
  "dataAvaliacao": "2026-02-24T10:30:00Z",
  "pesoKg": 85.5,
  "alturaCm": 178.0,
  "imc": 26.97,
  "classificacaoIMC": "Sobrepeso (pré-obeso)",
  "circunferencias": {
    "cinturaCm": 88.0,
    "quadrilCm": 102.0,
    "rcq": 0.86,
    "classificacaoRCQ": "Risco baixo"
  },
  "dobrasCutaneas": {
    "protocolo": 1,
    "peitoralMm": 12.0,
    "abdominalMm": 25.0,
    "coxaMm": 18.0,
    "somatorioDobras": 55.0,
    "densidadeCorporal": 1.0523,
    "percentualGorduraEstimado": 20.25
  },
  "calculos": {
    "tmbMifflinStJeor": 1812.5,
    "tmbHarrisBenedict": 1878.3,
    "tmbKatchMcArdle": 1843.1,
    "get": 2493.5,
    "taxaMetabolicaAjustada": 1994.8
  },
  "composicaoCorporal": {
    "percentualGordura": 20.25,
    "fontePercentualGordura": "Dobras cutâneas (JacksonPollock3)",
    "massaMagraKg": 68.18,
    "massaGordaKg": 17.31,
    "pesoIdealDevineKg": 73.3,
    "pesoIdealIMCKg": 69.7,
    "diferencaPesoIdealKg": 15.8
  },
  "macrosRecomendados": {
    "caloriasAlvo": 1994.8,
    "proteinaG": 188.0,
    "carboidratoG": 119.0,
    "gorduraG": 77.0,
    "fibraG": 28.0,
    "aguaLitros": 3.0,
    "percentualProteina": 37.7,
    "percentualCarboidrato": 23.8,
    "percentualGordura": 34.7
  },
  "fotosProgresso": [
    {
      "url": "https://storage.example.com/fotos/frente.jpg",
      "tipo": 1,
      "descricao": "Foto frontal antes do início"
    }
  ]
}
```

---

## Fluxo de Dados

```
1. Paciente ou Nutricionista envia medidas via POST
                    ↓
2. AvaliacaoNutricionalService recebe o DTO
                    ↓
3. Busca PerfilNutricional (idade, gênero, objetivo, nível atividade)
                    ↓
4. CalculadoraNutricionalService executa fórmulas:
   ├─ IMC + Classificação OMS
   ├─ RCQ + Risco cardiovascular
   ├─ TMB (Mifflin, Harris-Benedict, Katch-McArdle*)
   ├─ GET (TMB × Fator Atividade)
   ├─ %Gordura (Dobras JP3/JP7 e/ou Bioimpedância)
   ├─ Peso Ideal (Devine + IMC)
   ├─ Taxa Ajustada ao Objetivo
   └─ Distribuição de Macronutrientes
                    ↓
5. Persiste AvaliacaoAntropometrica + FotosProgresso
                    ↓
6. Atualiza PerfilNutricional com dados mais recentes
                    ↓
7. Retorna resultado completo mapeado
```

---

## Correções na Etapa 1

| Correção | Descrição |
|----------|-----------|
| Bug DI `IRefeicao` | `builder.Services.AddScoped<IRefeicao, RefeicaoService>()` adicionado ao `Program.cs` |
| `CalculadoraNutricionalService` | Métodos tornados `public` para uso pela `AvaliacaoNutricionalService` |
| `PerfilNutricional` | Adicionada navegação para `AvaliacoesAntropometricas` |

---

## Migração do Banco

Após a implementação, gerar a migração com:

```bash
dotnet ef migrations add AddAvaliacaoNutricional --project Nutra.csproj
dotnet ef database update --project Nutra.csproj
```
