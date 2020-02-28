using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goldnote.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Goldnote
{
    public class Options
    {
        /// <summary>
        /// 行き先を定義した集合
        /// </summary>
        public IEnumerable<SelectListItem> Destinations { get; } =new List<SelectListItem> {

            new SelectListItem { Text="ユニー行き",Value="true"},

            new SelectListItem { Text="本部行き",Value="false"}

        };
    }




}
