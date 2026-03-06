# Guia de Telas do Frontend - Nutra Food

## 📱 Estrutura de Navegação

### Layout Principal

```
┌─────────────────────────────────────────┐
│  Header/Navbar                          │
│  - Logo                                 │
│  - Navegação principal (role-based)     │
│  - Notificações                         │
│  - Avatar/Menu usuário                  │
├─────────────────────────────────────────┤
│                                         │
│  Conteúdo Principal                     │
│                                         │
│                                         │
└─────────────────────────────────────────┘
```

---

## 🔐 Módulo de Autenticação

### 1. Tela de Login
**Rota**: `/login`
**Público**: Não autenticado

**Componentes**:
- Form com email e senha
- Botão "Entrar"
- Link "Esqueci minha senha"
- Link "Criar conta"
- Divisor "ou"
- Botões social login (futuro)

**Fluxo**:
1. Usuário preenche credenciais
2. Submit → POST /api/Auth/login
3. Sucesso:
   - Salva token no localStorage/cookies
   - Redireciona baseado no role:
     - Paciente → Verifica PerfilNutricional
     - Nutricionista → Verifica PerfilProfissional
4. Erro: Mostra mensagem

**Validações Frontend**:
- Email formato válido
- Senha não vazia

---

### 2. Tela de Registro
**Rota**: `/register`
**Público**: Não autenticado

**Componentes**:
- Radio button/Toggle: "Sou Paciente" / "Sou Nutricionista"
- Campos:
  - Nome Completo *
  - CPF * (com máscara)
  - Email *
  - Senha * (com strength indicator)
  - Confirmar Senha *
  - Data Nascimento (opcional)
  - Telefone (opcional, com máscara)
- Checkbox aceitar termos
- Botão "Criar conta"
- Link "Já tenho conta"

**Fluxo**:
1. Escolhe tipo de conta
2. Preenche dados
3. Submit → POST /api/Auth/register
4. Sucesso:
   - Auto-login (salva token)
   - Redireciona para onboarding específico do role

**Validações Frontend**:
- CPF válido (validação algorítmica)
- Email único (verificar ao blur)
- Senhas coincidem
- Senha forte (min 8 chars, maiúsc, minúsc, número)

---

### 3. Esqueci Senha
**Rota**: `/forgot-password`
**Público**: Não autenticado

**Componentes**:
- Campo email
- Botão "Enviar link de recuperação"
- Mensagem de confirmação (após envio)
- Link voltar ao login

---

## 🎯 Módulo de Onboarding

### 4. Onboarding - Perfil Nutricional (Paciente)
**Rota**: `/onboarding/perfil`
**Acesso**: Paciente sem PerfilNutricional (redirect forçado)

**Layout**: Wizard com steps visível no topo

**Step 1: Dados Pessoais**
- Data de Nascimento (date picker)
- Gênero Biológico (radio: Masculino/Feminino)
- Botão "Próximo"

**Step 2: Medidas Corporais**
- Altura (cm) - campo numérico
- Peso Atual (kg)
- Percentual de Gordura (%) - opcional
- Circunferência Cintura (cm) - opcional
- Circunferência Quadril (cm) - opcional
- Circunferência Braço (cm) - opcional
- Botões "Voltar" / "Próximo"

**Step 3: Estilo de Vida**
- Nível de Atividade Física (select dropdown):
  - Sedentário
  - Levemente Ativo
  - Moderadamente Ativo
  - Muito Ativo
  - Extremamente Ativo
- Ocupação Profissional (text)
- Habilidade Culinária (select):
  - Básico
  - Intermediário
  - Avançado
  - Profissional
- Orçamento Mensal para Alimentação (select):
  - Até R$ 300
  - R$ 300 - R$ 600
  - R$ 600 - R$ 1000
  - Acima de R$ 1000
- Horas de sono por noite (number)
- Qualidade do sono (slider 1-5 estrelas)
- Fumante? (toggle sim/não)

**Step 4: Saúde**
- Possui doenças pré-existentes? (toggle)
  - Se sim: textarea "Descreva suas condições médicas"
- Botão "+ Adicionar condição clínica" (abre modal):
  - Select Condição (Diabetes, Hipertensão, etc.)
  - Data diagnóstico
  - Está ativa? (toggle)
  - Medicamentos em uso (text)
  - Observações (textarea)
- Lista de condições adicionadas (chips removíveis)

**Step 5: Objetivos**
- Qual seu objetivo? (cards selecionáveis):
  - Perda de Peso
  - Ganho de Massa Muscular
  - Manutenção
  - Emagrecimento Saudável
  - Definição Muscular
  - Performance Esportiva
- Peso Desejado (kg) - opcional

**Step 6: Alimentação**
- Preferência Alimentar (select):
  - Onívoro
  - Ovolactovegetariano
  - Vegano
  - Low Carb
  - Cetogênica
  - Paleo
  - Mediterrânea
- Quantas refeições por dia? (number, min: 3, max: 7)
- Tempo disponível para preparo (minutos)

**Step 7: Restrições**
- Você tem alguma alergia ou intolerância? (checkboxes):
  - [ ] Lactose
  - [ ] Glúten
  - [ ] Amendoim
  - [ ] Frutos do Mar
  - [ ] Ovos
  - [ ] Soja
  - [ ] Nozes
  - [ ] Peixe
  - [ ] Outro (permite adicionar)

**Step 8: Equipamentos**
- Quais equipamentos você tem em casa? (checkboxes):
  - [ ] Fogão
  - [ ] Forno
  - [ ] Micro-ondas
  - [ ] Air Fryer
  - [ ] Liquidificador
  - [ ] Processador
  - [ ] Grill
  - [ ] Slow Cooker
  - [ ] Batedeira

**Step 9: Preferências (Opcional)**
- "Vamos conhecer seus gostos alimentares" (pode pular)
- Campo de busca de alimentos
- Para cada alimento encontrado:
  - Botões: 😍 Gosta / 😐 Nunca experimentou / 😞 Não gosta
- Lista de preferências marcadas
- Link "Pular por enquanto"

**Step Final: Revisão**
- Resumo visual dos dados preenchidos
- Botão "Confirmar e Criar Perfil"
- Link "Voltar e editar"

**Fluxo**:
1. Completa todos os steps
2. Submit → POST /api/User/perfil-nutricional (com todo o payload)
3. Sucesso:
   - Mostra loading "Calculando suas metas nutricionais..."
   - Redireciona para `/dashboard`
   - Mostra toast "Perfil criado! Suas metas foram calculadas"

---

### 5. Onboarding - Perfil Profissional (Nutricionista)
**Rota**: `/onboarding/nutricionista`
**Acesso**: Nutricionista sem PerfilProfissional (redirect forçado)

**Layout**: Tela única (não wizard pois são poucos campos)

**Componentes**:
- Título "Complete seu perfil profissional"
- Campos:
  - CRN * (text, formato: CRN-3 12345)
  - Região do CRN * (select 1-11)
  - Especialidade (text)
  - Anos de Experiência (number)
  - Bio Profissional (textarea, max 2000 chars)
  - Upload Diploma (file upload - futuro)
- Botão "Criar Perfil Profissional"

**Fluxo**:
1. Preenche dados
2. Submit → POST /api/Nutricionista/cadastro
3. Sucesso:
   - Mostra modal boas-vindas com info do plano gratuito
   - Redireciona para `/dashboard-nutricionista`

---

## 🏠 Dashboards

### 6. Dashboard Paciente
**Rota**: `/dashboard` ou `/`
**Acesso**: Paciente com PerfilNutricional

**Layout**: Grid de cards

**Componentes Principais**:

**Card 1: Resumo do Dia**
- Data atual
- Calorias: Progress bar circular
  - Consumido / Meta
  - Cor: verde se ±10% da meta, amarelo se ±20%, vermelho caso contrário
- Macros: Mini progress bars
  - Proteínas
  - Carboidratos
  - Gorduras
- Botão grande "➕ Registrar Consumo"

**Card 2: Plano Ativo**
- Se tem plano ativo:
  - Nome do plano
  - Data início
  - Aderência média (%)
  - Link "Ver Plano"
- Se não tem:
  - CTA "Criar meu primeiro plano"

**Card 3: Próxima Refeição**
- Se tem plano ativo:
  - Nome da próxima refeição (baseado no horário)
  - Horário sugerido
  - Itens principais (3 primeiros)
  - Link "Ver detalhes"
- Se não tem:
  - Sugestão baseada no horário atual

**Card 4: Metas Nutricionais**
- Meta de calorias/dia
- Meta de proteínas
- Meta de carbos
- Meta de gorduras
- Meta de água
- Link "Ver detalhes"

**Card 5: Última Avaliação**
- Se tem avaliações:
  - Data da última
  - Peso registrado
  - IMC
  - Link "Ver evolução"
- Se não tem:
  - CTA "Fazer primeira avaliação"

**Card 6: Quick Actions**
- Botão "📸 Adicionar Foto de Refeição"
- Botão "⚖️ Registrar Peso"
- Botão "📊 Ver Relatórios"

**Card 7: Insights (gerado por IA futuramente)**
- "Você está consumindo 15% menos proteína que o ideal"
- "Parabéns! 7 dias seguidos registrando suas refeições"

---

### 7. Dashboard Nutricionista
**Rota**: `/dashboard-nutricionista`
**Acesso**: Nutricionista com PerfilProfissional

**Layout**: Grid de cards + lista

**Componentes Principais**:

**Card 1: Resumo da Assinatura**
- Plano atual
- Pacientes ativos / Limite
- Progress bar visual
- Se próximo do limite: Warning "Você está próximo do limite"
- Link "Gerenciar assinatura"

**Card 2: Estatísticas**
- Total de pacientes ativos
- Total de planos criados este mês
- Taxa média de aderência dos pacientes
- Avaliações realizadas este mês

**Card 3: Ações Rápidas**
- Botão "➕ Convidar Paciente"
- Botão "📋 Criar Template de Dieta"
- Botão "🏥 Gerenciar Clínicas"

**Seção: Meus Pacientes**
- Tabs:
  - [ ] Todos
  - [ ] Alta Aderência (>80%)
  - [ ] Aderência Média (50-80%)
  - [ ] Baixa Aderência (<50%)
  - [ ] Sem Plano Ativo
- Search bar
- Filtros: ordem (alfabética, última atualização, aderência)
- Lista de pacientes (cards):
  - Avatar
  - Nome
  - Idade
  - Objetivo
  - Status plano (ativo/rascunho/pausado)
  - Aderência % (com cor)
  - Última interação (data)
  - Botão "Ver Paciente"

**Seção: Convites Pendentes**
- Lista de convites aguardando resposta
- Data do convite
- Botão "Cancelar convite"

---

## 👤 Módulo de Perfil

### 8. Meu Perfil (Geral)
**Rota**: `/perfil`
**Acesso**: Todos

**Tabs**:
- **Dados Cadastrais**
  - Nome, Email (não editável), CPF (não editável)
  - Telefone, Data Nascimento
  - Foto de Perfil (upload)
  - Endereço completo (campos)
  - Botão "Salvar Alterações" → PUT /api/Accounts/perfil

- **Segurança**
  - Alterar Senha
  - Autenticação 2FA (futuro)
  - Sessões ativas

- **Preferências do Aplicativo**
  - Idioma
  - Notificações (email, push)
  - Tema (claro/escuro)

---

### 9. Perfil Nutricional (Paciente)
**Rota**: `/perfil-nutricional`
**Acesso**: Paciente

**Layout**: Similar ao onboarding, mas modo edição

**Seções** (em accordion ou tabs):
- Dados Pessoais (data nascimento, gênero)
- Medidas Corporais
- Estilo de Vida
- Saúde e Condições Clínicas
- Objetivos
- Preferências Alimentares
- Restrições
- Equipamentos

**Botões**:
- "Salvar Alterações" → PUT /api/User/perfil-nutricional
- "Cancelar"

**Observação**: Ao salvar, mostra aviso "Suas metas nutricionais serão recalculadas"

---

### 10. Perfil Profissional (Nutricionista)
**Rota**: `/perfil-profissional`
**Acesso**: Nutricionista

**Componentes**:
- CRN (não editável)
- CRN Verificado (badge)
- Região CRN
- Especialidade (editável)
- Bio Profissional (editável)
- Anos de Experiência (editável)
- Diploma (upload)

**Botão "Salvar"** → PUT /api/Nutricionista/perfil

---

## 🎯 Módulo de Metas

### 11. Metas Nutricionais
**Rota**: `/metas`
**Acesso**: Paciente

**Layout**: Visual tipo infográfico

**Seção 1: Metas Atuais**
- Cards grandes com ícones:
  - 🔥 Calorias: XX kcal/dia
  - 💪 Proteínas: XX g/dia
  - 🍚 Carboidratos: XX g/dia
  - 🥑 Gorduras: XX g/dia
  - 🥤 Água: XX litros/dia
  - 🌾 Fibras: XX g/dia
- Data do cálculo

**Seção 2: Como Foram Calculadas?**
- Expandable/accordion com explicação:
  - Sua TMB (Taxa Metabólica Basal): XX kcal
  - Fator de atividade: 1.XX
  - Seu GET (Gasto Energético Total): XX kcal
  - Ajuste pelo objetivo (perda/ganho): ±XX kcal
  - Distribuição de macros:
    - Proteínas: X g/kg peso × seu peso
    - Resto distribuído entre carbo e gordura

**Seção 3: Histórico de Metas**
- Timeline de mudanças
- Cada mudança mostra:
  - Data
  - Motivo (atualização de perfil, mudança de peso, etc.)
  - Comparação antes/depois

**Botão**: "⚙️ Ajustar Perfil" → link para /perfil-nutricional

---

## 🍽️ Módulo de Preferências

### 12. Gerenciar Preferências Alimentares
**Rota**: `/preferencias`
**Acesso**: Paciente

**Layout**: Search + Grid

**Componentes**:

**Search Bar**
- Campo busca alimentos
- Filtros:
  - Tabela (TBCA, Fabricante, Fast Food, Genéricos)
  - Categoria (Cereais, Carnes, Frutas, etc.)

**Grid de Resultados**
- Cards de alimentos com:
  - Foto (se disponível)
  - Nome
  - Categoria
  - Botões de preferência:
    - 😍 Gosto
    - 😐 Neutro
    - 😞 Não Gosto

**Seção: Minhas Preferências**
- Tabs:
  - Alimentos que gosto
  - Alimentos que não gosto
- Lista filtrada
- Botão "Remover" em cada

**Submit**: Cada clique nos botões → POST /api/User/preferencia-alimentar

---

## ⚖️ Módulo de Registro Biométrico

### 13. Registrar Peso/Medidas
**Rota**: `/peso/novo`
**Acesso**: Paciente

**Layout**: Form simples

**Componentes**:
- Data (date picker, default: hoje)
- Peso (kg) *
- Percentual de Gordura (%) - opcional
- Circunferência Cintura (cm) - opcional
- Observações (textarea) - opcional
- Botão "Registrar"

**Submit**: POST /api/User/registro-biometrico

**Sucesso**:
- Toast "Peso registrado!"
- Redireciona para `/peso`

---

### 14. Histórico de Peso
**Rota**: `/peso`
**Acesso**: Paciente

**Layout**: Gráfico + Tabela

**Componentes**:

**Gráfico de Linha**
- Eixo X: Data
- Eixo Y: Peso (kg)
- Linha de tendência
- Linha do peso desejado (referência)
- Hover mostra detalhes

**Filtro de Período**
- Botões rápidos: 7 dias, 30 dias, 3 meses, 6 meses, 1 ano, Tudo
- Date range picker customizado

**Estatísticas do Período**
- Peso inicial: XX kg
- Peso atual: XX kg
- Variação: ±XX kg (±XX%)
- Média: XX kg
- Peso mínimo: XX kg (data)
- Peso máximo: XX kg (data)

**Tabela de Registros**
- Colunas: Data, Peso, % Gordura, Cintura, Observações, Ações
- Ordenável por coluna
- Ação: Editar, Excluir

**Botão FAB**: "➕ Novo Registro" → link para `/peso/novo`

---

## 📊 Módulo de Avaliação Nutricional

### 15. Nova Avaliação
**Rota**: `/avaliacao/nova`
**Acesso**: Paciente ou Nutricionista (para paciente)

**Layout**: Wizard multi-step (5 steps)

**Step 1: Básico**
- Peso (kg) *
- Altura (cm) *
- Observações gerais (textarea)

**Step 2: Circunferências** (Todas opcionais)
- Ilustração do corpo humano com pontos de medição
- Campos:
  - Pescoço
  - Tórax
  - Cintura
  - Abdômen
  - Quadril
  - Braço Direito / Esquerdo
  - Antebraço Direito / Esquerdo
  - Coxa Direita / Esquerda
  - Panturrilha Direita / Esquerda

**Step 3: Dobras Cutâneas** (Opcional)
- Toggle "Incluir dobras cutâneas?"
- Se sim:
  - Select protocolo (Jackson-Pollock 3/7, outro)
  - Campos de dobras (mm):
    - Tríceps
    - Bíceps
    - Subescapular
    - Suprailiaca
    - Abdominal
    - Coxa
    - Panturrilha
    - Axilar média
    - Peitoral

**Step 4: Bioimpedância** (Opcional)
- Toggle "Incluir dados de bioimpedância?"
- Se sim:
  - % Gordura
  - Massa Magra (kg)
  - Massa Gorda (kg)
  - Água Corporal (L)
  - % Água
  - TMB (kcal) - do aparelho
  - Nível Gordura Visceral
  - Idade Metabólica
  - Massa Óssea (kg)

**Step 5: Fotos de Progresso** (Opcional)
- Upload múltiplo
- Para cada foto:
  - Select tipo (Frente, Costas, Lateral Direito, Lateral Esquerdo, Outro)
  - Descrição
- Preview das fotos

**Step Final: Preview**
- Resumo dos dados inseridos
- Preview dos cálculos (IMC, TMB, GET, etc.)
- Botão "Registrar Avaliação"

**Submit**: POST /api/Avaliacao/registrar

**Sucesso**:
- Mostra modal com resultados completos (cálculos)
- Link "Ver Detalhes da Avaliação"
- Botão "Voltar ao Dashboard"

---

### 16. Histórico de Avaliações
**Rota**: `/avaliacoes`
**Acesso**: Paciente ou Nutricionista (para paciente)

**Layout**: Timeline + Cards

**Filtros**:
- Período (date range)
- Tipo (Com bioimpedância, Com dobras, Com fotos, Todas)

**Timeline Vertical**:
- Cada avaliação:
  - Data
  - Card com resumo:
    - Peso: XX kg
    - IMC: XX (classificação)
    - % Gordura: XX% (se disponível)
    - GET: XX kcal
    - Badges: 📷 Tem fotos, 📏 Tem dobras, ⚡ Tem bioimpedância
  - Botões:
    - "Ver Detalhes"
    - "Comparar"
    - "Excluir"

**Botão FAB**: "➕ Nova Avaliação"

---

### 17. Detalhes da Avaliação
**Rota**: `/avaliacoes/{id}`
**Acesso**: Paciente ou Nutricionista (para paciente)

**Layout**: Tabs ou Accordion

**Seção 1: Informações Gerais**
- Data da avaliação
- Profissional responsável (se houver)
- Observações

**Seção 2: Medidas Básicas**
- Peso: XX kg
- Altura: XX cm
- IMC: XX (classificação com cor)

**Seção 3: Circunferências**
- Tabela com todas as medidas
- Comparação com avaliação anterior (se houver):
  - Verde: diminuiu
  - Vermelho: aumentou
  - Cinza: sem mudança significativa

**Seção 4: Composição Corporal**
- % Gordura: XX%
- Massa Magra: XX kg
- Massa Gorda: XX kg
- Classificação visual (progress bar)

**Seção 5: Cálculos Automáticos**
- TMB (3 fórmulas):
  - Mifflin-St Jeor: XX kcal
  - Harris-Benedict: XX kcal
  - Katch-McArdle: XX kcal (se disponível)
- GET: XX kcal/dia
- Peso Ideal:
  - Devine: XX kg
  - IMC: XX kg
- RCQ: X.XX (classificação)

**Seção 6: Fotos de Progresso**
- Galeria de fotos
- Lightbox ao clicar
- Opção de adicionar mais fotos

**Botões de Ação**:
- "✏️ Editar Avaliação" (futuro)
- "📊 Comparar com Outra"
- "🗑️ Excluir"
- "📄 Exportar PDF" (futuro)

---

### 18. Comparar Avaliações
**Rota**: `/avaliacoes/comparar?anterior={id1}&atual={id2}`
**Acesso**: Paciente ou Nutricionista

**Layout**: Duas colunas lado a lado

**Componentes**:

**Header**:
- Selectores de avaliação:
  - dropdown "Avaliação Anterior"
  - vs
  - dropdown "Avaliação Atual"
- Período entre avaliações: XX dias

**Coluna Esquerda**: Avaliação Anterior (data)
**Coluna Direita**: Avaliação Atual (data)

**Linhas Comparativas**:
- Peso: XX kg | XX kg → Delta: ±X kg 📈/📉
- IMC: XX | XX → Delta: ±X.X
- % Gordura: XX% | XX% → Delta: ±X.X%
- Massa Magra: XX kg | XX kg → Delta: ±X kg
- Massa Gorda: XX kg | XX kg → Delta: ±X kg
- GET: XX kcal | XX kcal → Delta: ±XX kcal
- Circunferências (principais):
  - Cintura: XX cm | XX cm → ±X cm
  - Quadril: XX cm | XX cm → ±X cm

**Seção Fotos**:
- Galeria comparativa
- Mesmas poses lado a lado (se disponível)
- Slider para comparar (before/after)

**Gráficos**:
- Evolução entre as duas avaliações
- Projeção se mantiver tendência

**Insights**:
- "Você perdeu X kg em Y dias"
- "Sua massa magra aumentou, parabéns!"
- "Reduziu X cm de cintura"

---

### 19. Anamnese Alimentar
**Rota**: `/anamnese`
**Acesso**: Paciente ou Nutricionista (para paciente)

**Layout**: Form extenso com seções expandíveis

**Seções**:

**1. Rotina Alimentar**
- Quantas refeições por dia?
- Horários habituais:
  - Café da manhã
  - Lanche manhã
  - Almoço
  - Lanche tarde
  - Jantar
  - Ceia
- Quais refeições costuma pular?

**2. Hidratação e Bebidas**
- Litros de água/dia
- Consumo de refrigerantes (frequência)
- Consumo de álcool (frequência)
- Consumo de café/chá (frequência)

**3. Hábitos Alimentares**
- Frequência de fast food
- Frequência de frutas
- Frequência de verduras/legumes
- Frequência de doces
- Frequência de frituras

**4. Comportamento**
- Come assistindo TV/celular? (sim/não)
- Sente compulsão alimentar? (sim/não)
- Já fez dietas restritivas? (sim/não)
  - Se sim: quais? (text)
- Usa suplementos? Quais?

**5. Digestão**
- Intestino regular? (sim/não)
- Evacuações por semana
- Queixas digestivas (gases, refluxo, etc.)

**6. Preferências**
- Alimentos que não gosta
- Alimentos preferidos

**7. Observações Gerais**
- Textarea livre

**Botões**:
- "Salvar como Rascunho"
- "Enviar Anamnese"

**Submit**: POST /api/Avaliacao/anamnese

---

## 🍴 Módulo de Planos Alimentares

### 20. Meus Planos
**Rota**: `/planos`
**Acesso**: Paciente ou Nutricionista (paciente selecionado)

**Layout**: Cards em grid

**Filtros/Tabs**:
- Todos
- Ativo (destaque especial)
- Rascunhos
- Pausados
- Finalizados

**Card de Plano**:
- Badge de status (Ativo/Rascunho/Pausado/Finalizado)
- Nome do plano
- Descrição (truncada)
- Data início - data fim
- Metas: XX kcal/dia
- Número de refeições: X refeições/dia
- Profissional criador (se aplicável)
- Aderência média (se ativo): XX%
- Botões:
  - "Ver Plano"
  - Menu ⋮ (Editar, Ativar, Pausar, Excluir)

**Plano Ativo** (destaque visual):
- Borda/sombra diferenciada
- Badge "ATIVO"
- Estatísticas adicionais:
  - Dias ativo
  - Aderência última semana

**Botão FAB**: "➕ Novo Plano"

---

### 21. Criar Plano - Escolher Origem
**Rota**: `/planos/novo`
**Acesso**: Paciente ou Nutricionista

**Layout**: Modal ou página de escolha

**Opções** (cards grandes):

**Opção 1: Criar do Zero**
- Ícone ✏️
- "Criar Plano Personalizado"
- Descrição: "Monte seu próprio plano alimentar"
- Botão: "Começar" → `/planos/editor/novo`

**Opção 2: Usar Template**
- Ícone 📋
- "Usar Template Pronto"
- Descrição: "Escolha um modelo e adapte às suas necessidades"
- Botão: "Ver Templates" → Modal de templates

**Modal de Templates**:
- Search bar
- Filtros:
  - Objetivo (perda peso, ganho massa, etc.)
  - Tipo de dieta (low carb, vegana, etc.)
  - Criador (públicos, meus templates)
- Grid de templates:
  - Nome
  - Objetivo
  - Tipo de dieta
  - Calorias base
  - Nº refeições
  - Botão "Usar Este"
- Ao selecionar → Redireciona para editor com template carregado

---

### 22. Editor de Plano
**Rota**: `/planos/editor/{id}` ou `/planos/editor/novo`
**Acesso**: Paciente ou Nutricionista

**Layout**: Form complexo com preview

**Divisão da Tela**:
- Esquerda (60%): Editor
- Direita (40%): Preview e Resumo

**Seção Editor - Informações Gerais**:
- Nome do Plano *
- Descrição
- Data Início *
- Data Fim
- Observações

**Seção Editor - Metas Diárias**:
- Calorias alvo /dia *
- Proteínas (g) *
- Carboidratos (g) *
- Gorduras (g) *
- Fibras (g)
- Água (L)
- Botão "Calcular a partir do meu perfil" (preenche automaticamente com base na meta nutricional)

**Seção Editor - Refeições**:
- Accordion/lista de refeições
- Botão "➕ Adicionar Refeição"

**Para Cada Refeição**:
- Select Tipo (Café, Lanche, Almoço, Jantar, Ceia)
- Time picker: Horário sugerido
- Ordem (número)
- Observações
- **Lista de Itens**:
  - Botão "➕ Adicionar Alimento"
  - Para cada item:
    - Nome do alimento (com busca)
    - Quantidade (g)
    - Preview macros: XX kcal | XX g prot | XX g carb | XX g gord
    - Botão "Substituições" (abre modal)
    - Botão "Remover"
- **Totais da Refeição**:
  - Energia: XX kcal
  - Macros: XX g / XX g / XX g
  - Progress bar comparando com meta diária (percentual)

**Modal Adicionar Alimento**:
- Search bar
- Filtros (tabela, categoria)
- Grid de resultados
- Para cada alimento:
  - Nome, categoria, tabela
  - Info nutricional (por 100g)
  - Campo quantidade
  - Botão "Adicionar"

**Modal Substituições**:
- Item original (referência)
- Search bar para buscar substitutos
- Critério: "Buscar alimentos com macros similares"
- Lista de substitutos adicionados:
  - Nome
  - Quantidade sugerida
  - Comparação macros (vs original)
  - Botão "Remover"

**Painel Direito - Preview**:
- **Resumo Geral**:
  - Total calorias das refeições: XX kcal
  - Meta: XX kcal
  - Progress bar e % de atingimento
  - Distribuição de macros (gráfico pizza ou barras)
- **Refeições (miniatura)**:
  - Lista simples de refeições
  - Horário, nome, calorias
- **Alertas**:
  - ⚠️ "Você está 200 kcal abaixo da meta"
  - ⚠️ "Baixo consumo de proteínas"
  - ✅ "Distribuição de macros adequada"

**Botões de Ação** (footer fixo):
- "Salvar como Rascunho"
- "Publicar" (muda status para Ativo se for o único, ou pergunta se quer ativar)
- "Cancelar"

---

### 23. Visualizar Plano
**Rota**: `/planos/{id}`
**Acesso**: Paciente, Nutricionista

**Layout**: Modo leitura

**Header**:
- Nome do plano
- Badge de status
- Descrição
- Período: data início - data fim
- Criado por: [Nome do profissional ou "Você mesmo"]
- Observações

**Seção: Informações Gerais**
- Cards com metas diárias (calorias, macros)

**Seção: Refeições**
- Timeline vertical ou cards por horário
- Para cada refeição:
  - Ícone do tipo
  - Horário sugerido
  - Nome da refeição
  - **Lista de alimentos**:
    - Nome
    - Quantidade
    - Macros
    - Substitutos (expandível)
  - **Totais**:
    - XX kcal
    - Macros

**Seção: Resumo Nutricional**
- Gráficos:
  - Distribuição calórica por refeição (pizza)
  - Distribuição de macros (barras)
- Comparação com meta

**Botões de Ação** (se for dono ou nutricionista do paciente):
- "✏️ Editar"
- "▶️ Ativar Plano" (se não estiver ativo)
- "⏸️ Pausar" (se estiver ativo)
- "🗑️ Excluir"
- "📋 Duplicar"
- "📄 Exportar PDF"

**Se for nutricionista**:
- Botão "Atribuir a Paciente" → modal com lista de pacientes

---

### 24. Templates (Nutricionista)
**Rota**: `/templates`
**Acesso**: Nutricionista

**Layout**: Similar a "Meus Planos"

**Tabs**:
- Meus Templates
- Templates Públicos

**Card de Template**:
- Nome
- Descrição
- Objetivo alvo
- Tipo de dieta alvo
- Calorias base
- Nº refeições
- Badge "Público"/"Privado"
- Ações:
  - Ver
  - Editar
  - Duplicar
  - Excluir
  - Tornar Público/Privado

**Botão FAB**: "➕ Novo Template"

---

### 25. Editor de Template
**Rota**: `/templates/editor/{id}` ou `/templates/editor/novo`
**Acesso**: Nutricionista

**Layout**: Similar ao Editor de Plano

**Diferenças**:
- Adiciona campos:
  - Objetivo Alvo (select)
  - Tipo de Dieta Alvo (select)
  - Público (toggle)
- Não tem data início/fim
- Macros são "base" (serão escalados ao usar)

---

## 📔 Módulo de Diário Alimentar

### 26. Dashboard Diário (Resumo do Dia)
**Rota**: `/diario` ou integrado no dashboard principal
**Acesso**: Paciente

**Layout**: Cards + progress

**Header**:
- Date picker (default: hoje)
- Botões: Voltar dia, Avançar dia
- Badge: dia útil, fim de semana

**Card Principal: Metas do Dia**
- Progress bar circular grande (calorias)
  - Consumido / Meta
  - Kcal restantes
- Mini progress bars (macros):
  - Proteínas: XX/XX g
  - Carbos: XX/XX g
  - Gorduras: XX/XX g
  - Água: XX/XX L

**Seção: Refeições**
- Accordion ou cards por refeição
- Para cada refeição:
  - Ícone + Nome (Café, Almoço, etc.)
  - Se tem plano:
    - "Planejado vs Consumido"
    - Progress bar de aderência
  - **Lista de consumos**:
    - Foto (se têm)
    - Nome alimento
    - Quantidade
    - Macros
    - Hora registro
    - Botão "Editar"/ "Remover"
  - Botão "➕ Adicionar Consumo"
  - **Totais da Refeição**:
    - XX kcal
    - Macros

**Seção: Fotos do Dia**
- Galeria horizontal
- Fotos organizadas por refeição

**Botão FAB**: "➕ Registrar Consumo" → modal de registro

---

### 27. Registrar Consumo
**Rota**: `/diario/registrar` ou modal
**Acesso**: Paciente

**Layout**: Modal ou página com form

**Componentes**:

**Step 1: Buscar Alimento**
- Search bar *
- Filtros (tabela, categoria)
- **Opção alternativa**: "Escanear código de barras" (futuro)
- Grid de resultados:
  - Nome
  - Categoria
  - Info nutricional (resumo)
  - Botão "Selecionar"

**Step 2: Detalhes do Consumo** (após selecionar alimento)
- Nome do alimento (display)
- Tabela nutricional (por 100g) - preview
- Campo Quantidade * (g ou ml)
  - Sugestão de porções comuns (1 unidade = XX g, 1 xícara = XX g, etc.)
- **Preview dinâmico**: "Para XX g, você consumirá:"
  - XX kcal
  - XX g proteína
  - XX g carboidrato
  - XX g gordura
  - XX g fibra

**Step 3: Quando?**
- Date time picker (default: agora)
- Select Tipo de Refeição * (Café, Lanche, Almoço, etc.)
- Se tem plano ativo:
  - Checkbox "Este consumo faz parte do plano?"
  - Se sim: select item do plano relacionado

**Step 4: Foto (Opcional)**
- Upload foto
- Webcam/câmera (mobile)

**Step 5: Observações**
- Textarea (opcional)

**Resumo Final**:
- Card com tudo preenchido
- Botão "Confirmar Registro"

**Submit**: POST /api/Diario/registro

**Sucesso**:
- Toast "Consumo registrado!"
- Opções:
  - "Registrar Outro"
  - "Voltar ao Diário"
- Atualiza dashboard em tempo real (se estiver aberto)

---

### 28. Calendário do Diário
**Rota**: `/diario/calendario`
**Acesso**: Paciente

**Layout**: Calendário mensal

**Componentes**:

**Seletor de Mês/Ano**
- Dropdown ou navegação ◀ Mês Ano ▶

**Grid Calendário**
- Cada dia:
  - Data
  - Indicador visual baseado em registros:
    - 🟢 Verde: alta aderência (>80%) ou meta atingida
    - 🟡 Amarelo: aderência média (50-80%)
    - 🔴 Vermelho: baixa aderência (<50%)
    - ⚪ Cinza: sem registros
  - Mini info ao hover:
    - Consumido: XX kcal
    - Meta: XX kcal
    - Registros: X refeições

**Click no Dia**: Redireciona para `/diario?data={yyyy-MM-dd}`

**Legenda**:
- Explicação das cores

**Estatísticas do Mês** (sidebar):
- Total de dias com registro: XX/30
- Aderência média: XX%
- Streak atual: X dias seguidos
- Melhor semana
- Pior semana

---

### 29. Relatórios e Estatísticas
**Rota**: `/diario/relatorios`
**Acesso**: Paciente ou Nutricionista (para paciente)

**Layout**: Dashboards analíticos

**Filtros**:
- Período (date range)
  - Botões rápidos: 7 dias, 30 dias, 3 meses
- Comparar com:
  - Plano ativo
  - Meta nutricional
  - Sem comparação

**Seção 1: Visão Geral**
- Cards:
  - Total de dias no período
  - Dias com registro: XX/XX (XX%)
  - Média calorias/dia: XX kcal
  - Aderência média ao plano: XX%

**Seção 2: Calorias**
- Gráfico de linha:
  - Eixo X: Dias
  - Eixo Y: Kcal
  - Linha: Consumo diário
  - Linha referência: Meta
  - Área sombreada: margem ±10%
- Estatísticas:
  - Maior consumo: XX kcal (data)
  - Menor consumo: XX kcal (data)
  - Desvio padrão
  - Dias dentro da meta: XX%

**Seção 3: Macronutrientes**
- Gráfico de barras empilhadas:
  - Cada dia uma barra
  - Cores: Proteínas (azul), Carbos (laranja), Gorduras (verde)
- Percentuais médios:
  - Proteínas: XX%
  - Carbos: XX%
  - Gorduras: XX%
- Comparação com recomendado

**Seção 4: Por Refeição**
- Gráfico pizza: distribuição calórica
  - Café: XX%
  - Almoço: XX%
  - Jantar: XX%
  - Lanches: XX%
- Tabela:
  - Refeição | Média kcal | % do dia | Aderência

**Seção 5: Análise de Aderência** (se tem plano)
- Progress bars por refeição
- Refeições com maior aderência
- Refeições com menor aderência
- Dias com 100% de aderência: X dias

**Seção 6: Alimentos Mais Consumidos**
- Top 10 alimentos
- Ranking com:
  - Nome
  - Vezes consumido
  - Total kcal acumuladas
  - % do consumo total

**Seção 7: Insights Automáticos** (gerados por IA futuramente)
- Lista de insights:
  - "Você consumiu 20% menos carboidratos neste período"
  - "Sua aderência melhorou 15% em relação ao mês passado"
  - "Você está consumindo proteínas de forma consistente"
  - "Considere aumentar o consumo de água"

**Botões**:
- "📄 Exportar Relatório (PDF)"
- "📧 Enviar para Nutricionista" (se tiver vínculo)

---

### 30. Fotos de Refeições
**Rota**: `/diario/fotos`
**Acesso**: Paciente

**Layout**: Galeria

**Filtros**:
- Período (date range)
- Tipo de refeição
- Com/sem vinculo a registro

**Grid de Fotos**:
- Thumbnails
- Hover mostra:
  - Data/hora
  - Tipo refeição
  - Registros vinculados (se houver)
- Click: Abre lightbox
  - Navegação entre fotos
  - Informações completas
  - Opção "Editar" / "Excluir"
  - Link para registro vinculado

**Botão FAB**: "➕ Adicionar Foto"

**Modal Adicionar Foto**:
- Upload ou câmera
- Select tipo refeição
- Descrição
- Vincular a registro? (busca registros do dia)
- Botão "Salvar"

---

## 🔗 Módulo de Vínculos

### 31. Convites e Vínculos (Paciente)
**Rota**: `/vinculos`
**Acesso**: Paciente

**Layout**: Tabs

**Tab 1: Convites Pendentes**
- Lista de convites
- Para cada convite:
  - Card com:
    - Avatar do nutricionista
    - Nome
    - CRN
    - Especialidade
    - Clínica (se informada)
    - Data do convite
    - Observações (se houver)
    - Botões:
      - "✅ Aceitar"
      - "❌ Recusar"

**Modal Aceitar Convite**:
- "Você tem certeza que deseja aceitar o convite de [Nome]?"
- Info sobre o que o nutricionista poderá acessar
- Checkbox "Li e concordo"
- Botões: "Sim, aceitar" / "Cancelar"

**Tab 2: Meu Nutricionista**
- Se tem vínculo ativo:
  - Card grande:
    - Avatar
    - Nome do profissional
    - CRN (verificado?)
    - Especialidade
    - Anos experiência
    - Bio
    - Clínica:
      - Nome
      - Endereço
      - Telefone
      - Email
    - Data início do vínculo
    - Botões:
      - "💬 Enviar Mensagem" (futuro)
      - "🔗 Encerrar Vínculo"
- Se não tem:
  - Empty state: "Você ainda não tem um nutricionista vinculado"
  - CTA: "Buscar Nutricionistas" (futuro)

**Modal Encerrar Vínculo**:
- "Você tem certeza?"
- Explicação do que acontecerá (perderá acesso, mas dados permanecerão)
- Motivo (opcional dropdown + textarea)
- Botões: "Sim, encerrar" / "Cancelar"

---

### 32. Gestão de Pacientes - Lista (Nutricionista)
**Rota**: `/pacientes`
**Acesso**: Nutricionista

**Layout**: Grid/lista pesquisável

**Header**:
- Search bar (nome, email, cpf)
- Filtros:
  - Status vínculo (Todos, Ativos, Pendentes, Encerrados)
  - Tem plano ativo (Sim/Não/Todos)
  - Aderência (Alta/Média/Baixa/Todos)
  - Clínica (se tem múltiplas)
- Ordenação:
  - Alfabética
  - Última atualização
  - Aderência
  - Data vínculo

**Tabs**:
- Pacientes Ativos (XX)
- Convites Pendentes (X)
- Encerrados

**Card/Linha de Paciente**:
- Avatar
- Nome
- Idade
- Objetivo (badge)
- Plano ativo: [Nome] ou "Sem plano"
- Aderência: progress bar mini + % (7 dias)
- Última interação: X dias atrás
- Última avaliação: X dias atrás
- Botões:
  - "Ver Perfil"
  - Menu ⋮:
    - Criar Plano
    - Registrar Avaliação
    - Ver Diário
    - Relatórios
    - Encerrar Vínculo

**Estatísticas** (cards no topo):
- Total pacientes ativos: XX/XX (limite plano)
- Aderência média: XX%
- Pacientes sem plano: X
- Pacientes precisando avaliação (>30 dias): X

**Botão FAB**: "➕ Convidar Paciente"

---

### 33. Convidar Paciente (Nutricionista)
**Rota**: `/pacientes/convidar`
**Acesso**: Nutricionista

**Layout**: Form modal ou página

**Componentes**:

**Verificar Limite**:
- Se chegou no limite:
  - Alert: "Você atingiu o limite de pacientes do seu plano"
  - CTA: "Fazer upgrade"
  - Bloqueia form

**Form**:
- Buscar paciente:
  - Radio: Por Email / Por CPF
  - Campo de busca
  - Botão "Buscar"
- Se encontrar:
  - Mostracard com info básica do usuário
  - Nome, email
  - Já é paciente? (aviso se já tem vínculo)
- Select Clínica (se tem múltiplas)
- Textarea Observações (opcional)
- Botão "Enviar Convite"

**Submit**: POST /api/Nutricionista/pacientes/convidar

**Sucesso**:
- Toast "Convite enviado!"
- Opções:
  - "Convidar Outro"
  - "Ver Meus Pacientes"

**Error Handling**:
- Usuário não encontrado: mensagem clara
- Limite atingido: redirect upgrade
- Vínculo já existe: mensagem informativa

---

### 34. Perfil do Paciente (Visão do Nutricionista)
**Rota**: `/pacientes/{id}`
**Acesso**: Nutricionista (apenas com vínculo ativo)

**Layout**: Tabs extensivas

**Header**:
- Avatar
- Nome do paciente
- Idade, gênero
- Status vínculo
- Data início vínculo
- Botões de ação:
  - "📋 Criar Plano"
  - "📊 Registrar Avaliação"
  - "📸 Ver Diário"
  - Menu ⋮:
    - Editar Observações
    - Encerrar Vínculo

**Tab 1: Resumo**
- **Objetivos**:
  - Objetivo atual
  - Peso atual: XX kg
  - Peso desejado: XX kg
  - Diferença: XX kg
- **Metas Nutricionais Atuais**:
  - Cards com valores
- **Últimas Atualizações**:
  - Última avaliação: [data] → link
  - Último registro peso: [data]
  - Último registro diário: [data]
- **Aderência Recente** (7/30 dias):
  - % aderência
  - Progress bar
  - Média calorias vs meta

**Tab 2: Perfil Nutricional Completo**
- Todas as informações do PerfilNutricional
- Modo leitura
- Botão "Sugerir Atualização" (envia notificação)

**Tab 3: Histórico de Avaliações**
- Similar à tela de avaliações do paciente
- Lista de avaliações
- Ver detalhes
- Comparar
- Botão "Nova Avaliação"

**Tab 4: Planos Alimentares**
- Lista de planos do paciente
- Status, datas, aderência
- Ver, editar (se criou esse plano)
- Botão "Criar Novo Plano"

**Tab 5: Diário Alimentar**
- Calendário do paciente (view-only ou comentários)
- Seletor data
- Ver registros
- Estatísticas
- Botão "Ver Relatórios Detalhados"

**Tab 6: Evolução e Peso**
- Gráficos de peso ao longo do tempo
- Comparação com objetivo
- Velocidade de ganho/perda

**Tab 7: Anamnese**
- Se preencheu: visualização completa
- Se não: CTA "Solicitar Preenchimento"

**Tab 8: Comunicação** (futuro)
- Histórico de mensagens
- Anotações privadas do profissional

---

## 🏥 Módulo de Clínicas (Nutricionista)

### 35. Gerenciar Clínicas
**Rota**: `/clinicas`
**Acesso**: Nutricionista

**Layout**: Cards

**Verificar Limite**:
- Se não é Enterprise e já tem 1 clínica:
  - Alert: "Seu plano permite apenas 1 clínica"
  - Link: "Fazer upgrade para Enterprise"
  - Botão adicionar desabilitado

**Card de Clínica**:
- Nome
- CNPJ
- Endereço
- Telefone / Email
- Logo (se tiver)
- Total pacientes vinculados a esta clínica: X
- Botões:
  - "✏️ Editar"
  - "🗑️ Remover"

**Botão FAB**: "➕ Nova Clínica" (se permitido pelo plano)

---

### 36. Criar/Editar Clínica
**Rota**: `/clinicas/nova` ou `/clinicas/{id}/editar`
**Acesso**: Nutricionista

**Layout**: Form

**Componentes**:
- Nome *
- CNPJ (com máscara) - opcional
- Telefone (com máscara)
- Email
- Upload Logo
- **Endereço**:
  - CEP (com busca automática)
  - Logradouro
  - Número
  - Complemento
  - Bairro
  - Cidade
  - Estado (select)
- Botões: "Salvar" / "Cancelar"

**Submit**:
- POST /api/Nutricionista/clinicas (criar)
- PUT /api/Nutricionista/clinicas/{id} (editar)

---

## 💳 Módulo de Assinatura (Nutricionista)

### 37. Minha Assinatura
**Rota**: `/assinatura`
**Acesso**: Nutricionista

**Layout**: Cards informativos + CTA

**Card 1: Plano Atual**
- Badge do plano (Gratuito, Básico, Profissional, Enterprise)
- Status (Trial, Ativa, Cancelada, Suspensa)
- Se trial: "Restam X dias de teste"
- Valor mensal (se pago)
- Data início / próxima cobrança
- Renovação automática (toggle se pago)

**Card 2: Limites e Uso**
- Progress bars:
  - Pacientes: XX/XX (visual de proximidade do limite)
  - Clínicas: X/X
- Multi-clínica: badge HABILITADO/DESABILITADO
- Recursos adicionais do plano (lista de checks)

**Card 3: Histórico de Pagamentos** (se pago)
- Lista últimos pagamentos
- Data, valor, status
- Botão "Ver Todos"

**Seção: Planos Disponíveis** (se não é Enterprise)
- Comparison table:
  - Colunas: Gratuito, Básico, Profissional, Enterprise
  - Linhas:
    - Preço /mês
    - Max pacientes
    - Clínicas
    - Relatórios avançados
    - Suporte prioritário
    - API access
    - Whitelabel (futuro)
  - Botão "Upgrade" (se não for o plano atual)

**Botões de Ação**:
- "Fazer Upgrade"
- "Cancelar Assinatura" (se pago)
- "Histórico Completo"

**Modal Upgrade**:
- Selecionar novo plano
- Resumo de diferenças
- Informações de cobrança (integração gateway pagamento)
- Botão "Confirmar Upgrade"

**Modal Cancelar**:
- "Tem certeza?"
- Consequências (perda de recursos, limite de pacientes)
- Motivo (select + textarea)
- Botão "Sim, cancelar" (downgrada para gratuito ao fim do período)

---

## 🎨 Componentes Reutilizáveis

### Componentes Globais

1. **SearchAlimentos**: Componente de busca de alimentos
   - Props: onSelect, filtroTabela, filtroCategoria
   - UI: Search bar + grid de resultados
   - Usado em: Registrar consumo, criar plano, criar template, preferências

2. **MacroDisplay**: Display visual de macronutrientes
   - Props: proteina, carboidrato, gordura, calorias, showLabels
   - UI: Badges coloridos ou barras horizontais
   - Usado em: múltiplas telas

3. **ProgressCircular**: Progress bar circular
   - Props: current, max, label, size, color
   - Usado em: dashboard, diário, metas

4. **AvaliacaoTimeline**: Componente de linha do tempo de avaliações
   - Props: avaliacoes[], onSelect, showCompare
   - Usado em: histórico avaliações

5. **PlanCard**: Card de plano alimentar
   - Props: plano, actions[], onAction
   - Usado em: lista planos, dashboard

6. **DateRangePicker**: Seletor de período
   - Props: startDate, endDate, onChange, presets[]
   - Usado em: relatórios, calendário

7. **WizardStepper**: Componente wizard multi-step
   - Props: steps[], currentStep, onNext, onBack, onComplete
   - Usado em: onboardings, avaliações

8. **ImageUploader**: Upload de imagens
   - Props: onUpload, maxFiles, allowCamera
   - Usado em: fotos perfil, fotos refeição, fotos progresso

9. **NutritionalLabel**: Rótulo nutricional visual (estilo tabela ANVISA)
   - Props: alimento, quantidade
   - Usado em: detalhes alimento, preview consumo

10. **EmptyState**: Estado vazio com CTA
    - Props: icon, title, description, action, onAction
    - Usado em: listas vazias

---

## 🚀 Priorização de Desenvolvimento

### Sprint 1 - MVP Base (2-3 semanas)
Objetivo: Usuário consegue registrar, criar perfil e registrar consumo

- [ ] Login
- [ ] Registro
- [ ] Onboarding Perfil Nutricional (Paciente)
- [ ] Onboarding Perfil Profissional (Nutricionista) - básico
- [ ] Dashboard Paciente - versão simples
- [ ] Visualizar Meta Nutricional
- [ ] Registrar Consumo (modal básico)
- [ ] Diário do Dia - resumo
- [ ] SearchAlimentos (componente)
- [ ] MacroDisplay (componente)

**Entregável**: Paciente pode se cadastrar, criar perfil, ver suas metas e registrar o que comeu.

---

### Sprint 2 - Gestão de Perfil e Registros (2 semanas)
Objetivo: Usuário consegue gerenciar perfil e acompanhar peso

- [ ] Editar Perfil Usuário
- [ ] Editar Perfil Nutricional
- [ ] Registrar Peso
- [ ] Histórico de Peso (com gráfico)
- [ ] Gerenciar Preferências Alimentares
- [ ] Dashboard Nutricionista - básico

**Entregável**: Usuário consegue atualizar informações e acompanhar evolução de peso.

---

### Sprint 3 - Planos Alimentares (3 semanas)
Objetivo: Usuário consegue criar e seguir planos

- [ ] Meus Planos
- [ ] Criar Plano (do zero, sem templates)
- [ ] Editor de Plano (completo)
- [ ] Visualizar Plano
- [ ] Ativar Plano
- [ ] Diário com Comparação Plano vs Consumo
- [ ] PlanCard (componente)

**Entregável**: Paciente cria plano e compara consumo com planejado.

---

### Sprint 4 - Diário Avançado (2 semanas)
Objetivo: Relatórios e análises

- [ ] Calendário Diário
- [ ] Relatórios e Estatísticas
- [ ] Fotos de Refeição (básico)
- [ ] DateRangePicker (componente)

**Entregável**: Paciente vê evolução e padrões de consumo.

---

### Sprint 5 - Vínculos Nutricionista-Paciente (2 semanas)
Objetivo: Nutricionista gerencia pacientes

- [ ] Convidar Paciente
- [ ] Aceitar/Recusar Convite (Paciente)
- [ ] Lista de Pacientes (Nutricionista)
- [ ] Perfil do Paciente (view nutricionista)
- [ ] Criar Plano para Paciente

**Entregável**: Fluxo de vínculo funcional.

---

### Sprint 6 - Avaliações Antropométricas (3 semanas)
Objetivo: Avaliações completas

- [ ] Nova Avaliação (wizard)
- [ ] Histórico de Avaliações
- [ ] Detalhes da Avaliação
- [ ] Comparar Avaliações
- [ ] Fotos de Progresso
- [ ] WizardStepper (componente)
- [ ] ImageUploader (componente)

**Entregável**: Avaliações físicas completas com cálculos automáticos.

---

### Sprint 7 - Templates e Recursos Avançados (2 semanas)
Objetivo: Templates de dieta e funcionalidades extras

- [ ] Templates (Nutricionista)
- [ ] Editor de Template
- [ ] Usar Template ao criar plano
- [ ] Clínicas (CRUD)
- [ ] Anamnese Alimentar

**Entregável**: Nutricionista cria templates reutilizáveis.

---

### Sprint 8 - Assinatura e Polimento (2 semanas)
Objetivo: Sistema de assinatura e melhorias gerais

- [ ] Minha Assinatura (Nutricionista)
- [ ] Upgrade de Plano (mockado gateway pagamento)
- [ ] Limites de plano (validações frontend)
- [ ] Notificações (básico)
- [ ] Melhorias UX gerais
- [ ] Testes E2E principais fluxos

**Entregável**: Sistema completo de assinatura.

---

### Backlog Futuro
- [ ] Chat Nutricionista-Paciente
- [ ] Scanner de código de barras
- [ ] IA para sugestões de alimentos
- [ ] Exportar relatórios PDF
- [ ] Notificações push
- [ ] Lembretes de refeições
- [ ] Integração com wearables
- [ ] Receitas (módulo novo)
- [ ] Listas de compras (geradas do plano)
- [ ] Social features (comunidade)
- [ ] Whitelabel para clínicas
- [ ] API pública

---

## 📱 Considerações Mobile

**Responsividade Obrigatória**:
- Todas as telas devem ser mobile-first
- Navegação adaptada (bottom nav em mobile)
- Modais → Full screen em mobile
- Upload de foto → integração com câmera nativa
- Date pickers → nativos mobile quando possível

**PWA**:
- Instalável
- Funciona offline (cache de dados essenciais)
- Push notifications

**Gestos**:
- Swipe em cards (ações rápidas)
- Pull to refresh (listas)
- Long press (menu contextual)

---

## 🎨 Design System Recomendado

**Cores**:
- Primária: Verde saúde (#28a745)
- Secundária: Azul (#007bff)
- Sucesso: Verde (#28a745)
- Atenção: Amarelo (#ffc107)
- Erro: Vermelho (#dc3545)
- Neutra: Cinzas (#f8f9fa, #dee2e6, #6c757d, #343a40)

**Cores de Macros**:
- Proteínas: Azul (#4a90e2)
- Carboidratos: Laranja (#f5a623)
- Gorduras: Verde (#7ed321)
- Calorias: Roxo (#9013fe)

**Tipografia**:
- Fonte: Inter ou Roboto
- Títulos: Bold
- Corpo: Regular
- Labels: Medium

**Espaçamento**:
- Grid 8px
- Margins: 8, 16, 24, 32, 48

**Ícones**:
- Biblioteca: Lucide, Heroicons ou Phosphor
- Consistência: mesmo peso/stroke

**Componentes Base**:
- Shadcn/ui (recomendado para Next.js)
- Radix UI primitives
- TailwindCSS para classes utility

---

**Fim do Documento de Telas**

Este guia detalha todas as telas necessárias para o frontend. Use em conjunto com o documento de fluxo completo para desenvolvimento.
