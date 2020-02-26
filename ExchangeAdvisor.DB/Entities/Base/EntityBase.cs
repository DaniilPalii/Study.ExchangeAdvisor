using System.ComponentModel.DataAnnotations;

namespace ExchangeAdvisor.DB.Entities.Base
{
    internal class EntityBase
    {
        [Key]
        public long Id { get; set; }
    }
}
