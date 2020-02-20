using System.ComponentModel.DataAnnotations;

namespace ExchangeAdvisor.DB.Entities.Base
{
    public class EntityBase
    {
        [Key]
        public long Id { get; set; }
    }
}
