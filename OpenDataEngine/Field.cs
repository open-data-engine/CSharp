﻿using OpenDataEngine.Query.Clause;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenDataEngine
{
    public class Field
    {
        public String Name { get; protected set; }
        public dynamic Value { get; protected set; }

        public static implicit operator String(Field self) => "";

        public static implicit operator Base(Field self) => new Base();
    }

    public class Field<T>: Field
    {
        public new T Value { get; protected set; }

        public static implicit operator Base(Field<T> self) => new Base();
    }
}