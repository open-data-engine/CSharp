﻿using System;

namespace OpenDataEngine.Schema
{
    public abstract class Base : ISchema
    {
        public Source.Source Source { get; set; }

        public abstract String ResolvePath(String path);

        public virtual String ResolveProperty(String property, Boolean alias = false)
        {
            return property;
        }
        public virtual String ReverseResolveProperty(String property)
        {
            return property;
        }
    }
}
