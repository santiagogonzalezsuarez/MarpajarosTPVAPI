using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MarpajarosTPVAPI.Model;

#nullable disable

namespace MarpajarosTPVAPI.Context
{
    public partial class MarpajarosContext : DbContext
    {
        public MarpajarosContext()
        {
        }

        public MarpajarosContext(DbContextOptions<MarpajarosContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AdmModulo> AdmModulos { get; set; }
        public virtual DbSet<AdmPermiso> AdmPermisos { get; set; }
        public virtual DbSet<AdmRole> AdmRoles { get; set; }
        public virtual DbSet<AdmRolesPermiso> AdmRolesPermisos { get; set; }
        public virtual DbSet<AdmUsuario> AdmUsuarios { get; set; }
        public virtual DbSet<AdmUsuariosAcceso> AdmUsuariosAccesos { get; set; }
        public virtual DbSet<AppConfiguracion> AppConfiguracions { get; set; }
        public virtual DbSet<FacCompra> FacCompras { get; set; }
        public virtual DbSet<FacComprasArticulo> FacComprasArticulos { get; set; }
        public virtual DbSet<FacComprasConcepto> FacComprasConceptos { get; set; }
        public virtual DbSet<FacConcepto> FacConceptos { get; set; }
        public virtual DbSet<FacCuadreCaja> FacCuadreCajas { get; set; }
        public virtual DbSet<FacVenta> FacVentas { get; set; }
        public virtual DbSet<FacVentasArticulo> FacVentasArticulos { get; set; }
        public virtual DbSet<FacVentasConcepto> FacVentasConceptos { get; set; }
        public virtual DbSet<TpvAperturasCajon> TpvAperturasCajons { get; set; }
        public virtual DbSet<TpvArticulo> TpvArticulos { get; set; }
        public virtual DbSet<TpvArticulosAnimale> TpvArticulosAnimales { get; set; }
        public virtual DbSet<TpvArticulosProducto> TpvArticulosProductos { get; set; }
        public virtual DbSet<TpvArticulosProveedore> TpvArticulosProveedores { get; set; }
        public virtual DbSet<TpvCategoria> TpvCategorias { get; set; }
        public virtual DbSet<TpvProveedore> TpvProveedores { get; set; }
        public virtual DbSet<TpvStock> TpvStocks { get; set; }
        public virtual DbSet<TpvTiposArticulo> TpvTiposArticulos { get; set; }
        public virtual DbSet<TpvVistaProducto> TpvVistaProductos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=[Database IP];Initial Catalog=[Database name];persist security info=True;User ID=[Database user];Password=[Database password];TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AdmModulo>(entity =>
            {
                entity.ToTable("ADM_Modulos");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Modulo).HasMaxLength(500);
            });

            modelBuilder.Entity<AdmPermiso>(entity =>
            {
                entity.ToTable("ADM_Permisos");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Permiso).HasMaxLength(500);

                entity.HasOne(d => d.Modulo)
                    .WithMany(p => p.AdmPermisos)
                    .HasForeignKey(d => d.ModuloId)
                    .HasConstraintName("FK__ADM_Permi__Modul__534D60F1");
            });

            modelBuilder.Entity<AdmRole>(entity =>
            {
                entity.ToTable("ADM_Roles");

                entity.Property(e => e.Rol).HasMaxLength(500);
            });

            modelBuilder.Entity<AdmRolesPermiso>(entity =>
            {
                entity.HasKey(e => new { e.PermisoId, e.RolId });

                entity.ToTable("ADM_Roles_Permisos");

                entity.HasOne(d => d.Permiso)
                    .WithMany(p => p.AdmRolesPermisos)
                    .HasForeignKey(d => d.PermisoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ADM_Roles__Permi__5441852A");

                entity.HasOne(d => d.Rol)
                    .WithMany(p => p.AdmRolesPermisos)
                    .HasForeignKey(d => d.RolId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ADM_Roles__RolId__0F975522");
            });

            modelBuilder.Entity<AdmUsuario>(entity =>
            {
                entity.ToTable("ADM_Usuarios");

                entity.Property(e => e.Apellidos).HasMaxLength(500);

                entity.Property(e => e.Nombre).HasMaxLength(500);

                entity.Property(e => e.Password).HasMaxLength(500);

                entity.Property(e => e.Username).HasMaxLength(500);

                entity.HasOne(d => d.Rol)
                    .WithMany(p => p.AdmUsuarios)
                    .HasForeignKey(d => d.RolId)
                    .HasConstraintName("FK__ADM_Usuar__RolId__108B795B");
            });

            modelBuilder.Entity<AdmUsuariosAcceso>(entity =>
            {
                entity.ToTable("ADM_Usuarios_Accesos");

                entity.Property(e => e.AdmUsuariosId).HasColumnName("ADM_Usuarios_Id");

                entity.Property(e => e.CreateDate).HasColumnType("datetime");

                entity.Property(e => e.Ipaddress).HasColumnName("IPAddress");

                entity.Property(e => e.Token).HasMaxLength(500);

                entity.HasOne(d => d.AdmUsuarios)
                    .WithMany(p => p.AdmUsuariosAccesos)
                    .HasForeignKey(d => d.AdmUsuariosId)
                    .HasConstraintName("FK_ADM_Usuarios_Accesos_ADM_Usuarios");
            });

            modelBuilder.Entity<AppConfiguracion>(entity =>
            {
                entity.ToTable("APP_Configuracion");

                entity.Property(e => e.Iva)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("IVA");
            });

            modelBuilder.Entity<FacCompra>(entity =>
            {
                entity.ToTable("FAC_Compras");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.ImporteTotal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.NumeroFactura).HasMaxLength(500);

                entity.HasOne(d => d.Proveedor)
                    .WithMany(p => p.FacCompras)
                    .HasForeignKey(d => d.ProveedorId)
                    .HasConstraintName("FK__FAC_Compr__Prove__571DF1D5");
            });

            modelBuilder.Entity<FacComprasArticulo>(entity =>
            {
                entity.ToTable("FAC_Compras_Articulos");

                entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Articulo)
                    .WithMany(p => p.FacComprasArticulos)
                    .HasForeignKey(d => d.ArticuloId)
                    .HasConstraintName("FK__FAC_Compr__Artic__4AB81AF0");

                entity.HasOne(d => d.Compra)
                    .WithMany(p => p.FacComprasArticulos)
                    .HasForeignKey(d => d.CompraId)
                    .HasConstraintName("FK__FAC_Compr__Compr__59063A47");
            });

            modelBuilder.Entity<FacComprasConcepto>(entity =>
            {
                entity.HasKey(e => new { e.ComprasId, e.ConceptoId });

                entity.ToTable("FAC_Compras_Conceptos");

                entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Compras)
                    .WithMany(p => p.FacComprasConceptos)
                    .HasForeignKey(d => d.ComprasId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FAC_Compr__Compr__59FA5E80");

                entity.HasOne(d => d.Concepto)
                    .WithMany(p => p.FacComprasConceptos)
                    .HasForeignKey(d => d.ConceptoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FAC_Compr__Conce__5AEE82B9");
            });

            modelBuilder.Entity<FacConcepto>(entity =>
            {
                entity.ToTable("FAC_Conceptos");

                entity.Property(e => e.Concepto).HasMaxLength(500);
            });

            modelBuilder.Entity<FacCuadreCaja>(entity =>
            {
                entity.ToTable("FAC_CuadreCaja");

                entity.Property(e => e.Descuadre).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.FechaApertura).HasColumnType("datetime");

                entity.Property(e => e.FechaCierre).HasColumnType("datetime");

                entity.Property(e => e.ImporteApertura).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ImporteCierre).HasColumnType("decimal(18, 2)");
            });

            modelBuilder.Entity<FacVenta>(entity =>
            {
                entity.ToTable("FAC_Ventas");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.ImporteTotal).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Nticket).HasColumnName("NTicket");
            });

            modelBuilder.Entity<FacVentasArticulo>(entity =>
            {
                entity.ToTable("FAC_Ventas_Articulos");

                entity.Property(e => e.Cantidad).HasColumnType("decimal(18, 3)");

                entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Articulo)
                    .WithMany(p => p.FacVentasArticulos)
                    .HasForeignKey(d => d.ArticuloId)
                    .HasConstraintName("FK__FAC_Venta__Artic__571DF1D5");

                entity.HasOne(d => d.Venta)
                    .WithMany(p => p.FacVentasArticulos)
                    .HasForeignKey(d => d.VentaId)
                    .HasConstraintName("FK__FAC_Venta__Venta__5629CD9C");
            });

            modelBuilder.Entity<FacVentasConcepto>(entity =>
            {
                entity.HasKey(e => new { e.VentaId, e.ConceptoId });

                entity.ToTable("FAC_Ventas_Conceptos");

                entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Concepto)
                    .WithMany(p => p.FacVentasConceptos)
                    .HasForeignKey(d => d.ConceptoId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FAC_Venta__Conce__5DCAEF64");

                entity.HasOne(d => d.Venta)
                    .WithMany(p => p.FacVentasConceptos)
                    .HasForeignKey(d => d.VentaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__FAC_Venta__Venta__5812160E");
            });

            modelBuilder.Entity<TpvAperturasCajon>(entity =>
            {
                entity.ToTable("TPV_AperturasCajon");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.HasOne(d => d.Usuario)
                    .WithMany(p => p.TpvAperturasCajons)
                    .HasForeignKey(d => d.UsuarioId)
                    .HasConstraintName("FK__TPV_Apert__Usuar__5FB337D6");
            });

            modelBuilder.Entity<TpvArticulo>(entity =>
            {
                entity.ToTable("TPV_Articulos");

                entity.Property(e => e.Imagen).HasMaxLength(500);

                entity.Property(e => e.PrecioVenta).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Tipo)
                    .WithMany(p => p.TpvArticulos)
                    .HasForeignKey(d => d.TipoId)
                    .HasConstraintName("FK__TPV_Produ__TipoI__267ABA7A");
            });

            modelBuilder.Entity<TpvArticulosAnimale>(entity =>
            {
                entity.HasKey(e => e.ArticuloId);

                entity.ToTable("TPV_Articulos_Animales");

                entity.Property(e => e.ArticuloId).ValueGeneratedNever();

                entity.Property(e => e.ControlVeterinario).HasColumnType("text");

                entity.Property(e => e.Especie).HasMaxLength(500);

                entity.Property(e => e.FechaAdquisicion).HasColumnType("datetime");

                entity.Property(e => e.Nidentificacion)
                    .HasMaxLength(500)
                    .HasColumnName("NIdentificacion");

                entity.Property(e => e.Procedencia).HasMaxLength(500);

                entity.Property(e => e.ReferenciaNombre).HasMaxLength(500);

                entity.Property(e => e.Variedad).HasMaxLength(500);

                entity.HasOne(d => d.Articulo)
                    .WithOne(p => p.TpvArticulosAnimale)
                    .HasForeignKey<TpvArticulosAnimale>(d => d.ArticuloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TPV_Artic__Artic__398D8EEE");
            });

            modelBuilder.Entity<TpvArticulosProducto>(entity =>
            {
                entity.HasKey(e => e.ArticuloId);

                entity.ToTable("TPV_Articulos_Productos");

                entity.Property(e => e.ArticuloId).ValueGeneratedNever();

                entity.Property(e => e.Animales).HasMaxLength(500);

                entity.Property(e => e.CodigoBarras).HasMaxLength(50);

                entity.Property(e => e.Iva)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("IVA");

                entity.Property(e => e.Marca).HasMaxLength(500);

                entity.Property(e => e.Producto).HasMaxLength(500);

                entity.HasOne(d => d.Articulo)
                    .WithOne(p => p.TpvArticulosProducto)
                    .HasForeignKey<TpvArticulosProducto>(d => d.ArticuloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TPV_Artic__Artic__3A81B327");

                entity.HasOne(d => d.Categoria)
                    .WithMany(p => p.TpvArticulosProductos)
                    .HasForeignKey(d => d.CategoriaId)
                    .HasConstraintName("FK__TPV_Artic__Categ__3B75D760");
            });

            modelBuilder.Entity<TpvArticulosProveedore>(entity =>
            {
                entity.HasKey(e => new { e.ArticuloId, e.ProveedorId })
                    .HasName("PK_TPV_Productos_Proveedores");

                entity.ToTable("TPV_Articulos_Proveedores");

                entity.Property(e => e.Observaciones).HasColumnType("text");

                entity.Property(e => e.PorcentajeGanancia).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PrecioCompra).HasColumnType("decimal(18, 12)");

                entity.Property(e => e.Referencia).HasMaxLength(50);

                entity.HasOne(d => d.Articulo)
                    .WithMany(p => p.TpvArticulosProveedores)
                    .HasForeignKey(d => d.ArticuloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TPV_Produ__Produ__2C3393D0");

                entity.HasOne(d => d.Proveedor)
                    .WithMany(p => p.TpvArticulosProveedores)
                    .HasForeignKey(d => d.ProveedorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__TPV_Produ__Prove__2D27B809");
            });

            modelBuilder.Entity<TpvCategoria>(entity =>
            {
                entity.ToTable("TPV_Categorias");

                entity.Property(e => e.Categoria).HasMaxLength(500);

                entity.HasOne(d => d.CategoriaPadre)
                    .WithMany(p => p.InverseCategoriaPadre)
                    .HasForeignKey(d => d.CategoriaPadreId)
                    .HasConstraintName("FK__TPV_Categ__Categ__1BFD2C07");
            });

            modelBuilder.Entity<TpvProveedore>(entity =>
            {
                entity.ToTable("TPV_Proveedores");

                entity.Property(e => e.Direccion).HasColumnType("text");

                entity.Property(e => e.Email).HasMaxLength(500);

                entity.Property(e => e.Horarios).HasColumnType("text");

                entity.Property(e => e.Nombre).HasMaxLength(500);

                entity.Property(e => e.Observaciones).HasColumnType("text");

                entity.Property(e => e.PaginaWeb).HasMaxLength(500);

                entity.Property(e => e.Telefono).HasMaxLength(50);
            });

            modelBuilder.Entity<TpvStock>(entity =>
            {
                entity.ToTable("TPV_Stock");

                entity.Property(e => e.Fecha).HasColumnType("datetime");

                entity.Property(e => e.StockAnterior).HasColumnType("decimal(18, 3)");

                entity.HasOne(d => d.Articulo)
                    .WithMany(p => p.TpvStocks)
                    .HasForeignKey(d => d.ArticuloId)
                    .HasConstraintName("FK__TPV_Stock__Artic__2B0A656D");
            });

            modelBuilder.Entity<TpvTiposArticulo>(entity =>
            {
                entity.ToTable("TPV_TiposArticulo");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Tipo).HasMaxLength(500);
            });

            modelBuilder.Entity<TpvVistaProducto>(entity =>
            {
                entity.HasNoKey();

                entity.ToView("TPV_VistaProductos");

                entity.Property(e => e.Categorias).HasMaxLength(4000);

                entity.Property(e => e.CodigoBarras).HasMaxLength(50);

                entity.Property(e => e.Marca).HasMaxLength(500);

                entity.Property(e => e.PrecioVenta).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Producto).HasMaxLength(500);

                entity.Property(e => e.Proveedor).HasMaxLength(4000);

                entity.Property(e => e.ProveedoresIds).HasMaxLength(4000);

                entity.Property(e => e.Referencias).HasMaxLength(4000);

                entity.Property(e => e.Stock).HasColumnType("decimal(38, 3)");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
