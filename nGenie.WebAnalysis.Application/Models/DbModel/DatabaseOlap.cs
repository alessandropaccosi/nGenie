namespace nGenie.WebAnalysis.Application.Models.DbModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("DatabasesOlap")]
    public partial class DatabaseOlap
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome del database non valorizzato")]
        [StringLength(50)]
        public string Nome { get; set; }

        public bool Attivo { get; set; }

        [ForeignKey("ServerOlap")]
        public int ServerOlapId { get; set; }

        public virtual ServerOlap ServerOlap { get; set; }
    }
}