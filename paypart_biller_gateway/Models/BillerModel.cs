using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using paypart_biller_gateway.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace paypart_biller_gateway.Models
{
    public class BillerModel
    {
        public Biller biller { get; set; }
        public BillerContact billercontact { get; set; }
    }
}
