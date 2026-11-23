using System;
using System.Collections.Generic;
using System.Linq;
using BodegaLaEsquinita.Helpers;
using BodegaLaEsquinita.Models;

namespace BodegaLaEsquinita.Services
{
    public static class ActividadService
    {
        // Guarda una acción del usuario que tiene la sesión activa.
        public static void Registrar(string accion, string detalle)
        {
            if (!SesionService.SesionActiva)
                return;

            ArchivoHelper.GuardarActividad(new Actividad
            {
                Fecha = DateTime.Now,
                Usuario = SesionService.ObtenerUsuarioActual(),
                Accion = accion,
                Detalle = detalle ?? string.Empty
            });
        }

        // Acciones del usuario actual desde que inició sesión.
        public static List<Actividad> ObtenerDelTurno()
        {
            string usuario = SesionService.ObtenerUsuarioActual();

            return ArchivoHelper.LeerActividades()
                .Where(a => a.Usuario.Equals(
                                usuario,
                                StringComparison.OrdinalIgnoreCase) &&
                            a.Fecha >= SesionService.InicioTurno)
                .OrderBy(a => a.Fecha)
                .ToList();
        }
    }
}
