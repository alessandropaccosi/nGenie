namespace nGenie.WebAnalysis.Application.Models.DbModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using nGenie.WebAnalysis.Application;

    [Table("Ruoli")]
    public partial class Ruolo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome del ruolo non valorizzato")]
        [StringLength(50)]
        public string Nome { get; set; }

        public bool IsAdministrator { get; set; }
    }
}