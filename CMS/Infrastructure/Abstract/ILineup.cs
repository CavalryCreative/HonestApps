using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface ILineup
    {
        IEnumerable<Lineup> Get(int matchId, bool isHomePlayer, bool isSub);
        string Save(Lineup updatedRecord);
        string Delete(Lineup updatedRecord);
    }
}
