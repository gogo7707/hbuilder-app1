using System.Collections.Generic;
using System.Data;

namespace Randao.Core.Data
{
    internal static class EntityHelper
    {
        public static List<T> GetList<T>(this IDataReader dr)
        {
            var list = new List<T>();

            if (null == dr)
            {
                return list;
            }

            var eblist = IDataReaderEntityBuilder<T>.CreateBuilder(dr);

            while (dr.Read())
            {
                list.Add(eblist.Build(dr));
            }
            return list;
        }
        /// <returns></returns>
		public static T GetEntity<T>(this IDataReader dr)
		{
			if (null == dr)
			{
				return default(T);
			}
			if (dr.Read())
			{
				return IDataReaderEntityBuilder<T>.CreateBuilder(dr).Build(dr);
			}
			return default(T);
		}

        public static T GetEntityByReaderNoCache<T>(this IDataReader dr)
        {
            if (null == dr)
            {
                return default(T);
            }
            return IDataReaderEntityBuilder<T>.CreateBuilderNoCache(dr).Build(dr);
        }
    }
}
