using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.DTOs
{
    public class UpdateResult
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
