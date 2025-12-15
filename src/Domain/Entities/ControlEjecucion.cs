using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ControlEjecucion
    {
        [Key]
        public int Id { get; set; }
        
        public string Proceso { get; set; } = string.Empty;
        
        public DateTime UltimaEjecucion { get; set; }
        
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}