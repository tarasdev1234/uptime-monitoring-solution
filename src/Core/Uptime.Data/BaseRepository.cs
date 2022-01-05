using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Uptime.Data {
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class {
        private readonly ApplicationDbContext dbContext;
        protected DbSet<TEntity> dbSet;

        public BaseRepository (ApplicationDbContext ctx) {
            dbContext = ctx;
            //dbSet = dbContext.Set<TEntity>();
        }
    }
}
