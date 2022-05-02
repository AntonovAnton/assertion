using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectAssertion
{
    /// <summary>
    /// Assert that objects are equal by value
    /// </summary>
    public static class ObjectAssert
    {
        public static ObjectAssertionConfiguration That()
        {
            var configuration = new ObjectAssertionConfiguration();
            return configuration;
        }
        
        public static ObjectAssertionConfiguration FailIf(this ObjectAssertionConfiguration configuration, bool result = true)
        {
            configuration.FailIf = result;
            configuration.Message = null;
            configuration.WithDetails = false;
            return configuration;
        }

        public static ObjectAssertionConfiguration With(this ObjectAssertionConfiguration configuration, string message, bool withDetails = true)
        {
            if (!configuration.FailIf.HasValue)
            {
                throw new InvalidOperationException("You should set a fail condition at first");
            }
            configuration.Message = message;
            configuration.WithDetails = withDetails;
            return configuration;
        }

        public static ObjectAssertionConfiguration Except<T>(this ObjectAssertionConfiguration configuration, Expression<Func<T, object>> getPropertyExpression)
        {
            configuration.ExceptProperty(getPropertyExpression);
            return configuration;
        }

        public static bool AreEqual(object expected, object actual)
        {
            var configuration = new ObjectAssertionConfiguration();
            return configuration.AreEqual(expected, actual);
        }

        public static bool AreNotEqual(object expected, object actual)
        {
            var configuration = new ObjectAssertionConfiguration();
            return configuration.AreNotEqual(expected, actual);
        }

        public static bool AreEqual(this ObjectAssertionConfiguration configuration, object expected, object actual)
        {
            var result = AreEqual(configuration, expected, actual, out var message);
            if (configuration.FailIf != result)
            {
                return result;
            }

            var details = message;
            message = configuration.Message ?? $"Values {(result ? "are" : "aren't")} equal: {expected ?? "<null>"} - {actual ?? "<null>"}";
            if (configuration.WithDetails && details != null)
            {
                message = $"{message} {Environment.NewLine}details: {details}";
            }
            throw new ObjectAssertionException(message);

        }

        public static bool AreNotEqual(this ObjectAssertionConfiguration configuration, object expected, object actual)
        {
            var result = !AreEqual(configuration, expected, actual, out var message);
            if (configuration.FailIf != result)
            {
                return result;
            }

            var details = message;
            message = configuration.Message ?? $"Values {(result ? "are" : "aren't")} equal: {expected ?? "<null>"} - {actual ?? "<null>"}";
            if (configuration.WithDetails && details != null)
            {
                message = $"{message} {Environment.NewLine}details: {details}";
            }
            throw new ObjectAssertionException(message);

        }
        
        private static bool AreEqual(ObjectAssertionConfiguration configuration, object expected, object actual, out string message)
        {
            message = null;
            if (expected == null && actual == null)
            {
                return true;
            }

            if (expected == null || actual == null)
            {
                message = $"Values aren't equal: {expected ?? "<null>"} - {actual ?? "<null>"}";
                return false;
            }

            var expectedType = expected.GetType();
            var actualType = actual.GetType();
            if (expectedType != actualType)
            {
                message = $"{expectedType} isn't {actualType}";
                return false;
            }

            if (expectedType.IsValueType || expected is string)
            {
                if (Equals(expected, actual))
                {
                    return true;
                }

                message = $"Values aren't equal: {expected} - {actual}";
                return false;
            }

            if (ReferenceEquals(expected, actual))
            {
                return true;
            }

            if (typeof(IDictionary).IsAssignableFrom(expectedType))
            {
                return AreEqual(configuration, (IDictionary)expected, (IDictionary)actual, out message);
            }

            if (typeof(IEnumerable).IsAssignableFrom(expectedType))
            {
                return AreEqual(configuration, (IEnumerable)expected, (IEnumerable)actual, out message);
            }

            foreach (var p in expectedType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (configuration.IsExcepted(p))
                {
                    continue;
                }

                var value1 = p.GetValue(expected, null);
                var p2 = actualType.GetProperty(p.Name, BindingFlags.Public | BindingFlags.Instance);
                if (p2 == null)
                {
                    message = $"There isn't a field {p.Name} in the object {actualType}";
                    return false;
                }

                var value2 = p2.GetValue(actual, null);
                if (value1 == null || value2 == null)
                {
                    if (value1 != null || value2 != null)
                    {
                        message = $"Not equals {p.DeclaringType?.Name}.{p.Name}, values: {value1 ?? "<null>"} - {value2 ?? "<null>"}";
                        return false;
                    }
                    continue;
                }

                try
                {
                    if (!AreEqual(configuration, value1, value2, out message))
                    {
                        message = $"Not equals {p.DeclaringType?.Name}.{p.Name} {Environment.NewLine}details: {message}";
                        return false;
                    }
                }
                catch (ObjectAssertionException exception)
                {
                    message = $"Not equals {p.DeclaringType?.Name}.{p.Name} {Environment.NewLine}details: {exception.Message}";
                    return false;
                }
            }

            return true;
        }

        private static bool AreEqual(ObjectAssertionConfiguration configuration, IDictionary expected, IDictionary actual, out string message)
        {
            if (expected.Count != actual.Count)
            {
                message = $"Count of elements is different, {expected.Count} and {actual.Count}";
                return false;
            }

            foreach (var key in expected.Keys)
            {
                var el1 = expected[key];
                object el2;
                if (actual.Contains(key))
                {
                    el2 = actual[key];
                }
                else
                {
                    message = $"A value wasn't found by {key}";
                    return false;
                }

                try
                {
                    if (AreEqual(configuration, el1, el2, out message))
                    {
                        continue;
                    }

                    message = $"Elements of dictionaries by {key} are not equal {Environment.NewLine}details: {message}";
                    return false;
                }
                catch (ObjectAssertionException exception)
                {
                    message = $"Elements of dictionaries by {key} are not equal {Environment.NewLine}details: {exception.Message}";
                    return false;
                }
            }

            message = null;
            return true;
        }

        private static bool AreEqual(ObjectAssertionConfiguration configuration, IEnumerable expected, IEnumerable actual, out string message)
        {
            if (expected is ICollection col1 && actual is ICollection col2)
            {
                if (col1.Count != col2.Count)
                {
                    message = $"Count of elements is different, {col1.Count} and {col2.Count}";
                    return false;
                }
            }

            var enum1 = expected.GetEnumerator();
            var enum2 = actual.GetEnumerator();

            var index = 0;
            while (enum1.MoveNext())
            {
                if (!enum2.MoveNext())
                {
                    message = $"Count of elements of sequences are not equal, {expected} more than {actual}";
                    return false;
                }
                try
                {
                    if (!AreEqual(configuration, enum1.Current, enum2.Current, out message))
                    {
                        message = $"Elements of sequences by index [{index}] are not equal {Environment.NewLine}details: {message}";
                        return false;
                    }

                    index++;
                }
                catch (ObjectAssertionException exception)
                {
                    message = $"Elements of sequences by index [{index}] are not equal {Environment.NewLine}details: {exception.Message}";
                    return false;
                }
            }

            if (enum2.MoveNext())
            {
                message = $"Count of elements of sequences are not equal, {expected} less than {actual}";
                return false;
            }

            message = null;
            return true;
        }
    }
}
