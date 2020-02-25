using System;

namespace OpenDataEngine.Query.Clause
{
    public class Select : IClause
    {
        private Field[] fields;

        public Select(params Field[] fields)
        {
            this.fields = fields;
        }

        public static implicit operator Select(Field field) => new Select(field);
    }
}
