using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Goldnote
{
    public class Options
    {
        public IEnumerable<SelectListItem> Destinations { get; } =new List<SelectListItem> {

            new SelectListItem { Text="ユニー行き",Value="true"},

            new SelectListItem { Text="本部行き",Value="false"}

        };
    }
}
