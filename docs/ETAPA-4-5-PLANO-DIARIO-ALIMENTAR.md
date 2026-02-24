# Etapa 4: Plano Alimentar — Documentação

## Visão Geral

O módulo de Plano Alimentar permite a criação, gerenciamento e acompanhamento de planos nutricionais personalizados. Suporta criação por pacientes ou profissionais, estruturação hierárquica (plano → refeições → itens), substituições equivalentes, modelos de dieta reutilizáveis e cálculo automático de macros.

---

## Arquitetura

### Entidades (Models)

| Entidade | Descrição |
|----------|-----------|
| `PlanoAlimentar` | Plano principal com metas diárias, status e período |
| `RefeicaoPlano` | Refeição dentro do plano (café, almoço, etc.) com horário e totais |
| `ItemRefeicao` | Alimento específico com quantidade e macros calculados |
| `SubstituicaoEquivalente` | Alternativa equivalente para um item |
| `ModeloDieta` | Template reutilizável de plano alimentar |
| `RefeicaoModeloDieta` | Refeição dentro de um modelo |
| `ItemModeloDieta` | Item dentro de refeição do modelo |

### Enum

| Enum | Valores |
|------|---------|
| `EStatusPlano` | Rascunho(1), Ativo(2), Pausado(3), Finalizado(4), Cancelado(5) |

### Hierarquia

```
PlanoAlimentar
├── RefeicaoPlano (café da manhã)
│   ├── ItemRefeicao (aveia 50g → 195kcal)
│   │   ├── SubstituicaoEquivalente (granola 40g → 180kcal)
│   │   └── SubstituicaoEquivalente (tapioca 60g → 190kcal)
│   └── ItemRefeicao (banana 120g → 107kcal)
├── RefeicaoPlano (almoço)
│   ├── ItemRefeicao (arroz 150g)
│   └── ItemRefeicao (frango 120g)
└── ...
```

---

## Endpoints da API

### PlanoAlimentarController (`/api/PlanoAlimentar`)

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/` | Cria plano para o usuário autenticado |
| `POST` | `/profissional` | Cria plano para paciente (profissional) |
| `GET` | `/` | Lista todos os planos do usuário |
| `GET` | `/{planoId}` | Obtém plano específico completo |
| `GET` | `/ativo` | Obtém o plano ativo do usuário |
| `PUT` | `/{planoId}` | Atualiza informações do plano |
| `DELETE` | `/{planoId}` | Exclui um plano |
| `POST` | `/{planoId}/ativar` | Ativa plano (desativa anterior) |
| `POST` | `/{planoId}/refeicoes` | Adiciona refeição ao plano |
| `DELETE` | `/refeicoes/{refeicaoId}` | Remove refeição |
| `POST` | `/refeicoes/{refeicaoId}/itens` | Adiciona item à refeição |
| `DELETE` | `/itens/{itemId}` | Remove item |
| `POST` | `/itens/{itemId}/substituicoes` | Adiciona substituição |
| `DELETE` | `/substituicoes/{substituicaoId}` | Remove substituição |
| `POST` | `/modelos` | Cria modelo de dieta (profissional) |
| `GET` | `/modelos` | Lista modelos disponíveis |
| `GET` | `/modelos/{modeloId}` | Detalhes do modelo |
| `DELETE` | `/modelos/{modeloId}` | Exclui modelo (soft delete) |
| `POST` | `/modelos/{modeloId}/criar-plano` | Cria plano a partir de modelo |

---

## Funcionalidades Principais

### 1. Criação de Plano
- Metas preenchidas automaticamente a partir do `MetaNutricional` do perfil
- Suporta definição manual de metas calóricas
- Criação completa (com refeições e itens) em um único POST

### 2. Distribuição por Horário
- Cada refeição tem `HorarioSugerido` (TimeSpan)
- Ordem de apresentação configurável
- Percentual calórico por refeição calculado automaticamente

### 3. Cálculo Automático de Macros
- Macros proporcionais calculados com base na porção de referência do alimento
- Totais por refeição recalculados ao adicionar/remover itens
- `TotaisCalculados` somatório de todas as refeições
- `DiferencaMetas` = metas - calculados (para ver se o plano está equilibrado)

### 4. Substituições Equivalentes
- Cada item pode ter N substituições
- Substituições são alimentos alternativos com quantidade ajustada
- Macros calculados proporcionalmente para cada substituição

### 5. Modelos de Dieta (Templates)
- Templates reutilizáveis com objetivo e preferência alimentar
- Podem ser públicos ou privados do profissional
- Escalonamento automático ao criar plano: ajusta quantidades proporcionalmente às metas calóricas do paciente

### 6. Ativação de Plano
- Apenas 1 plano ativo por vez
- Ao ativar um plano, o anterior é pausado automaticamente
- O plano ativo é usado no Diário Alimentar para comparação

---

## Exemplo de Payload

### Criar Plano Completo

```json
POST /api/PlanoAlimentar
{
  "nome": "Plano de Perda de Gordura - Semana 1",
  "descricao": "Plano hipocalórico focado em proteína",
  "dataInicio": "2025-02-01",
  "dataFim": "2025-02-28",
  "refeicoes": [
    {
      "tipoRefeicao": 1,
      "horarioSugerido": "07:00:00",
      "ordem": 1,
      "itens": [
        {
          "alimentoId": 42,
          "tipoTabela": 1,
          "quantidadeG": 50,
          "ordem": 1,
          "substituicoes": [
            { "alimentoId": 99, "tipoTabela": 4, "quantidadeG": 40 }
          ]
        }
      ]
    }
  ]
}
```

### Resposta

A resposta inclui o plano completo com:
- `metasDiarias` — calorias/macros alvo
- `totaisCalculados` — soma de todos os itens
- `diferencaMetas` — delta entre meta e calculado
- Cada refeição com `percentualCaloricoRefeicao`
- Cada item com macros individuais e substituições

---

# Etapa 5: Diário Alimentar — Documentação

## Visão Geral

O Diário Alimentar permite registrar o consumo real de alimentos, comparar com o plano alimentar ativo, adicionar fotos de refeições, e gerar relatórios de aderência. Suporta registro por código de barras e registro em lote.

---

## Endpoints da API

### DiarioAlimentarController (`/api/DiarioAlimentar`)

| Método | Rota | Descrição |
|--------|------|-----------|
| `POST` | `/consumo` | Registra consumo de um alimento |
| `POST` | `/consumo/lote` | Registra múltiplos consumos de uma vez |
| `DELETE` | `/consumo/{registroId}` | Exclui registro de consumo |
| `POST` | `/fotos` | Adiciona foto de refeição |
| `DELETE` | `/fotos/{fotoId}` | Remove foto |
| `GET` | `/fotos?data=` | Lista fotos do dia |
| `GET` | `/dia?data=` | Diário completo do dia (planejado vs consumido) |
| `GET` | `/periodo?dataInicio=&dataFim=` | Diário de múltiplos dias |
| `GET` | `/relatorio-adesao?dataInicio=&dataFim=` | Relatório de aderência próprio |
| `GET` | `/relatorio-adesao/paciente/{id}?dataInicio=&dataFim=` | Relatório do paciente (profissional) |

---

## Funcionalidades Principais

### 1. Registro de Consumo
- Busca o alimento nas 4 tabelas (Tbca, Fabricantes, FastFood, Genéricos)
- Calcula macros proporcionalmente à quantidade consumida
- Salva snapshot completo dos dados nutricionais em JSON
- Suporte a código de barras e vinculação com item do plano

### 2. Registro em Lote
- Permite registrar toda uma refeição de uma vez
- Aceita array de itens com diferentes alimentos e quantidades

### 3. Fotos de Refeição
- Upload via URL da foto
- Associação por tipo de refeição (café, almoço, etc.)
- Vinculação opcional a registro alimentar específico

### 4. Diário do Dia (Planejado vs Consumido)
- Compara automaticamente com o plano alimentar ativo
- Para cada refeição mostra:
  - **Planejado**: macros esperados do plano
  - **Consumido**: macros reais dos registros
  - **Percentual de aderência** por refeição
- Totais do dia com saldo restante
- Fotos do dia incluídas
- Fallback para MetaNutricional se não houver plano ativo

### 5. Relatório de Aderência
Gera análise completa de um período com:

| Métrica | Descrição |
|---------|-----------|
| `DiasComRegistro` | Quantos dias tiveram registros |
| `AderenciaCaloricoMediaPercent` | % médio de calorias consumidas vs meta |
| `AderenciaProteinaMediaPercent` | % médio de proteína |
| `AderenciaCarboidratoMediaPercent` | % médio de carboidrato |
| `AderenciaGorduraMediaPercent` | % médio de gordura |
| `MediaDiariaConsumida` | Macros médios por dia |
| `AderenciaPorRefeicao` | Aderência por tipo (café, almoço, etc.) |
| `HistoricoDiario` | Detalhamento dia a dia (tendência) |

---

## Exemplos de Payload

### Registrar Consumo

```json
POST /api/DiarioAlimentar/consumo
{
  "alimentoId": 42,
  "tipoTabela": 1,
  "quantidadeConsumidaG": 150,
  "tipoRefeicao": 2,
  "codigoBarras": "7891234567890",
  "itemRefeicaoPlanoId": 15
}
```

### Registrar Lote

```json
POST /api/DiarioAlimentar/consumo/lote
{
  "itens": [
    { "alimentoId": 42, "tipoTabela": 1, "quantidadeConsumidaG": 150, "tipoRefeicao": 2 },
    { "alimentoId": 10, "tipoTabela": 4, "quantidadeConsumidaG": 120, "tipoRefeicao": 2 },
    { "alimentoId": 55, "tipoTabela": 1, "quantidadeConsumidaG": 200, "tipoRefeicao": 2 }
  ]
}
```

### Diário do Dia (Resposta simplificada)

```json
GET /api/DiarioAlimentar/dia?data=2025-02-15
{
  "data": "2025-02-15",
  "metasDoDia": { "caloriasKcal": 2100, "proteinaG": 160, ... },
  "totalConsumido": { "caloriasKcal": 1850, "proteinaG": 145, ... },
  "saldoRestante": { "caloriasKcal": 250, "proteinaG": 15, ... },
  "percentualAderenciaCaloricas": 88.1,
  "refeicoes": [
    {
      "tipoRefeicao": 1,
      "horarioPlanejado": "07:00:00",
      "planejado": { "energiaKcal": 450, ... },
      "consumido": { "energiaKcal": 420, ... },
      "percentualAderencia": 93.3,
      "registros": [ ... ]
    }
  ],
  "fotos": [ ... ]
}
```

---

## Integração entre Etapas

```
Etapa 2 (Avaliação) → gera MetaNutricional → alimenta metas do Plano
                                                        ↓
Etapa 4 (Plano Alimentar) → define refeições planejadas
                                                        ↓
Etapa 5 (Diário Alimentar) → registra consumo real → compara com plano
                                                        ↓
                                            Relatório de Aderência
```

## Arquivos Criados/Modificados

### Novos Arquivos
- `Enum/EStatusPlano.cs`
- `Models/RegraNutricional/PlanoAlimentar.cs`
- `Models/RegraNutricional/RefeicaoPlano.cs`
- `Models/RegraNutricional/ItemRefeicao.cs`
- `Models/RegraNutricional/SubstituicaoEquivalente.cs`
- `Models/RegraNutricional/ModeloDieta.cs`
- `Models/RegraNutricional/RefeicaoModeloDieta.cs`
- `Models/RegraNutricional/ItemModeloDieta.cs`
- `Models/RegraNutricional/FotoRefeicao.cs`
- `Models/Dtos/PlanoAlimentarDtos.cs`
- `Models/Dtos/DiarioAlimentarDtos.cs`
- `Interfaces/IPlanoAlimentar.cs`
- `Interfaces/IDiarioAlimentar.cs`
- `Services/PlanoAlimentarService.cs`
- `Services/DiarioAlimentarService.cs`
- `Controllers/PlanoAlimentarController.cs`
- `Controllers/DiarioAlimentarController.cs`

### Arquivos Modificados
- `Data/AlimentosContext.cs` — novos DbSets e relationships
- `Program.cs` — DI registration para IPlanoAlimentar e IDiarioAlimentar
- `Models/RegraNutricional/RegistroAlimentar.cs` — campos PlanoAlimentarId, ItemRefeicaoPlanoId, CodigoBarras, FotosRefeicao
- `Models/Usuario/PerfilNutricional.cs` — navigation PlanosAlimentares
