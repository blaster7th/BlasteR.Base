/*
    This library is intended as a starting point for creating Business Logic Layer in database oriented applications.
    Copyright (C) 2017 Srdjan Rudic
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
#if NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETSTANDARD2_0
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif
using System.Linq;
using System.Reflection;

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
        /// <param name="id">ID of the record.</param>
        /// <returns>Single or default value of type T.</returns>
        public virtual T GetByID(int id)
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
        /// Gets all records of type T.
        /// </summary>
        /// <returns>IList of all entities of type T.</returns>
        public virtual IList<T> GetAll()
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            IList<T> result;
            try
            {
                result = DB.Set<T>().OrderBy(x => x.CreationTime).ToList();
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
        /// Returns elements of type T which is contained in entityIDs enumerable.
        /// </summary>
        /// <param name="entityIDs">Enumerable of entityIDs which should be returned.</param>
        /// <returns>IList of requested entities.</returns>
        public IList<T> GetByIDList(IEnumerable<int> entityIDs)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            List<T> result = DB.Set<T>().Where(x => entityIDs.Contains(x.ID)).OrderBy(x => x.CreationTime).ToList();

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
                PrivateInsert(entity, true);
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

        private void PrivateInsert(T entity, bool addToDbContext)
        {
            entity.CreationTime = DateTime.Now;
            entity.LastEditTime = null;

            foreach (var propertyInfo in entity.GetType().GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(BaseEntity))))
            {
                if (propertyInfo.GetValue(entity, null) != null)
                    PrivateSaveProperty(entity, propertyInfo);
            }

            if (addToDbContext)
                DB.Set<T>().Add(entity);
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
                foreach (T entity in entities)
                {
                    entity.CreationTime = DateTime.Now;
                    entity.LastEditTime = null;

                    PrivateInsert(entity, true);
                }

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
            return Save(entity, true, persist);
        }

        /// <summary>
        /// Inserts or updates entity of type T.
        /// </summary>
        /// <param name="entity">Entity of type T to insert or update.</param>
        /// <param name="addToDbContext">True if entity should be added to DbContext.</param>
        /// <param name="persist">If true, entity will be persisted in DB, otherwise, it will only be saved to DBContext.</param>
        /// <returns>Newly saved entity.</returns>
        public T Save(T entity, bool addToDbContext, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            try
            {
                IEnumerable<PropertyInfo> propertyInfos = typeof(T).GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(BaseEntity)));

                entity = PrivateSave(entity, propertyInfos, addToDbContext);
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

        private T PrivateSave(T entity, IEnumerable<PropertyInfo> propertyInfos, bool addToDbContext)
        {
            // Set last edit time to time of saving.
            entity.LastEditTime = DateTime.Now;

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetValue(entity, null) != null)
                    PrivateSaveProperty(entity, propertyInfo);
            }

            // Detached entity is not being tracked by the EntityFramework (new entity).
            if (DB.Entry<T>(entity).State == EntityState.Detached && entity.ID == 0)
            {
                PrivateInsert(entity, addToDbContext);
            }
            else
            {
                // Entity needs to be updated.
                try
                {
                    DB.Entry(entity).State = EntityState.Modified;
                }
                catch (InvalidOperationException)
                {
                    T attachedEntity = DB.Set<T>().Single(x => x.ID == entity.ID);

                    // Guard if CreationTime is not passed.
                    if (entity.CreationTime == DateTime.MinValue)
                        entity.CreationTime = attachedEntity.CreationTime;

                    DB.Entry<T>(attachedEntity).CurrentValues.SetValues(entity);
                    CopyBaseEntityProperties(entity, attachedEntity);

                    entity = attachedEntity;
                }
            }

            return entity;
        }

        private void CopyBaseEntityProperties(T entityFrom, T entityTo)
        {
            IEnumerable<PropertyInfo> propertyInfos = typeof(T).GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(BaseEntity)));
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object value = propertyInfo.GetValue(entityFrom, null);
                PropertyInfo piNavigationID = typeof(T).GetProperty(propertyInfo.Name + "ID");
                int? valueID = (int?)piNavigationID.GetValue(entityFrom, null);
                if (value != null || !valueID.HasValue || valueID == 0)
                {
                    propertyInfo.SetValue(entityTo, value, null);
                }
            }
        }

        private void PrivateSaveProperty(T entity, PropertyInfo propertyInfo)
        {
            var baseBLLType = typeof(BaseBLL<,>);
            var genericBLLType = baseBLLType.MakeGenericType(propertyInfo.PropertyType, typeof(U));

            Type specificBLLType = BLLTypes.SingleOrDefault(x => x.IsSubclassOf(genericBLLType));

            if (specificBLLType == null)
            {
                specificBLLType = BLLTypes.SingleOrDefault(x => x.Name == propertyInfo.PropertyType.Name + "BLL");
            }

            object materializedBLL = MaterializedBLLs.Where(x => x.GetType() == specificBLLType).SingleOrDefault();

            if (materializedBLL == null)
            {
                materializedBLL = Activator.CreateInstance(genericBLLType, DB);
            }

            object propertyValueToSave = propertyInfo.GetValue(entity, null);

            // If there is a value and this value is changed or not tracked, save it using it's BLL class.
            if (propertyValueToSave != null && DB.Entry(propertyValueToSave).State != EntityState.Unchanged)
            {
                MethodInfo methodToInvoke;
                if (specificBLLType != null)
                    methodToInvoke = specificBLLType.GetMethod("Save", new Type[] { propertyInfo.PropertyType, typeof(bool), typeof(bool) });
                else
                    methodToInvoke = genericBLLType.GetMethod("Save", new Type[] { propertyInfo.PropertyType, typeof(bool), typeof(bool) });

                object savedProperty = methodToInvoke.Invoke(materializedBLL, new object[] { propertyValueToSave, false, false });
                propertyInfo.SetValue(entity, savedProperty, null);
            }
        }

        /// <summary>
        /// Inserts or updates range of entities.
        /// </summary>
        /// <param name="entities">Entities of type T to insert or update.</param>
        /// <returns>Number of entities saved.</returns>
        public virtual int Save(IEnumerable<T> entities, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            int result = 0;
            try
            {
                IEnumerable<PropertyInfo> propertyInfos = typeof(T).GetProperties().Where(x => x.PropertyType.IsSubclassOf(typeof(BaseEntity)));

                // Since DB.SaveChanges() is called only once, this is not slow at all.
                foreach (T entity in entities)
                {
                    PrivateSave(entity, propertyInfos, true);
                }

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
        /// <param name="entity">Entity to delete.</param>
        /// <returns>True if successfully deleted.</returns>
        public virtual bool Delete(T entity, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            bool result = Delete(entity.ID, persist);

            BaseLogger.LogMethodEnd(this, methodStart);
            return result;
        }

        /// <summary>
        /// Deletes entity of type T from the database.
        /// </summary>
        /// <param name="id">ID of the entity to delete.</param>
        /// <returns>True if successfully deleted.</returns>
        public virtual bool Delete(int id, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            bool result = false;
            try
            {
                T entity = DB.Set<T>().Single(x => x.ID == id);
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
        /// Deletes range of entities from the database.
        /// </summary>
        /// <param name="entities">Range of entities to delete.</param>
        /// <returns>Number of deleted entities.</returns>
        public virtual int Delete(IEnumerable<T> entities, bool persist = false)
        {
            DateTime methodStart = BaseLogger.LogMethodStart(this);

            int result = 0;
            try
            {
                IEnumerable<int> idsToDelete = entities.Select(y => y.ID).ToList();
                IEnumerable<T> entitiesToDelete = DB.Set<T>().Where(x => idsToDelete.Contains(x.ID));
                DB.Set<T>().RemoveRange(entitiesToDelete);
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
        /// <param name="ID">ID of the entity to get or set.</param>
        /// <returns>Entity of type T.</returns>
        public virtual T this[int ID] {
            get {
                return GetByID(ID);
            }
            set {
                Save(value);
            }
        }

        private static List<Type> bllTypes;
        public static List<Type> BLLTypes {
            get {
                if (bllTypes == null)
                {
                    bllTypes = new List<Type>();
                    var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(y => y.IsSubclassOf(typeof(BaseBLL)))).ToArray();
                    foreach (var bllType in allTypes)
                    {
                        if (bllType.IsGenericType)
                        {
                            foreach (var entityType in EntityTypes)
                            {
                                var genericBLLType = bllType.MakeGenericType(entityType, typeof(U));
                                bllTypes.Add(genericBLLType);
                            }
                        }
                        else
                        {
                            bllTypes.Add(bllType);
                        }
                    }
                }

                return bllTypes;
            }
        }

        private static Type[] entityTypes;
        public static Type[] EntityTypes {
            get {
                if (entityTypes == null)
                {
                    entityTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes().Where(y => y.IsSubclassOf(typeof(BaseEntity)))).ToArray();
                }

                return entityTypes;
            }
        }

        private List<object> materializedBLLs;
        public List<object> MaterializedBLLs {
            get {
                if (materializedBLLs == null)
                {
                    materializedBLLs = new List<object>();

                    foreach (var bllType in BLLTypes)
                    {
                        materializedBLLs.Add(Activator.CreateInstance(bllType, DB));
                    }
                }

                return materializedBLLs;
            }
        }
    }
}
