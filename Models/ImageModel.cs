using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Goldnote.Models
{
    public class ImageModel
    {
        
        public string Id { get; set; }

        public byte[] image { get; set; }

    }
}
