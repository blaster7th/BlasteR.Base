using System;
using System.Data;

namespace BlasteR.Base
{

    public interface IUnitOfWork : IDisposable
    {
        IDbConnection DB { get; }
        IDbTransaction Transaction { get; }
        string User { get; }
        void Commit();
        void Rollback();
    }

    public class UnitOfWork : IUnitOfWork
    {
        public IDbConnection DB { get; protected set; }
        public IDbTransaction Transaction { get; protected set; }
        public string User { get; protected set; }

        public UnitOfWork(IDbConnection db, IDbTransaction transaction, string user)
        {
            DB = db;
            Transaction = transaction;
            User = user;
        }

        public void Commit()
        {
            Transaction?.Commit();
        }

        public void Rollback()
        {
            Transaction?.Rollback();
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            if (DB?.State == ConnectionState.Open)
                DB?.Close();
            DB?.Dispose();
        }
    }
}