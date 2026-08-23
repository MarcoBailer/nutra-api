using Nutra.Enum;
using Nutra.Models.Usuario;

namespace Nutra.Interfaces;

public interface ICalculadoraNutricional
{
    /// <summary>
    /// Gera a meta nutricional inicial a partir do perfil (fluxo original de cadastro).
    /// Não vai no banco - regra pura
    /// </summary>
    MetaNutricional GerarMetaInicial(PerfilNutricional perfil);

    // ===================== FÓRMULAS TMB =====================

    /// <summary>TMB por Mifflin-St Jeor (kcal/dia).</summary>
    double CalcularTMB_MifflinStJeor(double pesoKg, double alturaCm, int idade, EGeneroBiologico genero);

    /// <summary>TMB por Harris-Benedict revisada (kcal/dia).</summary>
    double CalcularTMB_HarrisBenedict(double pesoKg, double alturaCm, int idade, EGeneroBiologico genero);

    /// <summary>TMB por Katch-McArdle (kcal/dia) — requer massa magra.</summary>
    double CalcularTMB_KatchMcArdle(double massaMagraKg);

    // ===================== GASTO ENERGÉTICO =====================

    /// <summary>Gasto Energético Total = TMB × fator de atividade.</summary>
    double CalcularGET(double tmb, ENivelAtividadeFisica nivel);

    /// <summary>GET ajustado ao objetivo (déficit/superávit).</summary>
    double AjustarCaloriasPeloObjetivo(double get, ETipoObjetivo objetivo);

    // ===================== COMPOSIÇÃO CORPORAL =====================

    /// <summary>Calcula o IMC e retorna (imc, classificação).</summary>
    (decimal imc, string classificacao) CalcularIMC(double pesoKg, double alturaCm);

    /// <summary>Relação Cintura/Quadril e classificação de risco.</summary>
    (decimal rcq, string classificacao) CalcularRCQ(double cinturaCm, double quadrilCm, EGeneroBiologico genero);

    /// <summary>Percentual de gordura por dobras cutâneas (Jackson &amp; Pollock 3 dobras).</summary>
    (decimal densidade, decimal percentualGordura) CalcularGorduraPorDobras_JP3(
        double[] dobras, int idade, EGeneroBiologico genero);

    /// <summary>Percentual de gordura por dobras cutâneas (Jackson &amp; Pollock 7 dobras).</summary>
    (decimal densidade, decimal percentualGordura) CalcularGorduraPorDobras_JP7(
        double[] dobras, int idade, EGeneroBiologico genero);

    // ===================== PESO IDEAL =====================

    /// <summary>Peso ideal por Devine (kg).</summary>
    double CalcularPesoIdeal_Devine(double alturaCm, EGeneroBiologico genero);

    /// <summary>Peso ideal por IMC ideal (22 kg/m²).</summary>
    double CalcularPesoIdeal_IMC(double alturaCm);

    // ===================== MACRONUTRIENTES =====================

    /// <summary>Distribui macronutrientes com base no objetivo e peso corporal.</summary>
    (double protG, double carbG, double gordG, double fibraG, double aguaL) CalcularMacronutrientes(
        double pesoKg, double caloriasMeta, ETipoObjetivo objetivo);
}
