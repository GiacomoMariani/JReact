using System;
using System.Collections.Generic;

namespace JReact
{
    public static class JBoolExtensions
    {
        public static bool And(this ICollection<Func<bool>> andConditions)
        {
            foreach (Func<bool> condition in andConditions)
            {
                if (!condition()) { return false; }
            }
            return true;
        }
        
        
        public static bool Or(this ICollection<Func<bool>> orConditions)
        {
            foreach (Func<bool> condition in orConditions)
            {
                if (condition()) { return true; }
            }
            return false;
        }
    }
}
