using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class SanctionType :BaseEntity
    {
        [JsonIgnore]
        //many one to one relationship with Vacation
        public List<Sanction>? Sanctions { get; set; }
    }
}
