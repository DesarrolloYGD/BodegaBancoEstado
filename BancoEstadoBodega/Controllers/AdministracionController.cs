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
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity;

namespace BancoEstadoBodega.Controllers
{
    [Authorize]
    public class AdministracionController : Controller
    {

        private pruebatotalNewEntities db = new pruebatotalNewEntities();
        // GET: Administracion  
        public ActionResult Index()
        {
            var productos = db.PRODUCTO.Include(m => m.IDClienteFK);
            return View();
        }

        //Convertir imagen
        public ActionResult convertirImagen(string codigo)
        {
            try
            {
                var imagenProducto = db.PRODUCTO.Where(x => x.Codigo == codigo).FirstOrDefault();

                return File(imagenProducto.imagenProducto, "image/jpeg");
            }
            catch (Exception ex)
            { }
            return null;
        }


        // GET: PRODUCTO/Create
        public ActionResult Create()
        {
            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre");
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias");
            return View();
        }

        // POST: PRODUCTOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDProducto,Codigo,Nombre,UnidadesXCaja,CostoUnid,IDCategoriaFK,TotalCajas,TotalSueltas,FechaVencimiento,IDClienteFK,UrlImagen")] PRODUCTO pRODUCTO, HttpPostedFileBase imagenProducto)
        {
            if (imagenProducto != null && imagenProducto.ContentLength > 0)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(imagenProducto.InputStream))
                {
                    imageData = binaryReader.ReadBytes(imagenProducto.ContentLength);
                }
                //setear la imagen a la entidad que se creara
                pRODUCTO.imagenProducto = imageData;
            }

            if (ModelState.IsValid)
            {
                db.PRODUCTO.Add(pRODUCTO);
                db.SaveChanges();
                return RedirectToAction("Producto");
            }

            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }


        public ActionResult ServicioMoto()
        {
            
            return View();
        }

        // POST: PRODUCTOes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ServicioMoto([Bind(Include = "DireccionRetiro, DireccionEntrega, PersonaRecibe, Descripcion")] ServicioMoto moto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.ServicioMoto.Add(moto);
                    db.SaveChanges();
                    

                    /*-------------------------MENSAJE DE CORREO----------------------*/

                    //Creamos un nuevo Objeto de mensaje
                    System.Net.Mail.MailMessage mmsg = new System.Net.Mail.MailMessage();

                    //Direccion de correo electronico a la que queremos enviar el mensaje
                    //if (Request.IsAuthenticated)
                    //{
                    //mmsg.To.Add(User.Identity.GetUserName());
                    //}
                    mmsg.To.Add("logistica@promomas.cl");

                    //Nota: La propiedad To es una colección que permite enviar el mensaje a más de un destinatario

                    //Asunto
                    mmsg.Subject = "Servicio Moto";
                    mmsg.SubjectEncoding = System.Text.Encoding.UTF8;

                    //Direccion de correo electronico que queremos que reciba una copia del mensaje
                    if (Request.IsAuthenticated)
                    {
                        mmsg.Bcc.Add(User.Identity.GetUserName());
                    }
                    else
                    {
                        mmsg.To.Add("lbasic@promomas.cl");
                    }


                    //mmsg.Bcc.Add(user); //Opcional;

                    //LoginViewModel model3;

                    //Cuerpo del Mensaje
                    DateTime fecha = DateTime.Now;
                    string fech = fecha.ToString("dd/MM/yyyy");

                    mmsg.Body = "Hemos recibido su solicitud de servicio moto numero: " + moto.MotoID + "\nCon fecha: " + fech + "\nPara mas detalles porfavor visitar la página sección Servicio Moto";
                    mmsg.BodyEncoding = System.Text.Encoding.UTF8;
                    mmsg.IsBodyHtml = false; //Si no queremos que se envíe como HTML

                    //Correo electronico desde la que enviamos el mensaje
                    mmsg.From = new System.Net.Mail.MailAddress("solicitudes@promomas.cl");

                    /*-------------------------CLIENTE DE CORREO----------------------*/

                    //Creamos un objeto de cliente de correo
                    System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient();

                    //Hay que crear las credenciales del correo emisor
                    cliente.Credentials = new System.Net.NetworkCredential("solicitudes@promomas.cl", "medalla24");

                    //Lo siguiente es obligatorio si enviamos el mensaje desde Gmail
                    /*
                    cliente.Port = 587;
                    cliente.EnableSsl = true;
                    */
                    cliente.Host = "mail.promomas.cl"; //Para Gmail "smtp.gmail.com";

                    /*-------------------------ENVIO DE CORREO----------------------*/

                    try
                    {
                        //Enviamos el mensaje      
                        cliente.Send(mmsg);
                    }
                    catch (System.Net.Mail.SmtpException ex)
                    {
                        //Aquí gestionamos los errores al intentar enviar el correo
                    }
                    return RedirectToAction("IndexMoto");

                    
                }
                catch (DbEntityValidationException ex)
                {
                }

                
            }
            return View(moto);
        }

        public ActionResult IndexMoto()
        {
            int codigo = EnRol();
            List<ServicioMoto> lista = db.ServicioMoto.ToList();
            return View(lista);
        }

        public ViewResult DetalleMoto(int id)
        {
            ServicioMoto moto = db.ServicioMoto.Find(id);
            return View(moto);
        }
        #region CATEGORIA
        public ActionResult Categoria()
        {
            return View(/*db.CATEGORIA.ToList()*/);
        }

        [HttpPost]
        public ActionResult AgregarCategoria()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ModificarCategoria()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EliminarCategoria()
        {
            return View();
        }

        #endregion

        #region PRODUCTO
        public ActionResult Producto(int? IDEmpresa, int? IDCategoria)
        {
            int codigo = EnRol();

            ViewBag.IDEmpresa = new SelectList(db.CLIENTE.OrderBy(p => p.Alias).ToList(), "IDCliente", "Alias", IDEmpresa);
            ViewBag.IDCategoria = new SelectList(db.CATEGORIA.ToList(), "IDCategoria", "Nombre", IDCategoria);
            ViewBag.IDCategoriaFK = ViewBag.IDCategoria;
            ViewBag.IDClienteFK = ViewBag.IDEmpresa;

            List<PRODUCTO> lista = db.PRODUCTO.ToList();
            if (IDEmpresa != null)
            {
                lista = lista.Where(r => r.IDClienteFK == IDEmpresa).ToList();
            }
            if (IDCategoria != null)
            {
                lista = lista.Where(r => r.IDCategoriaFK == IDCategoria).ToList();
            }

            if (!User.IsInRole("administradores"))
            {
                lista = lista.Where(r => r.IDClienteFK == codigo).ToList();
            }



            return View(lista);
        }




        // Funcion que agrega los productos con imagen (ventana flotante) 
        public ActionResult AgregarProducto([Bind(Include = "IDProducto,Codigo,Nombre,UnidadesXCaja,StockQl,SueltasQL,StockDÑ,SueltasDÑ,stock_ideal,CostoUnid,Posicion,FechaVencimiento,IDCategoriaFK,IDClienteFK")]PRODUCTO model, FormCollection collection, HttpPostedFileBase imagenProducto)
        {
            PRODUCTOBODEGA item = new PRODUCTOBODEGA();
            PRODUCTOBODEGA item2 = new PRODUCTOBODEGA();

            item.BODEGA_IDBodega = 1;
            model.PRODUCTOBODEGA.Add(item);

            item2.Sueltas = Convert.ToInt32(collection["Bodega2Cajas"]);
            item2.Cajas = Convert.ToInt32(collection["Bodega2Sueltas"]);
            item2.BODEGA_IDBodega = 3;
            model.PRODUCTOBODEGA.Add(item);

            model.TotalSueltas = model.SueltasDÑ.Value + model.SueltasQL.Value;
            model.TotalCajas = model.StockDÑ.Value + model.StockQl.Value;
            model.CantidadTotal = (model.TotalCajas * model.UnidadesXCaja) + model.TotalSueltas;        
            //model.UrlImagen = model.Codigo + ".jpg";



            if (imagenProducto != null && imagenProducto.ContentLength > 0)
            {
                /*byte[] imageData = null;
                using (var binaryReader = new BinaryReader(imagenProducto.InputStream))
                {
                    imageData = binaryReader.ReadBytes(imagenProducto.ContentLength);
                }
                setear la imagen a la entidad que se creara
                model.imagenProducto = imageData;*/

                
                string imgName = model.Codigo + ".jpg";//variable local que concatena el codigo del producto mas .jpg(imagen)
                model.UrlImagen = imgName;//texto concatenado es asignado al valor UrilImagen de la variable local model
                new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService


            }

            db.PRODUCTO.Add(model);
            db.SaveChanges();
            return RedirectToAction("Producto");
        }

        //Obtiene los detalles de los productos y los muestra en el view Detalles
        public ViewResult Detalles(int id)
        {
            PRODUCTO producto = db.PRODUCTO.Find(id);
            //if que asigna una imagen por defecto en caso que el campo UrlImagen de la tabla productos sea nulo
            if (producto.UrlImagen == null)
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/noimage.jpg";
            }
            else
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/" + producto.Codigo + ".jpg";
            }
            return View(producto);
        }

        /*Obtiene la imagen asociada al producto y lo despliega en la grilla
        public ViewResult Producto(int id)
        {
            PRODUCTO producto = db.PRODUCTO.Find(id);
            ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/" + producto.Codigo + ".jpg";
            return ViewBag;
        }*/

        // GET: PRODUCTOes/Edit/5
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            if (pRODUCTO.UrlImagen == null)
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/noimage.jpg";
            }
            else
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/" + pRODUCTO.Codigo + ".jpg";
            }

            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }

        // POST: PRODUCTOes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "IDProducto,Codigo,Nombre,UnidadesXCaja,StockQl,CantidadTotal,TotalCajas,TotalSueltas,SueltasQL,StockDÑ,SueltasDÑ,CostoUnid,Posicion,FechaVencimiento,stock_ideal,IDCategoriaFK,IDClienteFK,FechaUltimaEdicion,UsuarioUltimaEdicion")] PRODUCTO pRODUCTO, HttpPostedFileBase imagenProducto)
        {
            string imgName = pRODUCTO.Codigo + ".jpg";//variable local que concatena el codigo del producto mas .jpg(imagen)
            if (pRODUCTO.UrlImagen == null)
            {
                pRODUCTO.UrlImagen = imgName;//texto concatenado es asignado al valor UrilImagen de la variable local model
                new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService
            }


            if (ModelState.IsValid)
            {
                db.Entry(pRODUCTO).State = EntityState.Modified;
                pRODUCTO.TotalSueltas = pRODUCTO.SueltasDÑ.Value + pRODUCTO.SueltasQL.Value;
                pRODUCTO.TotalCajas = pRODUCTO.StockDÑ.Value + pRODUCTO.StockQl.Value;
                pRODUCTO.CantidadTotal = (pRODUCTO.TotalCajas * pRODUCTO.UnidadesXCaja) + pRODUCTO.TotalSueltas;
                pRODUCTO.FechaUltimaEdicion = DateTime.Now;
                pRODUCTO.UsuarioUltimaEdicion = User.Identity.Name;

                db.SaveChanges();
                return RedirectToAction("Producto");
            }
            ViewBag.IDCategoriaFK = new SelectList(db.CATEGORIA, "IdCategoria", "Nombre", pRODUCTO.IDCategoriaFK);
            ViewBag.IDClienteFK = new SelectList(db.CLIENTE, "IDCliente", "Alias", pRODUCTO.IDClienteFK);
            return View(pRODUCTO);
        }

        public ActionResult EditarImagen(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PRODUCTO pRODUCTO = db.PRODUCTO.Find(id);
            if (pRODUCTO == null)
            {
                return HttpNotFound();
            }
            if (pRODUCTO.UrlImagen == null)
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/noimage.jpg";
            }
            else
            {
                ViewBag.imagerurl = "https://pruebasmarco.blob.core.windows.net/prueba-fotos/" + pRODUCTO.UrlImagen;
            }
            return View(pRODUCTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarImagen(EditarImagenProductoViewModel pRODUCTO, HttpPostedFileBase imagenProducto)
        {
            if (ModelState.IsValid)
            {
                var dbProd = db.PRODUCTO.FirstOrDefault(p => p.IDProducto == pRODUCTO.IdProducto);
                if (dbProd == null)
                {
                    return HttpNotFound();
                }
                string imgName = dbProd.Codigo + ".jpg";//variable local que concatena el codigo del producto mas .jpg(imagen)
                if (pRODUCTO.UrlImagen == null)
                {
                    pRODUCTO.UrlImagen = imgName;//texto concatenado es asignado al valor UrilImagen de la variable local model
                    new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService
                }
                else
                {
                    new BlobService().AddImgProducto(imagenProducto, imgName);//se activa la funcion addImgProducto de la clase BlobService
                }
                db.Entry(pRODUCTO).State = EntityState.Modified;
                dbProd.UrlImagen = pRODUCTO.UrlImagen;
                db.SaveChanges();

                return RedirectToAction("PRODUCTO", "Administracion");
            }
            return View(pRODUCTO);
        }





        //     [HttpPost]
        //    public ActionResult ModificarProducto()
        //     {
        //          return View();
        //     }

        [HttpPost]
        public ActionResult EliminarProducto()
        {
            return View();
        }



        #endregion

        #region AJAX
        [ActionName("DatosProducto")]
        public ActionResult DatosProductos(int IDProducto)
        {
            var producto = db.PRODUCTO.Find(IDProducto);

            var item = new
            {
                Codigo = producto.Codigo,
                Nombre = producto.Nombre,
                UnidadesXCajas = producto.UnidadesXCaja,
                StockQuilicura = producto.StockQl,
                StockDardignac = producto.StockDÑ,
                CantidadTotal = producto.CantidadTotal,
                TotalCajas = producto.TotalCajas,
                TotalSueltas = producto.TotalSueltas,
                FechaVencimiento = producto.FechaVencimiento,
                Imagen = producto.UrlImagen,
                Categoria = producto.CATEGORIA,
                Alias = producto.CLIENTE
            };
            return this.Json(item, JsonRequestBehavior.AllowGet);
        }
        #endregion

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