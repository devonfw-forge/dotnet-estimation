using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devon4Net.Application.WebAPI.Implementation.Business.SessionManagement.Service
{
    public enum MessageType
    {
        TaskCreated,
        TaskStatusModified,
        TaskDeleted,
        EstimationAdded,
        UserJoined,
        UserRefreshed,
    }
}
