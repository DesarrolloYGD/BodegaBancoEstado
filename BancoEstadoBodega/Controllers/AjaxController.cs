using BancoEstadoBodega.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace BancoEstadoBodega.Controllers
{
    public class AjaxController : Controller
    {

        private pruebatotalNewEntities db = new pruebatotalNewEntities();

        [Authorize]
        [ActionName("ComunasPorCiudades")]
        public ActionResult GetComunasPorCiudades(int id)
        {
            var paises = db.COMUNA.Where(r => r.IdCiudadFK == id);
            
            var lista = paises.Select(p => new { Nombre = p.Nombre, Id = p.IdComuna });
            return this.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ActionName("SucursalesPorComunas")]
        public ActionResult GetSucursalesPorComunas(int id)
        {
            var paises = db.Sucursales.Where(r => r.id_Comuna == id);
            var lista = paises.Select(p => new { Nombre = p.Nombre, Id = p.idSucursal });
            return this.Json(lista, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [ActionName("CiudadesPorRegiones")]
        public ActionResult GetCiudadesPorRegiones(int id)
        {
            var paises = db.CIUDAD.Where(r => r.IdRegionFK == id);
            var lista = paises.Select(p => new { Nombre = p.Nombre, Id = p.IdCiudad });
            return this.Json(lista, JsonRequestBehavior.AllowGet);
        }
        // GET: Ajax
        public ActionResult Index()
        {
            PruebaCascadaViewModel model = new PruebaCascadaViewModel()
            {
                RegionList = new SelectList(db.REGION.ToList(), "IdRegion", "Nombre")
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult CiudadList(int RegionId)
        {
            var regionList = db.REGION.ToList();

            var ciudadList = db.CIUDAD.Where(x => x.IdRegionFK == RegionId).ToList();

            PruebaCascadaViewModel model = new PruebaCascadaViewModel()
            {
                RegionList = new SelectList(regionList, "IdRegion", "Nombre"),
                RegionId = RegionId,
                CiudadList = new SelectList(ciudadList, "IdCiudad", "Nombre")
            };
            return View("Index", model);
        }

  

        // GET: Ajax/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Ajax/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Ajax/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ajax/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Ajax/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ajax/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Ajax/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
