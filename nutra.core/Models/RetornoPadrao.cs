namespace Nutra.Models;

/// <summary>
/// Envelope padrão de resposta da API.
/// <para>
/// O campo <see cref="StatusCode"/> existe porque nutra.core não conhece
/// <c>IActionResult</c>: é o canal pelo qual o service informa ao controller
/// qual status HTTP emitir. O controller faz <c>StatusCode(r.StatusCode, r)</c>.
/// </para>
/// <para>
/// Falha de negócio (não encontrado, conflito, sem permissão) é retorno, não exceção.
/// Exceção significa bug ou infra caída, e é traduzida em 500 pelo
/// <c>ExceptionMiddleware</c>.
/// </para>
/// </summary>
public class RetornoPadrao
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public int StatusCode { get; set; } = 200;

    public static RetornoPadrao Ok(string mensagem) =>
        new() { Sucesso = true, Mensagem = mensagem, StatusCode = 200 };

    public static RetornoPadrao Criado(string mensagem) =>
        new() { Sucesso = true, Mensagem = mensagem, StatusCode = 201 };

    /// <summary>400 — dados da requisição inválidos ou regra de negócio violada.</summary>
    public static RetornoPadrao Invalido(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 400 };

    /// <summary>403 — autenticado, mas sem permissão sobre o recurso.</summary>
    public static RetornoPadrao Proibido(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 403 };

    /// <summary>404 — recurso inexistente. O client decide se redireciona (ex.: onboarding).</summary>
    public static RetornoPadrao NaoEncontrado(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 404 };

    /// <summary>409 — o recurso já existe ou o estado atual impede a operação.</summary>
    public static RetornoPadrao Conflito(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 409 };
}

/// <summary>
/// Envelope padrão com payload. <see cref="Dados"/> é preenchido apenas em sucesso.
/// </summary>
public class RetornoPadrao<T> : RetornoPadrao
{
    public T? Dados { get; set; }

    public static RetornoPadrao<T> Ok(T dados, string mensagem = "") =>
        new() { Sucesso = true, Mensagem = mensagem, StatusCode = 200, Dados = dados };

    public static RetornoPadrao<T> Criado(T dados, string mensagem = "") =>
        new() { Sucesso = true, Mensagem = mensagem, StatusCode = 201, Dados = dados };

    public static new RetornoPadrao<T> Invalido(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 400 };

    public static new RetornoPadrao<T> Proibido(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 403 };

    public static new RetornoPadrao<T> NaoEncontrado(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 404 };

    public static new RetornoPadrao<T> Conflito(string mensagem) =>
        new() { Sucesso = false, Mensagem = mensagem, StatusCode = 409 };

    /// <summary>
    /// Propaga uma falha vinda de outro retorno, preservando mensagem e status.
    /// Usado quando um método público repassa a falha de uma etapa anterior.
    /// </summary>
    public static RetornoPadrao<T> Falha(RetornoPadrao origem) =>
        new() { Sucesso = false, Mensagem = origem.Mensagem, StatusCode = origem.StatusCode };
}
