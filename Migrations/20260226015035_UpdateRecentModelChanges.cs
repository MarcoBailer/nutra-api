using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nutra.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecentModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PerfilNutricional_MetasNutricionais_MetaNutricionalAtualId",
                table: "PerfilNutricional");

            migrationBuilder.DropIndex(
                name: "IX_PerfilNutricional_MetaNutricionalAtualId",
                table: "PerfilNutricional");

            migrationBuilder.AddColumn<string>(
                name: "CodigoBarras",
                table: "RegistroAlimentar",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemRefeicaoPlanoId",
                table: "RegistroAlimentar",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanoAlimentarId",
                table: "RegistroAlimentar",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OcupacaoProfissional",
                table: "PerfilNutricional",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "DescricaoCondicoesMedicas",
                table: "PerfilNutricional",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "AtualizadoEm",
                table: "PerfilNutricional",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CircunferenciaBracoCm",
                table: "PerfilNutricional",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CircunferenciaQuadrilCm",
                table: "PerfilNutricional",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "PerfilNutricional",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Fumante",
                table: "PerfilNutricional",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HabilidadeCulinaria",
                table: "PerfilNutricional",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "HorasSonoPorNoite",
                table: "PerfilNutricional",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrcamentoMensal",
                table: "PerfilNutricional",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "QualidadeSono",
                table: "PerfilNutricional",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "AspNetUsers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "AspNetUsers",
                type: "character varying(14)",
                maxLength: 14,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "AtualizadoEm",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "AspNetUsers",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "AspNetUsers",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CriadoEm",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "AspNetUsers",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilUrl",
                table: "AspNetUsers",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logradouro",
                table: "AspNetUsers",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "AspNetUsers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Telefone",
                table: "AspNetUsers",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

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
                    table.ForeignKey(
                        name: "FK_AnamnesesAlimentares_PerfilNutricional_PerfilNutricionalId",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvaliacoesAntropometricas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    ProfissionalResponsavelId = table.Column<string>(type: "text", nullable: true),
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
                        name: "FK_AvaliacoesAntropometricas_AspNetUsers_ProfissionalResponsav~",
                        column: x => x.ProfissionalResponsavelId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AvaliacoesAntropometricas_PerfilNutricional_PerfilNutricion~",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FotosRefeicao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
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
                        name: "FK_FotosRefeicao_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FotosRefeicao_RegistroAlimentar_RegistroAlimentarId",
                        column: x => x.RegistroAlimentarId,
                        principalTable: "RegistroAlimentar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
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
                    table.ForeignKey(
                        name: "FK_HistoricoClinicos_PerfilNutricional_PerfilNutricionalId",
                        column: x => x.PerfilNutricionalId,
                        principalTable: "PerfilNutricional",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                    CriadoPorProfissionalId = table.Column<string>(type: "text", nullable: true),
                    Publico = table.Column<bool>(type: "boolean", nullable: false),
                    Ativo = table.Column<bool>(type: "boolean", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelosDieta", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModelosDieta_AspNetUsers_CriadoPorProfissionalId",
                        column: x => x.CriadoPorProfissionalId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PerfisProfissionais",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
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
                        name: "FK_PerfisProfissionais_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "PlanosAlimentares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PerfilNutricionalId = table.Column<int>(type: "integer", nullable: false),
                    ProfissionalResponsavelId = table.Column<string>(type: "text", nullable: true),
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
                        name: "FK_PlanosAlimentares_AspNetUsers_ProfissionalResponsavelId",
                        column: x => x.ProfissionalResponsavelId,
                        principalTable: "AspNetUsers",
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
                    PacienteUserId = table.Column<string>(type: "text", nullable: false),
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
                        name: "FK_VinculosPacienteProfissional_AspNetUsers_PacienteUserId",
                        column: x => x.PacienteUserId,
                        principalTable: "AspNetUsers",
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
                    table.ForeignKey(
                        name: "FK_ItensRefeicao_RefeicoesPlanejadas_RefeicaoPlanoId",
                        column: x => x.RefeicaoPlanoId,
                        principalTable: "RefeicoesPlanejadas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAlimentar_ItemRefeicaoPlanoId",
                table: "RegistroAlimentar",
                column: "ItemRefeicaoPlanoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistroAlimentar_PlanoAlimentarId",
                table: "RegistroAlimentar",
                column: "PlanoAlimentarId");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilNutricional_MetaNutricionalAtualId",
                table: "PerfilNutricional",
                column: "MetaNutricionalAtualId");

            migrationBuilder.CreateIndex(
                name: "IX_MetasNutricionais_PerfilNutricionalId",
                table: "MetasNutricionais",
                column: "PerfilNutricionalId");

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
                name: "IX_ModelosDieta_CriadoPorProfissionalId",
                table: "ModelosDieta",
                column: "CriadoPorProfissionalId");

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
                name: "IX_RefeicoeModelosDieta_ModeloDietaId",
                table: "RefeicoeModelosDieta",
                column: "ModeloDietaId");

            migrationBuilder.CreateIndex(
                name: "IX_RefeicoesPlanejadas_PlanoAlimentarId",
                table: "RefeicoesPlanejadas",
                column: "PlanoAlimentarId");

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
                name: "FK_MetasNutricionais_PerfilNutricional_PerfilNutricionalId",
                table: "MetasNutricionais",
                column: "PerfilNutricionalId",
                principalTable: "PerfilNutricional",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfilNutricional_MetasNutricionais_MetaNutricionalAtualId",
                table: "PerfilNutricional",
                column: "MetaNutricionalAtualId",
                principalTable: "MetasNutricionais",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistroAlimentar_ItensRefeicao_ItemRefeicaoPlanoId",
                table: "RegistroAlimentar",
                column: "ItemRefeicaoPlanoId",
                principalTable: "ItensRefeicao",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_RegistroAlimentar_PlanosAlimentares_PlanoAlimentarId",
                table: "RegistroAlimentar",
                column: "PlanoAlimentarId",
                principalTable: "PlanosAlimentares",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MetasNutricionais_PerfilNutricional_PerfilNutricionalId",
                table: "MetasNutricionais");

            migrationBuilder.DropForeignKey(
                name: "FK_PerfilNutricional_MetasNutricionais_MetaNutricionalAtualId",
                table: "PerfilNutricional");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistroAlimentar_ItensRefeicao_ItemRefeicaoPlanoId",
                table: "RegistroAlimentar");

            migrationBuilder.DropForeignKey(
                name: "FK_RegistroAlimentar_PlanosAlimentares_PlanoAlimentarId",
                table: "RegistroAlimentar");

            migrationBuilder.DropTable(
                name: "AnamnesesAlimentares");

            migrationBuilder.DropTable(
                name: "Assinaturas");

            migrationBuilder.DropTable(
                name: "FotosProgresso");

            migrationBuilder.DropTable(
                name: "FotosRefeicao");

            migrationBuilder.DropTable(
                name: "HistoricoClinicos");

            migrationBuilder.DropTable(
                name: "ItensModelosDieta");

            migrationBuilder.DropTable(
                name: "SubstituicoesEquivalentes");

            migrationBuilder.DropTable(
                name: "VinculosPacienteProfissional");

            migrationBuilder.DropTable(
                name: "AvaliacoesAntropometricas");

            migrationBuilder.DropTable(
                name: "RefeicoeModelosDieta");

            migrationBuilder.DropTable(
                name: "ItensRefeicao");

            migrationBuilder.DropTable(
                name: "Clinicas");

            migrationBuilder.DropTable(
                name: "RefeicoesPlanejadas");

            migrationBuilder.DropTable(
                name: "PerfisProfissionais");

            migrationBuilder.DropTable(
                name: "PlanosAlimentares");

            migrationBuilder.DropTable(
                name: "ModelosDieta");

            migrationBuilder.DropIndex(
                name: "IX_RegistroAlimentar_ItemRefeicaoPlanoId",
                table: "RegistroAlimentar");

            migrationBuilder.DropIndex(
                name: "IX_RegistroAlimentar_PlanoAlimentarId",
                table: "RegistroAlimentar");

            migrationBuilder.DropIndex(
                name: "IX_PerfilNutricional_MetaNutricionalAtualId",
                table: "PerfilNutricional");

            migrationBuilder.DropIndex(
                name: "IX_MetasNutricionais_PerfilNutricionalId",
                table: "MetasNutricionais");

            migrationBuilder.DropColumn(
                name: "CodigoBarras",
                table: "RegistroAlimentar");

            migrationBuilder.DropColumn(
                name: "ItemRefeicaoPlanoId",
                table: "RegistroAlimentar");

            migrationBuilder.DropColumn(
                name: "PlanoAlimentarId",
                table: "RegistroAlimentar");

            migrationBuilder.DropColumn(
                name: "AtualizadoEm",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "CircunferenciaBracoCm",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "CircunferenciaQuadrilCm",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "Fumante",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "HabilidadeCulinaria",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "HorasSonoPorNoite",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "OrcamentoMensal",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "QualidadeSono",
                table: "PerfilNutricional");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AtualizadoEm",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CEP",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CriadoEm",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FotoPerfilUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Logradouro",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Telefone",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "OcupacaoProfissional",
                table: "PerfilNutricional",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "DescricaoCondicoesMedicas",
                table: "PerfilNutricional",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "NomeCompleto",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(14)",
                oldMaxLength: 14);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilNutricional_MetaNutricionalAtualId",
                table: "PerfilNutricional",
                column: "MetaNutricionalAtualId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PerfilNutricional_MetasNutricionais_MetaNutricionalAtualId",
                table: "PerfilNutricional",
                column: "MetaNutricionalAtualId",
                principalTable: "MetasNutricionais",
                principalColumn: "Id");
        }
    }
}
