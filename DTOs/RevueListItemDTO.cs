using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevueCraftersSystem.DTOs
{
    public class RevueListItemDTO
    {
        [JsonProperty("id")]
        public string RevueId { get; set; }
    }
}
