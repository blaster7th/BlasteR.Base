using System;
using System.ComponentModel.DataAnnotations;

namespace BlasteR.Base
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public BaseEntity()
        {
            Id = 0;
            CreatedAt = DateTime.Now;
            ModifiedAt = null;
        }
    }
}
