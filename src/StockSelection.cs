using System;
using System.Collections.Generic;
namespace BaronShopCustomizer
{
    public static class StockSelection
    {
        public static T FindCompatible<T>(IList<T> pool, Func<T, bool> compatible, int offset) where T : class
        {
            if (pool == null || pool.Count == 0) return null;
            int start = ((offset % pool.Count) + pool.Count) % pool.Count;
            for (int i = 0; i < pool.Count; i++)
            {
                var item = pool[(start + i) % pool.Count];
                if (item != null && compatible(item)) return item;
            }
            return null;
        }
    }
}
