namespace Nutra.Models.Alimentos;

public class Fabricantes
{
    public int Id { get; set; }
    public string? Fabricante { get; set; }
    public string? Produto { get; set; }
    /// <summary>
    /// Texto bruto extraido da base de origem, por exemplo: "1 scoop (15 g)".
    /// </summary>
    public string? PorcaoTexto { get; set; }
    /// <summary>
    /// Unidade de medida da porção, "g", "ml", etc.
    /// </summary>
    public string? Unidade { get; set; } 
    /// <summary>
    /// Como é servido o alimento, "1 colher de sopa", "1 copo", "1 scoop", "1 cápsula", 1 xícara, etc.
    /// </summary>
    public string? Dose { get; set; } 
    /// <summary>
    /// Quantidade numerica da porção em relacao a unidade. Exemplo: se Dose = "1 colher de sopa", Unidade = "g" e Porcao = 15, entao 1 colher de sopa equivale a 15g.
    /// </summary>
    public double? Porcao { get; set; }
    public double? EnergiaKcal { get; set; }
    public double? EnergiaKj { get; set; }
    public double? Proteinas { get; set; }
    public double? Carboidratos { get; set; }
    public double? Acucar { get; set; }
    public double? Gorduras { get; set; }
    public double? GorduraSaturada { get; set; }
    public double? GorduraPoliinsaturada { get; set; }
    public double? GorduraMonoinsaturada { get; set; }
    public double? GorduraTrans { get; set; }
    public double? Colesterol { get; set; }
    public double? Fibras { get; set; }
    public double? Sodio { get; set; }
    public double? Potassio { get; set; }
}
