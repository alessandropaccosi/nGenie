namespace nGenie.WebAnalysis.Application.Models.DbModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Log")]
    public partial class Log
    {
        public Log()
        {
            Messaggio = "";
            StackTrace = "";
        }

        [Key]
        public int Id { get; set; }
        public string Messaggio { get; set; }

        public string StackTrace { get; set; }
    }
}