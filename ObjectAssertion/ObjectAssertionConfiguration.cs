using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectAssertion
{
    public class ObjectAssertionConfiguration
    {
        private readonly Dictionary<Type, List<PropertyInfo>> _exceptedProperties =
            new Dictionary<Type, List<PropertyInfo>>();

        public bool? FailIf { get; set; }
        public string Message { get; set; }
        public bool WithDetails { get; set; }

        public void ExceptProperty<T>(Expression<Func<T, object>> getPropertyExpression)
        {
            if (getPropertyExpression == null)
            {
                throw new ArgumentNullException(nameof(getPropertyExpression));
            }

            var property = GetProperty(getPropertyExpression);
            var type = typeof(T);
            if (type != property.DeclaringType)
            {
                var message =
                    $"Expression isn't correct because of property {property.Name} there isn't in {type.Name}";
                throw new ArgumentException(message);
            }

            if (_exceptedProperties.TryGetValue(type, out var properties))
            {
                properties.Add(property);
            }
            else
            {
                _exceptedProperties.Add(type, new List<PropertyInfo> { property });
            }
        }

        public void IncludeProperty<T>(Expression<Func<T, object>> getPropertyExpression)
        {
            if (getPropertyExpression == null)
            {
                throw new ArgumentNullException(nameof(getPropertyExpression));
            }

            var type = typeof(T);
            if (!_exceptedProperties.TryGetValue(type, out var properties))
            {
                return;
            }

            var property = GetProperty(getPropertyExpression);
            if (type != property.DeclaringType)
            {
                var message =
                    $"Expression isn't correct because of property {property.Name} there isn't in {type.Name}";
                throw new ArgumentException(message);
            }

            properties.Remove(property);
            if (properties.Count == 0)
            {
                _exceptedProperties.Remove(type);
            }
        }

        public bool IsExcepted(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return property.DeclaringType != null &&
                   _exceptedProperties.TryGetValue(property.DeclaringType, out var properties) &&
                   properties.Contains(property);
        }

        private static PropertyInfo GetProperty<TParam>(Expression<Func<TParam, object>> memberInfo)
        {
            return (PropertyInfo)GetMember(memberInfo);
        }

        private static MemberInfo GetMember(Expression expression)
        {
            MemberInfo member;
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    var lambdaExpression = (LambdaExpression)expression;
                    member = GetMember(lambdaExpression.Body);
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = (MemberExpression)expression;
                    member = memberExpression.Member;
                    break;
                case ExpressionType.Call:
                    var callExpression = (MethodCallExpression)expression;
                    member = callExpression.Method;
                    break;
                case ExpressionType.Convert:
                    var unaryExpression = (UnaryExpression)expression;
                    member = GetMember(unaryExpression.Operand);
                    break;
                case ExpressionType.Parameter:
                    member = null;
                    break;
                default:
                    const string message = "The expression is not a member access or method call expression";
                    throw new ArgumentException(message);
            }

            return member;
        }
    }
}