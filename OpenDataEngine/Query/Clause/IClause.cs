using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine.Query.Clause
{
    public interface IClause
    {
    }

    public sealed class Base: IClause
    {
        public static implicit operator Where(Base v) => new Where("");
    }

    public static class ClauseExtensions
    {
        public static T Is<T>(this Field subject, String value)
        {
            return ((Base)subject).Is<T>(value);
        }

        public static T Is<T>(this Field subject, String key, Object value)
        {
            return ((Base)subject).Is<T>(key, value);
        }

        public static Base Is<T>(this Field<T> subject, String value)
        {
            return subject;
        }

        public static Base Is<T>(this Field<T> subject, String key, Object value)
        {
            return subject;
        }

        public static T Is<T>(this Base subject, String value)
        {
            return subject.Is<T>(value);
        }

        public static T Is<T>(this Base subject, String key, Object value)
        {
            return subject.Is<T>(key, value);
        }

        public static T Is<T>(this T subject, String value) where T : IClause
        {
            return subject;
        }

        public static T Is<T>(this T subject, String key, Object value) where T : IClause
        {
            return subject;
        }
    }
}
