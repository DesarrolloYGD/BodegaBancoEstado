using BancoEstadoBodega.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BancoEstadoBodega.Controllers
{
    public class CampañaController : Controller
    {
        private pruebatotalNewEntities db = new pruebatotalNewEntities();
        public ActionResult Datos()
        {
            var model = new CampañaViewModel();
            var productos = db.PRODUCTO.ToList();
            model.Productos = new SelectList(productos, "IDProducto", "Nombre");
            return View(model);
        }
        // GET: Campaña
        public ActionResult Index()
        {
            return View();
        }

        // GET: Campaña/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Campaña/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Campaña/Create
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

        // GET: Campaña/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Campaña/Edit/5
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

        // GET: Campaña/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Campaña/Delete/5
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
