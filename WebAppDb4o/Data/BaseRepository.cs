using System;

namespace WebAppDb4o.Data
{
    public class BaseRepository
    {
        /// <summary>
        /// Filename
        /// </summary>
        protected readonly string Path = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\dbase.db4o";
        /// <summary>
        /// AutoMapper
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="proto"></param>
        protected void ObjectMapper<T>(T model, T proto)
        {
            foreach (var propertyInfo in model.GetType().GetProperties())
            {
                var property = proto.GetType().GetProperty(propertyInfo.Name);
                if (property != null)
                    property.SetValue(proto, propertyInfo.GetValue(model, null), null);
            }
        }
    }
}