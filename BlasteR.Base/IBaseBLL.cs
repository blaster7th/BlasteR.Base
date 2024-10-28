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

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BlasteR.Base
{
    public interface IBaseBLL<T, U>
        where T : BaseEntity
        where U : DbContext
    {
        T this[int id] { get; set; }

        U DB { get; }

        int Delete(IEnumerable<int> entityIds, bool persist = false);
        int Delete(IEnumerable<T> entities, bool persist = false);
        bool Delete(int id, bool persist = false);
        bool Delete(T entity, bool persist = false);
        int DeleteAll(bool persist = false);
        IList<T> GetAll();
        T GetById(int id);
        IList<T> GetByIds(IEnumerable<int> entityIds);
        int Insert(IEnumerable<T> entities, bool persist = false);
        T Insert(T entity, bool persist = false);
        int Save(IEnumerable<T> entities, bool persist = false);
        T Save(T entity, bool persist = false);
    }
}