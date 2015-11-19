using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface IStat
    {
        IEnumerable<Stat> Get(Guid? matchId);
        string Save(Stat updatedRecord);
        string Delete(Stat updatedRecord);
    }
}
