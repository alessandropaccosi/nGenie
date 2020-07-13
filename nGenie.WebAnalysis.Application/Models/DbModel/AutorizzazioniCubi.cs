using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nGenie.WebAnalysis.Application.Models.DbModel
{
    [Table("AutorizzazioniCubi")]
    public class AutorizzazioneCubo
    {
        [Key]
        public int Id { get; set; }

        public int UtenteId { get; set; }
        public int CuboId { get; set; }
    }
}