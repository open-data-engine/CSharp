using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine.Query.Clause
{
    public interface IClause
    {
    }

    public static class ClauseExtensions
    {
        public static T Is<T>(this T subject, String value) where T : IClause
        {
            return subject;
        }
    }
}
