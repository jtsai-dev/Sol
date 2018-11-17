using System.Web;

namespace JT.Infrastructure.AppCommon
{
    public class SessionHelper
    {
        #region add
        /// <summary>
        /// add session(rewrite if existed)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(string key, object value)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session[key] != null)
                    HttpContext.Current.Session.Remove(key);

                HttpContext.Current.Session.Add(key, value);
            }
        }
        #endregion

        #region get
        public static object Get(string key)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session[key] != null)
                    return HttpContext.Current.Session[key];
            }

            return null;
        }

        public static T Get<T>(string key)
        {
            //if (HttpContext.Current != null)
            //{
            //    if (HttpContext.Current.Session[key] != null)
            //        return (T)HttpContext.Current.Session[key];
            //}

            //return default(T);

            return (T)Get(key);
        }
        #endregion

        #region remove
        public static void Remove(string key)
        {
            if (HttpContext.Current != null)
            {
                if (HttpContext.Current.Session[key] != null)
                    HttpContext.Current.Session[key] = null;
            }
        }

        public static void Clear()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session.Clear();
            }
        }
        #endregion
    }
}
