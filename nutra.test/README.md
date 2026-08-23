# nutra.test

Testes unitários da solução Nutra, em **xUnit v3**.

## Rodar

```bash
# da raiz do repositório nutra/
dotnet test nutra.test/nutra.test.csproj

# um arquivo só
dotnet test nutra.test/nutra.test.csproj --filter "FullyQualifiedName~CalculadoraNutricionalServiceTests"

# um teste só
dotnet test nutra.test/nutra.test.csproj --filter "FullyQualifiedName~CalcularIMC_MedidasValidas_ArredondaEmDuasCasas"
```

## Estrutura

Espelha a estrutura do código de produção. Um arquivo de teste por classe testada.

```
Core/Helper/          -> nutra.core/Helper/
Core/Models/          -> nutra.core/Models/
Core/Services/        -> nutra.core/Services/
Data/Helper/          -> nutra.data/Helper/
Fakes/                -> dublês de interface usados pelos testes
```

## Como escrever um teste

```csharp
public class MinhaClasseTests
{
    // O construtor roda antes de CADA teste (equivalente ao [SetUp] do NUnit).
    // xUnit cria uma instância nova da classe por teste, então não há estado vazando.

    [Fact]   // um caso fixo
    public void Metodo_Cenario_ResultadoEsperado()
    {
        var resultado = new MinhaClasse().Metodo(entrada);

        Assert.Equal(esperado, resultado);
    }

    [Theory] // o mesmo teste com várias entradas
    [InlineData(1, 2)]
    [InlineData(3, 6)]
    public void Metodo_Entrada_Dobra(int entrada, int esperado)
    {
        Assert.Equal(esperado, new MinhaClasse().Dobrar(entrada));
    }
}
```

Convenção de nome: `Metodo_Cenario_ResultadoEsperado`. O nome tem que explicar a
falha sem precisar abrir o arquivo.

Asserções usadas neste projeto: `Assert.Equal`, `Assert.True/False`, `Assert.Null/NotNull`,
`Assert.Throws<T>`. Para `double`, sempre use a sobrecarga com precisão —
`Assert.Equal(esperado, atual, 4)` — porque comparar ponto flutuante por igualdade
exata quebra por ruído de arredondamento.

## Cobertura atual

| Classe testada | Onde | Por que é testável |
|---|---|---|
| `CalculadoraNutricionalService` | `nutra.core/Services` | Função pura, zero dependências |
| `AccountsService` | `nutra.core/Services` | Depende só de `IApplicationUserService` (interface) |
| `RetornoPadrao` / `RetornoPadrao<T>` | `nutra.core/Models` | Fábricas puras |
| `DateTimeHelper` | `nutra.core/Helper` | Função pura |
| `Conversor` | `nutra.data/Helper` | Função pura estática |
| `PorcaoParser` | `nutra.data/Helper` | Função pura estática |

## Fora de escopo (ainda)

Estes serviços recebem `AlimentosContext` (o `DbContext` concreto) direto no
construtor: `ApplicationUserService`, `AvaliacaoNutricionalService`, `BuscaService`,
`DiarioAlimentarService`, `NutricionistaService`, `PlanoAlimentarService`,
`RefeicaoService`, `UserProfileService`.

Não dá para testá-los unitariamente sem subir um banco. Testá-los exige **testes de
integração** com SQLite in-memory ou Postgres em container — outro tipo de teste,
outra pasta, outra decisão de arquitetura. Não foi feito aqui.

O mesmo vale para os controllers de `nutra.api`, que só repassam chamadas aos
services e não contêm lógica própria.

## Testes marcados `_BugConhecido`

Documentam comportamento **errado** que existe hoje no código de produção, para que
ninguém o "descubra" de novo em produção. Cada um traz no XML doc o arquivo, a linha
e a correção. Quando o bug for corrigido, o teste falha — esse é o sinal para
atualizá-lo ou removê-lo.
