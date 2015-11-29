using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface IMatch
    {
        IEnumerable<Match> Get(Guid? matchId);
        IEnumerable<Match> GetByAPIId(int id);
        string Save(Match updatedRecord);
        string SaveTeam(Match updatedRecord, int teamAPIId);
    }
}
