﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface IYellowCard
    {
        IEnumerable<YellowCard> Get(Guid? summaryId, bool IsHomeTeam);
        string Save(YellowCard updatedRecord);
        string Delete(YellowCard updatedRecord);
    }
}