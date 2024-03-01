using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PopugCommon.KafkaMessages
{
    public class User
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
    }
}
