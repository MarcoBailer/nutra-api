using Nutra.Helper;

namespace Nutra.Test.Core.Helper;

/// <summary>
/// Testes de <see cref="DateTimeHelper"/>.
/// <para>
/// O PostgreSQL com <c>timestamp with time zone</c> recusa <see cref="DateTime"/>
/// que não seja UTC. Este helper é a barreira que impede esse erro de chegar ao
/// banco, então cada um dos três <see cref="DateTimeKind"/> precisa de teste.
/// </para>
/// </summary>
public class DateTimeHelperTests
{
    [Fact]
    public void EnsureUtcDateTime_KindUnspecified_ApenasMarcaComoUtcSemMudarOHorario()
    {
        // Data vinda de JSON sem fuso: o horário é assumido como já sendo UTC.
        var entrada = new DateTime(2026, 3, 15, 10, 30, 0, DateTimeKind.Unspecified);

        var resultado = DateTimeHelper.EnsureUtcDateTime(entrada);

        Assert.Equal(DateTimeKind.Utc, resultado.Kind);
        Assert.Equal(entrada.Ticks, resultado.Ticks);
    }

    [Fact]
    public void EnsureUtcDateTime_KindUtc_DevolveOMesmoValor()
    {
        var entrada = new DateTime(2026, 3, 15, 10, 30, 0, DateTimeKind.Utc);

        var resultado = DateTimeHelper.EnsureUtcDateTime(entrada);

        Assert.Equal(entrada, resultado);
        Assert.Equal(DateTimeKind.Utc, resultado.Kind);
    }

    [Fact]
    public void EnsureUtcDateTime_KindLocal_ConverteOHorarioParaUtc()
    {
        var entrada = new DateTime(2026, 3, 15, 10, 30, 0, DateTimeKind.Local);

        var resultado = DateTimeHelper.EnsureUtcDateTime(entrada);

        Assert.Equal(DateTimeKind.Utc, resultado.Kind);
        Assert.Equal(entrada.ToUniversalTime(), resultado);
    }

    [Fact]
    public void EnsureUtcDateTime_AplicadoDuasVezes_NaoMudaOResultado()
    {
        // Idempotência: chamar de novo sobre a saída não pode deslocar o horário.
        var entrada = new DateTime(2026, 3, 15, 10, 30, 0, DateTimeKind.Local);

        var umaVez = DateTimeHelper.EnsureUtcDateTime(entrada);
        var duasVezes = DateTimeHelper.EnsureUtcDateTime(umaVez);

        Assert.Equal(umaVez, duasVezes);
    }
}
