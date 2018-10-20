using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlasteR.Base
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastEditTime { get; set; }
        
        public BaseEntity()
        {
            Id = 0;
            CreationTime = DateTime.Now;
            LastEditTime = null;
        }
    }
}
