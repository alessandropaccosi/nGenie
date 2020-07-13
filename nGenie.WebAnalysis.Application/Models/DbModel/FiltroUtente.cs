//@@@@Cambiare namespace
//namespace nGenie.WebAnalysis.Application.Models.DbModel
namespace nGenie.WebAnalysis.Application
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FiltroUtente")]
    public partial class FiltroUtente
    {
        [Key]
        public int PKFiltroId { get; set; }

        [Required(ErrorMessage = "Riportare il nome del Report")]
        [StringLength(100)]
        public string Nome { get; set; }

        public DateTime DataCreazione { get; set; }

        public DateTime DataUltimaModifica { get; set; }

        [ForeignKey("Utente")]
        public int FK_UtenteId { get; set; }

        [ForeignKey("CategoriaFiltro")]
        public int FK_CategoriaId { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Il filtro 'Colonne' non risulta valorizzato")]
        public string Colonne { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Il filtro 'Righe' non risulta valorizzato")]
        public string Righe { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Il filtro 'Misure' non risulta valorizzato")]
        public string Misure { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Il filtro 'Transport' non risulta valorizzato")]
        public string Transport { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Il filtro 'Filtri' non risulta valorizzato")]
        public string Filtri { get; set; }

        [Required(AllowEmptyStrings = true, ErrorMessage = "Il filtro 'Sort' non risulta valorizzato")]
        public string Sort { get; set; }

        public bool Attivo { get; set; }

        public bool Predefinito { get; set; }

        public bool Condiviso { get; set; }

        public virtual CategoriaFiltro CategoriaFiltro { get; set; }

        public virtual Utente Utente { get; set; }
    }
}