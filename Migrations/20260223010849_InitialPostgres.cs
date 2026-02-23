using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Nutra.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NomeCompleto = table.Column<string>(type: "text", nullable: false),
                    CPF = table.Column<string>(type: "text", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fabricantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fabricante = table.Column<string>(type: "text", nullable: true),
                    Produto = table.Column<string>(type: "text", nullable: true),
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
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistroAlimentar",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
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
                    DadosNutricionaisCompletosJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistroAlimentar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistroAlimentar_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PerfilNutricional",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    MetaNutricionalAtualId = table.Column<int>(type: "integer", nullable: true),
                    AlturaCm = table.Column<double>(type: "double precision", nullable: false),
                    PesoAtualKg = table.Column<double>(type: "double precision", nullable: false),
                    PercentualGorduraCorporal = table.Column<double>(type: "double precision", nullable: true),
                    FatorAtividade = table.Column<double>(type: "double precision", nullable: false),
                    OcupacaoProfissional = table.Column<string>(type: "text", nullable: false),
                    PossuiDoencasPreExistentes = table.Column<bool>(type: "boolean", nullable: false),
                    DescricaoCondicoesMedicas = table.Column<string>(type: "text", nullable: false),
                    PesoDesejadoKg = table.Column<double>(type: "double precision", nullable: true),
                    RefeicoesPorDiaDesejadas = table.Column<int>(type: "integer", nullable: false),
                    TempoDisponivelPreparoMinutos = table.Column<int>(type: "integer", nullable: false),
                    CircunferenciaCinturaCm = table.Column<double>(type: "double precision", nullable: true),
                    DataNascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Genero = table.Column<int>(type: "integer", nullable: false),
                    Objetivo = table.Column<int>(type: "integer", nullable: false),
                    NivelAtividade = table.Column<int>(type: "integer", nullable: false),
                    PreferenciaDieta = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfilNutricional", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PerfilNutricional_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PerfilNutricional_MetasNutricionais_MetaNutricionalAtualId",
                        column: x => x.MetaNutricionalAtualId,
                        principalTable: "MetasNutricionais",
                        principalColumn: "Id");
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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilNutricional_MetaNutricionalAtualId",
                table: "PerfilNutricional",
                column: "MetaNutricionalAtualId",
                unique: true);

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
                name: "IX_PreferenciaAlimentar_PerfilNutricionalId",
                table: "PreferenciaAlimentar",
                column: "PerfilNutricionalId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Fabricantes");

            migrationBuilder.DropTable(
                name: "FastFoods");

            migrationBuilder.DropTable(
                name: "Genericos");

            migrationBuilder.DropTable(
                name: "PerfisEquipamentos");

            migrationBuilder.DropTable(
                name: "PreferenciaAlimentar");

            migrationBuilder.DropTable(
                name: "RegistroAlimentar");

            migrationBuilder.DropTable(
                name: "RegistroBiometrico");

            migrationBuilder.DropTable(
                name: "RestricaoAlimentar");

            migrationBuilder.DropTable(
                name: "Tbcas");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PerfilNutricional");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MetasNutricionais");
        }
    }
}
