using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebAppDb4o.Models;

namespace WebAppDb4o.Data
{
    public interface ICrud<T> where T : class
    {
        /// <summary>
        /// Add
        /// </summary>
        /// <param name="model"></param>
        void Add(T model);
        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="model"></param>
        void Edit(T model);
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="model"></param>
        void Delete(T model);
        /// <summary>
        /// Find
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        T Find(T model);
        ParallelQuery<T> ParallelQuery();
        /// <summary>
        /// All
        /// </summary>
        /// <returns></returns>
        List<Person> All();
    }
}