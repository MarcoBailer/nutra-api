# Fluxo Completo da Aplicação - Nutra Food API

## 📋 Índice
1. [Visão Geral](#visão-geral)
2. [Fluxo de Cadastro e Autenticação](#1-fluxo-de-cadastro-e-autenticação)
3. [Fluxo do Paciente](#2-fluxo-do-paciente)
4. [Fluxo do Nutricionista](#3-fluxo-do-nutricionista)
5. [Fluxo de Avaliação Nutricional](#4-fluxo-de-avaliação-nutricional)
6. [Fluxo de Plano Alimentar](#5-fluxo-de-plano-alimentar)
7. [Fluxo de Diário Alimentar](#6-fluxo-de-diário-alimentar)
8. [Mapa de Dependências](#7-mapa-de-dependências)
9. [Guia de Telas para Frontend](#8-guia-de-telas-para-frontend)

---

## Visão Geral

A aplicação Nutra Food é um sistema de gerenciamento nutricional que suporta dois tipos de usuários:
- **Pacientes**: Pessoas que buscam acompanhamento nutricional
- **Nutricionistas**: Profissionais que gerenciam pacientes

### Hierarquia Principal de Entidades

```
ApplicationUser (AspNetUser)
├── Role: Paciente
│   ├── PerfilNutricional (OBRIGATÓRIO)
│   │   ├── MetaNutricional (AUTO-GERADA)
│   │   ├── RestricoesAlimentares
│   │   ├── PreferenciasAlimentares
│   │   ├── EquipamentoDisponivel
│   │   ├── HistoricoClinico
│   │   ├── RegistroBiometrico
│   │   ├── AnamneseAlimentar (OPCIONAL)
│   │   ├── AvaliacaoAntropometrica (OPCIONAL)
│   │   └── PlanoAlimentar (OPCIONAL)
│   └── VinculosComoPaciente (com nutricionistas)
│
└── Role: Nutricionista
    ├── PerfilProfissional (OBRIGATÓRIO)
    │   ├── Assinatura
    │   └── Clinicas (OPCIONAL - depende do plano)
    └── Pacientes (vínculos)
```

---

## 1. Fluxo de Cadastro e Autenticação

### 1.1 Registro de Usuário

**Endpoint**: `POST /api/Auth/register`

**Modelo**: `RegisterModelDto`
- Email **(obrigatório)**
- Password **(obrigatório)**
- NomeCompleto **(obrigatório)**
- CPF **(obrigatório)**
- Role **(obrigatório)**: `Paciente` ou `Nutricionista`
- DataNascimento (opcional)
- Telefone (opcional)

**Processo**:
1. Sistema cria `ApplicationUser`
2. Define `Role` (Paciente ou Nutricionista)
3. Retorna token JWT

**Tela Frontend**:
- ✅ **Tela de Registro** (`/register`)
  - Radio button para escolher tipo de conta (Paciente/Nutricionista)
  - Campos: Email, Senha, Nome Completo, CPF
  - Campos opcionais: Data Nascimento, Telefone

### 1.2 Login

**Endpoint**: `POST /api/Auth/login`

**Modelo**: `LoginModelDto`
- Email
- Password

**Retorno**: `AuthResponseDto`
- Token JWT
- User info (id, email, role, nomeCompleto)

**Tela Frontend**:
- ✅ **Tela de Login** (`/login`)
  - Email e senha
  - Link para registro

---

## 2. Fluxo do Paciente

### 2.1 Setup Inicial - Perfil Nutricional (OBRIGATÓRIO)

Após o login, **TODO PACIENTE DEVE TER UM PERFIL NUTRICIONAL**.

**Verificação**:
```
GET /api/User/perfil-nutricional
- Se retornar 404 → Redirecionar para criação de perfil
- Se retornar 200 → Perfil existe
```

**Endpoint de Criação**: `POST /api/User/perfil-nutricional`

**Modelo**: `PerfilNutricionalDto`

#### Campos Obrigatórios:
- **Dados Pessoais**:
  - DataNascimento
  - Genero (`EGeneroBiologico`: Masculino, Feminino)

- **Medidas Corporais**:
  - AlturaCm
  - PesoAtualKg
  - PercentualGorduraCorporal (opcional)
  - CircunferenciaCinturaCm (opcional)
  - CircunferenciaQuadrilCm (opcional)
  - CircunferenciaBracoCm (opcional)

- **Atividade e Estilo de Vida**:
  - FatorAtividade (decimal)
  - NivelAtividade (`ENivelAtividadeFisica`: Sedentario, Leve, Moderado, Intenso, MuitoIntenso)
  - OcupacaoProfissional
  - HabilidadeCulinaria (`ENivelHabilidadeCulinaria`: Basico, Intermediario, Avancado, Profissional)
  - OrcamentoMensal (`EOrcamentoMensalEstimado`: Baixo, Medio, Alto, Muito Alto)

- **Saúde**:
  - PossuiDoencasPreExistentes (bool)
  - DescricaoCondicoesMedicas (se possui doenças)
  - Fumante (bool)
  - QualidadeSono (1-5)
  - HorasSonoPorNoite

- **Objetivos**:
  - Objetivo (`ETipoObjetivo`: Perda Peso, Ganho Massa, Manutencao, Cutting, Bulking, etc.)
  - PesoDesejadoKg (opcional)

- **Preferências Alimentares**:
  - PreferenciaDieta (`EPreferenciaAlimentar`: Onivoro, Vegetariano, Vegano, Cetogenica, Paleo, etc.)
  - RefeicoesPorDiaDesejadas
  - TempoDisponivelPreparoMinutos

- **Arrays/Listas**:
  - RestricoesIds (array de `EAlergico`): Lactose, Gluten, Amendoim, etc.
  - EquipamentosIds (array de `EEquipamentoDisponivel`): Fogao, Microondas, AirFryer, etc.
  - Preferencias (array de `PreferenciaCadastroDto`):
    - AlimentoId
    - Tabela (`ETipoTabela`)
    - Tipo (`ETipoPreferencia`: Gosta, NaoGosta, NuncaExperimentou)
  - HistoricoClinicos (array de `HistoricoClinicoDto`):
    - Condicao (`ECondicaoClinica`)
    - DescricaoOutra
    - DataDiagnostico
    - AtivaAtualmente
    - MedicamentosEmUso
    - Observacoes

**Processo Automático**:
1. Sistema cria `PerfilNutricional`
2. Sistema **GERA AUTOMATICAMENTE** `MetaNutricional`
3. Sistema cria primeiro `RegistroBiometrico`
4. Sistema vincula perfil ao usuário

**Telas Frontend**:
- ✅ **Wizard/Formulário Multi-Step de Criação de Perfil** (`/onboarding/perfil`)
  - **Step 1**: Dados Pessoais (data nascimento, gênero)
  - **Step 2**: Medidas Corporais (altura, peso, circunferências)
  - **Step 3**: Estilo de Vida (atividade física, ocupação, sono)
  - **Step 4**: Saúde (doenças, fumante, histórico clínico)
  - **Step 5**: Objetivos (perda/ganho de peso, peso desejado)
  - **Step 6**: Preferências Alimentares (dieta, refeições/dia, tempo preparo)
  - **Step 7**: Restrições (alergias, intolerâncias)
  - **Step 8**: Equipamentos (fogão, micro-ondas, air fryer, etc.)
  - **Step 9**: Preferências de Alimentos (buscar alimentos e marcar como gosta/não gosta)
  - **Conclusão**: Mostra resumo e cria perfil

### 2.2 Meta Nutricional (AUTO-GERADA)

**IMPORTANTE**: A MetaNutricional é **SEMPRE GERADA AUTOMATICAMENTE** ao criar/atualizar o perfil.

**Conteúdo**:
- CaloriasDiarias (calculado)
- ProteinasDiarias (calculado)
- CarboidratosDiarios (calculado)
- GordurasDiarias (calculado)
- AguaDiaria (calculado)
- FibraDiaria (calculado)

**Visualização**: `GET /api/User/meta-nutricional`

**Tela Frontend**:
- ✅ **Dashboard/Home do Paciente** (`/dashboard`)
  - Card com metas diárias
  - Progress bars para cada macro
  - Comparação com consumo do dia

### 2.3 Atualização de Perfil

**Endpoint**: `PUT /api/User/perfil-nutricional`

**Processo**:
1. Atualiza dados do perfil
2. **RECALCULA** automaticamente a `MetaNutricional`
3. Cria nova meta com novo ID

**Tela Frontend**:
- ✅ **Editar Perfil Nutricional** (`/perfil/editar`)
  - Mesmo formulário do onboarding, mas com dados pré-preenchidos

### 2.4 Registro Biométrico

**Endpoint**: `POST /api/User/perfil-nutricional/registro-biometrico`

**Modelo**: `RegistroBiometricoDto`
- PesoKg
- PercentualGordura (opcional)
- CircunferenciaCinturaCm (opcional)
- Data

**Propósito**: Acompanhar evolução do peso e medidas ao longo do tempo.

**Telas Frontend**:
- ✅ **Registrar Peso/Medidas** (`/registros/peso`)
  - Form simples para registrar peso e medidas
- ✅ **Histórico de Peso** (`/registros/peso/historico`)
  - Gráfico de evolução
  - Lista de registros

### 2.5 Preferências Alimentares (Dinâmicas)

**Endpoint**: `POST /api/User/preferencia-alimentar`

**Parâmetros**:
- AlimentoId
- Tabela (`ETipoTabela`)
- Tipo (`ETipoPreferencia`)

**Tela Frontend**:
- ✅ **Gerenciar Preferências** (`/preferencias`)
  - Buscar alimentos
  - Marcar como: Gosta / Não Gosta / Nunca Experimentou

---

## 3. Fluxo do Nutricionista

### 3.1 Setup Inicial - Perfil Profissional (OBRIGATÓRIO)

Após login como Nutricionista, **DEVE TER PERFIL PROFISSIONAL**.

**Verificação**:
```
GET /api/Nutricionista/perfil
- Se retornar 404 → Redirecionar para criação
- Se retornar 200 → Perfil existe
```

**Endpoint**: `POST /api/Nutricionista/cadastro`

**Modelo**: `CadastroNutricionistaDto`
- Email
- NomeCompleto
- CPF
- Telefone
- **CRN** (obrigatório)
- **CRNRegiao** (1-11)
- Especialidade (opcional)
- BioProfissional (opcional)
- AnosExperiencia (opcional)

**Processo Automático**:
1. Cria/atualiza `ApplicationUser` com Role Nutricionista
2. Cria `PerfilProfissional`
3. Cria `Assinatura` inicial (Gratuito/Trial)
4. Define limites: MaxPacientes = 5, MultiClinicaHabilitado = false

**Tela Frontend**:
- ✅ **Cadastro de Nutricionista** (`/onboarding/nutricionista`)
  - Campos profissionais: CRN, região, especialidade
  - Upload de diploma (futuro)

### 3.2 Assinatura

**Modelo**: `Assinatura`
- Plano (`EPlanoAssinatura`): Gratuito, Basico, Profissional, Enterprise
- Status (`EStatusAssinatura`): Trial, Ativa, Cancelada, Suspensa
- MaxPacientes (baseado no plano)
- MultiClinicaHabilitado (apenas Enterprise)

**Limites por Plano**:
- Gratuito: 5 pacientes, 1 clínica
- Basico: 15 pacientes, 1 clínica
- Profissional: 50 pacientes, 1 clínica
- Enterprise: Ilimitado, múltiplas clínicas

**Tela Frontend**:
- ✅ **Gerenciar Assinatura** (`/assinatura`)
  - Mostrar plano atual
  - Limites e uso atual
  - Opção de upgrade

### 3.3 Clínicas (OPCIONAL)

**Endpoints**:
- `POST /api/Nutricionista/clinicas` - Criar
- `PUT /api/Nutricionista/clinicas/{id}` - Atualizar
- `DELETE /api/Nutricionista/clinicas/{id}` - Remover
- `GET /api/Nutricionista/clinicas` - Listar

**Modelo**: `ClinicaDto`
- Nome
- CNPJ (opcional)
- Telefone, Email
- Endereço completo
- LogoUrl (opcional)

**Restrições**:
- Planos Gratuito/Basico/Profissional: apenas 1 clínica
- Plano Enterprise: múltiplas clínicas

**Telas Frontend**:
- ✅ **Gerenciar Clínicas** (`/clinicas`)
  - Listar clínicas
  - Criar/editar clínica
  - Bloquear criação se limite atingido

### 3.4 Vínculo com Pacientes

**Modelo**: `VinculoPacienteProfissional`
- Status: Pendente, Ativo, Recusado, Encerrado
- DataConvite, DataAceite, DataEncerramento
- ClinicaId (opcional)
- Observacoes

**Fluxo**:
1. Nutricionista envia convite: `POST /api/Nutricionista/pacientes/convidar`
2. Paciente aceita/recusa: `POST /api/User/vinculos/{id}/aceitar` ou `/recusar`
3. Vínculo fica Ativo ou Recusado
4. Nutricionista pode encerrar: `POST /api/Nutricionista/pacientes/{id}/encerrar`

**Endpoints Nutricionista**:
- `POST /api/Nutricionista/pacientes/convidar` - Enviar convite
- `GET /api/Nutricionista/pacientes` - Listar pacientes ativos
- `GET /api/Nutricionista/pacientes/{id}` - Ver detalhes do paciente
- `POST /api/Nutricionista/pacientes/{id}/encerrar` - Encerrar vínculo

**Endpoints Paciente**:
- `GET /api/User/vinculos` - Listar convites e vínculos
- `POST /api/User/vinculos/{id}/aceitar` - Aceitar convite
- `POST /api/User/vinculos/{id}/recusar` - Recusar convite

**Telas Frontend**:

**Para Nutricionista**:
- ✅ **Lista de Pacientes** (`/pacientes`)
  - Card/lista de pacientes ativos
  - Informações resumidas (nome, objetivo, meta nutricional)
  - Botão para ver detalhes
- ✅ **Convidar Paciente** (`/pacientes/convidar`)
  - Buscar paciente por email ou CPF
  - Selecionar clínica (se tiver múltiplas)
  - Adicionar observações
- ✅ **Detalhes do Paciente** (`/pacientes/{id}`)
  - Perfil nutricional completo
  - Avaliações antropométricas
  - Planos alimentares
  - Diário alimentar
  - Opção de encerrar vínculo

**Para Paciente**:
- ✅ **Convites Pendentes** (`/vinculos`)
  - Lista de convites de nutricionistas
  - Botões aceitar/recusar
- ✅ **Meu Nutricionista** (`/nutricionista`)
  - Dados do nutricionista vinculado
  - Informações da clínica
  - Opção de encerrar vínculo

---

## 4. Fluxo de Avaliação Nutricional

### 4.1 Visão Geral

Avaliação Antropométrica é um **SNAPSHOT** detalhado do estado físico do paciente em um momento específico.

**Características**:
- OPCIONAL (não obrigatória)
- Pode ser feita pelo próprio paciente (auto-avaliação)
- Pode ser feita por nutricionista (mais completa)
- Histórico de múltiplas avaliações para acompanhamento

### 4.2 Registrar Avaliação

**Endpoints**:
- `POST /api/Avaliacao/registrar` - Auto-avaliação (paciente)
- `POST /api/Avaliacao/profissional/registrar` - Avaliação por nutricionista

**Modelo**: `AvaliacaoAntropometricaDto`

#### Campos Básicos:
- PesoKg **(obrigatório)**
- AlturaCm **(obrigatório)**
- Observacoes

#### Circunferências (todas opcionais):
- CircunferenciaPescocoCm
- CircunferenciaToraxCm
- CircunferenciaCinturaCm
- CircunferenciaAbdomenCm
- CircunferenciaQuadrilCm
- CircunferenciaBracoDireitoCm / EsquerdoCm
- CircunferenciaAntebracoDireitoCm / EsquerdoCm
- CircunferenciaCoxaDireitaCm / EsquerdaCm
- CircunferenciaPanturrilhaDireitaCm / EsquerdaCm

#### Dobras Cutâneas (opcionais):
- ProtocoloDobrasCutaneas (Jackson-Pollock 3/7, etc.)
- DobraTricepsMm
- DobraBicepsMm
- DobraSubescapularMm
- DobraSuprailiacaMm
- DobraAbdominalMm
- DobraCoxaMm
- DobraPanturrilhaMm
- DobraAxilarMediaMm
- DobraPeitoralMm

#### Bioimpedância (opcional):
- PossuiBioimpedancia (bool)
- BioPercentualGordura
- BioMassaMagraKg
- BioMassaGordaKg
- BioAguaCorporalLitros
- BioPercentualAgua
- BioTMBKcal
- BioGorduraVisceralNivel
- BioIdadeMetabolica
- BioMassaOsseaKg

**Cálculos Automáticos**:
Sistema calcula automaticamente:
- IMC e classificação
- TMB (Mifflin-St Jeor, Harris-Benedict, Katch-McArdle)
- GET (Gasto Energético Total)
- Peso ideal (Devine, IMC)
- Taxa ajustada ao objetivo
- Densidade corporal (se tem dobras)
- Percentual de gordura estimado
- Massa magra e massa gorda
- RCQ (Relação Cintura/Quadril)

**Retorno**: `AvaliacaoAntropometricaResultadoDto` com todos os cálculos.

### 4.3 Fotos de Progresso

**Endpoints**:
- `POST /api/Avaliacao/{avaliacaoId}/fotos` - Adicionar fotos
- `DELETE /api/Avaliacao/fotos/{fotoId}` - Remover foto

**Modelo**: `FotoProgressoDto`
- Url (URL da imagem)
- Tipo (`ETipoFotoProgresso`): Frente, Costas, LateralDireito, LateralEsquerdo, Rosto, Outro
- Descricao
- DataFoto

### 4.4 Comparação de Avaliações

**Endpoint**: `GET /api/Avaliacao/comparar?anteriorId={id1}&atualId={id2}`

**Retorno**: `ComparacaoAvaliacoesDto`
- Delta de peso
- Delta de IMC
- Delta de % gordura
- Delta de massa magra/gorda
- Delta de circunferências
- Dias entre avaliações

**Telas Frontend**:

**Para Paciente**:
- ✅ **Nova Avaliação** (`/avaliacao/nova`)
  - Form multi-step para avaliação
  - Step 1: Peso e altura
  - Step 2: Circunferências (opcional)
  - Step 3: Dobras cutâneas (opcional)
  - Step 4: Bioimpedância (opcional)
  - Step 5: Upload de fotos (opcional)
  - Mostrar preview dos cálculos antes de salvar
  
- ✅ **Histórico de Avaliações** (`/avaliacoes`)
  - Timeline de avaliações
  - Cards com resumo (data, peso, IMC, % gordura)
  - Link para detalhes
  
- ✅ **Detalhes da Avaliação** (`/avaliacoes/{id}`)
  - Todos os dados e cálculos
  - Fotos de progresso
  - Gráficos de evolução
  
- ✅ **Comparar Avaliações** (`/avaliacoes/comparar`)
  - Selecionar 2 avaliações
  - Visualização lado a lado
  - Deltas calculados
  - Comparação de fotos

**Para Nutricionista**:
- ✅ **Registrar Avaliação do Paciente** (`/pacientes/{id}/avaliacao`)
  - Mesmo form, mas mais completo
  - Pode adicionar observações profissionais
  
- ✅ **Ver Avaliações do Paciente** (`/pacientes/{id}/avaliacoes`)
  - Histórico completo
  - Comparações

### 4.5 Anamnese Alimentar (OPCIONAL)

**Endpoint**: `POST /api/Avaliacao/anamnese`

**Modelo**: `AnamneseAlimentarDto`

Questionário detalhado sobre hábitos alimentares:
- RefeicoesPorDia
- Horários das refeições
- RefeicoesPuladas
- ConsumoAguaLitrosDia
- Frequências de consumo (refrigerantes, álcool, café, fast food, frutas, verduras, doces, frituras)
- Comportamento alimentar (come com distração, compulsão, histórico de dietas)
- SuplementosEmUso
- IntestinoRegular, FrequenciaEvacuacaoSemana
- QueixasDigestivas
- AlimentosQueNaoGosta / AlimentosPreferidos
- ObservacoesGerais

**Tela Frontend**:
- ✅ **Anamnese Alimentar** (`/anamnese`)
  - Questionário extenso
  - Dividido em seções
  - Salvar e continuar depois (opcional)

---

## 5. Fluxo de Plano Alimentar

### 5.1 Visão Geral

Plano Alimentar é uma **prescrição nutricional estruturada** com refeições e alimentos específicos.

**Características**:
- OPCIONAL
- Pode ser criado pelo próprio paciente
- Pode ser criado por nutricionista (recomendado)
- Baseado em modelos/templates ou criado do zero
- Contém refeições → itens → substituições

**Status do Plano**:
- Rascunho
- Ativo (apenas 1 pode estar ativo por vez)
- Pausado
- Finalizado

### 5.2 Estrutura do Plano

```
PlanoAlimentar
├── Nome, Descrição
├── Data início/fim
├── Status
├── Metas diárias (calorias, macros)
├── RefeicoesPlanejadas[]
│   ├── TipoRefeicao (CafeManha, Almoco, Lanche, Jantar, Ceia)
│   ├── HorarioSugerido
│   ├── Ordem
│   ├── Itens[]
│   │   ├── Alimento (id, tabela, nome)
│   │   ├── QuantidadeG
│   │   ├── Macros calculados
│   │   └── SubstituicoesEquivalentes[]
│   │       ├── Alimento alternativo
│   │       ├── QuantidadeG
│   │       └── Macros calculados
│   └── Totais calculados (soma dos itens)
└── Observações
```

### 5.3 Criar Plano

**Endpoints**:
- `POST /api/PlanoAlimentar/criar` - Paciente cria próprio plano
- `POST /api/PlanoAlimentar/profissional/criar` - Nutricionista cria para paciente

**Modelo**: `CriarPlanoAlimentarDto` / `CriarPlanoProfissionalDto`
- Nome
- Descricao
- DataInicio
- DataFim (opcional)
- CaloriasAlvoDiarias
- ProteinaAlvoG, CarboidratoAlvoG, GorduraAlvoG, FibraAlvoG, AguaAlvoL
- ModeloDietaOrigemId (opcional - se baseado em template)
- Observacoes
- PacienteUserId (apenas para nutricionista)

### 5.4 Gerenciar Refeições e Itens

**Refeições**:
- `POST /api/PlanoAlimentar/{planoId}/refeicoes` - Adicionar
- `DELETE /api/PlanoAlimentar/refeicoes/{refeicaoId}` - Remover

**Itens**:
- `POST /api/PlanoAlimentar/refeicoes/{refeicaoId}/itens` - Adicionar item
- `DELETE /api/PlanoAlimentar/itens/{itemId}` - Remover item

**Substituições**:
- `POST /api/PlanoAlimentar/itens/{itemId}/substituicoes` - Adicionar substituição
- `DELETE /api/PlanoAlimentar/substituicoes/{substituicaoId}` - Remover

### 5.5 Ativar Plano

**Endpoint**: `POST /api/PlanoAlimentar/{planoId}/ativar`

**Processo**:
1. Desativa plano ativo anterior (muda para Pausado)
2. Ativa o novo plano
3. Esse plano passa a ser usado no Diário Alimentar

### 5.6 Modelos de Dieta (Templates)

**Endpoints**:
- `POST /api/PlanoAlimentar/modelos` - Criar modelo (nutricionistas)
- `GET /api/PlanoAlimentar/modelos` - Listar modelos públicos
- `GET /api/PlanoAlimentar/modelos/{id}` - Ver detalhes
- `DELETE /api/PlanoAlimentar/modelos/{id}` - Excluir (se for criador)

**Modelo**: `ModeloDieta`
- Nome, Descrição
- ObjetivoAlvo, PreferenciaAlimentarAlvo
- CaloriasBase (e macros base)
- NumeroRefeicoesDia
- Publico (se outros podem usar)
- Refeicoes e Itens (estrutura similar ao plano)

**Uso**: Paciente/nutricionista pode criar plano baseado em modelo, que é então adaptado às metas específicas.

**Telas Frontend**:

**Para Paciente**:
- ✅ **Meus Planos** (`/planos`)
  - Lista de planos (rascunho, ativo, pausados)
  - Destacar plano ativo
  - Criar novo
  
- ✅ **Criar Plano - Escolher Template** (`/planos/novo`)
  - Opção 1: Criar do zero
  - Opção 2: Usar template
  - Browse templates públicos
  
- ✅ **Editor de Plano** (`/planos/{id}/editar`)
  - Informações gerais (nome, datas, metas)
  - Lista de refeições
  - Para cada refeição:
    - Adicionar/remover itens
    - Buscar alimentos
    - Definir quantidades
    - Adicionar substituições
  - Preview de totais (calorias e macros por refeição)
  - Ajuda visual: progress bar comparando com meta
  
- ✅ **Visualizar Plano** (`/planos/{id}`)
  - Modo leitura
  - Refeições organizadas por horário
  - Totais diários
  - Botões: Editar, Ativar, Arquivar
  
**Para Nutricionista**:
- ✅ **Criar Plano para Paciente** (`/pacientes/{id}/planos/novo`)
  - Mesmo editor, mas cria para o paciente selecionado
  
- ✅ **Meus Templates** (`/templates`)
  - Templates criados pelo nutricionista
  - Criar novo
  - Editar/excluir
  - Marcar como público
  
- ✅ **Editor de Template** (`/templates/novo`)
  - Similar ao editor de plano
  - Define objetivo alvo e preferências

---

## 6. Fluxo de Diário Alimentar

### 6.1 Visão Geral

Diário Alimentar é onde o paciente **REGISTRA O QUE REALMENTE COMEU** no dia a dia.

**Características**:
- Registro de consumo real
- Comparação com plano ativo (se houver)
- Fotos de refeições (opcional)
- Relatórios de aderência
- Acompanhamento diário de metas

### 6.2 Registrar Consumo

**Endpoints**:
- `POST /api/Diario/registro` - Registrar consumo individual
- `POST /api/Diario/registro/lote` - Registrar múltiplos itens
- `DELETE /api/Diario/registro/{id}` - Excluir registro

**Modelo**: `RegistroConsumoDto`
- AlimentoId
- TipoTabela
- QuantidadeConsumidaG
- DataConsumo
- Refeicao (`ETipoRefeicao`)
- CodigoBarras (opcional - para scanner futuro)
- PlanoAlimentarId (opcional - se está seguindo um plano)
- ItemRefeicaoPlanoId (opcional - se está cumprindo item específico do plano)

**Processo**:
Sistema calcula automaticamente:
- Energia, proteínas, carboidratos, gorduras, fibras, água
- Armazena snapshot do alimento (nome, dados nutricionais completos em JSON)

### 6.3 Fotos de Refeição

**Endpoints**:
- `POST /api/Diario/fotos` - Adicionar foto
- `DELETE /api/Diario/fotos/{id}` - Remover foto
- `GET /api/Diario/fotos?data={yyyy-MM-dd}` - Fotos do dia

**Modelo**: `FotoRefeicaoDto`
- TipoRefeicao
- FotoUrl
- Descricao
- RegistroAlimentarId (opcional - se vinculada a registro específico)

### 6.4 Diário do Dia

**Endpoint**: `GET /api/Diario/dia?data={yyyy-MM-dd}`

**Retorno**: `DiarioDiaDto`
- Data
- **MetasDoDia**: Metas do plano ativo ou meta nutricional
- **TotalConsumido**: Somatório de tudo registrado
- **SaldoRestante**: Diferença (meta - consumido)
- **Refeicoes[]**: Por tipo de refeição
  - TipoRefeicao
  - HorarioPlanejado (se tem plano)
  - **Planejado**: Macros do plano (se houver)
  - **Consumido**: Macros registrados
  - **PercentualAderencia**: % aderência ao plano
  - **Registros[]**: Lista de itens consumidos
- PhotosDia[]
- PercentualAderenciaDiaria

### 6.5 Diário por Período

**Endpoint**: `GET /api/Diario/periodo?dataInicio={}&dataFim={}`

**Retorno**: Array de `DiarioDiaDto`

### 6.6 Relatório de Aderência

**Endpoints**:
- `GET /api/Diario/relatorio?dataInicio={}&dataFim={}` - Paciente
- `GET /api/Diario/profissional/relatorio?pacienteId={}&dataInicio={}&dataFim={}` - Nutricionista

**Retorno**: `RelatorioAdesaoDto`
- TotalDias
- DiasComRegistro
- PercentualDiasComRegistro
- MediaCaloriasDia
- MediaAderenciaPlano (se tinha plano ativo)
- DistribuicaoMacros (% proteína, carbo, gordura)
- RefeicoesComMaiorAderencia
- RefeicoesComMenorAderencia
- Insights automáticos

**Telas Frontend**:

**Para Paciente**:
- ✅ **Home/Dashboard** (`/dashboard`)
  - Resumo do dia atual
  - Progress bars: calorias e macros consumidos vs meta
  - Acesso rápido para registrar consumo
  - Últimas fotos
  
- ✅ **Registrar Consumo** (`/diario/registrar`)
  - Buscar alimento
  - Selecionar quantidade
  - Escolher tipo de refeição
  - Tirar/upload foto (opcional)
  - Vincular ao plano (se ativo)
  
- ✅ **Diário do Dia** (`/diario`)
  - Seletor de data (default: hoje)
  - Card para cada refeição:
    - Itens registrados
    - Comparação planejado vs consumido
    - Progress bar de aderência
  - Totais do dia
  - Fotos do dia
  - Botão: Adicionar registro
  
- ✅ **Calendário** (`/diario/calendario`)
  - Visão mensal
  - Indicadores visuais por dia:
    - Verde: alta aderência
    - Amarelo: média aderência
    - Vermelho: baixa aderência ou sem registro
  - Click no dia → vai para diário daquele dia
  
- ✅ **Relatórios** (`/relatorios`)
  - Seletor de período
  - Gráficos:
    - Evolução de calorias ao longo do tempo
    - Distribuição de macros
    - Peso vs calorias
  - Métricas de aderência
  - Insights automáticos

**Para Nutricionista**:
- ✅ **Diário do Paciente** (`/pacientes/{id}/diario`)
  - Mesmo que paciente, mas leitura
  - Pode adicionar comentários/observações
  
- ✅ **Relatório do Paciente** (`/pacientes/{id}/relatorios`)
  - Relatórios detalhados
  - Comparação com plano prescrito
  - Recomendações

---

## 7. Mapa de Dependências

### Obrigatórios na Sequência:

```
1. ApplicationUser (Registro)
   ↓
2. [Se Paciente] PerfilNutricional (OBRIGATÓRIO após login)
   ↓ (automático)
3. MetaNutricional (GERADA AUTOMATICAMENTE com perfil)
```

```
1. ApplicationUser (Registro)
   ↓
2. [Se Nutricionista] PerfilProfissional (OBRIGATÓRIO após login)
   ↓ (automático)
3. Assinatura (GERADA AUTOMATICAMENTE)
```

### Opcionais (sem ordem específica):

**Para Paciente**:
- RegistroBiometrico (múltiplos ao longo do tempo)
- PreferenciasAlimentares (adicionar quando quiser)
- AnamneseAlimentar
- AvaliacaoAntropometrica (múltiplas)
- PlanoAlimentar (pode ter vários, 1 ativo)
- RegistroAlimentar (diário)
- FotosRefeicao
- VinculoComoPaciente (com nutricionistas)

**Para Nutricionista**:
- Clinica (opcional, máximo baseado no plano)
- VinculosPacientes
- ModeloDieta (templates próprios ou públicos)
- CriarPlanoParaPaciente

### Relações Importantes:

1. **PerfilNutricional ← MetaNutricional**: 1:N (perfil pode ter histórico de metas)
2. **PerfilNutricional ← AvaliacaoAntropometrica**: 1:N (múltiplas avaliações)
3. **PerfilNutricional ← PlanoAlimentar**: 1:N (múltiplos planos, 1 ativo)
4. **PlanoAlimentar ← RefeicaoPlano**: 1:N
5. **RefeicaoPlano ← ItemRefeicao**: 1:N
6. **ItemRefeicao ← SubstituicaoEquivalente**: 1:N
7. **ApplicationUser ← RegistroAlimentar**: 1:N (diário do paciente)
8. **PerfilProfissional ←→ ApplicationUser (Paciente)**: N:N via VinculoPacienteProfissional

---

## 8. Guia de Telas para Frontend

### 8.1 Fluxo de Primeiro Acesso - PACIENTE

1. **Login** → Verifica se tem PerfilNutricional
2. Se NÃO tem → **Redireciona para Onboarding de Perfil** (OBRIGATÓRIO)
3. Completa onboarding → Sistema cria PerfilNutricional + MetaNutricional
4. **Redireciona para Dashboard**

### 8.2 Fluxo de Primeiro Acesso - NUTRICIONISTA

1. **Login** → Verifica se tem PerfilProfissional
2. Se NÃO tem → **Redireciona para Onboarding Profissional** (OBRIGATÓRIO)
3. Completa onboarding → Sistema cria PerfilProfissional + Assinatura
4. **Redireciona para Dashboard Nutricionista**

### 8.3 Checklist Completo de Telas

#### Autenticação (3 telas)
- [ ] Login
- [ ] Registro
- [ ] Recuperar Senha

#### Onboarding Paciente (1 wizard multi-step)
- [ ] Wizard Perfil Nutricional (9 steps)

#### Onboarding Nutricionista (1 tela)
- [ ] Cadastro Profissional

#### Dashboard (2 telas)
- [ ] Dashboard Paciente (resumo dia, metas, quick actions)
- [ ] Dashboard Nutricionista (lista pacientes, agenda, estatísticas)

#### Perfil e Configurações (4 telas)
- [ ] Ver Perfil do Usuário (dados cadastrais)
- [ ] Editar Perfil do Usuário
- [ ] Ver/Editar Perfil Nutricional (paciente)
- [ ] Ver/Editar Perfil Profissional (nutricionista)

#### Metas e Macros (2 telas)
- [ ] Visualizar Meta Nutricional Atual
- [ ] Histórico de Metas

#### Preferências (1 tela)
- [ ] Gerenciar Preferências Alimentares

#### Registro Biométrico (3 telas)
- [ ] Registrar Peso/Medidas
- [ ] Histórico de Peso
- [ ] Gráficos de Evolução

#### Avaliação Nutricional (5 telas)
- [ ] Nova Avaliação (wizard)
- [ ] Histórico de Avaliações
- [ ] Detalhes da Avaliação
- [ ] Comparar Avaliações
- [ ] Anamnese Alimentar

#### Planos Alimentares - Paciente (4 telas)
- [ ] Meus Planos
- [ ] Criar Plano (escolher template)
- [ ] Editor de Plano
- [ ] Visualizar Plano

#### Templates - Nutricionista (3 telas)
- [ ] Meus Templates
- [ ] Criar Template
- [ ] Editar Template

#### Diário Alimentar (5 telas)
- [ ] Dashboard Diário (resumo do dia)
- [ ] Registrar Consumo
- [ ] Diário do Dia (detalhado)
- [ ] Calendário Mensal
- [ ] Relatórios e Estatísticas

#### Fotos (2 telas)
- [ ] Upload/Galeria Fotos Refeição
- [ ] Upload/Galeria Fotos Progresso (vinculadas a avaliações)

#### Vínculos - Paciente (2 telas)
- [ ] Convites Pendentes
- [ ] Meu Nutricionista

#### Gestão de Pacientes - Nutricionista (4 telas)
- [ ] Lista de Pacientes
- [ ] Convidar Paciente
- [ ] Detalhes do Paciente (visão completa: perfil, avaliações, planos, diário)
- [ ] Criar Plano para Paciente

#### Clínicas - Nutricionista (2 telas)
- [ ] Gerenciar Clínicas
- [ ] Criar/Editar Clínica

#### Assinatura - Nutricionista (2 telas)
- [ ] Minha Assinatura (plano atual, limites)
- [ ] Upgrade de Plano

#### Busca (1 tela reutilizável)
- [ ] Buscar Alimentos (componente reutilizável em várias telas)

---

## 9. Fluxo Simplificado para Início do Desenvolvimento

### Fase 1: MVP Básico

**Para começar, desenvolva nesta ordem**:

1. **Autenticação**
   - Login
   - Registro (com escolha de Role)

2. **Onboarding Obrigatório**
   - Wizard Perfil Nutricional (Paciente)
   - Cadastro Profissional (Nutricionista)

3. **Dashboard Básico**
   - Dashboard Paciente: mostra meta nutricional
   - Dashboard Nutricionista: lista vazia inicialmente

4. **Diário Básico**
   - Buscar alimentos
   - Registrar consumo
   - Ver consumo do dia vs meta

Com isso, já tem um MVP funcional!

### Fase 2: Funcionalidades Intermediárias

5. **Perfil e Registros**
   - Editar perfil nutricional
   - Registrar peso
   - Ver histórico de peso

6. **Planos Simples**
   - Criar plano do zero (sem templates ainda)
   - Visualizar plano
   - Ativar plano

7. **Diário Avançado**
   - Calendário
   - Relatórios básicos

### Fase 3: Funcionalidades Avançadas

8. **Nutricionista**
   - Convidar pacientes
   - Ver pacientes
   - Criar planos para pacientes

9. **Avaliação Antropométrica**
   - Nova avaliação
   - Histórico
   - Comparações

10. **Templates e Fotos**
    - Templates de dieta
    - Fotos de refeição
    - Fotos de progresso

---

## 10. Endpoints de Referência Rápida

### Auth
- `POST /api/Auth/register` - Registro
- `POST /api/Auth/login` - Login

### User (Paciente)
- `GET /api/User/perfil` - Dados do usuário
- `PUT /api/User/perfil` - Atualizar dados
- `POST /api/User/perfil-nutricional` - Criar perfil nutricional
- `GET /api/User/perfil-nutricional` - Ver perfil nutricional
- `PUT /api/User/perfil-nutricional` - Atualizar perfil nutricional
- `GET /api/User/meta-nutricional` - Ver meta atual
- `POST /api/User/preferencia-alimentar` - Adicionar preferência
- `POST /api/User/registro-biometrico` - Registrar peso
- `GET /api/User/vinculos` - Ver vínculos com nutricionistas
- `POST /api/User/vinculos/{id}/aceitar` - Aceitar convite

### Nutricionista
- `POST /api/Nutricionista/cadastro` - Cadastro profissional
- `GET /api/Nutricionista/perfil` - Ver perfil profissional
- `PUT /api/Nutricionista/perfil` - Atualizar perfil
- `POST /api/Nutricionista/clinicas` - Criar clínica
- `GET /api/Nutricionista/clinicas` - Listar clínicas
- `POST /api/Nutricionista/pacientes/convidar` - Convidar paciente
- `GET /api/Nutricionista/pacientes` - Listar pacientes
- `GET /api/Nutricionista/pacientes/{id}` - Detalhes do paciente

### Avaliação
- `POST /api/Avaliacao/registrar` - Nova avaliação
- `GET /api/Avaliacao` - Listar avaliações
- `GET /api/Avaliacao/{id}` - Detalhes da avaliação
- `GET /api/Avaliacao/comparar` - Comparar 2 avaliações
- `POST /api/Avaliacao/{id}/fotos` - Adicionar fotos
- `POST /api/Avaliacao/anamnese` - Registrar anamnese

### Plano Alimentar
- `POST /api/PlanoAlimentar/criar` - Criar plano
- `GET /api/PlanoAlimentar` - Listar planos
- `GET /api/PlanoAlimentar/{id}` - Ver plano
- `GET /api/PlanoAlimentar/ativo` - Ver plano ativo
- `POST /api/PlanoAlimentar/{id}/ativar` - Ativar plano
- `POST /api/PlanoAlimentar/{id}/refeicoes` - Adicionar refeição
- `POST /api/PlanoAlimentar/refeicoes/{id}/itens` - Adicionar item
- `POST /api/PlanoAlimentar/itens/{id}/substituicoes` - Adicionar substituição
- `GET /api/PlanoAlimentar/modelos` - Listar templates
- `POST /api/PlanoAlimentar/modelos` - Criar template

### Diário
- `POST /api/Diario/registro` - Registrar consumo
- `GET /api/Diario/dia` - Diário do dia
- `GET /api/Diario/periodo` - Diário por período
- `GET /api/Diario/relatorio` - Relatório de aderência
- `POST /api/Diario/fotos` - Adicionar foto
- `GET /api/Diario/fotos` - Fotos do dia

### Busca
- `GET /api/Busca/alimentos?termo={}` - Buscar alimentos
- `GET /api/Busca/alimento/{id}?tabela={}` - Detalhes do alimento

---

## 11. Regras de Negócio Importantes

### ✅ OBRIGATÓRIOS

1. **Paciente DEVE ter PerfilNutricional** para usar o sistema
2. **Nutricionista DEVE ter PerfilProfissional** para atuar
3. **MetaNutricional é SEMPRE gerada automaticamente** ao criar/atualizar perfil
4. **Apenas 1 PlanoAlimentar pode estar Ativo** por vez
5. **Nutricionista só pode acessar dados de pacientes com vínculo Ativo**

### ⚠️ VALIDAÇÕES

1. **Limites de Assinatura**: Nutricionista não pode ter mais pacientes que o limite do plano
2. **Clínicas**: Apenas plano Enterprise permite múltiplas clínicas
3. **CRN único**: Não pode ter 2 nutricionistas com mesmo CRN
4. **Vínculo Ativo**: Paciente aceita convite para vínculo ficar ativo
5. **Datas**: DataFim de plano deve ser >= DataInicio

### 🎯 RECOMENDAÇÕES

1. **Anamnese** deve ser preenchida antes de criar plano (não obrigatório, mas recomendado)
2. **Avaliação Antropométrica** deve ser feita periodicamente
3. **Registro Biométrico** idealmente semanal
4. **Fotos de Progresso** mensais
5. **Plano criado por Nutricionista** é mais adequado que auto-criado

---

## 12. Pontos de Atenção para UX

### Indicadores Visuais

1. **Badge de "Incompleto"** se usuário não tem perfil nutricional
2. **Progress indicators** no onboarding
3. **Alertas de limite** quando nutricionista está próximo do limite de pacientes
4. **Status visual** de planos (ativo, rascunho, pausado)
5. **Health checks** no dashboard:
   - ✅ Perfil completo
   - ✅ Meta definida
   - ✅ Plano ativo (opcional, mas recomendável)
   - ⚠️ Sem registros hoje
   - ⚠️ Última avaliação há mais de 30 dias

### Onboarding Suave

1. **Wizard visual** com progress bar
2. **Salvar progresso** entre steps
3. **Validação inline** (não esperar submit)
4. **Explicações** sobre cada campo (tooltips)
5. **Pular campos opcionais** com "Preencher depois"

### Feedback Constante

1. **Toasts/Notifications** para ações bem-sucedidas
2. **Loading states** em operações assíncronas
3. **Error handling** amigável
4. **Confirmações** para ações destrutivas
5. **Insights automáticos** baseados nos dados

---

## 13. Dados de Exemplo para Testes

### Paciente Teste

```json
{
  "email": "paciente@teste.com",
  "password": "Senha123!",
  "nomeCompleto": "João Silva Santos",
  "cpf": "12345678900",
  "role": "Paciente",
  "dataNascimento": "1990-05-15",
  "telefone": "11987654321"
}
```

### Perfil Nutricional Teste

```json
{
  "userId": "...",
  "dataNascimento": "1990-05-15",
  "genero": "Masculino",
  "alturaCm": 175,
  "pesoAtualKg": 85,
  "fatorAtividade": 1.55,
  "nivelAtividade": "Moderado",
  "ocupacaoProfissional": "Desenvolvedor",
  "habilidadeCulinaria": "Intermediario",
  "orcamentoMensal": "Medio",
  "possuiDoencasPreExistentes": false,
  "fumante": false,
  "qualidadeSono": 4,
  "horasSonoPorNoite": 7,
  "objetivo": "PerdaPeso",
  "pesoDesejadoKg": 75,
  "preferenciaDieta": "Onivoro",
  "refeicoesPorDiaDesejadas": 5,
  "tempoDisponivelPreparoMinutos": 30,
  "restricoesIds": ["Lactose"],
  "equipamentosIds": ["Fogao", "Microondas", "AirFryer"],
  "preferencias": [],
  "historicoClinicos": []
}
```

---

**Fim do Documento**

Este documento deve ser usado como referência principal para desenvolvimento do frontend Next.js. Qualquer dúvida sobre fluxos, endpoints ou regras de negócio, consulte este guia.
