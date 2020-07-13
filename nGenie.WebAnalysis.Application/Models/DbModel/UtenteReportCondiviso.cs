using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models.DbModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UtentiReportCondivisi")]
    public partial class UtenteReportCondiviso
    {
        [Key]
        public int Id { get; set; }

        public int ReportCondivisoId { get; set; }

        /// <summary>
        /// Nome del Report condiviso
        /// </summary>
        public int DestinatarioUtenteId { get; set; }
    }
}