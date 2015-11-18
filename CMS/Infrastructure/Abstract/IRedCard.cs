using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface IRedCard
    {
        IEnumerable<RedCard> Get(Guid? summaryId, bool IsHomeTeam);
        string Save(RedCard updatedRecord);
        string Delete(RedCard updatedRecord);
    }
}
