using System;
using System.ComponentModel.DataAnnotations;

namespace BlasteR.Base
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        
        public BaseEntity()
        {
            Id = 0;
            CreatedTime = DateTime.Now;
            ModifiedTime = null;
        }
    }
}
