using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Linq;
using System.Threading.Tasks;

namespace paypart_biller_gateway.Models
{
    public class BillerContact
    {
        [Key]
        public int id { get; set; }
        public int billerid { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public int stateid { get; set; }
        public int countryid { get; set; }
        public string postcode { get; set; }
        public string email { get; set; }
        public string emailaux { get; set; }
        public string phone { get; set; }
        public string phoneaux { get; set; }
        public int status { get; set; }
    }
}
