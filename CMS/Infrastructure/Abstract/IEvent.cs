using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface IEvent
    {
        IEnumerable<Event> Get(Guid? matchId);
        string Save(Event updatedRecord);
        string Delete(Event updatedRecord);
    }
}
