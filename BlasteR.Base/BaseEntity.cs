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
        public int ID { get; set; }
        [DataMember]
        public DateTime CreationTime { get; set; }
        [DataMember]
        public DateTime? LastEditTime { get; set; }
        
        public BaseEntity()
        {
            ID = 0;
            CreationTime = DateTime.Now;
            LastEditTime = null;
        }
    }
}
