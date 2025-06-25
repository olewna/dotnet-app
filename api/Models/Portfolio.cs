using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    [Table("Portfolios")]
    public class Portfolio // join table
    {
        public string UserId { get; set; } //forgein key for user
        public int StockId { get; set; } //forgein key for stocks
        public User User { get; set; }
        public Stock Stock { get; set; }
    }
}