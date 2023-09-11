using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Consumer.Config
{
    public class KafkaConsumerConfig
    {
        public string BootstrapServers { get; set; }
        public string GroupId { get; set; }
        public string Earliest { get; set; }
    }
}
