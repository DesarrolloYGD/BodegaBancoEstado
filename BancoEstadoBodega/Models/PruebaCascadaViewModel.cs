using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BancoEstadoBodega.Models
{
    public class PruebaCascadaViewModel
    {
        public PruebaCascadaViewModel()
        {
           
        }

        [DisplayName("Region")]
        public int RegionId { get; set; }
        public SelectList RegionList { get; set; }


        [DisplayName("Ciudad")]
        public int CiudadId { get; set; }
        public SelectList CiudadList { get; set; }

        [DisplayName("Comuna")]
        public SelectList ComunaList { get; set; }
        public List<int> ComunasSeleccionadas { get; set; }
    }
}