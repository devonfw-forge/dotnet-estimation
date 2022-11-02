using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(long id) : base($"Failed to find element matching {id}") { }
    }

}
