using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface IEleven
    {
        IEnumerable<Eleven> Get(Guid? matchId, bool IsHomeTeam);
        string Save(Eleven updatedRecord);
        string Delete(Eleven updatedRecord);
    }
}
