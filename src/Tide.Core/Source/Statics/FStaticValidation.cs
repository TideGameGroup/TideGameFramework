using System;
using System.Collections.Generic;
using System.Text;

namespace Tide.Core
{
    public class FStaticValidation
    {
        /// <summary>
        /// This function is syntactic sugar for checking passed values are non-null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="o"></param>
        public static void NullCheck<T>(T t)
        {
            if (t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }
        }

        /// <summary>
        /// This function is syntactic sugar for checking passed values are non-null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="o"></param>
        public static void TrySetDefault<T>(T t, out T o)
        {
            o = t ?? throw new ArgumentNullException(nameof(t));
        }

        /// <summary>
        /// This function is syntactic sugar for setting passed arguments
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="o"></param>
        public static void TrySetOptional<T>(T t, out T o)
        {
            o = t;
        }
    }
}
