using Microsoft.EntityFrameworkCore;
using PuntoVentaBin.Shared.Identidades;
using PuntoVentaBin.Shared.Identidades.Adm_PerfilTareas;
using PuntoVentaBin.Shared.Identidades.Pedidos;
using PuntoVentaBin.Shared.Identidades.Productos;
using PuntoVentaBin.Shared.Identidades.Catalogos;
using PuntoVentaBin.Shared.LogServices;
using PuntoVentaBin.Shared.Identidades.DTOs;

namespace PuntoVentaBin.Shared.AccesoDatos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Negocio> Negocios { get; set; }
        public DbSet<Usuario> UsuariosBin { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Provedor> Provedores { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
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


        #region Productos
        public DbSet<Producto> Productos { get; set; }
        public DbSet<ProductoPaquete> ProductoPaquetes { get; set; }
        public DbSet<Categoria> ProductoCategorias { get; set; }
        #endregion


        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolesPermisos { get; set; }
        public DbSet<UsuarioRolNegocio> UsuariosRolesNegocios { get; set; }

        public DbSet<Log> TablaLogs { get; set; }
        public DbSet<ErrorLog> TablaErroresLogs { get; set; }
        public DbSet<UsuarioConNegocioPorAsignar> UsuariosConNegociosPorAsignar { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la clave primaria compuesta para la tabla intermedia
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasKey(un => new { un.UsuarioId, un.NegocioId, un.RolId });

            // Configuración de la relación con Usuario
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasOne(un => un.Usuario)
                .WithMany(u => u.UsuariosRolesNegocios)
                .HasForeignKey(un => un.UsuarioId);

            // Configuración de la relación con Negocio
            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasOne(un => un.Negocio)
                .WithMany(n => n.UsuariosRolesNegocios)
                .HasForeignKey(un => un.NegocioId);

            modelBuilder.Entity<UsuarioRolNegocio>()
                .HasOne(un => un.Rol)
                .WithMany(n => n.UsuariosRolesNegocios)
                .HasForeignKey(un => un.RolId);



            modelBuilder.Entity<UsuarioConNegocioPorAsignar>()
                .HasKey(un => new { un.Email, un.NegocioId, un.RolId });


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
