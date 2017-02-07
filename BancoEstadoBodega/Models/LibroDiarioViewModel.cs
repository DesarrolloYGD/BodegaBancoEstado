using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BancoEstadoBodega.Models
{
    public class LibroDiarioViewModel
    {
        public List<SelectListItem> Empresas { get; set; }

        public List<LibroDiario> Libros { get; set; }

        public TipoEmpaque tipoempaque { get; set; }

        public string EmpresaSeleccionada { get; set; }
    }
}