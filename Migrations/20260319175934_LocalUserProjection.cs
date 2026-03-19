using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace nutra.api.Migrations
{
    /// <inheritdoc />
    public partial class LocalUserProjection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    NomeCompleto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FotoPerfilUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fabricantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fabricante = table.Column<string>(type: "text", nullable: true),
                    Produto = table.Column<string>(type: "text", nullable: true),
                    PorcaoTexto = table.Column<string>(type: "text", nullable: true),
                    Unidade = table.Column<string>(type: "text", nullable: true),
                    Dose = table.Column<string>(type: "text", nullable: true),
                    Porcao = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKj = table.Column<double>(type: "double precision", nullable: true),
                    Proteinas = table.Column<double>(type: "double precision", nullable: true),
                    Carboidratos = table.Column<double>(type: "double precision", nullable: true),
                    Acucar = table.Column<double>(type: "double precision", nullable: true),
                    Gorduras = table.Column<double>(type: "double precision", nullable: true),
                    GorduraSaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraPoliinsaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraMonoinsaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraTrans = table.Column<double>(type: "double precision", nullable: true),
                    Colesterol = table.Column<double>(type: "double precision", nullable: true),
                    Fibras = table.Column<double>(type: "double precision", nullable: true),
                    Sodio = table.Column<double>(type: "double precision", nullable: true),
                    Potassio = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fabricantes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FastFoods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fabricante = table.Column<string>(type: "text", nullable: true),
                    Produto = table.Column<string>(type: "text", nullable: true),
                    PorcaoTexto = table.Column<string>(type: "text", nullable: true),
                    Unidade = table.Column<string>(type: "text", nullable: true),
                    Dose = table.Column<string>(type: "text", nullable: true),
                    Porcao = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKj = table.Column<double>(type: "double precision", nullable: true),
                    Proteinas = table.Column<double>(type: "double precision", nullable: true),
                    Carboidratos = table.Column<double>(type: "double precision", nullable: true),
                    Acucar = table.Column<double>(type: "double precision", nullable: true),
                    Gorduras = table.Column<double>(type: "double precision", nullable: true),
                    GorduraSaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraPoliinsaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraMonoinsaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraTrans = table.Column<double>(type: "double precision", nullable: true),
                    Colesterol = table.Column<double>(type: "double precision", nullable: true),
                    Fibras = table.Column<double>(type: "double precision", nullable: true),
                    Sodio = table.Column<double>(type: "double precision", nullable: true),
                    Potassio = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FastFoods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genericos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoriaPrincipal = table.Column<string>(type: "text", nullable: true),
                    SubCategoria = table.Column<string>(type: "text", nullable: true),
                    Produto = table.Column<string>(type: "text", nullable: true),
                    PorcaoTexto = table.Column<string>(type: "text", nullable: true),
                    Unidade = table.Column<string>(type: "text", nullable: true),
                    Dose = table.Column<string>(type: "text", nullable: true),
                    Porcao = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKj = table.Column<double>(type: "double precision", nullable: true),
                    Proteinas = table.Column<double>(type: "double precision", nullable: true),
                    Carboidratos = table.Column<double>(type: "double precision", nullable: true),
                    Acucar = table.Column<double>(type: "double precision", nullable: true),
                    Gorduras = table.Column<double>(type: "double precision", nullable: true),
                    GorduraSaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraPoliinsaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraMonoinsaturada = table.Column<double>(type: "double precision", nullable: true),
                    GorduraTrans = table.Column<double>(type: "double precision", nullable: true),
                    Colesterol = table.Column<double>(type: "double precision", nullable: true),
                    Fibras = table.Column<double>(type: "double precision", nullable: true),
                    Sodio = table.Column<double>(type: "double precision", nullable: true),
                    Potassio = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genericos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tbcas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "text", nullable: true),
                    NomeCientifico = table.Column<string>(type: "text", nullable: true),
                    Grupo = table.Column<string>(type: "text", nullable: true),
                    Marca = table.Column<string>(type: "text", nullable: true),
                    AlfaTocoferolVitaminaEMg = table.Column<double>(type: "double precision", nullable: true),
                    AcucarDeAdicaoG = table.Column<double>(type: "double precision", nullable: true),
                    CarboidratoDisponivelG = table.Column<double>(type: "double precision", nullable: true),
                    CarboidratoTotalG = table.Column<double>(type: "double precision", nullable: true),
                    CinzasG = table.Column<double>(type: "double precision", nullable: true),
                    CobreMg = table.Column<double>(type: "double precision", nullable: true),
                    ColesterolMg = table.Column<double>(type: "double precision", nullable: true),
                    CalcioMg = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKJ = table.Column<double>(type: "double precision", nullable: true),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: true),
                    EquivalenteDeFolatoMcg = table.Column<double>(type: "double precision", nullable: true),
                    FerroMg = table.Column<double>(type: "double precision", nullable: true),
                    FibraAlimentarG = table.Column<double>(type: "double precision", nullable: true),
                    FosforoMg = table.Column<double>(type: "double precision", nullable: true),
                    LipidiosG = table.Column<double>(type: "double precision", nullable: true),
                    MagnesioMg = table.Column<double>(type: "double precision", nullable: true),
                    ManganesMg = table.Column<double>(type: "double precision", nullable: true),
                    NiacinaMg = table.Column<double>(type: "double precision", nullable: true),
                    PotassioMg = table.Column<double>(type: "double precision", nullable: true),
                    ProteinaG = table.Column<double>(type: "double precision", nullable: true),
                    RiboflavinaMg = table.Column<double>(type: "double precision", nullable: true),
                    SalDeAdicaoG = table.Column<double>(type: "double precision", nullable: true),
                    SelenioMcg = table.Column<double>(type: "double precision", nullable: true),
                    SodioMg = table.Column<double>(type: "double precision", nullable: true),
                    TiaminaMg = table.Column<double>(type: "double precision", nullable: true),
                    UmidadeG = table.Column<double>(type: "double precision", nullable: true),
                    VitaminaARaeMcg = table.Column<double>(type: "double precision", nullable: true),
                    VitaminaAReMcg = table.Column<double>(type: "double precision", nullable: true),
                    VitaminaB12Mcg = table.Column<double>(type: "double precision", nullable: true),
                    VitaminaB6Mg = table.Column<double>(type: "double precision", nullable: true),
                    VitaminaCMg = table.Column<double>(type: "double precision", nullable: true),
                    VitaminaDMcg = table.Column<double>(type: "double precision", nullable: true),
                    ZincoMg = table.Column<double>(type: "double precision", nullable: true),
                    AcidosGraxosMonoinsaturadosG = table.Column<double>(type: "double precision", nullable: true),
                    AcidosGraxosPoliinsaturadosG = table.Column<double>(type: "double precision", nullable: true),
                    AcidosGraxosSaturadosG = table.Column<double>(type: "double precision", nullable: true),
                    AcidosGraxosTransG = table.Column<double>(type: "double precision", nullable: true),
                    AlcoolG = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tbcas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModelosDieta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ObjetivoAlvo = table.Column<int>(type: "integer", nullable: false),
                    PreferenciaAlimentarAlvo = table.Column<int>(type: "integer", nullable: false),
                    CaloriasBase = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaBaseG = table.Column<double>(type: "double precision", nullable: false),
                    CarboidratoBaseG = table.Column<double>(type: "double precision", nullable: false),
                    GorduraBaseG = table.Column<double>(type: "double precision", nullable: false),
                    NumeroRefeicoesDia = table.Column<int>(type: "integer", nullable: false),
                    CriadoPorProfissionalId = table.Column<string>(type: "character varying(128)", nullable: true),
                    Publico = table.Column<bool>(type: "boolean", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelosDieta_ApplicationUsers_CriadoPorProfissionalId",
                        column: x => x.CriadoPorProfissionalId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PerfisProfissionais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(128)", nullable: false),
                    CRN = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CRNRegiao = table.Column<int>(type: "integer", nullable: false),
                    Especialidade = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BioProfissional = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    AnosExperiencia = table.Column<int>(type: "integer", nullable: true),
                    UrlDiploma = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CRNVerificado = table.Column<bool>(type: "boolean", nullable: false),
                    DataVerificacaoCRN = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    MaxPacientes = table.Column<int>(type: "integer", nullable: false),
                    MultiClinicaHabilitado = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisProfissionais", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfisProfissionais_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefeicoeModelosDieta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ModeloDietaId = table.Column<int>(type: "integer", nullable: false),
                    TipoRefeicao = table.Column<int>(type: "integer", nullable: false),
                    HorarioSugerido = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PercentualCaloricoSugerido = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefeicoeModelosDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefeicoeModelosDieta_ModelosDieta_ModeloDietaId",
                        column: x => x.ModeloDietaId,
                        principalTable: "ModelosDieta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assinaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilProfissionalId = table.Column<int>(type: "integer", nullable: false),
                    Plano = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataExpiracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataCancelamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GatewaySubscriptionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ValorMensal = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    RenovacaoAutomatica = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assinaturas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assinaturas_PerfisProfissionais_PerfilProfissionalId",
                        column: x => x.PerfilProfissionalId,
                        principalTable: "PerfisProfissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clinicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CNPJ = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: true),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LogoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Numero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Cidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Estado = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CEP = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: true),
                    PerfilProfissionalId = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clinicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clinicas_PerfisProfissionais_PerfilProfissionalId",
                        column: x => x.PerfilProfissionalId,
                        principalTable: "PerfisProfissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensModelosDieta",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefeicaoModeloDietaId = table.Column<int>(type: "integer", nullable: false),
                    AlimentoId = table.Column<int>(type: "integer", nullable: false),
                    TipoTabela = table.Column<int>(type: "integer", nullable: false),
                    NomeAlimentoSnapshot = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    QuantidadeG = table.Column<double>(type: "double precision", nullable: false),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaG = table.Column<double>(type: "double precision", nullable: false),
                    CarboidratoG = table.Column<double>(type: "double precision", nullable: false),
                    GorduraG = table.Column<double>(type: "double precision", nullable: false),
                    FibraG = table.Column<double>(type: "double precision", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensModelosDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensModelosDieta_RefeicoeModelosDieta_RefeicaoModeloDietaId",
                        column: x => x.RefeicaoModeloDietaId,
                        principalTable: "RefeicoeModelosDieta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VinculosPacienteProfissional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PacienteUserId = table.Column<string>(type: "character varying(128)", nullable: false),
                    PerfilProfissionalId = table.Column<int>(type: "integer", nullable: false),
                    ClinicaId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DataConvite = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAceite = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DataEncerramento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VinculosPacienteProfissional", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VinculosPacienteProfissional_ApplicationUsers_PacienteUserId",
                        column: x => x.PacienteUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VinculosPacienteProfissional_Clinicas_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "Clinicas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VinculosPacienteProfissional_PerfisProfissionais_PerfilProf~",
                        column: x => x.PerfilProfissionalId,
                        principalTable: "PerfisProfissionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AnamnesesAlimentares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    DataPreenchimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RefeicoesPorDia = table.Column<int>(type: "integer", nullable: false),
                    HorarioCafeManha = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorarioAlmoco = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorarioLancheTarde = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorarioJantar = table.Column<TimeSpan>(type: "interval", nullable: true),
                    HorarioCeia = table.Column<TimeSpan>(type: "interval", nullable: true),
                    RefeicoesPuladas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ConsumoAguaLitrosDia = table.Column<double>(type: "double precision", nullable: false),
                    ConsumoRefrigerantes = table.Column<int>(type: "integer", nullable: false),
                    ConsumoAlcool = table.Column<int>(type: "integer", nullable: false),
                    ConsumoCafeCha = table.Column<int>(type: "integer", nullable: false),
                    ConsumoFastFood = table.Column<int>(type: "integer", nullable: false),
                    ConsumoFrutas = table.Column<int>(type: "integer", nullable: false),
                    ConsumoVerduras = table.Column<int>(type: "integer", nullable: false),
                    ConsumoDoces = table.Column<int>(type: "integer", nullable: false),
                    ConsumoFrituras = table.Column<int>(type: "integer", nullable: false),
                    ComeComDistracao = table.Column<bool>(type: "boolean", nullable: false),
                    CompulsaoAlimentar = table.Column<bool>(type: "boolean", nullable: false),
                    HistoricoDietasRestritivas = table.Column<bool>(type: "boolean", nullable: false),
                    DescricaoDietasAnteriores = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SuplementosEmUso = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IntestinoRegular = table.Column<bool>(type: "boolean", nullable: false),
                    FrequenciaEvacuacaoSemana = table.Column<int>(type: "integer", nullable: true),
                    QueixasDigestivas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AlimentosQueNaoGosta = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    AlimentosPreferidos = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ObservacoesGerais = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnamnesesAlimentares", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvaliacoesAntropometricas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    ProfissionalResponsavelId = table.Column<string>(type: "character varying(128)", nullable: true),
                    DataAvaliacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    PesoKg = table.Column<double>(type: "double precision", nullable: false),
                    AlturaCm = table.Column<double>(type: "double precision", nullable: false),
                    IMC = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    ClassificacaoIMC = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CircunferenciaPescocoCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaToraxCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaCinturaCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaAbdomenCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaQuadrilCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaBracoDireitoCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaBracoEsquerdoCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaAntebracoDireitoCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaAntebracoEsquerdoCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaCoxaDireitaCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaCoxaEsquerdaCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaPanturrilhaDireitaCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaPanturrilhaEsquerdaCm = table.Column<double>(type: "double precision", nullable: true),
                    RCQ = table.Column<decimal>(type: "numeric(4,2)", nullable: true),
                    ProtocoloDobrasCutaneas = table.Column<int>(type: "integer", nullable: true),
                    DobraTricepsMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraBicepsMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraSubescapularMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraSuprailiacaMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraAbdominalMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraCoxaMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraPanturrilhaMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraAxilarMediaMm = table.Column<double>(type: "double precision", nullable: true),
                    DobraPeitoralMm = table.Column<double>(type: "double precision", nullable: true),
                    SomatorioDobras = table.Column<double>(type: "double precision", nullable: true),
                    DensidadeCorporal = table.Column<decimal>(type: "numeric(6,4)", nullable: true),
                    PercentualGorduraDobrasCutaneas = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    PossuiBioimpedancia = table.Column<bool>(type: "boolean", nullable: false),
                    BioPercentualGordura = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    BioMassaMagraKg = table.Column<double>(type: "double precision", nullable: true),
                    BioMassaGordaKg = table.Column<double>(type: "double precision", nullable: true),
                    BioAguaCorporalLitros = table.Column<double>(type: "double precision", nullable: true),
                    BioPercentualAgua = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    BioTMBKcal = table.Column<double>(type: "double precision", nullable: true),
                    BioGorduraVisceralNivel = table.Column<int>(type: "integer", nullable: true),
                    BioIdadeMetabolica = table.Column<int>(type: "integer", nullable: true),
                    BioMassaOsseaKg = table.Column<double>(type: "double precision", nullable: true),
                    TMBMifflinStJeor = table.Column<double>(type: "double precision", nullable: true),
                    TMBHarrisBenedict = table.Column<double>(type: "double precision", nullable: true),
                    TMBKatchMcArdle = table.Column<double>(type: "double precision", nullable: true),
                    GET = table.Column<double>(type: "double precision", nullable: true),
                    PercentualGorduraEstimado = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    MassaMagraEstimadaKg = table.Column<double>(type: "double precision", nullable: true),
                    MassaGordaEstimadaKg = table.Column<double>(type: "double precision", nullable: true),
                    PesoIdealDevineKg = table.Column<double>(type: "double precision", nullable: true),
                    PesoIdealIMCKg = table.Column<double>(type: "double precision", nullable: true),
                    TaxaMetabolicaAjustada = table.Column<double>(type: "double precision", nullable: true),
                    ProteinaRecomendadaG = table.Column<double>(type: "double precision", nullable: true),
                    CarboidratoRecomendadoG = table.Column<double>(type: "double precision", nullable: true),
                    GorduraRecomendadaG = table.Column<double>(type: "double precision", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvaliacoesAntropometricas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvaliacoesAntropometricas_ApplicationUsers_ProfissionalResp~",
                        column: x => x.ProfissionalResponsavelId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FotosProgresso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AvaliacaoAntropometricaId = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    DataFoto = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosProgresso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotosProgresso_AvaliacoesAntropometricas_AvaliacaoAntropome~",
                        column: x => x.AvaliacaoAntropometricaId,
                        principalTable: "AvaliacoesAntropometricas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FotosRefeicao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(128)", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoRefeicao = table.Column<int>(type: "integer", nullable: false),
                    FotoUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RegistroAlimentarId = table.Column<long>(type: "bigint", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosRefeicao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotosRefeicao_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistoricoClinicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    Condicao = table.Column<int>(type: "integer", nullable: false),
                    DescricaoOutra = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DataDiagnostico = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AtivaAtualmente = table.Column<bool>(type: "boolean", nullable: false),
                    MedicamentosEmUso = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoClinicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItensRefeicao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RefeicaoPlanoId = table.Column<int>(type: "integer", nullable: false),
                    AlimentoId = table.Column<int>(type: "integer", nullable: false),
                    TipoTabela = table.Column<int>(type: "integer", nullable: false),
                    NomeAlimentoSnapshot = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    QuantidadeG = table.Column<double>(type: "double precision", nullable: false),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaG = table.Column<double>(type: "double precision", nullable: false),
                    CarboidratoG = table.Column<double>(type: "double precision", nullable: false),
                    GorduraG = table.Column<double>(type: "double precision", nullable: false),
                    FibraG = table.Column<double>(type: "double precision", nullable: false),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensRefeicao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubstituicoesEquivalentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ItemRefeicaoId = table.Column<int>(type: "integer", nullable: false),
                    AlimentoId = table.Column<int>(type: "integer", nullable: false),
                    TipoTabela = table.Column<int>(type: "integer", nullable: false),
                    NomeAlimento = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    QuantidadeG = table.Column<double>(type: "double precision", nullable: false),
                    EnergiaKcal = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaG = table.Column<double>(type: "double precision", nullable: false),
                    CarboidratoG = table.Column<double>(type: "double precision", nullable: false),
                    GorduraG = table.Column<double>(type: "double precision", nullable: false),
                    FibraG = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubstituicoesEquivalentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubstituicoesEquivalentes_ItensRefeicao_ItemRefeicaoId",
                        column: x => x.ItemRefeicaoId,
                        principalTable: "ItensRefeicao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetasNutricionais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DataCalculo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CaloriasDiarias = table.Column<double>(type: "double precision", nullable: false),
                    ProteinasDiarias = table.Column<double>(type: "double precision", nullable: false),
                    CarboidratosDiarios = table.Column<double>(type: "double precision", nullable: false),
                    GordurasDiarias = table.Column<double>(type: "double precision", nullable: false),
                    AguaDiaria = table.Column<double>(type: "double precision", nullable: false),
                    FibraDiaria = table.Column<double>(type: "double precision", nullable: false),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetasNutricionais", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfilNutricional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(128)", nullable: false),
                    MetaNutricionalAtualId = table.Column<int>(type: "integer", nullable: true),
                    AlturaCm = table.Column<double>(type: "double precision", nullable: false),
                    PesoAtualKg = table.Column<double>(type: "double precision", nullable: false),
                    PercentualGorduraCorporal = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaCinturaCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaQuadrilCm = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaBracoCm = table.Column<double>(type: "double precision", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Genero = table.Column<int>(type: "integer", nullable: false),
                    FatorAtividade = table.Column<double>(type: "double precision", nullable: false),
                    NivelAtividade = table.Column<int>(type: "integer", nullable: false),
                    OcupacaoProfissional = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    HabilidadeCulinaria = table.Column<int>(type: "integer", nullable: false),
                    OrcamentoMensal = table.Column<int>(type: "integer", nullable: false),
                    PossuiDoencasPreExistentes = table.Column<bool>(type: "boolean", nullable: false),
                    DescricaoCondicoesMedicas = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Fumante = table.Column<bool>(type: "boolean", nullable: false),
                    QualidadeSono = table.Column<int>(type: "integer", nullable: true),
                    HorasSonoPorNoite = table.Column<double>(type: "double precision", nullable: true),
                    Objetivo = table.Column<int>(type: "integer", nullable: false),
                    PesoDesejadoKg = table.Column<double>(type: "double precision", nullable: true),
                    PreferenciaDieta = table.Column<int>(type: "integer", nullable: false),
                    RefeicoesPorDiaDesejadas = table.Column<int>(type: "integer", nullable: false),
                    TempoDisponivelPreparoMinutos = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilNutricional", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilNutricional_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilNutricional_MetasNutricionais_MetaNutricionalAtualId",
                        column: x => x.MetaNutricionalAtualId,
                        principalTable: "MetasNutricionais",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PerfisEquipamentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    Equipamento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisEquipamentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfisEquipamentos_PerfilNutricional_PerfilNutricionalId",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanosAlimentares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    ProfissionalResponsavelId = table.Column<string>(type: "character varying(128)", nullable: true),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CaloriasAlvoDiarias = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaAlvoG = table.Column<double>(type: "double precision", nullable: false),
                    CarboidratoAlvoG = table.Column<double>(type: "double precision", nullable: false),
                    GorduraAlvoG = table.Column<double>(type: "double precision", nullable: false),
                    FibraAlvoG = table.Column<double>(type: "double precision", nullable: false),
                    AguaAlvoL = table.Column<double>(type: "double precision", nullable: false),
                    ModeloDietaOrigemId = table.Column<int>(type: "integer", nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanosAlimentares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanosAlimentares_ApplicationUsers_ProfissionalResponsavelId",
                        column: x => x.ProfissionalResponsavelId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PlanosAlimentares_ModelosDieta_ModeloDietaOrigemId",
                        column: x => x.ModeloDietaOrigemId,
                        principalTable: "ModelosDieta",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PlanosAlimentares_PerfilNutricional_PerfilNutricionalId",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PreferenciaAlimentar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    AlimentoId = table.Column<int>(type: "integer", nullable: false),
                    Tabela = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenciaAlimentar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PreferenciaAlimentar_PerfilNutricional_PerfilNutricionalId",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistroBiometrico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Data = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PesoKg = table.Column<double>(type: "double precision", nullable: false),
                    PercentualGordura = table.Column<double>(type: "double precision", nullable: true),
                    CircunferenciaCinturaCm = table.Column<double>(type: "double precision", nullable: true),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroBiometrico", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroBiometrico_PerfilNutricional_PerfilNutricionalId",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestricaoAlimentar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    CompostoOrganico = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestricaoAlimentar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestricaoAlimentar_PerfilNutricional_PerfilNutricionalId",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefeicoesPlanejadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlanoAlimentarId = table.Column<int>(type: "integer", nullable: false),
                    TipoRefeicao = table.Column<int>(type: "integer", nullable: false),
                    HorarioSugerido = table.Column<TimeSpan>(type: "interval", nullable: true),
                    Ordem = table.Column<int>(type: "integer", nullable: false),
                    Observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TotalEnergiaKcal = table.Column<double>(type: "double precision", nullable: false),
                    TotalProteinaG = table.Column<double>(type: "double precision", nullable: false),
                    TotalCarboidratoG = table.Column<double>(type: "double precision", nullable: false),
                    TotalGorduraG = table.Column<double>(type: "double precision", nullable: false),
                    TotalFibraG = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefeicoesPlanejadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefeicoesPlanejadas_PlanosAlimentares_PlanoAlimentarId",
                        column: x => x.PlanoAlimentarId,
                        principalTable: "PlanosAlimentares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistroAlimentar",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "character varying(128)", nullable: false),
                    AlimentoIdOrigem = table.Column<int>(type: "integer", nullable: false),
                    NomeAlimentoSnapshot = table.Column<string>(type: "text", nullable: false),
                    TipoTabela = table.Column<int>(type: "integer", nullable: false),
                    QuantidadeConsumidaG = table.Column<double>(type: "double precision", nullable: false),
                    DataConsumo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Refeicao = table.Column<int>(type: "integer", nullable: false),
                    EnergiaKcalTotal = table.Column<double>(type: "double precision", nullable: false),
                    ProteinaTotal = table.Column<double>(type: "double precision", nullable: false),
                    CarboTotal = table.Column<double>(type: "double precision", nullable: false),
                    GorduraTotal = table.Column<double>(type: "double precision", nullable: false),
                    FibraTotal = table.Column<double>(type: "double precision", nullable: false),
                    AguaTotal = table.Column<double>(type: "double precision", nullable: false),
                    DadosNutricionaisCompletosJson = table.Column<string>(type: "text", nullable: false),
                    PlanoAlimentarId = table.Column<int>(type: "integer", nullable: true),
                    ItemRefeicaoPlanoId = table.Column<int>(type: "integer", nullable: true),
                    CodigoBarras = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAlimentar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroAlimentar_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistroAlimentar_ItensRefeicao_ItemRefeicaoPlanoId",
                        column: x => x.ItemRefeicaoPlanoId,
                        principalTable: "ItensRefeicao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistroAlimentar_PlanosAlimentares_PlanoAlimentarId",
                        column: x => x.PlanoAlimentarId,
                        principalTable: "PlanosAlimentares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnamnesesAlimentares_PerfilNutricionalId",
                table: "AnamnesesAlimentares",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_Assinaturas_PerfilProfissionalId",
                table: "Assinaturas",
                column: "PerfilProfissionalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesAntropometricas_PerfilNutricionalId",
                table: "AvaliacoesAntropometricas",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_AvaliacoesAntropometricas_ProfissionalResponsavelId",
                table: "AvaliacoesAntropometricas",
                column: "ProfissionalResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinicas_PerfilProfissionalId",
                table: "Clinicas",
                column: "PerfilProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosProgresso_AvaliacaoAntropometricaId",
                table: "FotosProgresso",
                column: "AvaliacaoAntropometricaId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosRefeicao_RegistroAlimentarId",
                table: "FotosRefeicao",
                column: "RegistroAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosRefeicao_UserId",
                table: "FotosRefeicao",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoClinicos_PerfilNutricionalId",
                table: "HistoricoClinicos",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensModelosDieta_RefeicaoModeloDietaId",
                table: "ItensModelosDieta",
                column: "RefeicaoModeloDietaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensRefeicao_RefeicaoPlanoId",
                table: "ItensRefeicao",
                column: "RefeicaoPlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_MetasNutricionais_PerfilNutricionalId",
                table: "MetasNutricionais",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_ModelosDieta_CriadoPorProfissionalId",
                table: "ModelosDieta",
                column: "CriadoPorProfissionalId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilNutricional_MetaNutricionalAtualId",
                table: "PerfilNutricional",
                column: "MetaNutricionalAtualId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilNutricional_UserId",
                table: "PerfilNutricional",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisEquipamentos_PerfilNutricionalId",
                table: "PerfisEquipamentos",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfisProfissionais_CRN",
                table: "PerfisProfissionais",
                column: "CRN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfisProfissionais_UserId",
                table: "PerfisProfissionais",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanosAlimentares_ModeloDietaOrigemId",
                table: "PlanosAlimentares",
                column: "ModeloDietaOrigemId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosAlimentares_PerfilNutricionalId",
                table: "PlanosAlimentares",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanosAlimentares_ProfissionalResponsavelId",
                table: "PlanosAlimentares",
                column: "ProfissionalResponsavelId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenciaAlimentar_PerfilNutricionalId",
                table: "PreferenciaAlimentar",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_RefeicoeModelosDieta_ModeloDietaId",
                table: "RefeicoeModelosDieta",
                column: "ModeloDietaId");

            migrationBuilder.CreateIndex(
                name: "IX_RefeicoesPlanejadas_PlanoAlimentarId",
                table: "RefeicoesPlanejadas",
                column: "PlanoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAlimentar_ItemRefeicaoPlanoId",
                table: "RegistroAlimentar",
                column: "ItemRefeicaoPlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAlimentar_PlanoAlimentarId",
                table: "RegistroAlimentar",
                column: "PlanoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAlimentar_UserId",
                table: "RegistroAlimentar",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroBiometrico_PerfilNutricionalId",
                table: "RegistroBiometrico",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_RestricaoAlimentar_PerfilNutricionalId",
                table: "RestricaoAlimentar",
                column: "PerfilNutricionalId");

            migrationBuilder.CreateIndex(
                name: "IX_SubstituicoesEquivalentes_ItemRefeicaoId",
                table: "SubstituicoesEquivalentes",
                column: "ItemRefeicaoId");

            migrationBuilder.CreateIndex(
                name: "IX_VinculosPacienteProfissional_ClinicaId",
                table: "VinculosPacienteProfissional",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_VinculosPacienteProfissional_PacienteUserId_PerfilProfissio~",
                table: "VinculosPacienteProfissional",
                columns: new[] { "PacienteUserId", "PerfilProfissionalId" },
                unique: true,
                filter: "\"Status\" IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_VinculosPacienteProfissional_PerfilProfissionalId",
                table: "VinculosPacienteProfissional",
                column: "PerfilProfissionalId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnamnesesAlimentares_PerfilNutricional_PerfilNutricionalId",
                table: "AnamnesesAlimentares",
                column: "PerfilNutricionalId",
                principalTable: "PerfilNutricional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AvaliacoesAntropometricas_PerfilNutricional_PerfilNutricion~",
                table: "AvaliacoesAntropometricas",
                column: "PerfilNutricionalId",
                principalTable: "PerfilNutricional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FotosRefeicao_RegistroAlimentar_RegistroAlimentarId",
                table: "FotosRefeicao",
                column: "RegistroAlimentarId",
                principalTable: "RegistroAlimentar",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoClinicos_PerfilNutricional_PerfilNutricionalId",
                table: "HistoricoClinicos",
                column: "PerfilNutricionalId",
                principalTable: "PerfilNutricional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItensRefeicao_RefeicoesPlanejadas_RefeicaoPlanoId",
                table: "ItensRefeicao",
                column: "RefeicaoPlanoId",
                principalTable: "RefeicoesPlanejadas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MetasNutricionais_PerfilNutricional_PerfilNutricionalId",
                table: "MetasNutricionais",
                column: "PerfilNutricionalId",
                principalTable: "PerfilNutricional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetasNutricionais_PerfilNutricional_PerfilNutricionalId",
                table: "MetasNutricionais");

            migrationBuilder.DropTable(
                name: "AnamnesesAlimentares");

            migrationBuilder.DropTable(
                name: "Assinaturas");

            migrationBuilder.DropTable(
                name: "Fabricantes");

            migrationBuilder.DropTable(
                name: "FastFoods");

            migrationBuilder.DropTable(
                name: "FotosProgresso");

            migrationBuilder.DropTable(
                name: "FotosRefeicao");

            migrationBuilder.DropTable(
                name: "Genericos");

            migrationBuilder.DropTable(
                name: "HistoricoClinicos");

            migrationBuilder.DropTable(
                name: "ItensModelosDieta");

            migrationBuilder.DropTable(
                name: "PerfisEquipamentos");

            migrationBuilder.DropTable(
                name: "PreferenciaAlimentar");

            migrationBuilder.DropTable(
                name: "RegistroBiometrico");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "SubstituicoesEquivalentes");

            migrationBuilder.DropTable(
                name: "Tbcas");

            migrationBuilder.DropTable(
                name: "VinculosPacienteProfissional");

            migrationBuilder.DropTable(
                name: "AvaliacoesAntropometricas");

            migrationBuilder.DropTable(
                name: "RegistroAlimentar");

            migrationBuilder.DropTable(
                name: "RefeicoeModelosDieta");

            migrationBuilder.DropTable(
                name: "Clinicas");

            migrationBuilder.DropTable(
                name: "ItensRefeicao");

            migrationBuilder.DropTable(
                name: "PerfisProfissionais");

            migrationBuilder.DropTable(
                name: "RefeicoesPlanejadas");

            migrationBuilder.DropTable(
                name: "PlanosAlimentares");

            migrationBuilder.DropTable(
                name: "ModelosDieta");

            migrationBuilder.DropTable(
                name: "PerfilNutricional");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "MetasNutricionais");
        }
    }
}
