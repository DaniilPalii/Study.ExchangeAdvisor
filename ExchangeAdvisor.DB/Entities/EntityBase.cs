using System.ComponentModel.DataAnnotations;

namespace ExchangeAdvisor.DB.Entities
{
    public class EntityBase
    {
        [Key]
        public long Id { get; set; }
    }
}
