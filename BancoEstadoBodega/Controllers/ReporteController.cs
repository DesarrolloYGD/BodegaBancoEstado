using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BancoEstadoBodega.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data.Entity;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage type
using System.Net;
using System.Data;
using System.IO;
using System.Xml.Serialization;
using System.Collections;
using System.Web.UI.WebControls;
using System.Web.UI;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Xml;


namespace BancoEstadoBodega.Controllers
{
    public class ReporteController : Controller
    {
        private pruebatotalNewEntities db = new pruebatotalNewEntities();
        // GET: Reporte
        public ActionResult Index()
        {
            var productos = db.PRODUCTO.Include(m => m.IDClienteFK);
            return View();
        }

        public ActionResult Critico(int? IDEmpresa, int? IDCategoria, int?  Stock,int? StockCritico, PRODUCTO model)
        {
            int cod = EnRol();
            ViewBag.IDEmpresa = new SelectList(db.CLIENTE.ToList(), "IDCliente", "Alias", IDEmpresa);
            ViewBag.IDCategoria = new SelectList(db.CATEGORIA.ToList(), "IDCategoria", "Nombre", IDCategoria);
            ViewBag.IDCategoriaFK = ViewBag.IDCategoria;
            ViewBag.IDClienteFK = ViewBag.IDEmpresa;
            // var productos = db.PRODUCTO.Include(m => m.stock_ideal);
            //var stock2 = db.PRODUCTO.Include(m => m.CantidadTotal);
            //int porcentje = (stock2 * 100) / productos;
            //var porcentaje = (model.CantidadTotal.Value * 100) / model.stock_ideal;
            List<PRODUCTO> lista = db.PRODUCTO.ToList();
            lista = lista.Where(r => r.CantidadTotal.Value * 100 / r.stock_ideal.Value <= 20).ToList();
            if (!User.IsInRole("administradores"))
            {
                lista = lista.Where(r => r.IDClienteFK == cod).ToList();
            }
            //lista = lista.Where(r => r.CantidadTotal.Value *100 / r.stock_ideal.Value<=20).ToList();
            return View(lista);
        }
        // GET: Reporte/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult IndexLibro(int? IDTipoEmpaque, int? IDEmpresas, int? IDTraslado, int? IDValija, int? IDTipoCampaña, int? IDArea, int? IDMeca, int? IDTipoDespacho)
        {
            int codigo = EnRol();

            ViewBag.IDTipoEmpaque = new SelectList(db.TipoEmpaque.OrderBy(p => p.descripcion).ToList(), "idTipoEmpaque", "descripcion", IDTipoEmpaque);
            ViewBag.IDEmpresas = new SelectList(db.Empresas.OrderBy(p => p.Nombre).ToList(), "ID", "Nombre", IDEmpresas);
            ViewBag.IDTraslado = new SelectList(db.TrasladoDespacho.OrderBy(p => p.descripcion).ToList(), "idTrasladoDespacho", "descripcion", IDTraslado);
            ViewBag.IDValija = new SelectList(db.ValijaInterna.OrderBy(p => p.Nombre).ToList(), "ID", "Nombre", IDValija);
            ViewBag.IDTipoCampaña = new SelectList(db.TipoCampaña.OrderBy(p => p.Nombre).ToList(), "ID", "Nombre", IDTipoCampaña);
            ViewBag.IDArea = new SelectList(db.Area.OrderBy(p => p.nombre).ToList(), "idArea", "nombre", IDArea);
            ViewBag.IDMeca = new SelectList(db.Mecanizado.OrderBy(p => p.Descripcion).ToList(), "IdMeca", "Descripcion", IDMeca);
            ViewBag.IDTipoDespacho = new SelectList(db.TipoDespacho.OrderBy(p => p.descripcion).ToList(), "idTipoDespacho", "descripcion", IDTipoDespacho);
            ViewBag.id_TipoEmpaque = ViewBag.IDTipoEmpaque;
            ViewBag.id_Empresa = ViewBag.IDEmpresas;
            ViewBag.id_Trasladodespacho = ViewBag.IDTraslado;
            ViewBag.id_Valija = ViewBag.IDValija;
            ViewBag.id_TipoCampaña = ViewBag.IDTipoCampaña;
            ViewBag.id_Area = ViewBag.IDArea;
            ViewBag.id_Meca = ViewBag.IDMeca;
            ViewBag.id_TipoDespacho = ViewBag.IDTipoDespacho;
            List<LibroDiario> lista = db.LibroDiario.ToList();
            //List<PRODUCTO> lista = db.PRODUCTO.ToList();
            if(IDTipoEmpaque != null)
            {
                lista = lista.Where(r => r.id_TipoEmpaque == IDTipoEmpaque).ToList();
            }
            if (IDEmpresas != null)
            {
                lista = lista.Where(r => r.id_Empresa == IDEmpresas).ToList();
            }
            if (IDTraslado != null)
            {
                lista = lista.Where(r => r.id_Trasladodespacho == IDTraslado).ToList();
            }

            if (IDValija != null)
            {
                lista = lista.Where(r => r.id_Valija == IDValija).ToList();
            }

            if (IDTipoCampaña != null)
            {
                lista = lista.Where(r => r.id_TipoCampaña == IDTipoCampaña).ToList();
            }

            if (IDArea != null)
            {
                lista = lista.Where(r => r.id_Area == IDArea).ToList();
            }

            if (IDMeca != null)
            {
                lista = lista.Where(r => r.id_Meca == IDMeca).ToList();
            }

            if (!User.IsInRole("administradores"))
            {
                lista = lista.Where(r => r.id_Area == codigo).ToList();
            }



            return View(lista);
        }

        // GET: LibroDiario
        public ActionResult CrearLibro()
        {
            ViewBag.Id_TipoEmpaque = new SelectList(db.TipoEmpaque, "idTipoEmpaque", "descripcion");
            ViewBag.Id_Empresa = new SelectList(db.Empresas, "ID", "Nombre");
            ViewBag.Id_Trasladodespacho = new SelectList(db.TrasladoDespacho, "idTrasladoDespacho", "descripcion");
            ViewBag.Id_Valija = new SelectList(db.ValijaInterna, "ID", "Nombre");
            ViewBag.Id_TipoCampaña = new SelectList(db.TipoCampaña, "ID", "Nombre");
            ViewBag.Id_Area = new SelectList(db.Area, "idArea", "nombre");
            ViewBag.Id_Meca = new SelectList(db.Mecanizado, "IdMeca", "Descripcion");
            ViewBag.Id_TipoDespacho = new SelectList(db.TipoDespacho, "idTipoDespacho", "descripcion");
            ViewBag.id_TipoEncomienda = new SelectList(db.TipoEncomienda, "idTipoEncomienda", "tipoEncomienda");
            return View();
        }
        [HttpPost]
        //POST LibroDiario
        public ActionResult CrearLibro([Bind(Include = "id_TipoEmpaque,id_Empresa,id_Trasladodespacho,id_Valija,Destino,Comuna,FechaSolicitud,FechaEntrega,	CodigoSeguimiento,id_TipoCampaña,CampañaObs,id_Area,Mandante,Receptor,Bultos,Obs,id_Meca,id_TipoDespacho,id_TipoEncomienda,costedespacho, costeMeca, TotalMeca")]LibroDiario model)
        {
            if(model.id_Meca == 1)
            {
                model.costeMeca = 582;
            }
            else
                if(model.id_Meca == 2)
            {
                model.costeMeca = 2185;
            }

            model.TotalMeca = model.Bultos * model.costeMeca;
            
            db.LibroDiario.Add(model);
            db.SaveChanges();
            return RedirectToAction("IndexLibro");
        }
        public ViewResult DetalleLibro(int id)
        {
            LibroDiario libro = db.LibroDiario.Find(id);
            return View(libro);
        }

        // GET: PRODUCTOes/Edit/5
        public ActionResult EditarLibro(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LibroDiario libro = db.LibroDiario.Find(id);
            if (libro == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_TipoEmpaque = new SelectList(db.TipoEmpaque, "idTipoEmpaque", "descripcion", libro.id_TipoEmpaque);
            ViewBag.Id_Empresa = new SelectList(db.Empresas, "ID", "Nombre", libro.id_Empresa);
            ViewBag.Id_Trasladodespacho = new SelectList(db.TrasladoDespacho, "idTrasladoDespacho", "descripcion", libro.id_Trasladodespacho);
            ViewBag.Id_Valija = new SelectList(db.ValijaInterna, "ID", "Nombre", libro.id_Valija);
            ViewBag.Id_TipoCampaña = new SelectList(db.TipoCampaña, "ID", "Nombre",libro.id_TipoCampaña);
            ViewBag.Id_Area = new SelectList(db.Area, "idArea", "nombre", libro.id_Area);
            ViewBag.Id_Meca = new SelectList(db.Mecanizado, "IdMeca", "Descripcion", libro.id_Meca);
            ViewBag.Id_TipoDespacho = new SelectList(db.TipoDespacho, "idTipoDespacho", "descripcion", libro.id_TipoDespacho);
            ViewBag.id_TipoEncomienda = new SelectList(db.TipoEncomienda, "idTipoEncomienda", "tipoEncomienda", libro.id_TipoEncomienda);
            return View(libro);
        }

        // POST: PRODUCTOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarLibro([Bind(Include = "id_TipoEmpaque,id_Empresa,id_Trasladodespacho,id_Valija,Destino,Comuna,FechaSolicitud,FechaEntrega,CodigoSeguimiento,id_TipoCampaña,CampañaObs,id_Area,Mandante,Receptor,Bultos,Obs,id_Meca,id_TipoDespacho,id_TipoEncomienda,costedespacho, costeMeca, TotalMeca")] LibroDiario libro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(libro).State = EntityState.Modified;
                ViewBag.Id_TipoEmpaque = new SelectList(db.TipoEmpaque, "idTipoEmpaque", "descripcion", libro.id_TipoEmpaque);
                ViewBag.Id_Empresa = new SelectList(db.Empresas, "ID", "Nombre", libro.id_Empresa);
                ViewBag.Id_Trasladodespacho = new SelectList(db.TrasladoDespacho, "idTrasladoDespacho", "descripcion", libro.id_Trasladodespacho);
                ViewBag.Id_Valija = new SelectList(db.ValijaInterna, "ID", "Nombre", libro.id_Valija);
                ViewBag.Id_TipoCampaña = new SelectList(db.TipoCampaña, "ID", "Nombre", libro.id_TipoCampaña);
                ViewBag.Id_Area = new SelectList(db.Area, "idArea", "nombre", libro.id_Area);
                ViewBag.Id_Meca = new SelectList(db.Mecanizado, "IdMeca", "Descripcion", libro.id_Meca);
                ViewBag.Id_TipoDespacho = new SelectList(db.TipoDespacho, "idTipoDespacho", "descripcion", libro.id_TipoDespacho);
                ViewBag.id_TipoEncomienda = new SelectList(db.TipoEncomienda, "idTipoEncomienda", "tipoEncomienda", libro.id_TipoEncomienda);
                
                db.SaveChanges();
                return RedirectToAction("IndexLibro");
            }
            return View(libro);
        }

        /* public ActionResult ResumenDeVentas(string empresa, string fechaEmis, string fechaVenc, string submitButton)
         {
             string EmpresaAux = "Todos";
             if (!string.IsNullOrEmpty(empresa))
                 EmpresaAux = empresa;
             string periodo = "Desde: " + "-" + " Hasta: " + "-";
             List<LibroDiario> libros = new List<LibroDiario>();
             #region FILTROS
             if (!string.IsNullOrEmpty(empresa))
             {
                 empresa = empresa.Replace(".", "");
                 libros = libros.Where(r => r.Empresas.Nombre == empresa).ToList();
             }
             if (!string.IsNullOrEmpty(fechaEmis) && string.IsNullOrEmpty(fechaVenc))
             {
                 DateTime fecha = Convert.ToDateTime(fechaEmis);
                 libros = libros.Where(r => r.FechaSolicitud == fecha).ToList();
                 periodo = "Desde: " + fechaEmis + " Hasta: " + "-";
             }
             if (!string.IsNullOrEmpty(fechaVenc) && string.IsNullOrEmpty(fechaEmis))
             {
                 DateTime fecha = Convert.ToDateTime(fechaVenc);
                 libros = libros.Where(r => r.FechaSolicitud == fecha).ToList();
                 periodo = "Desde: " + "-" + " Hasta: " + fechaVenc;
             }
             if (!string.IsNullOrEmpty(fechaVenc) && !string.IsNullOrEmpty(fechaEmis))
             {
                 DateTime fecEmis = Convert.ToDateTime(fechaEmis);
                 DateTime fecVenc = Convert.ToDateTime(fechaVenc);
                 libros = libros.Where(r => r.FechaSolicitud >= fecEmis && r.FechaSolicitud <= fecVenc).ToList();
                 periodo = "Desde: " + fechaEmis + " Hasta: " + fechaVenc;
             }
             if (string.IsNullOrEmpty(fechaVenc) && string.IsNullOrEmpty(fechaEmis))
             {
                 DateTime fecEmis = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                 DateTime fecVenc = fecEmis.AddMonths(1).AddDays(-1);
                 libros = libros.Where(r => r.FechaSolicitud >= fecEmis && r.FechaSolicitud <= fecVenc).ToList();
                 periodo = "Desde: " + fechaEmis + " Hasta: " + fechaVenc;
             }
             #endregion

             #region CARGA DATOS INICIALES
             string fechaEmision = string.Empty;
             string fechaVencimiento = string.Empty;
             string Vendedor = string.Empty;
             string Empresa = string.Empty;

             if (!string.IsNullOrEmpty(fechaEmis))
             {
                 fechaEmision = fechaEmis;
             }

             if (!string.IsNullOrEmpty(fechaVenc))
             {
                 fechaVencimiento = fechaVenc;
             }

             #endregion

             #region CALCULA TOTALES

             #endregion
             ViewBag.fechaEmision = fechaEmision;
             ViewBag.fechaVencimiento = fechaVencimiento;

             //DONWLOAD
             switch (submitButton)
             {
                 case "Descargar Excel": DownloadExcel(libros, periodo);
                     break;
             }

             return View();
         }

         public void DownloadExcel(List<LibroDiario> libro, string periodo)
         {
             List<String> empresas = libro.Select(r => r.Empresas.Nombre).Distinct().ToList();
             List<LibroDiario> libroAux = new List<LibroDiario>();


             IWorkbook workbook = new XSSFWorkbook();
             ISheet sheet1;

             ICellStyle style = workbook.CreateCellStyle();
             style.DataFormat = workbook.CreateDataFormat().GetFormat("$#,##0_);($#,##0)");

             IFont font = workbook.CreateFont();
             font.FontHeightInPoints = 11;
             font.FontName = "Calibri";
             font.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;


             IFont fontTitulo = workbook.CreateFont();
             fontTitulo.FontHeightInPoints = 16;
             fontTitulo.FontName = "Calibri";
             fontTitulo.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;

             foreach (string empresa in empresas)
             {
                 sheet1 = workbook.CreateSheet(empresa);
                 libroAux = libro.Where(r => r.Empresas.Nombre == empresa).ToList();


                 IRow rowEmpresa = sheet1.CreateRow(1);
                 rowEmpresa.CreateCell(0).SetCellValue(empresa);
                 rowEmpresa.CreateCell(6).SetCellValue("Fecha:");
                 rowEmpresa.CreateCell(7).SetCellValue(DateTime.Now.ToString("dd/MM/yyyy"));


                 IRow rowTitulo = sheet1.CreateRow(6);
                 rowTitulo.CreateCell(3).SetCellValue("Libro De Ventas");
                 rowTitulo.GetCell(3).CellStyle.SetFont(fontTitulo);

                 IRow rowPeriodo = sheet1.CreateRow(7);
                 rowPeriodo = sheet1.CreateRow(8);
                 rowPeriodo.CreateCell(0).SetCellValue(periodo);

                 IRow rowCol1 = sheet1.CreateRow(10);
                 rowCol1.CreateCell(0).SetCellValue("Encomienda");
                 rowCol1.CreateCell(1).SetCellValue("Empaque");
                 rowCol1.CreateCell(2).SetCellValue("Origen");
                 rowCol1.CreateCell(3).SetCellValue("Forma de despacho");
                 rowCol1.CreateCell(4).SetCellValue("Camioneta/Moto");
                 rowCol1.CreateCell(5).SetCellValue("Valija Interna");
                 rowCol1.CreateCell(6).SetCellValue("Destino");
                 rowCol1.CreateCell(7).SetCellValue("Comuna");
                 rowCol1.CreateCell(8).SetCellValue("Fecha Solicitud");
                 rowCol1.CreateCell(9).SetCellValue("Fecha Entrega");
                 rowCol1.CreateCell(10).SetCellValue("Seguimiento");
                 rowCol1.CreateCell(11).SetCellValue("Clasificacion");
                 rowCol1.CreateCell(12).SetCellValue("Descripcion de la entrega");
                 rowCol1.CreateCell(13).SetCellValue("Area Mandante");
                 rowCol1.CreateCell(14).SetCellValue("Usuario Mandante");
                 rowCol1.CreateCell(15).SetCellValue("Usuario Receptor");
                 rowCol1.CreateCell(16).SetCellValue("Bultos");
                 rowCol1.CreateCell(17).SetCellValue("Observaciones");
                 rowCol1.CreateCell(18).SetCellValue("Coste Unitario");
                 rowCol1.CreateCell(19).SetCellValue("Servicio Mecanizado");
                 rowCol1.CreateCell(20).SetCellValue("SubTotal");

                 int cantidadRegistros = libroAux.Count + 20;

                 #region FACTURAS
                 if (libroAux.Count > 0)
                 {
                     for (int i = 20; i < cantidadRegistros; i++)
                     {
                         IRow row = sheet1.CreateRow(i);
                         row.CreateCell(0).SetCellValue(libroAux[i - 20].TipoEncomienda.tipoEncomienda1);
                         row.CreateCell(1).SetCellValue(libroAux[i - 20].TipoEmpaque.descripcion);
                         row.CreateCell(2).SetCellValue(libroAux[i - 20].Empresas.Nombre);
                         row.CreateCell(3).SetCellValue(libroAux[i - 20].TipoDespacho.descripcion);
                         row.CreateCell(4).SetCellValue(libroAux[i - 20].TrasladoDespacho.descripcion);
                         row.CreateCell(5).SetCellValue(libroAux[i - 20].ValijaInterna.Nombre);
                         row.CreateCell(6).SetCellValue(libroAux[i - 20].Destino);
                         row.CreateCell(7).SetCellValue(libroAux[i - 20].Comuna);
                         row.CreateCell(8).SetCellValue(libroAux[i - 20].FechaSolicitud.ToString("dd/MM/yyyy"));
                         row.CreateCell(9).SetCellValue(libroAux[i - 20].FechaEntrega.ToString("dd/MM/yyyy"));
                         row.CreateCell(10).SetCellValue(libroAux[i - 20].CodigoSeguimiento);
                         row.CreateCell(11).SetCellValue(libroAux[i - 20].TipoCampaña.Nombre);
                         row.CreateCell(12).SetCellValue(libroAux[i - 20].CampañaObs);
                         row.CreateCell(13).SetCellValue(libroAux[i - 20].Area.nombre);
                         row.CreateCell(14).SetCellValue(libroAux[i - 20].Mandante);
                         row.CreateCell(15).SetCellValue(libroAux[i - 20].Receptor);
                         row.CreateCell(16).SetCellValue(libroAux[i - 20].Bultos);
                         row.CreateCell(17).SetCellValue(libroAux[i - 20].Obs);
                         row.CreateCell(18).SetCellValue(libroAux[i - 20].CosteUnitario.Value);
                         row.CreateCell(19).SetCellValue(libroAux[i - 20].Mecanizado.Descripcion);
                         row.CreateCell(20).SetCellValue(libroAux[i - 20].SubTotal.Value); 
                     }


                 }

                 #endregion

                 #region Ajustes de largo
                 sheet1.AutoSizeColumn((short)1);
                 sheet1.AutoSizeColumn((short)2);
                 sheet1.AutoSizeColumn((short)3);
                 sheet1.AutoSizeColumn((short)4);
                 sheet1.AutoSizeColumn((short)5);
                 sheet1.AutoSizeColumn((short)6);
                 sheet1.AutoSizeColumn((short)7);
                 sheet1.AutoSizeColumn((short)8);
                 sheet1.AutoSizeColumn((short)9);
                 sheet1.AutoSizeColumn((short)10);
                 sheet1.AutoSizeColumn((short)11);
                 sheet1.AutoSizeColumn((short)12);
                 sheet1.AutoSizeColumn((short)13);
                 sheet1.AutoSizeColumn((short)14);
                 sheet1.AutoSizeColumn((short)15);
                 sheet1.AutoSizeColumn((short)16);
                 sheet1.AutoSizeColumn((short)17);
                 sheet1.AutoSizeColumn((short)18);
                 sheet1.AutoSizeColumn((short)19);
                 sheet1.AutoSizeColumn((short)20);

                 #endregion
             }

             string tura = this.Server.MapPath("~/Upload/");
             FileStream sw = System.IO.File.Create(tura + "LibroDiario.xlsx");
             workbook.Write(sw);
             sw.Close();

             Response.Clear();
             Response.ContentType = "application/pdf";
             Response.AppendHeader("Content-Disposition", "attachment; filename=LibroDiario-" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".xlsx");
             Response.TransmitFile(this.Server.MapPath("~/Upload/LibroDiario.xlsx"));
             Response.End();

         }
         */
        // GET: Reporte/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reporte/Create
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

        // GET: Reporte/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reporte/Edit/5
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

        // GET: Reporte/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reporte/Delete/5
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
        #region Roles
        public int EnRol()
        {
            int codigo = 0;

            if (User.IsInRole("Banca Mayorista"))
            {
                codigo = 1;
                return codigo;
            }
            else if (User.IsInRole("Banca Personas"))
            {
                codigo = 2;
                return codigo;
            }
            else if (User.IsInRole("Caja Vecina"))
            {
                codigo = 3;
                return codigo;
            }
            else if (User.IsInRole("Capital de Riesgo"))
            {
                codigo = 4;
                return codigo;
            }
            else if (User.IsInRole("Cliente Calidad"))
            {
                codigo = 5;
                return codigo;
            }
            else if (User.IsInRole("Comunicaciones"))
            {
                codigo = 6;
                return codigo;
            }
            else if (User.IsInRole("Convenio"))
            {
                codigo = 7;
                return codigo;
            }
            else if (User.IsInRole("Desayuno Cliente"))
            {
                codigo = 8;
                return codigo;
            }
            else if (User.IsInRole("Educacion Financiera"))
            {
                codigo = 9;
                return codigo;
            }
            else if (User.IsInRole("Fondos Mutuos"))
            {
                codigo = 10;
                return codigo;
            }
            else if (User.IsInRole("Gerencia de Ventas"))
            {
                codigo = 11;
                return codigo;
            }
            else if (User.IsInRole("Hipotecario"))
            {
                codigo = 12;
                return codigo;
            }
            else if (User.IsInRole("Inversiones"))
            {
                codigo = 13;
                return codigo;
            }
            else if (User.IsInRole("Marketing"))
            {
                codigo = 14;
                return codigo;
            }
            else if (User.IsInRole("Negocios Internacionales"))
            {
                codigo = 15;
                return codigo;
            }
            else if (User.IsInRole("Pequeña Empresa"))
            {
                codigo = 16;
                return codigo;
            }
            else if (User.IsInRole("Mesa de Dinero"))
            {
                codigo = 17;
                return codigo;
            }
            else if (User.IsInRole("RRHH"))
            {
                codigo = 18;
                return codigo;
            }
            else if (User.IsInRole("Serviestado"))
            {
                codigo = 19;
                return codigo;
            }
            else if (User.IsInRole("Sucursales"))
            {
                codigo = 20;
                return codigo;
            }
            else if (User.IsInRole("Transito"))
            {
                codigo = 21;
                return codigo;
            }


            else { return codigo; }
        }
        #endregion
    }
}
