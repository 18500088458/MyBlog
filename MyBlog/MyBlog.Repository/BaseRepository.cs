using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MyBlog.IRepository;
using SqlSugar;
using SqlSugar.IOC;
using MyBlog.Model;

namespace MyBlog.Repository
{
    public class BaseRepository<TEntity> : SimpleClient<TEntity>, IBaseRepository<TEntity> where TEntity : class, new()
    {
        public BaseRepository(ISqlSugarClient context = null) : base(context)
        {
            base.Context = DbScoped.Sugar;

            //base.Context.DbMaintenance.CreateDatabase();
            //base.Context.CodeFirst.InitTables(
            //    typeof(BlogNews),
            //    typeof(WriterInfo),
            //    typeof(TypeInfo)
            //    );

            //if (context == null)
            //{
            //    base.Context = new SqlSugarClient(new ConnectionConfig()
            //    {
            //        DbType = SqlSugar.DbType.SqlServer,
            //        InitKeyType = InitKeyType.Attribute,
            //        IsAutoCloseConnection = true,
            //        ConnectionString = Config.ConnectionString
            //    });
            //}
        }

        public async Task<bool> CreateAsync(TEntity entity)
        {
            return await base.InsertAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await base.DeleteByIdAsync(id);
        }

        public async Task<bool> EditAsync(TEntity entity)
        {
            return await base.UpdateAsync(entity);
        }

        public async Task<TEntity> FindAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        public async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> func)
        {
            return await base.GetSingleAsync(func);
        }

        public async virtual Task<List<TEntity>> QueryAsync()
        {
            return await base.GetListAsync();
        }

        public async virtual Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func)
        {
            return await base.Context.Queryable<TEntity>().Where(func).ToListAsync();
        }

        public async virtual Task<List<TEntity>> QueryAsync(int page, int size, RefAsync<int> total)
        {
            return await base.Context.Queryable<TEntity>().ToPageListAsync(page, size, total);
        }

        public async virtual Task<List<TEntity>> QueryAsync(Expression<Func<TEntity, bool>> func, int page, int size, RefAsync<int> total)
        {
            return await base.Context.Queryable<TEntity>().Where(func).ToPageListAsync(page, size, total);
        }        
    }
}
