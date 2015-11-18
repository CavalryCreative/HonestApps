using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface ISub
    {
        IEnumerable<Sub> Get(Guid? matchId, bool IsHomeTeam);
        string Save(Sub updatedRecord);
        string Delete(Sub updatedRecord);
    }
}
