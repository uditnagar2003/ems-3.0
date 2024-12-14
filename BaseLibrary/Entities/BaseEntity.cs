using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BaseLibrary.Entities
{
    public class BaseEntity
    {
        public int id { get; set; }
        [Required]
        public string? name { get; set; }= string.Empty;

       /* [JsonIgnore]

        public List<Employee> Employees { get; set; }*/

    }
}
