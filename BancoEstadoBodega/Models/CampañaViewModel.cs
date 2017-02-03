using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BancoEstadoBodega.Models
{
    public class CampañaViewModel
    {
        public String nombreCampaña { get; set; }
        public SelectList Productos { get; set; }
    }
}