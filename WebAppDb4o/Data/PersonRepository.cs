using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Db4objects.Db4o;
using WebAppDb4o.Models;

namespace WebAppDb4o.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class PersonRepository : BaseRepository, ICrud<Person>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="person"></param>
        public void Add(Person person)
        {

            using (var db = Db4oFactory.OpenFile(Path))
            {
                db.Store(person);
                db.Commit();
                db.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Person> All()
        {
            var lista = new List<Person>();
            using (var db = Db4oFactory.OpenFile(Path))
            {
                var result = db.QueryByExample(new Person(null, null, null, 0, null));
                while (result != null && result.HasNext()) lista.Add((Person)result.Next());

                db.Close();
            }
            return lista;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void Delete(Person model)
        {
            using (var db = Db4oFactory.OpenFile(Path))
            {
                var result = db.QueryByExample(new Person {RowGuid = model.RowGuid});
                var proto = (Person) result[0];
                db.Delete(proto);
                db.Commit();
                db.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        public void Edit(Person model)
        {
            using (var db = Db4oFactory.OpenFile(Path))
            {
                var result = db.QueryByExample(new Person { RowGuid = model.RowGuid });
                var proto = (Person)result[0];
                ObjectMapper(model, proto);
                db.Store(proto);
                db.Commit();
                db.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public Person Find(Person model)
        {
            Person proto;
            using (var db = Db4oFactory.OpenFile(Path))
            {
                var result = db.QueryByExample(model);
                proto = (Person)result[0];
                db.Close();
            }
            return proto;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ParallelQuery<Person> ParallelQuery()
        {
            var lista = new List<Person>();
            using (var db = Db4oFactory.OpenFile(Path))
            {
                var result = db.QueryByExample(new Person(null, null, null, 0, null));
                while (result != null && result.HasNext()) lista.Add((Person)result.Next());
                db.Close();
            }
            return lista.AsParallel();
        }

        

        
    }
}