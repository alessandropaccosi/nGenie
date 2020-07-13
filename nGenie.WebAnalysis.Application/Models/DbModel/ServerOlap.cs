namespace nGenie.WebAnalysis.Application.Models.DbModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ServerOlap")]
    public partial class ServerOlap
    {
        public ServerOlap()
        {
            DatabasesOlap = new HashSet<DatabaseOlap>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome del server Olap non valorizzato")]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Url del server Olap non valorizzata")]
        [StringLength(200)]
        public string Url { get; set; }

        public bool Attivo { get; set; }

        public virtual ICollection<DatabaseOlap> DatabasesOlap { get; set; }
    }
}