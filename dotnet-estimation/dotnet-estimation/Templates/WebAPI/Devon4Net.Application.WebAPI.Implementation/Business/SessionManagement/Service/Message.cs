using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public class Message<T>
    {
        public MessageType Type { get; set; }
        public T Payload { get; set; }

    }
}
