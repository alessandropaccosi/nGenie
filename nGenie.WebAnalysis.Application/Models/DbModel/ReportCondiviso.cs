﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace nGenie.WebAnalysis.Application.Models.DbModel
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ReportCondivisi")]
    public partial class ReportCondiviso
    {
        [Key]
        public int Id { get; set; }

        public int ReportId { get; set; }

        /// <summary>
        /// Nome del Report condiviso
        /// </summary>
        public string Nome { get; set; }
    }
}