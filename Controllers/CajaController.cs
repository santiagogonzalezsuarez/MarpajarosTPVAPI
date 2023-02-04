using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using MarpajarosTPVAPI.Classes;
using MarpajarosTPVAPI.Business;
using MarpajarosTPVAPI.Model;

namespace MarpajarosTPVAPI.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CajaController : ControllerBase
    {

        #region Funciones

        [ActionName("getIsCajaAbierta")]
        [HttpPost]
        [APIReturn(typeof(IsCajaAbiertaResult))]
        public ActionResult IsCajaAbierta(IsCajaAbiertaRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Ventas_RealizarVentas())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                var lastCuadreCaja = bs.FacCuadreCaja.getByCajaId(request.CajaId).OrderByDescending(p => p.Id).FirstOrDefault();
                if (lastCuadreCaja == null)
                    return ResultClass.WithContent(new IsCajaAbiertaResult {
                        CajaAbierta = false
                    });

                if (lastCuadreCaja.FechaCierre != null)
                    return ResultClass.WithContent(new IsCajaAbiertaResult {
                        CajaAbierta = false
                    });

                return ResultClass.WithContent(new IsCajaAbiertaResult {
                    CajaAbierta = true
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("getUltimaAperturaCaja")]
        [HttpPost]
        [APIReturn(typeof(GetUltimaAperturaCajaResult))]
        public ActionResult GetUltimaAperturaCaja(GetUltimaAperturaCajaRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Ventas_RealizarVentas())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                var lastCuadreCaja = bs.FacCuadreCaja.getByCajaId(request.CajaId).OrderByDescending(p => p.Id).FirstOrDefault();
                if (lastCuadreCaja == null) {
                    return ResultClass.WithContent(new GetUltimaAperturaCajaResult {
                        CajaAbierta = false,
                        ImporteApertura = 0,
                        ImporteCierre = 0,
                        ImporteVendido = 0
                    });
                }
                
                bool cajaAbierta = true;

                if (lastCuadreCaja.FechaCierre != null)
                    cajaAbierta = false;

                var result = new GetUltimaAperturaCajaResult();
                result.CajaAbierta = cajaAbierta;
                result.ImporteApertura = lastCuadreCaja.ImporteApertura;
                result.ImporteCierre = lastCuadreCaja.ImporteCierre;
                if (cajaAbierta) {
                    var importeVendido = bs.FacVenta.getAll().Where(p => (p.Borrado == null || p.Borrado == false) && p.CajaId == request.CajaId && p.Fecha != null && p.Fecha > lastCuadreCaja.FechaApertura).Sum(p => p.ImporteTotal).GetValueOrDefault(0);
                    result.ImporteVendido = importeVendido;
                }
                
                return ResultClass.WithContent(result);

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("abrirCaja")]
        [HttpPost]
        [APIReturn(typeof(AbrirCajaResult))]
        public ActionResult AbrirCaja(AbrirCajaRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Ventas_RealizarVentas())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Verificamos que la caja no esté ya abierta
                var lastCuadreCaja = bs.FacCuadreCaja.getByCajaId(request.CajaId).OrderByDescending(p => p.Id).FirstOrDefault();
                if (lastCuadreCaja != null && lastCuadreCaja.FechaCierre == null) {
                    return ResultClass.WithError($"La caja ya está abierta. Fecha y hora de apertura: {lastCuadreCaja.FechaApertura?.ToShortDateString()} {lastCuadreCaja.FechaApertura?.ToShortTimeString()}");
                }

                var cuadreCaja = new FacCuadreCaja();
                cuadreCaja.CajaId = request.CajaId;
                cuadreCaja.ImporteApertura = request.ImporteInicial;
                cuadreCaja.FechaApertura = DateTime.Now;

                bs.FacCuadreCaja.insert(cuadreCaja);
                bs.save();

                return ResultClass.WithContent(new AbrirCajaResult {
                    Correcto = true
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        [ActionName("cerrarCaja")]
        [HttpPost]
        [APIReturn(typeof(CerrarCajaResult))]
        public ActionResult CerrarCaja(CerrarCajaRequest request)
        {

            try
            {

                var bs = new BS();
                if (!(bs.AdmPermiso.Ventas_RealizarVentas())) {
                    return ResultClass.NotAuthorized("Acceso denegado.");
                }

                // Verificamos que la caja no esté ya cerrada
                var lastCuadreCaja = bs.FacCuadreCaja.getByCajaId(request.CajaId).OrderByDescending(p => p.Id).FirstOrDefault();
                if (lastCuadreCaja != null && lastCuadreCaja.FechaCierre != null) {
                    return ResultClass.WithError($"La caja ya está cerrada. Fecha y hora de cierre: {lastCuadreCaja.FechaCierre?.ToShortDateString()} {lastCuadreCaja.FechaCierre?.ToShortTimeString()}");
                }

                var importeVendidoActual = bs.FacVenta.getAll().Where(p => (p.Borrado == null || p.Borrado == false) && p.CajaId == request.CajaId && p.Fecha != null && p.Fecha > lastCuadreCaja.FechaApertura).Sum(p => p.ImporteTotal).GetValueOrDefault(0);
                if (importeVendidoActual != request.ImporteVendido) {
                    return ResultClass.WithError("El importe total vendido no cuadra con el que aparece en la base de datos, es posible que se hayan realizado ventas en otro terminal mientras se estaba realizando el cierre de caja. Verifique de nuevo el cuadre.");
                }

                lastCuadreCaja.FechaCierre = DateTime.Now;
                lastCuadreCaja.ImporteCierre = request.ImporteCierre;
                lastCuadreCaja.Descuadre = request.ImporteCierre - (lastCuadreCaja.ImporteApertura - importeVendidoActual);

                bs.save();

                return ResultClass.WithContent(new CerrarCajaResult {
                    Correcto = true
                });

            }
            catch (Exception ex)
            {
                return ResultClass.WithError(ex.Message);
            }

        }

        #endregion

        #region Clases

        public class IsCajaAbiertaRequest
        {
            public int CajaId;
        }

        public class IsCajaAbiertaResult
        {
            public bool CajaAbierta;
        }

        public class GetUltimaAperturaCajaRequest
        {
            public int CajaId;
        }

        public class GetUltimaAperturaCajaResult
        {
            public bool CajaAbierta;
            public decimal? ImporteApertura;
            public decimal? ImporteCierre;
            public decimal? ImporteVendido;
        }

        public class AbrirCajaRequest
        {
            public int CajaId;
            public decimal ImporteInicial;
        }

        public class AbrirCajaResult
        {
            public bool Correcto;
        }

        public class CerrarCajaRequest
        {
            public int CajaId;
            public decimal ImporteVendido;
            public decimal ImporteCierre;
        }

        public class CerrarCajaResult
        {
            public bool Correcto;
        }

        #endregion

    }
}