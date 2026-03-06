# Diagramas de Fluxo - Nutra Food API

Este documento contém diagramas visuais dos principais fluxos da aplicação.

---

## 1. Fluxo de Primeiro Acesso - Paciente

```mermaid
graph TD
    A[Usuário Acessa App] --> B{Autenticado?}
    B -->|Não| C[Tela de Login]
    C --> D[Login/Registro]
    D --> E[Registro como Paciente]
    E --> F[POST /api/Auth/register]
    F --> G[Login Automático]
    
    B -->|Sim| H{Tem PerfilNutricional?}
    
    G --> H
    
    H -->|Não| I[REDIRECT FORÇADO]
    I --> J[Wizard Onboarding Perfil]
    J --> K[Step 1: Dados Pessoais]
    K --> L[Step 2: Medidas]
    L --> M[Step 3: Estilo Vida]
    M --> N[Step 4: Saúde]
    N --> O[Step 5: Objetivos]
    O --> P[Step 6: Alimentação]
    P --> Q[Step 7: Restrições]
    Q --> R[Step 8: Equipamentos]
    R --> S[Step 9: Preferências]
    S --> T[POST /api/User/perfil-nutricional]
    T --> U[Sistema Cria PerfilNutricional]
    U --> V[Sistema GERA AUTOMATICAMENTE MetaNutricional]
    V --> W[Dashboard Paciente]
    
    H -->|Sim| W
    
    style I fill:#ff6b6b
    style U fill:#51cf66
    style V fill:#51cf66
```

---

## 2. Fluxo de Primeiro Acesso - Nutricionista

```mermaid
graph TD
    A[Usuário Acessa App] --> B{Autenticado?}
    B -->|Não| C[Login/Registro]
    C --> D[Registro como Nutricionista]
    D --> E[POST /api/Auth/register]
    E --> F[Login Automático]
    
    B -->|Sim| G{Tem PerfilProfissional?}
    
    F --> G
    
    G -->|Não| H[REDIRECT FORÇADO]
    H --> I[Cadastro Profissional]
    I --> J[Preenche CRN, Região, Especialidade]
    J --> K[POST /api/Nutricionista/cadastro]
    K --> L[Sistema Cria PerfilProfissional]
    L --> M[Sistema Cria Assinatura Gratuita/Trial]
    M --> N[MaxPacientes = 5]
    N --> O[Dashboard Nutricionista]
    
    G -->|Sim| O
    
    style H fill:#ff6b6b
    style L fill:#51cf66
    style M fill:#51cf66
```

---

## 3. Hierarquia de Dados - Paciente

```mermaid
graph TD
    A[ApplicationUser<br/>Role: Paciente] --> B[PerfilNutricional<br/>OBRIGATÓRIO]
    
    B --> C[MetaNutricional<br/>AUTO-GERADO]
    B --> D[RestricoesAlimentares]
    B --> E[PreferenciasAlimentares]
    B --> F[EquipamentoDisponivel]
    B --> G[HistoricoClinico]
    B --> H[RegistroBiometrico<br/>múltiplos]
    B --> I[AnamneseAlimentar<br/>OPCIONAL]
    B --> J[AvaliacaoAntropometrica<br/>OPCIONAL, múltiplas]
    B --> K[PlanoAlimentar<br/>OPCIONAL, múltiplos]
    
    J --> J1[FotosProgresso]
    
    K --> K1[RefeicaoPlano]
    K1 --> K2[ItemRefeicao]
    K2 --> K3[SubstituicaoEquivalente]
    
    A --> L[RegistroAlimentar<br/>diário, múltiplos]
    L --> L1[FotosRefeicao]
    
    A --> M[VinculosComoPaciente]
    M --> M1[VinculoPacienteProfissional]
    
    style B fill:#4dabf7
    style C fill:#51cf66
    style K fill:#ffd43b
```

---

## 4. Hierarquia de Dados - Nutricionista

```mermaid
graph TD
    A[ApplicationUser<br/>Role: Nutricionista] --> B[PerfilProfissional<br/>OBRIGATÓRIO]
    
    B --> C[Assinatura<br/>AUTO-CRIADA]
    C --> C1[Plano: Gratuito/Basico/<br/>Profissional/Enterprise]
    C1 --> C2[MaxPacientes]
    C1 --> C3[MultiClinicaHabilitado]
    
    B --> D[Clinicas<br/>OPCIONAL]
    D --> D1[Limite baseado no Plano]
    
    B --> E[VinculosPacientes]
    E --> E1[Status: Pendente/<br/>Ativo/Recusado/Encerrado]
    
    B --> F[ModelosDieta<br/>Templates criados]
    F --> F1[Publico/Privado]
    
    style B fill:#4dabf7
    style C fill:#51cf66
    style C1 fill:#ffd43b
```

---

## 5. Fluxo de Criação de Perfil Nutricional

```mermaid
sequenceDiagram
    participant U as Usuário
    participant F as Frontend
    participant API as Backend API
    participant DB as Database
    participant CALC as CalculadoraNutricional

    U->>F: Preenche Wizard Onboarding
    F->>F: Valida campos
    U->>F: Clica "Criar Perfil"
    F->>API: POST /api/User/perfil-nutricional
    
    API->>DB: Verifica se já existe perfil
    DB-->>API: false
    
    API->>DB: Cria PerfilNutricional
    API->>DB: Cria RestricoesAlimentares
    API->>DB: Cria EquipamentoDisponivel
    API->>DB: Cria PreferenciasAlimentares
    API->>DB: Cria HistoricoClinicos
    API->>DB: Cria RegistroBiometrico inicial
    
    API->>CALC: GerarMetaInicial(perfil)
    CALC->>CALC: Calcula TMB
    CALC->>CALC: Calcula GET
    CALC->>CALC: Ajusta por Objetivo
    CALC->>CALC: Calcula Macros
    CALC-->>API: MetaNutricional
    
    API->>DB: Cria MetaNutricional
    API->>DB: Vincula MetaNutricionalAtualId
    
    API-->>F: Sucesso + PerfilId
    F->>F: Mostra "Calculando metas..."
    F->>U: Redireciona Dashboard
    F->>F: Toast "Perfil criado!"
```

---

## 6. Fluxo de Atualização de Perfil → Recálculo de Meta

```mermaid
sequenceDiagram
    participant U as Usuário
    participant F as Frontend
    participant API as Backend API
    participant DB as Database
    participant CALC as CalculadoraNutricional

    U->>F: Edita Perfil Nutricional
    U->>F: Altera peso/objetivo/atividade
    F->>API: PUT /api/User/perfil-nutricional
    
    API->>DB: Atualiza PerfilNutricional
    
    Note over API,CALC: META É SEMPRE RECALCULADA
    
    API->>CALC: GerarMetaInicial(perfilAtualizado)
    CALC-->>API: Nova MetaNutricional
    
    API->>DB: Cria NOVA MetaNutricional
    API->>DB: Atualiza MetaNutricionalAtualId
    
    API-->>F: Sucesso
    F->>F: Toast "Perfil atualizado"
    F->>F: Toast "Metas recalculadas"
    F->>U: Mostra novas metas
```

---

## 7. Fluxo de Vínculo Nutricionista-Paciente

```mermaid
sequenceDiagram
    participant N as Nutricionista
    participant FN as Frontend Nutricionista
    participant API as Backend API
    participant FP as Frontend Paciente
    participant P as Paciente

    N->>FN: Acessa "Convidar Paciente"
    FN->>FN: Verifica limite de pacientes
    
    alt Limite atingido
        FN->>N: Mostra alerta "Limite atingido"
        FN->>N: CTA "Fazer upgrade"
    else Dentro do limite
        N->>FN: Busca paciente (email/CPF)
        FN->>API: GET /api/User/buscar?email=
        API-->>FN: Dados do usuário
        
        N->>FN: Seleciona clínica (opcional)
        N->>FN: Adiciona observações
        N->>FN: Clica "Enviar Convite"
        
        FN->>API: POST /api/Nutricionista/pacientes/convidar
        API->>API: Cria VinculoPacienteProfissional
        API->>API: Status = Pendente
        API-->>FN: Sucesso
        
        Note over API: Sistema notifica paciente
        
        FP->>P: Notificação "Novo convite"
        P->>FP: Acessa /vinculos
        FP->>API: GET /api/User/vinculos
        API-->>FP: Lista convites pendentes
        
        FP->>P: Mostra card do convite
        
        alt Paciente Aceita
            P->>FP: Clica "Aceitar"
            FP->>API: POST /api/User/vinculos/{id}/aceitar
            API->>API: Status = Ativo
            API->>API: DataAceite = now
            API-->>FP: Sucesso
            FP->>P: Toast "Vínculo estabelecido"
            
            Note over API: Nutricionista pode acessar dados
        else Paciente Recusa
            P->>FP: Clica "Recusar"
            FP->>API: POST /api/User/vinculos/{id}/recusar
            API->>API: Status = Recusado
            API-->>FP: Sucesso
        end
    end
```

---

## 8. Fluxo de Criação de Plano Alimentar

```mermaid
graph TD
    A[Usuário: Novo Plano] --> B{Escolhe Origem}
    
    B -->|Do Zero| C[Editor Vazio]
    B -->|Template| D[Buscar Templates]
    
    D --> E[Filtrar por Objetivo/Dieta]
    E --> F[Selecionar Template]
    F --> G[Carregar Template no Editor]
    G --> H[Editor com Template]
    
    C --> I[Preenche Info Geral]
    H --> I
    
    I --> J[Nome, Descrição, Datas]
    J --> K[Define Metas Diárias]
    K --> L{Usar Metas do Perfil?}
    
    L -->|Sim| M[Carrega metas automáticas]
    L -->|Não| N[Define manualmente]
    
    M --> O[Adicionar Refeições]
    N --> O
    
    O --> P[Para cada Refeição]
    P --> Q[Define Tipo, Horário, Ordem]
    Q --> R[Adicionar Itens]
    
    R --> S[Buscar Alimento]
    S --> T[Selecionar Alimento]
    T --> U[Definir Quantidade]
    U --> V[Sistema Calcula Macros]
    V --> W{Adicionar Substituição?}
    
    W -->|Sim| X[Buscar Alimento Substituto]
    X --> Y[Definir Quantidade]
    Y --> Z[Sistema Calcula Macros]
    Z --> AA[Adiciona à lista]
    
    W -->|Não| AB[Item Adicionado]
    AA --> AB
    
    AB --> AC{Mais Itens?}
    AC -->|Sim| R
    AC -->|Não| AD[Sistema Calcula Totais Refeição]
    
    AD --> AE{Mais Refeições?}
    AE -->|Sim| O
    AE -->|Não| AF[Sistema Calcula Totais Plano]
    
    AF --> AG[Preview: Comparação com Meta]
    AG --> AH{Quer Ajustar?}
    AH -->|Sim| O
    AH -->|Não| AI{Salvar como?}
    
    AI -->|Rascunho| AJ[POST com Status=Rascunho]
    AI -->|Publicar| AK{Tem outro Ativo?}
    
    AK -->|Sim| AL[Desativa Ativo Anterior]
    AL --> AM[POST com Status=Ativo]
    AK -->|Não| AM
    
    AJ --> AN[Plano Salvo]
    AM --> AO[Plano Ativo]
    
    AN --> AP[Redireciona /planos]
    AO --> AP
    
    style AM fill:#51cf66
    style AO fill:#51cf66
```

---

## 9. Fluxo de Registro de Consumo

```mermaid
sequenceDiagram
    participant U as Usuário
    participant F as Frontend
    participant API as Backend API
    participant DB as Database
    participant S as Busca Service

    U->>F: Clica "Registrar Consumo"
    F->>U: Abre modal/page
    
    U->>F: Digita nome alimento
    F->>API: GET /api/Busca/alimentos?termo=
    API->>S: BuscaAlimentos(termo)
    S->>DB: Query em múltiplas tabelas
    DB-->>S: Resultados
    S-->>API: Lista alimentos
    API-->>F: JSON alimentos
    F->>U: Mostra grid resultados
    
    U->>F: Seleciona alimento
    F->>U: Mostra detalhes nutricionais (por 100g)
    
    U->>F: Informa quantidade (g)
    F->>F: Calcula macros proporcionalmente
    F->>U: Preview "Para XXg, você consumirá..."
    
    U->>F: Seleciona tipo refeição
    U->>F: Define data/hora (default: agora)
    
    opt Upload foto
        U->>F: Adiciona foto
    end
    
    opt Vincular ao plano
        F->>F: Verifica se tem plano ativo
        alt Tem plano ativo
            F->>U: Checkbox "Faz parte do plano?"
            U->>F: Marca checkbox
            F->>API: GET /api/PlanoAlimentar/ativo
            API-->>F: Plano ativo
            F->>U: Select item do plano
            U->>F: Seleciona item
        end
    end
    
    U->>F: Clica "Confirmar"
    
    F->>API: POST /api/Diario/registro
    Note over API: Payload completo
    
    API->>API: Calcula todos os macros
    API->>API: Cria snapshot do alimento (JSON)
    API->>DB: Insere RegistroAlimentar
    
    opt Se tem foto
        API->>DB: Insere FotoRefeicao vinculada
    end
    
    API-->>F: Sucesso + RegistroId
    F->>F: Toast "Consumo registrado!"
    F->>F: Atualiza dashboard em tempo real
    F->>U: Opções: "Registrar outro" / "Ver diário"
```

---

## 10. Fluxo de Diário do Dia

```mermaid
graph TD
    A[Usuário: Diário do Dia] --> B[GET /api/Diario/dia?data=]
    B --> C[Backend Processa]
    
    C --> D[Busca Registros do Dia]
    C --> E[Busca Plano Ativo]
    C --> F[Busca Meta Nutricional]
    C --> G[Busca Fotos do Dia]
    
    D --> H[Agrupa por Tipo Refeição]
    H --> I[Calcula Totais Consumidos]
    
    E --> J{Tem Plano Ativo?}
    J -->|Sim| K[Pega Metas do Plano]
    J -->|Não| L[Usa Meta Nutricional]
    
    K --> M[Metas do Dia]
    L --> M
    
    M --> N[Compara Consumido vs Meta]
    N --> O[Calcula Saldo Restante]
    
    I --> P[Para cada Refeição]
    P --> Q{Tem Planejado?}
    Q -->|Sim| R[Pega Itens Planejados da Refeição]
    R --> S[Calcula Totais Planejados]
    S --> T[Compara Planejado vs Consumido]
    T --> U[Calcula % Aderência]
    
    Q -->|Não| V[Apenas Consumido]
    
    U --> W[Monta Refeição Comparativa]
    V --> W
    
    W --> X{Mais Refeições?}
    X -->|Sim| P
    X -->|Não| Y[Calcula Aderência Diária Total]
    
    Y --> Z[Monta DiarioDiaDto]
    O --> Z
    G --> Z
    
    Z --> AA[Retorna JSON]
    AA --> AB[Frontend Renderiza]
    
    AB --> AC[Progress Bars]
    AB --> AD[Cards de Refeição]
    AB --> AE[Galeria Fotos]
    AB --> AF[Totais e Saldo]
    
    style K fill:#51cf66
    style U fill:#ffd43b
    style Y fill:#ffd43b
```

---

## 11. Fluxo de Avaliação Antropométrica

```mermaid
sequenceDiagram
    participant U as Usuário
    participant F as Frontend
    participant API as Backend API
    participant CALC as CalculadoraNutricional
    participant DB as Database

    U->>F: Nova Avaliação
    F->>U: Wizard Step 1: Básico
    U->>F: Peso, Altura, Observações
    
    F->>U: Step 2: Circunferências (opcional)
    U->>F: Medidas circunferências
    
    F->>U: Step 3: Dobras (opcional)
    U->>F: Seleciona protocolo
    U->>F: Informa dobras (mm)
    
    F->>U: Step 4: Bioimpedância (opcional)
    U->>F: Dados bioimpedância
    
    F->>U: Step 5: Fotos (opcional)
    U->>F: Upload fotos
    
    F->>U: Preview Dados
    U->>F: Confirma
    
    F->>API: POST /api/Avaliacao/registrar
    API->>API: Recebe AvaliacaoDto
    
    Note over API,CALC: CÁLCULOS AUTOMÁTICOS
    
    API->>CALC: CalcularIMC(peso, altura)
    CALC-->>API: IMC + Classificação
    
    API->>CALC: CalcularTMB_MifflinStJeor
    CALC-->>API: TMB1
    
    API->>CALC: CalcularTMB_HarrisBenedict
    CALC-->>API: TMB2
    
    opt Se tem massa magra
        API->>CALC: CalcularTMB_KatchMcArdle
        CALC-->>API: TMB3
    end
    
    API->>CALC: CalcularGET(TMB, nivelAtividade)
    CALC-->>API: GET
    
    API->>CALC: CalcularPesoIdeal_Devine
    CALC-->>API: PesoIdeal1
    
    API->>CALC: CalcularPesoIdeal_IMC
    CALC-->>API: PesoIdeal2
    
    opt Se tem dobras
        API->>CALC: CalcularDensidadeCorporal(dobras, protocolo)
        CALC-->>API: DensidadeCorporal
        
        API->>CALC: CalcularPercentualGordura(densidade)
        CALC-->>API: %GorduraDobras
    end
    
    opt Se tem cintura e quadril
        API->>CALC: CalcularRCQ(cintura, quadril)
        CALC-->>API: RCQ + Classificação
    end
    
    API->>API: Determina melhor %Gordura (bio > dobras)
    API->>API: Calcula Massa Magra e Gorda
    API->>API: Ajusta TMB/GET
    
    API->>DB: Insere AvaliacaoAntropometrica
    
    opt Se tem fotos
        API->>DB: Insere FotosProgresso vinculadas
    end
    
    API->>DB: Atualiza Peso/Medidas no PerfilNutricional
    
    API-->>F: AvaliacaoResultadoDto (completo)
    F->>U: Modal com resultados
    F->>U: Mostra todos os cálculos
    F->>U: Link "Ver Detalhes"
```

---

## 12. Fluxo de Comparação de Avaliações

```mermaid
graph TD
    A[Usuário: Comparar Avaliações] --> B[GET /api/Avaliacao/comparar?anterior=X&atual=Y]
    B --> C[Backend Busca Avaliação Anterior]
    B --> D[Backend Busca Avaliação Atual]
    
    C --> E[Avaliação X]
    D --> F[Avaliação Y]
    
    E --> G[Calcula Deltas]
    F --> G
    
    G --> H[Delta Peso = Y.peso - X.peso]
    G --> I[Delta IMC = Y.imc - X.imc]
    G --> J[Delta %Gordura = Y.gordura - X.gordura]
    G --> K[Delta Massa Magra]
    G --> L[Delta Massa Gorda]
    G --> M[Delta GET]
    G --> N[Delta Circunferências]
    
    H --> O[Dias entre Avaliações]
    I --> O
    
    O --> P{Delta Peso > 0?}
    P -->|Sim| Q[Ganhou X kg em Y dias]
    P -->|Não| R[Perdeu X kg em Y dias]
    
    Q --> S[Insights Automáticos]
    R --> S
    
    S --> T{Massa Magra Aumentou?}
    T -->|Sim| U[Insight: Ganho muscular!]
    T -->|Não| V[Insight: Atenção à massa magra]
    
    U --> W[Monta ComparacaoAvaliacoesDto]
    V --> W
    
    J --> W
    K --> W
    L --> W
    M --> W
    N --> W
    
    W --> X[Retorna JSON]
    X --> Y[Frontend Renderiza]
    
    Y --> Z[Layout Duas Colunas]
    Z --> AA[Coluna Esquerda: Anterior]
    Z --> AB[Coluna Direita: Atual]
    Z --> AC[Centro: Deltas com Setas]
    Z --> AD[Gráficos Evolução]
    Z --> AE[Comparação Fotos]
    
    style Q fill:#51cf66
    style R fill:#ff6b6b
    style U fill:#51cf66
    style V fill:#ffd43b
```

---

## 13. Estados de Plano Alimentar

```mermaid
stateDiagram-v2
    [*] --> Rascunho: Criar Novo
    
    Rascunho --> Ativo: Ativar
    Rascunho --> Pausado: Pausar
    Rascunho --> Finalizado: Finalizar
    Rascunho --> [*]: Excluir
    
    Ativo --> Pausado: Pausar
    Ativo --> Finalizado: Finalizar
    
    note right of Ativo
        Apenas 1 pode estar Ativo
        Ao ativar outro, o anterior vai para Pausado
    end note
    
    Pausado --> Ativo: Reativar
    Pausado --> Finalizado: Finalizar
    Pausado --> [*]: Excluir
    
    Finalizado --> [*]: Excluir
    
    note right of Finalizado
        Estado final
        Não pode ser reativado
    end note
```

---

## 14. Estados de Vínculo Paciente-Nutricionista

```mermaid
stateDiagram-v2
    [*] --> Pendente: Nutricionista Envia Convite
    
    note right of Pendente
        Aguardando resposta do paciente
    end note
    
    Pendente --> Ativo: Paciente Aceita
    Pendente --> Recusado: Paciente Recusa
    Pendente --> [*]: Nutricionista Cancela
    
    note right of Ativo
        Nutricionista tem acesso aos dados
        Pode criar planos, avaliações
    end note
    
    Ativo --> Encerrado: Encerrar Vínculo
    
    note right of Encerrado
        Nutricionista perde acesso
        Dados históricos permanecem
    end note
    
    Recusado --> [*]: Fim
    Encerrado --> [*]: Fim
```

---

## 15. Fluxo Completo de Uso - Caso de Sucesso

```mermaid
graph TD
    A[Usuário se Registra] --> B[Cria Perfil Nutricional]
    B --> C[Sistema Gera Meta Nutricional]
    
    C --> D{Primeira Semana}
    D --> E[Registra Consumo Diariamente]
    E --> F[Compara com Meta]
    F --> G[Vê Relatórios]
    
    G --> H{Segunda Semana}
    H --> I[Registra Peso]
    I --> J[Vê Evolução]
    J --> K[Faz Primeira Avaliação]
    
    K --> L{Terceira Semana}
    L --> M[Cria Plano Alimentar]
    M --> N[Ativa Plano]
    N --> O[Registra Seguindo Plano]
    O --> P[Acompanha Aderência]
    
    P --> Q{Quarta Semana}
    Q --> R[Busca Nutricionista]
    R --> S[Aceita Convite]
    S --> T[Nutricionista Acessa Perfil]
    
    T --> U{Um Mês Depois}
    U --> V[Nutricionista Registra Avaliação]
    V --> W[Compara com Primeira]
    W --> X[Vê Progresso Real]
    
    X --> Y{Continuação}
    Y --> Z[Nutricionista Cria Plano Personalizado]
    Z --> AA[Paciente Segue Novo Plano]
    AA --> AB[Registra Fotos Progresso]
    AB --> AC[Acompanhamento Contínuo]
    
    AC --> AD{3 Meses Depois}
    AD --> AE[Nova Avaliação]
    AE --> AF[Comparação Múltiplas Avaliações]
    AF --> AG[Ajuste de Metas]
    AG --> AH[Atualiza Perfil]
    AH --> AI[Recalcula Metas]
    AI --> AJ[Continua Ciclo]
    
    style C fill:#51cf66
    style N fill:#ffd43b
    style S fill:#51cf66
    style X fill:#40c057
    style AG fill:#4dabf7
```

---

## 16. Arquitetura de Dados Simplificada

```mermaid
erDiagram
    ApplicationUser ||--o| PerfilNutricional : "tem (1:1, obrigatório)"
    ApplicationUser ||--o| PerfilProfissional : "tem (1:1, se nutricionista)"
    ApplicationUser ||--o{ RegistroAlimentar : "registra (1:N)"
    ApplicationUser ||--o{ FotoRefeicao : "tira (1:N)"
    
    PerfilNutricional ||--o{ MetaNutricional : "possui histórico (1:N)"
    PerfilNutricional ||--|| MetaNutricional : "meta atual (1:1)"
    PerfilNutricional ||--o{ RestricaoAlimentar : "possui (1:N)"
    PerfilNutricional ||--o{ PreferenciaAlimentar : "possui (1:N)"
    PerfilNutricional ||--o{ PerfilEquipamento : "possui (1:N)"
    PerfilNutricional ||--o{ HistoricoClinico : "possui (1:N)"
    PerfilNutricional ||--o{ RegistroBiometrico : "possui histórico (1:N)"
    PerfilNutricional ||--o{ AnamneseAlimentar : "possui (1:N)"
    PerfilNutricional ||--o{ AvaliacaoAntropometrica : "possui (1:N)"
    PerfilNutricional ||--o{ PlanoAlimentar : "possui (1:N)"
    
    AvaliacaoAntropometrica ||--o{ FotoProgresso : "possui (1:N)"
    
    PlanoAlimentar ||--o{ RefeicaoPlano : "contém (1:N)"
    RefeicaoPlano ||--o{ ItemRefeicao : "contém (1:N)"
    ItemRefeicao ||--o{ SubstituicaoEquivalente : "possui (1:N)"
    
    PerfilProfissional ||--|| Assinatura : "possui (1:1, auto-criada)"
    PerfilProfissional ||--o{ Clinica : "gerencia (1:N)"
    PerfilProfissional ||--o{ VinculoPacienteProfissional : "tem vínculos (1:N)"
    PerfilProfissional ||--o{ ModeloDieta : "cria templates (1:N)"
    
    ApplicationUser ||--o{ VinculoPacienteProfissional : "vínculo paciente (1:N)"
    
    RegistroAlimentar ||--o{ FotoRefeicao : "possui (1:N)"
    RegistroAlimentar }o--|| PlanoAlimentar : "referencia (N:1, opcional)"
    RegistroAlimentar }o--|| ItemRefeicao : "referencia (N:1, opcional)"
```

---

## 17. Pipeline de Cálculo de Metas

```mermaid
graph LR
    A[Dados Perfil] --> B{Cálculo TMB}
    
    B --> C[Mifflin-St Jeor]
    C --> C1[peso × 10]
    C --> C2[altura × 6.25]
    C --> C3[idade × 5]
    C --> C4[constante gênero]
    C1 --> C5[+ C2 - C3 ± C4]
    C5 --> D[TMB]
    
    D --> E{Aplicar Fator Atividade}
    E --> E1[Sedentário: 1.2]
    E --> E2[Leve: 1.375]
    E --> E3[Moderado: 1.55]
    E --> E4[Intenso: 1.725]
    E --> E5[Muito Intenso: 1.9]
    
    E1 --> F[GET Base]
    E2 --> F
    E3 --> F
    E4 --> F
    E5 --> F
    
    F --> G{Ajustar por Objetivo}
    G --> G1[Perda Peso: -300 a -500]
    G --> G2[Manutenção: 0]
    G --> G3[Ganho Massa: +300 a +500]
    
    G1 --> H[Calorias Alvo]
    G2 --> H
    G3 --> H
    
    H --> I{Distribuir Macros}
    
    I --> J[Proteínas]
    J --> J1[peso × 1.8g<br/>ou<br/>peso × 2.2g se ganho]
    
    I --> K[Gorduras]
    K --> K1[20-35% das calorias]
    
    I --> L[Carboidratos]
    L --> L1[Calorias restantes / 4]
    
    J1 --> M[Macros Definidos]
    K1 --> M
    L1 --> M
    
    M --> N[Fibras: 25-38g]
    M --> O[Água: 35ml × peso]
    
    N --> P[MetaNutricional]
    O --> P
    
    style P fill:#51cf66
```

---

## 18. Fluxo de Relatório de Aderência

```mermaid
graph TD
    A[Usuário: Ver Relatório] --> B[Seleciona Período]
    B --> C[GET /api/Diario/relatorio?dataInicio&dataFim]
    
    C --> D[Backend: Loop em cada dia]
    D --> E[Busca Registros do Dia]
    D --> F[Busca Plano Ativo do Dia se houver]
    D --> G[Busca Meta Nutricional do Dia]
    
    E --> H{Tem Registros?}
    H -->|Sim| I[Dia com Registro = true]
    H -->|Não| J[Dia com Registro = false]
    
    I --> K[Soma Calorias Consumidas]
    F --> L{Tem Plano?}
    L -->|Sim| M[Pega Meta do Plano]
    L -->|Não| N[Pega Meta Nutricional]
    
    M --> O[Meta do Dia]
    N --> O
    
    K --> P[Compara Consumido vs Meta]
    O --> P
    
    P --> Q[Calcula % Aderência Dia]
    Q --> R{Aderência >= 80%?}
    R -->|Sim| S[Dia de Alta Aderência]
    R -->|Não| T{Aderência >= 50%?}
    T -->|Sim| U[Dia de Média Aderência]
    T -->|Não| V[Dia de Baixa Aderência]
    
    S --> W[Acumula Estatísticas]
    U --> W
    V --> W
    J --> W
    
    W --> X{Mais Dias?}
    X -->|Sim| D
    X -->|Não| Y[Calcula Médias Globais]
    
    Y --> Z[Total Dias Período]
    Y --> AA[Dias com Registro]
    Y --> AB[% Dias com Registro]
    Y --> AC[Média Calorias/Dia]
    Y --> AD[Média Aderência]
    Y --> AE[Distribuição Macros Média]
    
    Z --> AF[Top 10 Alimentos]
    AA --> AF
    
    AF --> AG[Gera Insights]
    AG --> AH{Aderência > 70%?}
    AH -->|Sim| AI[Insight positivo]
    AH -->|Não| AJ[Insight sugestão melhoria]
    
    AI --> AK[Monta RelatorioAdesaoDto]
    AJ --> AK
    AB --> AK
    AC --> AK
    AD --> AK
    AE --> AK
    
    AK --> AL[Retorna JSON]
    AL --> AM[Frontend Renderiza]
    
    AM --> AN[Cards Estatísticas]
    AM --> AO[Gráficos]
    AM --> AP[Tabelas]
    AM --> AQ[Lista Insights]
    
    style AI fill:#51cf66
    style AJ fill:#ffd43b
```

---

**Fim dos Diagramas**

Use estes diagramas em conjunto com os documentos de fluxo e telas para ter uma visão completa da aplicação.
