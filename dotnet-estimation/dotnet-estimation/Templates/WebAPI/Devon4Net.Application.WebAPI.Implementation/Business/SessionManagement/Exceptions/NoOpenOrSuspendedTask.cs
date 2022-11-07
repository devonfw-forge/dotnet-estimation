using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions
{
    public class NoOpenOrSuspendedTask : Exception
    {
        public NoOpenOrSuspendedTask() : base($"The Session has no open or supended Taks") { }
    }
}
