﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline.Conversion;

namespace Baseline.Binding
{
    public class BoundProperty : BoundMember
    {
        private readonly PropertyInfo _property;

        public BoundProperty(PropertyInfo property) : base(property.PropertyType, property)
        {
            _property = property;
        }

        public static IEnumerable<IBoundMember> FindMembers(Type type, Conversions conversions)
        {
            var allProps = type.GetProperties()
                .Where(x => x.CanWrite).ToArray();

            var simples = allProps
                .Where(x => conversions.Has(x.PropertyType))
                .Select(x => new BoundProperty(x));

            var nested =
                allProps.Where(x => !conversions.Has(x.PropertyType) && x.PropertyType.IsConcreteWithDefaultCtor())
                .Select(x => typeof(NestedPropertyMember<>).CloseAndBuildAs<IBoundMember>(conversions, x, x.PropertyType));

            return simples.Concat(nested);
        }

        protected override Expression toSetter(Expression target, Expression value)
        {
            var method = _property.SetMethod;

            return Expression.Call(target, method, value);
        }
    }
}