using System;
using System.Linq;
using BodegaLaEsquinita.Helpers;
using BodegaLaEsquinita.Models;

namespace BodegaLaEsquinita.Services
{
    public static class SesionService
    {
        // =========================================================
        // SESIÓN ACTUAL
        // =========================================================

        public static Empleado EmpleadoActual { get; private set; }

        // Momento en que el usuario actual inició sesión (inicio de su turno).
        public static DateTime InicioTurno { get; private set; }

        public static bool SesionActiva
        {
            get
            {
                return EmpleadoActual != null;
            }
        }

        // =========================================================
        // INICIALIZACIÓN
        // =========================================================

        public static void Inicializar()
        {
            ArchivoHelper.InicializarCarpetaData();
            ArchivoHelper.CrearEmpleadosIniciales();
        }

        // =========================================================
        // INICIAR SESIÓN
        // =========================================================

        public static bool IniciarSesion(
            string usuario,
            string contraseña)
        {
            Inicializar();

            if (string.IsNullOrWhiteSpace(usuario) ||
                string.IsNullOrWhiteSpace(contraseña))
            {
                return false;
            }

            var empleados = ArchivoHelper.LeerEmpleados();

            Empleado empleado = empleados.FirstOrDefault(e =>
                e.Usuario.Equals(
                    usuario.Trim(),
                    StringComparison.Ordinal)
                &&
                e.Contraseña == contraseña);

            if (empleado == null)
            {
                EmpleadoActual = null;
                return false;
            }

            EmpleadoActual = empleado;
            InicioTurno = DateTime.Now;

            return true;
        }

        // =========================================================
        // CERRAR SESIÓN
        // =========================================================

        public static void CerrarSesion()
        {
            EmpleadoActual = null;
            InicioTurno = DateTime.MinValue;
        }

        // =========================================================
        // INFORMACIÓN DEL USUARIO ACTUAL
        // =========================================================

        public static Empleado ObtenerEmpleadoActual()
        {
            return EmpleadoActual;
        }

        public static string ObtenerNombreEmpleadoActual()
        {
            if (!SesionActiva)
                return string.Empty;

            return EmpleadoActual.NombreCompleto;
        }

        public static string ObtenerCargoEmpleadoActual()
        {
            if (!SesionActiva)
                return string.Empty;

            return EmpleadoActual.Cargo;
        }

        public static string ObtenerUsuarioActual()
        {
            if (!SesionActiva)
                return string.Empty;

            return EmpleadoActual.Usuario;
        }

        // =========================================================
        // IDENTIFICACIÓN DE PERFIL
        // =========================================================
        // El sistema tiene solo dos roles:
        //   Administrador: acceso total (propietaria).
        //   Vendedor     : vende, consulta stock, envía solicitudes y
        //                  genera el reporte de su turno (ayudante).

        public const string RolAdministrador = "Administrador";
        public const string RolVendedor = "Vendedor";

        public static bool EsAdministrador()
        {
            return SesionActiva &&
                   EmpleadoActual.Cargo.Equals(
                       RolAdministrador,
                       StringComparison.OrdinalIgnoreCase);
        }

        public static bool EsVendedor()
        {
            return SesionActiva &&
                   EmpleadoActual.Cargo.Equals(
                       RolVendedor,
                       StringComparison.OrdinalIgnoreCase);
        }

        // =========================================================
        // PERMISOS GENERALES
        // =========================================================

        public static bool TieneAccesoTotal()
        {
            return EsAdministrador();
        }

        // =========================================================
        // PERMISOS POR FUNCIÓN
        // =========================================================

        // Usuarios: solo el Administrador.
        public static bool PuedeGestionarUsuarios()
        {
            return EsAdministrador();
        }

        public static bool PuedeVerUsuarios()
        {
            return EsAdministrador();
        }

        // Solicitudes: el Vendedor las crea y el Administrador las revisa.
        public static bool PuedeCrearSolicitud()
        {
            return EsVendedor();
        }

        public static bool PuedeAutorizarSolicitudes()
        {
            return EsAdministrador();
        }

        // Productos y precios: solo el Administrador.
        public static bool PuedeGestionarProductos()
        {
            return EsAdministrador();
        }

        // Inventario: ambos consultan; el Administrador opera directamente
        // y el Vendedor envía solicitudes.
        public static bool PuedeConsultarInventario()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeRegistrarIngreso()
        {
            return EsAdministrador();
        }

        public static bool PuedeRegistrarSalida()
        {
            return EsAdministrador();
        }

        public static bool PuedeActualizarExistencias()
        {
            return EsAdministrador();
        }

        public static bool PuedeGestionarDiferencias()
        {
            return EsAdministrador();
        }

        public static bool PuedeConsultarMovimientos()
        {
            return EsAdministrador();
        }

        // Ventas y atención al cliente: ambos roles.
        public static bool PuedeGestionarClientes()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeGestionarProformas()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeConvertirProforma()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeGestionarVentas()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeConfirmarVenta()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeAnularVenta()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeGenerarComprobante()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        public static bool PuedeGestionarEntregas()
        {
            return EsAdministrador() ||
                   EsVendedor();
        }

        // Reportes: el Administrador genera el reporte general y el
        // Vendedor solo el reporte de su turno (desde que inició sesión).
        public static bool PuedeGenerarReportes()
        {
            return EsAdministrador();
        }

        public static bool PuedeGenerarReporteTurno()
        {
            return EsVendedor();
        }

        // Historial de operaciones: solo el Administrador.
        public static bool PuedeConsultarHistorial()
        {
            return EsAdministrador();
        }
    }
}
