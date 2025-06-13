
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace servidor.Data.Migrations
{
    [DbContext(typeof(TiendaDbContext))]
    [Migration("20250611231012_NombreDeLaMigracion")]
    partial class NombreDeLaMigracion
    {

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.5");

            modelBuilder.Entity("Compartido.Compra", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ApellidoCliente")
                        .HasColumnType("TEXT");

                    b.Property<string>("EmailCliente")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("FechaCompra")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("FechaCreacion")
                        .HasColumnType("TEXT");

                    b.Property<string>("NombreCliente")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Total")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("Compras");
                });

            modelBuilder.Entity("Compartido.ItemCompra", b =>
                {
                    b.Property<Guid>("CompraId")
                        .HasColumnType("TEXT");

                    b.Property<int>("ProductoId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Cantidad")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("PrecioUnitario")
                        .HasColumnType("TEXT");

                    b.Property<int?>("ProductoId1")
                        .HasColumnType("INTEGER");

                    b.HasKey("CompraId", "ProductoId");

                    b.HasIndex("ProductoId");

                    b.HasIndex("ProductoId1");

                    b.ToTable("ItemsCompra");
                });

            modelBuilder.Entity("Compartido.Producto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ImagenUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Precio")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("Stock")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Productos");
                });

            modelBuilder.Entity("Compartido.ItemCompra", b =>
                {
                    b.HasOne("Compartido.Compra", "Compra")
                        .WithMany("Items")
                        .HasForeignKey("CompraId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Compartido.Producto", "Producto")
                        .WithMany()
                        .HasForeignKey("ProductoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Compartido.Producto", null)
                        .WithMany("ItemsCompra")
                        .HasForeignKey("ProductoId1");

                    b.Navigation("Compra");

                    b.Navigation("Producto");
                });

            modelBuilder.Entity("Compartido.Compra", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Compartido.Producto", b =>
                {
                    b.Navigation("ItemsCompra");
                });
#pragma warning restore 612, 618
        }
    }
}
