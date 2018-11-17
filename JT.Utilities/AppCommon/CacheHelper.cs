using System;
using System.Runtime.Caching;

namespace JT.Infrastructure.AppCommon
{
    public class CacheHelper
    {
        #region private
        private static ObjectCache _globalCache = new MemoryCache("Global");
        #endregion

        #region add
        /// <summary>
        /// add cache(rewrite if existed)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(string key, object value)
        {
            Add(key, value, DateTimeOffset.MaxValue);
        }

        /// <summary>
        /// add cache(rewrite if existed)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="timeOffset"></param>
        public static void Add(string key, object value, DateTimeOffset timeOffset)
        {
            if (_globalCache.Contains(key))
                _globalCache.Remove(key);

            _globalCache.Add(key, value, timeOffset);
        }
        #endregion

        #region get
        public static object Get(string key)
        {
            return _globalCache.Get(key);
        }

        public static T Get<T>(string key)
        {
            if (_globalCache.Contains(key))
                return (T)_globalCache[key];

            return default(T);
        }
        #endregion

        #region remove
        public static void Remove(string key)
        {
            _globalCache.Remove(key);
        }

        public static void Clear()
        {
            foreach (var item in _globalCache)
            {
                _globalCache.Remove(item.Key);
            }
        }
        #endregion
    }
}
