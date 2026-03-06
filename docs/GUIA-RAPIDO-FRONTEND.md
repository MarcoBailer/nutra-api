# Guia Rápido para Desenvolvedor Frontend - Nutra Food

## 🎯 Resumo Executivo

Este é um **sistema de gerenciamento nutricional** com dois tipos de usuários:

### **Pacientes** 
Pessoas que querem acompanhamento nutricional. Precisam:
1. ✅ **OBRIGATÓRIO**: Criar PerfilNutricional após registro
2. ✅ **AUTOMÁTICO**: Sistema gera MetaNutricional
3. ⚙️ **OPCIONAL**: Criar planos, fazer avaliações, registrar diário

### **Nutricionistas**
Profissionais que gerenciam pacientes. Precisam:
1. ✅ **OBRIGATÓRIO**: Criar PerfilProfissional após registro
2. ✅ **AUTOMÁTICO**: Sistema cria Assinatura (limites de pacientes)
3. ⚙️ **OPCIONAL**: Criar clínicas, convidar pacientes, criar planos

---

## 🚨 REGRAS CRÍTICAS (NÃO PULE!)

### 1. PerfilNutricional é OBRIGATÓRIO para Paciente
```tsx
// Após login do paciente
const checkProfile = async () => {
  try {
    await api.get('/api/User/perfil-nutricional');
    // Tem perfil → Dashboard
  } catch (404) {
    // NÃO TEM → Redirecionar FORÇADO para /onboarding/perfil
    router.push('/onboarding/perfil');
  }
};
```

### 2. PerfilProfissional é OBRIGATÓRIO para Nutricionista
```tsx
// Após login do nutricionista
const checkProfessionalProfile = async () => {
  try {
    await api.get('/api/Nutricionista/perfil');
    // Tem perfil → Dashboard Nutricionista
  } catch (404) {
    // NÃO TEM → Redirecionar FORÇADO para /onboarding/nutricionista
    router.push('/onboarding/nutricionista');
  }
};
```

### 3. MetaNutricional é SEMPRE GERADA AUTOMATICAMENTE
- ❌ **NUNCA** criar endpoint de criar/editar meta manualmente
- ✅ **SEMPRE** é gerada ao criar/atualizar PerfilNutricional
- ✅ **SEMPRE** recalculada quando perfil muda

### 4. Apenas 1 PlanoAlimentar pode estar Ativo
- Ao ativar novo plano → Sistema desativa o anterior automaticamente
- Backend faz isso, mas frontend deve mostrar claramente qual é o ativo

### 5. Limites de Assinatura (Nutricionista)
```tsx
// Antes de convidar paciente
const canInvitePatient = async () => {
  const profile = await api.get('/api/Nutricionista/perfil');
  if (profile.totalPacientesAtivos >= profile.maxPacientes) {
    // Bloquear e mostrar CTA upgrade
    showUpgradeModal();
    return false;
  }
  return true;
};
```

---

## 📋 Checklist MVP (Ordem de Implementação)

### Fase 1: Autenticação e Base (Sprint 1)
- [ ] **Tela de Login** (`/login`)
  - Form email/senha
  - POST `/api/Auth/login`
  - Salvar token
  - Redirect baseado em role
  
- [ ] **Tela de Registro** (`/register`)
  - Radio Paciente/Nutricionista
  - Form completo
  - POST `/api/Auth/register`
  - Auto-login
  
- [ ] **Auth Guard/Middleware**
  - Verificar token
  - Redirect não autenticado → `/login`
  - Verificar perfil completo

### Fase 2: Onboarding Paciente (Sprint 1)
- [ ] **Wizard Perfil Nutricional** (`/onboarding/perfil`)
  - [ ] Step 1: Dados Pessoais (data, gênero)
  - [ ] Step 2: Medidas (altura, peso, circunferências)
  - [ ] Step 3: Estilo Vida (atividade, ocupação, sono)
  - [ ] Step 4: Saúde (doenças, histórico clínico)
  - [ ] Step 5: Objetivos (perda/ganho peso)
  - [ ] Step 6: Alimentação (preferência dieta, refeições/dia)
  - [ ] Step 7: Restrições (alergias)
  - [ ] Step 8: Equipamentos (fogão, micro-ondas, etc)
  - [ ] Step 9: Preferências (opcional, pode pular)
  - [ ] Submit → POST `/api/User/perfil-nutricional`
  - [ ] Loading "Calculando metas..."
  - [ ] Redirect `/dashboard`

### Fase 3: Onboarding Nutricionista (Sprint 1)
- [ ] **Cadastro Profissional** (`/onboarding/nutricionista`)
  - Form: CRN, região, especialidade, bio
  - POST `/api/Nutricionista/cadastro`
  - Redirect `/dashboard-nutricionista`

### Fase 4: Dashboard Básico (Sprint 1)
- [ ] **Dashboard Paciente** (`/dashboard`)
  - Card Resumo do Dia (calorias consumidas vs meta)
  - Progress bars macros
  - Card Metas Nutricionais
  - Botão "Registrar Consumo"
  - Link "Ver Meta Nutricional"
  
- [ ] **Dashboard Nutricionista** (`/dashboard-nutricionista`)
  - Card Assinatura (pacientes ativos/limite)
  - Lista básica de pacientes
  - Botão "Convidar Paciente"

### Fase 5: Visualizar Metas (Sprint 1)
- [ ] **Tela Metas Nutricionais** (`/metas`)
  - GET `/api/User/meta-nutricional`
  - Cards grandes: calorias, proteínas, carbos, gorduras, água, fibras
  - Explicação de como foram calculadas
  - Link "Ajustar Perfil"

### Fase 6: Busca de Alimentos (componente) (Sprint 1)
- [ ] **Componente SearchAlimentos**
  - Input busca
  - GET `/api/Busca/alimentos?termo=`
  - Grid resultados
  - Filtros (tabela, categoria)
  - Callback onSelect
  - **Reutilizável em múltiplas telas**

### Fase 7: Registrar Consumo (Sprint 1)
- [ ] **Modal/Tela Registrar Consumo** (`/diario/registrar`)
  - Usa SearchAlimentos
  - Seleciona alimento
  - Input quantidade
  - Preview macros calculados
  - Select tipo refeição
  - DateTime picker (default: agora)
  - Optional: foto
  - POST `/api/Diario/registro`
  - Toast sucesso
  - Atualiza dashboard

### Fase 8: Diário do Dia (Sprint 1)
- [ ] **Tela Diário** (`/diario`)
  - Date picker (default: hoje)
  - GET `/api/Diario/dia?data=`
  - Card Metas vs Consumido (progress bars)
  - Seção por refeição:
    - Lista registros
    - Totais refeição
    - Botão "Adicionar consumo"
  - Totais do dia
  - Botão FAB "Registrar Consumo"

---

## 🚀 MVP Mínimo Funcional (2-3 semanas)

Com as fases acima, você terá:

✅ **Paciente pode:**
- Registrar conta
- Criar perfil nutricional completo
- Ver suas metas calculadas automaticamente
- Registrar o que comeu
- Ver resumo do dia comparando com meta

✅ **Nutricionista pode:**
- Registrar conta profissional
- Criar perfil profissional
- Ver dashboard básico

**Este é um MVP testável e funcional!**

---

## 🔄 Fluxo de Desenvolvimento Recomendado

### Semana 1
- Setup projeto Next.js
- Configurar autenticação (JWT)
- Telas Login/Registro
- Guards/Middleware

### Semana 2
- Onboarding Paciente (wizard completo)
- Onboarding Nutricionista
- Dashboard básico de ambos

### Semana 3
- Componente de busca alimentos
- Registrar consumo
- Diário do dia
- Visualizar metas

**Ao fim da semana 3: DEPLOY MVP!**

---

## 📦 Stack Recomendada

```json
{
  "framework": "Next.js 14+ (App Router)",
  "linguagem": "TypeScript",
  "styling": "TailwindCSS",
  "componentes": "shadcn/ui ou Radix UI",
  "forms": "React Hook Form + Zod",
  "state": "Zustand ou Context API",
  "http": "Axios ou fetch",
  "charts": "Recharts ou Chart.js",
  "dates": "date-fns",
  "imagens": "React Dropzone + Next Image"
}
```

### Instalação Rápida
```bash
npx create-next-app@latest nutra-frontend --typescript --tailwind --app
cd nutra-frontend
npm install axios react-hook-form zod @hookform/resolvers
npm install date-fns recharts
npx shadcn-ui@latest init
```

---

## 🗂️ Estrutura de Pastas Sugerida

```
src/
├── app/                          # App Router (Next.js 14+)
│   ├── (auth)/                   # Grupo autenticação
│   │   ├── login/
│   │   └── register/
│   ├── (onboarding)/             # Grupo onboarding
│   │   ├── perfil/
│   │   └── nutricionista/
│   ├── dashboard/                # Dashboard paciente
│   ├── dashboard-nutricionista/  # Dashboard nutricionista
│   ├── diario/
│   ├── metas/
│   ├── planos/
│   ├── avaliacoes/
│   ├── peso/
│   ├── preferencias/
│   ├── perfil/
│   ├── vinculos/
│   ├── pacientes/                # Nutricionista
│   ├── clinicas/                 # Nutricionista
│   ├── templates/                # Nutricionista
│   └── assinatura/               # Nutricionista
│
├── components/
│   ├── ui/                       # shadcn/ui components
│   ├── layout/
│   │   ├── Header.tsx
│   │   ├── Sidebar.tsx
│   │   └── Footer.tsx
│   ├── auth/
│   │   ├── LoginForm.tsx
│   │   └── RegisterForm.tsx
│   ├── onboarding/
│   │   ├── WizardStepper.tsx
│   │   └── PerfilSteps/
│   ├── shared/
│   │   ├── SearchAlimentos.tsx   # ⭐ Componente chave
│   │   ├── MacroDisplay.tsx
│   │   ├── ProgressCircular.tsx
│   │   ├── DateRangePicker.tsx
│   │   └── ImageUploader.tsx
│   ├── dashboard/
│   ├── diario/
│   ├── planos/
│   └── avaliacoes/
│
├── lib/
│   ├── api.ts                    # Axios instance
│   ├── auth.ts                   # Auth helpers
│   ├── validations.ts            # Zod schemas
│   └── utils.ts                  # Helpers
│
├── types/
│   ├── user.ts
│   ├── perfil.ts
│   ├── meta.ts
│   ├── plano.ts
│   ├── diario.ts
│   └── avaliacao.ts
│
├── hooks/
│   ├── useAuth.ts
│   ├── usePerfil.ts
│   ├── useMeta.ts
│   └── useDiario.ts
│
└── store/                        # Zustand stores (opcional)
    ├── authStore.ts
    └── perfilStore.ts
```

---

## 🔗 Endpoints Mais Usados (Referência Rápida)

### Auth
```ts
POST /api/Auth/register          // Registro
POST /api/Auth/login             // Login
```

### Paciente - Perfil
```ts
POST /api/User/perfil-nutricional           // Criar perfil (onboarding)
GET /api/User/perfil-nutricional            // Ver perfil
PUT /api/User/perfil-nutricional            // Atualizar perfil
GET /api/User/meta-nutricional              // Ver meta atual
POST /api/User/preferencia-alimentar        // Adicionar preferência
POST /api/User/registro-biometrico          // Registrar peso
GET /api/User/vinculos                      // Ver vínculos
POST /api/User/vinculos/{id}/aceitar        // Aceitar convite
```

### Nutricionista
```ts
POST /api/Nutricionista/cadastro            // Criar perfil profissional
GET /api/Nutricionista/perfil               // Ver perfil
POST /api/Nutricionista/clinicas            // Criar clínica
POST /api/Nutricionista/pacientes/convidar  // Convidar paciente
GET /api/Nutricionista/pacientes            // Listar pacientes
GET /api/Nutricionista/pacientes/{id}       // Ver paciente
```

### Busca
```ts
GET /api/Busca/alimentos?termo={busca}              // Buscar alimentos
GET /api/Busca/alimento/{id}?tabela={ETipoTabela}  // Detalhes alimento
```

### Diário
```ts
POST /api/Diario/registro                   // Registrar consumo
GET /api/Diario/dia?data={yyyy-MM-dd}       // Diário do dia
GET /api/Diario/relatorio?dataInicio=&dataFim=  // Relatório aderência
POST /api/Diario/fotos                      // Adicionar foto refeição
```

### Planos
```ts
POST /api/PlanoAlimentar/criar              // Criar plano
GET /api/PlanoAlimentar                     // Listar planos
GET /api/PlanoAlimentar/{id}                // Ver plano
GET /api/PlanoAlimentar/ativo               // Ver plano ativo
POST /api/PlanoAlimentar/{id}/ativar        // Ativar plano
POST /api/PlanoAlimentar/{id}/refeicoes     // Adicionar refeição
GET /api/PlanoAlimentar/modelos             // Listar templates
```

### Avaliações
```ts
POST /api/Avaliacao/registrar               // Nova avaliação
GET /api/Avaliacao                          // Listar avaliações
GET /api/Avaliacao/{id}                     // Detalhes avaliação
GET /api/Avaliacao/comparar?anterior=&atual=  // Comparar 2
POST /api/Avaliacao/{id}/fotos              // Adicionar fotos
POST /api/Avaliacao/anamnese                // Anamnese alimentar
```

---

## 💡 Dicas de Implementação

### 1. Autenticação
```tsx
// lib/api.ts
import axios from 'axios';

const api = axios.create({
  baseURL: process.env.NEXT_PUBLIC_API_URL,
});

// Interceptor para adicionar token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Interceptor para tratar 401
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;
```

### 2. Type Safety (TypeScript)
```tsx
// types/perfil.ts
export interface PerfilNutricionalDto {
  userId: string;
  dataNascimento: string; // ISO date
  genero: 'Masculino' | 'Feminino';
  alturaCm: number;
  pesoAtualKg: number;
  fatorAtividade: number;
  nivelAtividade: 'Sedentario' | 'Leve' | 'Moderado' | 'Intenso' | 'MuitoIntenso';
  ocupacaoProfissional: string;
  habilidadeCulinaria: 'Basico' | 'Intermediario' | 'Avancado' | 'Profissional';
  orcamentoMensal: 'Baixo' | 'Medio' | 'Alto' | 'MuitoAlto';
  // ... demais campos
  restricoesIds: EAlergico[];
  equipamentosIds: EEquipamentoDisponivel[];
  preferencias: PreferenciaCadastroDto[];
  historicoClinicos: HistoricoClinicoDto[];
}

// types/meta.ts
export interface MetaNutricional {
  id: number;
  dataCalculo: string;
  caloriasDiarias: number;
  proteinasDiarias: number;
  carboidratosDiarios: number;
  gordurasDiarias: number;
  aguaDiaria: number;
  fibraDiaria: number;
}
```

### 3. Hook Customizado de Perfil
```tsx
// hooks/usePerfil.ts
import { useState, useEffect } from 'react';
import api from '@/lib/api';
import { PerfilNutricionalDto } from '@/types/perfil';

export function usePerfil() {
  const [perfil, setPerfil] = useState<PerfilNutricionalDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchPerfil();
  }, []);

  const fetchPerfil = async () => {
    try {
      const { data } = await api.get('/api/User/perfil-nutricional');
      setPerfil(data);
    } catch (err: any) {
      if (err.response?.status === 404) {
        // Não tem perfil
        setPerfil(null);
      } else {
        setError(err.message);
      }
    } finally {
      setLoading(false);
    }
  };

  const updatePerfil = async (data: Partial<PerfilNutricionalDto>) => {
    await api.put('/api/User/perfil-nutricional', data);
    await fetchPerfil(); // Recarrega
  };

  return { perfil, loading, error, updatePerfil, refetch: fetchPerfil };
}
```

### 4. Middleware para Verificar Perfil
```tsx
// middleware.ts (Next.js 14+)
import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export async function middleware(request: NextRequest) {
  const token = request.cookies.get('token')?.value;
  
  // Se não tem token e não está em rota pública
  if (!token && !isPublicRoute(request.nextUrl.pathname)) {
    return NextResponse.redirect(new URL('/login', request.url));
  }
  
  // Se tem token, verificar se tem perfil completo
  if (token && needsProfileCheck(request.nextUrl.pathname)) {
    try {
      const response = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/User/perfil-nutricional`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      
      if (response.status === 404) {
        // Não tem perfil → forçar onboarding
        return NextResponse.redirect(new URL('/onboarding/perfil', request.url));
      }
    } catch (error) {
      console.error('Error checking profile:', error);
    }
  }
  
  return NextResponse.next();
}

function isPublicRoute(pathname: string) {
  return ['/login', '/register', '/forgot-password'].includes(pathname);
}

function needsProfileCheck(pathname: string) {
  return !['/onboarding', '/login', '/register'].some(route => pathname.startsWith(route));
}

export const config = {
  matcher: ['/((?!api|_next/static|_next/image|favicon.ico).*)'],
};
```

### 5. Componente SearchAlimentos Exemplo
```tsx
// components/shared/SearchAlimentos.tsx
'use client';

import { useState, useEffect } from 'react';
import { Search } from 'lucide-react';
import api from '@/lib/api';
import { Input } from '@/components/ui/input';
import { debounce } from '@/lib/utils';

interface Alimento {
  id: number;
  nome: string;
  tabela: string;
  categoria: string;
  energiaKcal: number;
  proteina: number;
  carboidrato: number;
  gordura: number;
}

interface SearchAlimentosProps {
  onSelect: (alimento: Alimento) => void;
}

export function SearchAlimentos({ onSelect }: SearchAlimentosProps) {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<Alimento[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (query.length < 3) {
      setResults([]);
      return;
    }

    const searchDebounced = debounce(async () => {
      setLoading(true);
      try {
        const { data } = await api.get(`/api/Busca/alimentos`, {
          params: { termo: query }
        });
        setResults(data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    }, 500);

    searchDebounced();
  }, [query]);

  return (
    <div className="space-y-4">
      <div className="relative">
        <Search className="absolute left-3 top-3 h-4 w-4 text-gray-400" />
        <Input
          type="text"
          placeholder="Buscar alimento..."
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          className="pl-10"
        />
      </div>

      {loading && <p className="text-sm text-gray-500">Buscando...</p>}

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {results.map((alimento) => (
          <button
            key={`${alimento.tabela}-${alimento.id}`}
            onClick={() => onSelect(alimento)}
            className="p-4 border rounded-lg hover:bg-gray-50 text-left"
          >
            <h3 className="font-medium">{alimento.nome}</h3>
            <p className="text-sm text-gray-500">{alimento.categoria}</p>
            <div className="mt-2 flex gap-4 text-xs text-gray-600">
              <span>{alimento.energiaKcal} kcal</span>
              <span>{alimento.proteina}g prot</span>
              <span>{alimento.carboidrato}g carb</span>
              <span>{alimento.gordura}g gord</span>
            </div>
          </button>
        ))}
      </div>
    </div>
  );
}
```

### 6. Wizard Multi-Step (Onboarding)
```tsx
// components/onboarding/WizardStepper.tsx
'use client';

import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Progress } from '@/components/ui/progress';

interface Step {
  title: string;
  component: React.ReactNode;
}

interface WizardStepperProps {
  steps: Step[];
  onComplete: (data: any) => void;
}

export function WizardStepper({ steps, onComplete }: WizardStepperProps) {
  const [currentStep, setCurrentStep] = useState(0);
  const [formData, setFormData] = useState<any>({});

  const progress = ((currentStep + 1) / steps.length) * 100;

  const handleNext = (stepData: any) => {
    const newData = { ...formData, ...stepData };
    setFormData(newData);

    if (currentStep === steps.length - 1) {
      onComplete(newData);
    } else {
      setCurrentStep(currentStep + 1);
    }
  };

  const handleBack = () => {
    if (currentStep > 0) {
      setCurrentStep(currentStep - 1);
    }
  };

  return (
    <div className="max-w-2xl mx-auto p-6">
      <div className="mb-8">
        <Progress value={progress} className="h-2" />
        <p className="text-sm text-gray-500 mt-2">
          Passo {currentStep + 1} de {steps.length}
        </p>
      </div>

      <div className="mb-8">
        <h2 className="text-2xl font-bold mb-4">
          {steps[currentStep].title}
        </h2>
        {steps[currentStep].component}
      </div>

      <div className="flex justify-between">
        <Button
          variant="outline"
          onClick={handleBack}
          disabled={currentStep === 0}
        >
          Voltar
        </Button>
        {/* O botão Next está no componente filho */}
      </div>
    </div>
  );
}
```

---

## 🎨 Componentes UI Essenciais

Instale shadcn/ui components básicos:

```bash
npx shadcn-ui@latest add button
npx shadcn-ui@latest add input
npx shadcn-ui@latest add card
npx shadcn-ui@latest add select
npx shadcn-ui@latest add dialog
npx shadcn-ui@latest add toast
npx shadcn-ui@latest add progress
npx shadcn-ui@latest add tabs
npx shadcn-ui@latest add accordion
npx shadcn-ui@latest add checkbox
npx shadcn-ui@latest add radio-group
npx shadcn-ui@latest add calendar
npx shadcn-ui@latest add dropdown-menu
```

---

## ✅ Checklist de Qualidade

Antes de considerar uma funcionalidade "pronta":

- [ ] **TypeScript**: Tipos definidos para todas as entidades
- [ ] **Loading States**: Skeletons ou spinners
- [ ] **Error Handling**: Mensagens claras de erro
- [ ] **Validação**: Validação frontend com Zod
- [ ] **Responsivo**: Testado mobile/tablet/desktop
- [ ] **Acessibilidade**: Labels, aria-labels, keyboard navigation
- [ ] **Toast/Feedback**: Confirmações de ações
- [ ] **Confirmação**: Modais para ações destrutivas (excluir, encerrar)

---

## 📚 Documentos de Referência

1. **FLUXO-COMPLETO-APLICACAO.md**: Explicação detalhada de toda lógica de negócio
2. **FRONTEND-TELAS-DETALHADAS.md**: Mockups e detalhes de cada tela
3. **FLUXOS-VISUAIS-DIAGRAMAS.md**: Diagramas Mermaid dos fluxos

---

## 🐛 Troubleshooting Comum

### "Perfil não encontrado após criar"
- Verifique se o endpoint POST retornou sucesso
- Aguarde 1-2s antes de fazer GET (race condition)
- Use `await` corretamente

### "Meta não aparece"
- Meta é criada AUTOMATICAMENTE ao criar perfil
- Sempre é gerada, não precisa criar endpoint separado
- Se não aparece, perfil pode não ter sido criado corretamente

### "Plano ativo não funciona"
- Apenas 1 pode estar ativo
- Backend gerencia isso, mas verifique se Status === "Ativo"
- Ao ativar novo, GET lista novamente para atualizar

### "Limite de pacientes não funciona"
- Nutricionista tem `maxPacientes` no perfil
- Compare `totalPacientesAtivos >= maxPacientes`
- Bloquear UI se limite atingido

### "Cálculos de macros errados"
- Backend calcula proporcionalmente baseado em 100g
- Frontend só mostra, não calcula
- Se parecer errado, API pode ter dado alimento errado

---

## 🎯 Próximos Passos Após MVP

Quando MVP estiver funcionando:

1. **Sprint 2**: Edição de perfis, histórico peso, preferências
2. **Sprint 3**: Planos alimentares completos
3. **Sprint 4**: Diário avançado com relatórios
4. **Sprint 5**: Vínculos nutricionista-paciente
5. **Sprint 6**: Avaliações antropométricas
6. **Sprint 7**: Templates e recursos avançados
7. **Sprint 8**: Assinatura e polimento

---

## 📞 Resumo Final: 3 Coisas Mais Importantes

1. **PerfilNutricional é OBRIGATÓRIO para paciente** → Redirecionar forçado se não tiver
2. **MetaNutricional é AUTO-GERADA** → Não criar manualmente
3. **Apenas 1 PlanoAlimentar pode estar Ativo** → Validar no frontend

**Comece pelo MVP (checklist acima) e vá iterando!**

Boa sorte! 🚀
