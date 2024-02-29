using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopugCommon.KafkaMessages
{
    public class UserStreamMessage
    {
        public string MessageId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public DateTime OpertionDate { get; set; }
        public CudOperation Operation { get; set; }
    }
    public enum CudOperation
    {
        Add = 1,
        Update = 2,
        Delete = 3
    }

    
    

}
