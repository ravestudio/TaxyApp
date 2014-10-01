using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxyApp.Core
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;

        protected Singleton()
        {
        }

        //private static T CreateInstance()
        //{
        //    ConstructorInfo cInfo = typeof(T).GetConstructor(
        //        BindingFlags.Instance | BindingFlags.NonPublic,
        //        null,
        //        new Type[0],
        //        new ParameterModifier[0]);

        //    return (T)cInfo.Invoke(null);
        //}

        private static T CreateInstance()
        {
            return new T();
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance();
                }

                return _instance;
            }
        }
    }
}
