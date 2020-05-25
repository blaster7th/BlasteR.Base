/*
    This library is intended as a starting point for creating Business Logic Layer in database oriented applications.
    Copyright (C) 2019 Srdjan Rudic
    Email: blaster7th@gmail.com

    This library is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this library.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BlasteR.Base
{
    public class BaseBLL
    {

    }

    /// <summary>
    /// Generic class for accessing data through BLL layer. Should be used as base class for every other BLL class.
    /// It has access to DbContext-derived class which acts as DAL.
    /// Every BLL class derived from BaseBLL can use DB (DAL) or BLL to get data.
    /// Note that using DAL enables EntityFramework to generate SQL queries (LINQ to SQL), while BLL does not (LINQ to objects).
    /// </summary>
    /// <typeparam name="T">Type used for accessing data.</typeparam>
    /// <typeparam name="U">DbContext derived type.</typeparam>
    public class BaseBLL<T, U> : BaseBLL
        where T : BaseEntity
        where U : DbContext
    {
        /// <summary>
        /// Used as DAL.
        /// </summary>
        public U DB { get; private set; }

        /// <summary>
        /// Constructor creates instance of the BaseBLL class.
        /// </summary>
        /// <param name="db">DbContext to work with.</param>
        public BaseBLL(U db)
        {
            try
            {
                DB = db;
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }
        }

        /// <summary>
        /// Gets single entity of type T, or default (null) if entity with that value does not exist.
        /// </summary>
        /// <param name="id">Id of the record.</param>
        /// <returns>Single or default value of type T.</returns>
        public virtual T GetById(int id)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            T result;
            try
            {
                result = DB.Set<T>().Find(id);
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Returns elements of type T which is contained in entityIds enumerable.
        /// </summary>
        /// <param name="entityIds">IEnumerable of entityIds which should be returned.</param>
        /// <returns>IEnumerable of requested entities.</returns>
        public IList<T> GetByIds(IEnumerable<int> entityIds)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            List<T> result = DB.Set<T>().Where(x => entityIds.Contains(x.Id)).OrderBy(x => x.CreatedTime).ToList();

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Gets all records of type T.
        /// </summary>
        /// <returns>IList of all entities of type T.</returns>
        public virtual IList<T> GetAll()
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            IList<T> result;
            try
            {
                result = DB.Set<T>().OrderBy(x => x.CreatedTime).ToList();
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Sets creation time and last edit time to the moment of insertion and inserts record to database.
        /// </summary>
        /// <param name="entity">Entity of type T to insert.</param>
        /// <returns>Newly inserted entity.</returns>
        public virtual T Insert(T entity, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            try
            {
                entity.CreatedTime = DateTime.Now;

                DB.Set<T>().Add(entity);

                if (persist)
                    DB.SaveChanges();
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return entity;
        }

        /// <summary>
        /// Inserts range of entities to the database.
        /// </summary>
        /// <param name="entities">Entities of type T to insert.</param>
        /// <returns>Number of entities inserted.</returns>
        public virtual int Insert(IEnumerable<T> entities, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            int result;
            try
            {
                DB.Set<T>().AddRange(entities);

                if (persist)
                    result = DB.SaveChanges();
                else
                    result = entities.Count();
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Inserts or updates entity of type T.
        /// </summary>
        /// <param name="entity">Entity of type T to insert or update.</param>
        /// <param name="persist">If true, entity will be persisted in DB, otherwise, it will only be saved to DBContext.</param>
        /// <returns>Newly saved entity.</returns>
        public virtual T Save(T entity, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            try
            {
                if (entity.Id != 0)
                {
                    entity.ModifiedTime = DateTime.Now;
                }

                DB.Set<T>().Update(entity);

                if (persist)
                    DB.SaveChanges();
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return entity;
        }

        /// <summary>
        /// Inserts or updates range of entities.
        /// </summary>
        /// <param name="entities">Entities of type T to insert or update.</param>
        /// <returns>Number of entities saved.</returns>
        public virtual int Save(IEnumerable<T> entities, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            int result;
            try
            {
                foreach (T entity in entities)
                {
                    if (entity.Id != 0)
                    {
                        entity.ModifiedTime = DateTime.Now;
                    }
                }

                DB.Set<T>().UpdateRange(entities);

                if (persist)
                    result = DB.SaveChanges();
                else
                    result = entities.Count();
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Deletes entity of type T from the database.
        /// </summary>
        /// <param name="id">Id of the entity to delete.</param>
        /// <returns>True if successfully deleted.</returns>
        public virtual bool Delete(int id, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            bool result = false;
            try
            {
                T entity = DB.Set<T>().Single(x => x.Id == id);
                if (entity != null)
                {
                    DB.Set<T>().Remove(entity);
                    if (persist)
                        result = DB.SaveChanges() > 0;
                    else
                        result = true;
                }
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Deletes entity of type T from the database.
        /// </summary>
        /// <param name="entity">Entity to delete.</param>
        /// <returns>True if successfully deleted.</returns>
        public virtual bool Delete(T entity, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            bool result = Delete(entity.Id, persist);

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Deletes range of entities from the database.
        /// </summary>
        /// <param name="entityIds">IEnumerable of entityIds to delete.</param>
        /// <returns>Number of deleted entities.</returns>
        public virtual int Delete(IEnumerable<int> entityIds, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            int result = 0;
            try
            {
                IEnumerable<T> entitiesToDelete = DB.Set<T>().Where(x => entityIds.Contains(x.Id));
                DB.Set<T>().RemoveRange(entitiesToDelete);
                if (persist)
                    result = DB.SaveChanges();
                else
                    result = entityIds.Count();
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Deletes range of entities from the database.
        /// </summary>
        /// <param name="entities">IEnumerable of entites to delete.</param>
        /// <returns>Number of deleted entities.</returns>
        public virtual int Delete(IEnumerable<T> entities, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            int result = Delete(entities.Select(y => y.Id).ToList());

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Deletes all entities of type T from the database.
        /// </summary>
        /// <returns>Number of deleted entities.</returns>
        public virtual int DeleteAll(bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            int result = 0;
            try
            {
                IEnumerable<T> entities = DB.Set<T>();
                DB.Set<T>().RemoveRange(entities);
                if (persist)
                    result = DB.SaveChanges();
                else
                    result = entities.Count();
            }
            catch (Exception ex)
            {
                BaseLogger.Log(LogLevel.Error, ex.Message, ex);
                throw ex;
            }

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Gets or sets entity of type T.
        /// </summary>
        /// <param name="id">Id of the entity to get or set.</param>
        /// <returns>Entity of type T.</returns>
        public virtual T this[int id] {
            get {
                return GetById(id);
            }
            set {
                Save(value);
            }
        }
    }
}
