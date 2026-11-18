using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VeterinariaMVC.Data;

#nullable disable

namespace VeterinariaMVC.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261118061134_Inicial")]
    partial class Inicial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("VeterinariaMVC.Models.Especie", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Nombre")
                        .IsUnique();

                    b.ToTable("Especies");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Examen", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ArchivoRuta")
                        .IsRequired()
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<int>("AutorId")
                        .HasColumnType("int");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTime>("FechaRegistro")
                        .HasColumnType("datetime2");

                    b.Property<int>("MascotaId")
                        .HasColumnType("int");

                    b.Property<string>("NombreOriginal")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("TipoContenido")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AutorId");

                    b.HasIndex("MascotaId");

                    b.ToTable("Examenes");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.ItemCatalogo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Tipo")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Catalogo");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Mascota", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DuenoId")
                        .HasColumnType("int");

                    b.Property<int>("EspecieId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaNacimiento")
                        .HasColumnType("datetime2");

                    b.Property<string>("FotoRuta")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(80)
                        .HasColumnType("nvarchar(80)");

                    b.Property<int>("RazaId")
                        .HasColumnType("int");

                    b.Property<int>("Sexo")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UltimaAtencion")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DuenoId");

                    b.HasIndex("EspecieId");

                    b.HasIndex("RazaId");

                    b.ToTable("Mascotas");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.NotaHistorial", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AutorId")
                        .HasColumnType("int");

                    b.Property<string>("ContenidoHtml")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("FechaHora")
                        .HasColumnType("datetime2");

                    b.Property<int>("MascotaId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AutorId");

                    b.HasIndex("MascotaId");

                    b.ToTable("Notas");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Raza", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EspecieId")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("EspecieId", "Nombre")
                        .IsUnique();

                    b.ToTable("Razas");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.TratamientoAsignado", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AutorId")
                        .HasColumnType("int");

                    b.Property<int?>("AutorSuspensionId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaInicio")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaSuspension")
                        .HasColumnType("datetime2");

                    b.Property<string>("Indicaciones")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int>("ItemCatalogoId")
                        .HasColumnType("int");

                    b.Property<int>("MascotaId")
                        .HasColumnType("int");

                    b.Property<bool>("Suspendido")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("AutorId");

                    b.HasIndex("AutorSuspensionId");

                    b.HasIndex("ItemCatalogoId");

                    b.HasIndex("MascotaId");

                    b.ToTable("Tratamientos");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Bloqueado")
                        .HasColumnType("bit");

                    b.Property<string>("Cedula")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("ContrasenaHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Correo")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("Perfil")
                        .HasColumnType("int");

                    b.Property<int?>("VeterinarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Cedula")
                        .IsUnique();

                    b.HasIndex("Correo")
                        .IsUnique();

                    b.HasIndex("VeterinarioId");

                    b.ToTable("Usuarios");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Vacuna", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AutorId")
                        .HasColumnType("int");

                    b.Property<DateTime>("FechaAplicacion")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaProximaDosis")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("FechaRegistro")
                        .HasColumnType("datetime2");

                    b.Property<int>("MascotaId")
                        .HasColumnType("int");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AutorId");

                    b.HasIndex("MascotaId");

                    b.ToTable("Vacunas");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Veterinario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FotoRuta")
                        .HasMaxLength(300)
                        .HasColumnType("nvarchar(300)");

                    b.Property<string>("NombreCompleto")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("NumeroColegiado")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("Id");

                    b.HasIndex("NumeroColegiado")
                        .IsUnique();

                    b.ToTable("Veterinarios");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Examen", b =>
                {
                    b.HasOne("VeterinariaMVC.Models.Usuario", "Autor")
                        .WithMany()
                        .HasForeignKey("AutorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VeterinariaMVC.Models.Mascota", "Mascota")
                        .WithMany("Examenes")
                        .HasForeignKey("MascotaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Autor");

                    b.Navigation("Mascota");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Mascota", b =>
                {
                    b.HasOne("VeterinariaMVC.Models.Usuario", "Dueno")
                        .WithMany("Mascotas")
                        .HasForeignKey("DuenoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VeterinariaMVC.Models.Especie", "Especie")
                        .WithMany("Mascotas")
                        .HasForeignKey("EspecieId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VeterinariaMVC.Models.Raza", "Raza")
                        .WithMany("Mascotas")
                        .HasForeignKey("RazaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Dueno");

                    b.Navigation("Especie");

                    b.Navigation("Raza");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.NotaHistorial", b =>
                {
                    b.HasOne("VeterinariaMVC.Models.Usuario", "Autor")
                        .WithMany()
                        .HasForeignKey("AutorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VeterinariaMVC.Models.Mascota", "Mascota")
                        .WithMany("Notas")
                        .HasForeignKey("MascotaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Autor");

                    b.Navigation("Mascota");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Raza", b =>
                {
                    b.HasOne("VeterinariaMVC.Models.Especie", "Especie")
                        .WithMany("Razas")
                        .HasForeignKey("EspecieId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Especie");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.TratamientoAsignado", b =>
                {
                    b.HasOne("VeterinariaMVC.Models.Usuario", "Autor")
                        .WithMany()
                        .HasForeignKey("AutorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VeterinariaMVC.Models.Usuario", "AutorSuspension")
                        .WithMany()
                        .HasForeignKey("AutorSuspensionId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("VeterinariaMVC.Models.ItemCatalogo", "ItemCatalogo")
                        .WithMany("Asignaciones")
                        .HasForeignKey("ItemCatalogoId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VeterinariaMVC.Models.Mascota", "Mascota")
                        .WithMany("Tratamientos")
                        .HasForeignKey("MascotaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Autor");

                    b.Navigation("AutorSuspension");

                    b.Navigation("ItemCatalogo");

                    b.Navigation("Mascota");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Usuario", b =>
                {
                    b.HasOne("VeterinariaMVC.Models.Veterinario", "Veterinario")
                        .WithMany()
                        .HasForeignKey("VeterinarioId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Veterinario");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Vacuna", b =>
                {
                    b.HasOne("VeterinariaMVC.Models.Usuario", "Autor")
                        .WithMany()
                        .HasForeignKey("AutorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("VeterinariaMVC.Models.Mascota", "Mascota")
                        .WithMany("Vacunas")
                        .HasForeignKey("MascotaId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Autor");

                    b.Navigation("Mascota");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Especie", b =>
                {
                    b.Navigation("Mascotas");

                    b.Navigation("Razas");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.ItemCatalogo", b =>
                {
                    b.Navigation("Asignaciones");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Mascota", b =>
                {
                    b.Navigation("Examenes");

                    b.Navigation("Notas");

                    b.Navigation("Tratamientos");

                    b.Navigation("Vacunas");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Raza", b =>
                {
                    b.Navigation("Mascotas");
                });

            modelBuilder.Entity("VeterinariaMVC.Models.Usuario", b =>
                {
                    b.Navigation("Mascotas");
                });
#pragma warning restore 612, 618
        }
    }
}
