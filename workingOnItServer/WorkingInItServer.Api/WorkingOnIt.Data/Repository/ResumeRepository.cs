﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkingOnIt.Core.Entities;
using WorkingOnIt.Core.InterfaceRepository;

namespace WorkingOnIt.Data.Repository
{
    public class ResumeRepository : RepositoryGeneric<Resume>, IResumeRepository
    {
        public ResumeRepository(DataContext context) : base(context)
        {
        }

            public async Task<Resume?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(r => r.UserId == userId)
                .FirstOrDefaultAsync();
        }
    }
 }

