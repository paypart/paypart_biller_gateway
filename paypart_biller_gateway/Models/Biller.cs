using System;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace paypart_biller_gateway.Models
{
    public class Biller
    {
        [Key]
        public int id { get; set; }
        public string title { get; set; }
        public string logo { get; set; }
        public string category_id { get; set; }
        public DateTime created_on { get; set; }
        public int status { get; set; }

        public virtual BillerContact billercontact { get; set; }

    }
}
