using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace BlasteR.Base
{
    [DataContract(IsReference = false)]
    public class BaseEntity
    {
        [Key]
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime CreationTime { get; set; }
        [DataMember]
        public DateTime? LastEditTime { get; set; }
        
        public BaseEntity()
        {
            Id = 0;
            CreationTime = DateTime.Now;
            LastEditTime = null;
        }
    }
}
