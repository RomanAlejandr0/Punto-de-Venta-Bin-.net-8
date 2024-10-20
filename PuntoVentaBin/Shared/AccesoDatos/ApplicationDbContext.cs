using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas;
using PuntoVentaBin.Shared.Identidades.Pedidos;
using PuntoVentaBin.Shared.Identidades.Productos;
using PuntoVentaBin.Shared.Identidades.Catalogos;

namespace PuntoVentaBin.Shared.AccesoDatos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Negocio> Negocios { get; set; }
        public DbSet<UsuarioBin> UsuariosBin { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Provedor> Provedores { get; set; }
        public DbSet<Domicilio> Domicilios { get; set; }



        public DbSet<CodigosPostales> Catalogo_CodigosPostales { get; set; }
        public DbSet<Localidades> Catalogo_Localidades { get; set; }
        public DbSet<Municipios> Catalogo_Municipios { get; set; }
        public DbSet<Estados> Catalogo_Estados { get; set; }
        public DbSet<Colonias> Catalogo_Colonias { get; set; }



        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }
        public DbSet<PedidoExtension> PedidosExtensiones { get; set; }

        public DbSet<Movimiento> Movimientos { get; set; }
        public DbSet<UsuarioRolNegocio> UsuariosRolesNegocios { get; set; }


        #region Productos
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoPaquete> ProductoPaquetes { get; set; }
        public DbSet<Categoria> ProductoCategorias { get; set; }
        #endregion


        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolesPermisos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Configuración de la clave primaria compuesta
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasKey(urn => new { urn.UsuarioId, urn.RolId, urn.NegocioId });

            // Relación con Usuario
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasOne(urn => urn.Usuario)
                .WithMany(u => u.UsuariosRolesNegocios)
                .HasForeignKey(urn => urn.UsuarioId);

            // Relación con Rol
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasOne(urn => urn.Rol)
                .WithMany(r => r.UsuariosRolesNegocios)
                .HasForeignKey(urn => urn.RolId);

            // Relación con Negocio
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasOne(urn => urn.Negocio)
                .WithMany(n => n.UsuariosRolesNegocios)
                .HasForeignKey(urn => urn.NegocioId);

            // Índice único para evitar duplicados de UsuarioId y RolId en un Negocio
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasIndex(urn => new { urn.UsuarioId, urn.RolId })
                .IsUnique();


            modelBuilder.Entity<Cliente>().Ignore(x => x.Ventas);

            modelBuilder.Entity<Venta>().Ignore(x => x.NombreCliente);
            modelBuilder.Entity<Venta>().Ignore(x => x.EsPedido);
            modelBuilder.Entity<Venta>().Ignore(x => x.PedidoExtension);

            modelBuilder.Entity<Venta>()
                .HasMany(x => x.VentaDetalles)
                .WithOne(x => x.Venta)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CodigosPostales>().Ignore(u => u.Estado);
            modelBuilder.Entity<CodigosPostales>().Ignore(u => u.Localidad);
            modelBuilder.Entity<CodigosPostales>().Ignore(u => u.Municipio);
            modelBuilder.Entity<CodigosPostales>().Ignore(u => u.Colonias);
        }
    }
}
