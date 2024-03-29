﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CMS.Infrastructure.Entities;

namespace CMS.Infrastructure.Abstract
{
    public interface ISummary
    {
        IEnumerable<Summary> Get(Guid? matchId);
        string Save(Summary updatedRecord);
        string Delete(Summary updatedRecord);
    }
}
