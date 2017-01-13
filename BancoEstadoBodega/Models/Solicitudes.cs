//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BancoEstadoBodega.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Solicitudes
    {
        public int idSolicitud { get; set; }
        public string descripcion { get; set; }
        public Nullable<int> idTipoEncomienda { get; set; }
        public Nullable<int> idTipoEmpaque { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }
        public Nullable<int> idTipoDespacho { get; set; }
        public Nullable<int> idTrasladoDespacho { get; set; }
        public System.DateTime fechaSolicitud { get; set; }
        public Nullable<System.DateTime> fechaEntrega { get; set; }
        public string codigoSeguimiento { get; set; }
        public Nullable<int> idTipoPedido { get; set; }
        public Nullable<int> idArea { get; set; }
        public string usuarioMandante { get; set; }
        public string usuarioReceptor { get; set; }
        public Nullable<int> bultos { get; set; }
        public string observacion { get; set; }
        public Nullable<long> costeUnitario { get; set; }
        public Nullable<long> servicioMecanizado { get; set; }
        public Nullable<long> subtotal { get; set; }
        public string estado { get; set; }
    }
}
