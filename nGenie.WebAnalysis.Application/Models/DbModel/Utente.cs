//@@@@Cambiare namespace
//namespace nGenie.WebAnalysis.Application.Models.DbModel
namespace nGenie.WebAnalysis.Application
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using nGenie.WebAnalysis.Application.Models.DbModel;

    [Table("Utente")]
    public partial class Utente
    {
        public Utente()
        {
            Nome = "";
            Cognome = "";
        }

        [Key]
        public int PkUtenteId { get; set; }

        [Required(ErrorMessage = "Username non valorizzata")]
        [StringLength(50)]
        public string Username { get; set; }

        public bool Abilitato { get; set; }

        [ForeignKey("Ruoli")]
        public int RuoloId { get; set; }

        [StringLength(50)]
        public string Nome { get; set; }

        [StringLength(50)]
        public string Cognome { get; set; }

        public virtual Ruolo Ruoli { get; set; }

        public virtual ICollection<FiltroUtente> FiltroUtente { get; set; }

        //public virtual ICollection<AutorizzazioniCubi> AutorizzazioniCubi { get; set; }
    }
}