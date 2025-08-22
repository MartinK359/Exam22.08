using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RevueCraftersSystem.DTOs
{
    public class ApiResponseDTO
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("revueId")]
        public string RevueId { get; set; }
    }
}
