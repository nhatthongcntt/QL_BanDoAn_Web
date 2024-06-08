using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QL_BanDoAn.Models
{
    public class Foods
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public double Star { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public bool BestFood { get; set; }
        public double Price { get; set; }
        public int PriceId { get; set; }
        public int LocationId { get; set; }
        public int TimeId { get; set; }
        public int TimeValue { get; set; }


    }
}