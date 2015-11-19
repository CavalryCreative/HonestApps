using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface IGoal
    {
        IEnumerable<Goal> Get(Guid? summaryId, bool IsHomeTeam);
        string Save(Goal updatedRecord);
        string Delete(Goal updatedRecord);
    }
}
