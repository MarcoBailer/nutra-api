using Nutra.Enum;
using Nutra.Interfaces;
using Nutra.Models.Usuario;

namespace Nutra.Services;

/// <summary>
/// Serviço central de cálculos nutricionais e antropométricos.
/// Implementa múltiplas fórmulas validadas pela literatura científica.
/// </summary>
public class CalculadoraNutricionalService : ICalculadoraNutricional
{
    // Constantes Nutricionais (Calorias por grama)
    private const double CALORIAS_POR_GRAMA_PROTEINA = 4.0;
    private const double CALORIAS_POR_GRAMA_CARBO = 4.0;
    private const double CALORIAS_POR_GRAMA_GORDURA = 9.0;

    // =====================================================================
    //  META NUTRICIONAL (fluxo original de cadastro)
    // =====================================================================

    /// <inheritdoc />
    public MetaNutricional GerarMetaInicial(PerfilNutricional perfil)
    {
        var idade = CalcularIdade(perfil.DataNascimento);
        double tmb = CalcularTMB_MifflinStJeor(perfil.PesoAtualKg, perfil.AlturaCm, idade, perfil.Genero);
        double get = CalcularGET(tmb, perfil.NivelAtividade);
        double caloriasMeta = AjustarCaloriasPeloObjetivo(get, perfil.Objetivo);

        var (protG, carbG, gordG, fibraG, aguaL) = CalcularMacronutrientes(perfil.PesoAtualKg, caloriasMeta, perfil.Objetivo);

        return new MetaNutricional
        {
            DataCalculo = DateTime.UtcNow,
            CaloriasDiarias = Math.Round(caloriasMeta),
            ProteinasDiarias = protG,
            CarboidratosDiarios = carbG,
            GordurasDiarias = gordG,
            FibraDiaria = fibraG,
            AguaDiaria = aguaL,
            PerfilNutricionalId = perfil.Id
        };
    }

    // =====================================================================
    //  TMB — TAXA METABÓLICA BASAL
    // =====================================================================

    /// <summary>
    /// Mifflin-St Jeor (1990) — Considerada a mais precisa para adultos saudáveis.
    /// Referência: Mifflin MD, St Jeor ST, et al. Am J Clin Nutr. 1990;51(2):241-7.
    /// </summary>
    public double CalcularTMB_MifflinStJeor(double pesoKg, double alturaCm, int idade, EGeneroBiologico genero)
    {
        // Base: (10 × peso) + (6.25 × altura) - (5 × idade)
        double tmbBase = (10 * pesoKg) + (6.25 * alturaCm) - (5 * idade);

        return genero == EGeneroBiologico.Masculino
            ? Math.Round(tmbBase + 5, 1)
            : Math.Round(tmbBase - 161, 1);
    }

    /// <summary>
    /// Harris-Benedict revisada (Roza &amp; Shizgal, 1984).
    /// Referência: Roza AM, Shizgal HM. Am J Clin Nutr. 1984;40(1):168-82.
    /// </summary>
    public double CalcularTMB_HarrisBenedict(double pesoKg, double alturaCm, int idade, EGeneroBiologico genero)
    {
        if (genero == EGeneroBiologico.Masculino)
            return Math.Round(88.362 + (13.397 * pesoKg) + (4.799 * alturaCm) - (5.677 * idade), 1);
        else
            return Math.Round(447.593 + (9.247 * pesoKg) + (3.098 * alturaCm) - (4.330 * idade), 1);
    }

    /// <summary>
    /// Katch-McArdle (1996) — Usa massa magra, ideal quando se dispõe de bioimpedância.
    /// TMB = 370 + (21.6 × massa magra em kg)
    /// Referência: Katch, Frank, et al. Exercise Physiology, 1996.
    /// </summary>
    public double CalcularTMB_KatchMcArdle(double massaMagraKg)
    {
        return Math.Round(370 + (21.6 * massaMagraKg), 1);
    }

    // =====================================================================
    //  GET — GASTO ENERGÉTICO TOTAL
    // =====================================================================

    /// <summary>
    /// GET = TMB × Fator de Atividade Física (FAF).
    /// Fatores baseados na WHO/FAO/UNU (2001).
    /// </summary>
    public double CalcularGET(double tmb, ENivelAtividadeFisica nivel)
    {
        double fator = nivel switch
        {
            ENivelAtividadeFisica.Sedentario            => 1.2,   // Escritório + zero exercício
            ENivelAtividadeFisica.LevementeAtivo        => 1.375, // Exercício leve 1-3x/semana
            ENivelAtividadeFisica.ModeradamenteAtivo     => 1.55,  // Exercício moderado 3-5x/semana
            ENivelAtividadeFisica.MuitoAtivo             => 1.725, // Exercício intenso 6-7x/semana
            ENivelAtividadeFisica.ExtremamenteAtivo      => 1.9,   // Atleta / trabalho braçal pesado
            _ => 1.2
        };

        return Math.Round(tmb * fator, 1);
    }

    /// <summary>
    /// Aplica déficit ou superávit calórico de acordo com o objetivo.
    /// </summary>
    public double AjustarCaloriasPeloObjetivo(double get, ETipoObjetivo objetivo)
    {
        double fator = objetivo switch
        {
            ETipoObjetivo.PerdaDeGordura        => 0.80,  // Déficit de 20%
            ETipoObjetivo.Hipertrofia            => 1.10,  // Superávit de 10%
            ETipoObjetivo.RecomposicaoCorporal   => 0.95,  // Déficit leve de 5%
            ETipoObjetivo.SaudeMetabolica        => 1.00,  // Manutenção
            ETipoObjetivo.PerformanceEsportiva   => 1.15,  // Superávit moderado
            ETipoObjetivo.GanhoDeEnergia         => 1.05,  // Leve superávit
            _ => 1.0
        };

        return Math.Round(get * fator, 1);
    }

    // =====================================================================
    //  IMC — ÍNDICE DE MASSA CORPORAL
    // =====================================================================

    /// <summary>
    /// IMC = Peso (kg) / Altura² (m).
    /// Classificação OMS (WHO, 2000).
    /// </summary>
    public (decimal imc, string classificacao) CalcularIMC(double pesoKg, double alturaCm)
    {
        double alturaM = alturaCm / 100.0;
        decimal imc = Math.Round((decimal)(pesoKg / (alturaM * alturaM)), 2);

        string classificacao = imc switch
        {
            < 16.0m     => "Magreza grau III (grave)",
            < 17.0m     => "Magreza grau II (moderada)",
            < 18.5m     => "Magreza grau I (leve)",
            < 25.0m     => "Eutrófico (normal)",
            < 30.0m     => "Sobrepeso (pré-obeso)",
            < 35.0m     => "Obesidade grau I",
            < 40.0m     => "Obesidade grau II",
            _           => "Obesidade grau III (mórbida)"
        };

        return (imc, classificacao);
    }

    // =====================================================================
    //  RCQ — RELAÇÃO CINTURA/QUADRIL
    // =====================================================================

    /// <summary>
    /// RCQ = Cintura / Quadril.
    /// Classificação de risco cardiovascular (WHO, 2008).
    /// </summary>
    public (decimal rcq, string classificacao) CalcularRCQ(double cinturaCm, double quadrilCm, EGeneroBiologico genero)
    {
        if (quadrilCm <= 0) return (0, "Dado insuficiente");

        decimal rcq = Math.Round((decimal)(cinturaCm / quadrilCm), 2);

        string classificacao;
        if (genero == EGeneroBiologico.Masculino)
        {
            classificacao = rcq switch
            {
                <= 0.90m => "Risco baixo",
                <= 0.99m => "Risco moderado",
                _        => "Risco alto"
            };
        }
        else
        {
            classificacao = rcq switch
            {
                <= 0.80m => "Risco baixo",
                <= 0.84m => "Risco moderado",
                _        => "Risco alto"
            };
        }

        return (rcq, classificacao);
    }

    // =====================================================================
    //  GORDURA CORPORAL POR DOBRAS CUTÂNEAS
    // =====================================================================

    /// <summary>
    /// Jackson &amp; Pollock 3 dobras (1985).
    /// Homens: peitoral, abdominal, coxa.
    /// Mulheres: tríceps, suprailíaca, coxa.
    /// </summary>
    public (decimal densidade, decimal percentualGordura) CalcularGorduraPorDobras_JP3(
        double[] dobras, int idade, EGeneroBiologico genero)
    {
        if (dobras == null || dobras.Length < 3)
            throw new ArgumentException("São necessárias pelo menos 3 dobras cutâneas.");

        double soma = dobras[0] + dobras[1] + dobras[2];
        double densidade;

        if (genero == EGeneroBiologico.Masculino)
        {
            // Dc = 1.10938 – 0.0008267(S) + 0.0000016(S²) – 0.0002574(idade)
            densidade = 1.10938 - (0.0008267 * soma) + (0.0000016 * soma * soma) - (0.0002574 * idade);
        }
        else
        {
            // Dc = 1.0994921 – 0.0009929(S) + 0.0000023(S²) – 0.0001392(idade)
            densidade = 1.0994921 - (0.0009929 * soma) + (0.0000023 * soma * soma) - (0.0001392 * idade);
        }

        // Fórmula de Siri: %G = (4.95/Dc - 4.50) × 100
        double percentualGordura = ((4.95 / densidade) - 4.50) * 100;

        return (
            Math.Round((decimal)densidade, 4),
            Math.Round((decimal)Math.Max(0, percentualGordura), 2)
        );
    }

    /// <summary>
    /// Jackson &amp; Pollock 7 dobras (1978).
    /// Dobras: peitoral, axilar média, tríceps, subescapular, abdominal, suprailíaca, coxa.
    /// </summary>
    public (decimal densidade, decimal percentualGordura) CalcularGorduraPorDobras_JP7(
        double[] dobras, int idade, EGeneroBiologico genero)
    {
        if (dobras == null || dobras.Length < 7)
            throw new ArgumentException("São necessárias 7 dobras cutâneas para o protocolo JP7.");

        double soma = 0;
        foreach (var d in dobras) soma += d;

        double densidade;

        if (genero == EGeneroBiologico.Masculino)
        {
            // Dc = 1.112 – 0.00043499(S) + 0.00000055(S²) – 0.00028826(idade)
            densidade = 1.112 - (0.00043499 * soma) + (0.00000055 * soma * soma) - (0.00028826 * idade);
        }
        else
        {
            // Dc = 1.097 – 0.00046971(S) + 0.00000056(S²) – 0.00012828(idade)
            densidade = 1.097 - (0.00046971 * soma) + (0.00000056 * soma * soma) - (0.00012828 * idade);
        }

        double percentualGordura = ((4.95 / densidade) - 4.50) * 100;

        return (
            Math.Round((decimal)densidade, 4),
            Math.Round((decimal)Math.Max(0, percentualGordura), 2)
        );
    }

    // =====================================================================
    //  PESO IDEAL
    // =====================================================================

    /// <summary>
    /// Devine (1974) — Formula mais utilizada na prática clínica.
    /// Homens: 50 + 2.3 × (altura_polegadas - 60)
    /// Mulheres: 45.5 + 2.3 × (altura_polegadas - 60)
    /// </summary>
    public double CalcularPesoIdeal_Devine(double alturaCm, EGeneroBiologico genero)
    {
        double alturaPolegadas = alturaCm / 2.54;
        double pesoIdeal;

        if (genero == EGeneroBiologico.Masculino)
            pesoIdeal = 50.0 + 2.3 * (alturaPolegadas - 60);
        else
            pesoIdeal = 45.5 + 2.3 * (alturaPolegadas - 60);

        return Math.Round(Math.Max(pesoIdeal, 30), 1); // piso de segurança
    }

    /// <summary>
    /// Peso ideal baseado no IMC ideal (22 kg/m²).
    /// PI = 22 × altura(m)²
    /// </summary>
    public double CalcularPesoIdeal_IMC(double alturaCm)
    {
        double alturaM = alturaCm / 100.0;
        return Math.Round(22.0 * alturaM * alturaM, 1);
    }

    // =====================================================================
    //  MACRONUTRIENTES
    // =====================================================================

    /// <summary>
    /// Calcula a distribuição de macronutrientes com base no objetivo.
    /// Proteína: definida por g/kg (varia por objetivo).
    /// Gordura: 0.8-1.0 g/kg (regulação hormonal).
    /// Carboidrato: restante das calorias (combustível).
    /// </summary>
    public (double protG, double carbG, double gordG, double fibraG, double aguaL) CalcularMacronutrientes(
        double pesoKg, double caloriasMeta, ETipoObjetivo objetivo)
    {
        // Proteína (g/kg)
        double gramasProteinaPorKg = objetivo switch
        {
            ETipoObjetivo.Hipertrofia            => 2.0,
            ETipoObjetivo.PerdaDeGordura         => 2.2,  // Mais alta para proteger músculo no déficit
            ETipoObjetivo.RecomposicaoCorporal    => 2.4,  // Altíssima — cenário mais difícil
            ETipoObjetivo.PerformanceEsportiva    => 2.0,
            _ => 1.8
        };

        double protG = Math.Round(pesoKg * gramasProteinaPorKg);

        // Gordura (g/kg) — 0.9g/kg é média segura para hormônios
        double gordG = Math.Round(pesoKg * 0.9);

        // Carboidrato — restante das calorias
        double calsProtGord = (protG * CALORIAS_POR_GRAMA_PROTEINA) + (gordG * CALORIAS_POR_GRAMA_GORDURA);
        double carbG = Math.Max(0, Math.Round((caloriasMeta - calsProtGord) / CALORIAS_POR_GRAMA_CARBO));

        // Fibra: 14g/1000kcal (IOM/DRI)
        double fibraG = Math.Round((caloriasMeta / 1000) * 14);

        // Água: 35ml/kg → litros
        double aguaL = Math.Round(pesoKg * 0.035, 1);

        return (protG, carbG, gordG, fibraG, aguaL);
    }

    // =====================================================================
    //  UTILITÁRIOS
    // =====================================================================

    public static int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - dataNascimento.Year;
        if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;
        return idade;
    }
}
