using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LZA.Models
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        TEntity Get(int Id);
        IEnumerable<TEntity> GetAll();
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Update(TEntity entity);
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected IConnectionFactory _connectionFactory;

        public GenericRepository(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public void Add(TEntity entity)
        {
            using (var c = _connectionFactory.GetConnection())
            {
                c.Insert<TEntity>(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            using (var c = _connectionFactory.GetConnection())
            {
                c.Delete<TEntity>(entity);
            }
        }

        public void Update(TEntity entity)
        {
            using (var c = _connectionFactory.GetConnection())
            {
                c.Update<TEntity>(entity);
            }
        }

        public TEntity Get(int Id)
        {
            using (var c = _connectionFactory.GetConnection())
            {
                return c.Get<TEntity>(Id);
            }
        }

        public IEnumerable<TEntity> GetAll()
        {
            using (var c = _connectionFactory.GetConnection())
            {
                return c.GetList<TEntity>();
            }
        }
    }
}