//@@@@Cambiare namespace
using nGenie.WebAnalysis.Application.Models.DbModel;
namespace nGenie.WebAnalysis.Application
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CategoriaFiltro")]
    public partial class CategoriaFiltro
    {
        public CategoriaFiltro()
        {
            FiltroUtente = new HashSet<FiltroUtente>();
        }
        //CategoriaFiltro.PK_CategoriaFiltroId
        [Key]
        public int PK_CategoriaFiltroId { get; set; }

        [Required(ErrorMessage = "Riportare il nome dela categoria")]
        [StringLength(50)]
        public string NomeCategoria { get; set; }

        public string CubeName { get; set; }

        public int DatabaseOlapId { get; set; }

        public bool Attivo { get; set; }

        public virtual ICollection<FiltroUtente> FiltroUtente { get; set; }
    }
}