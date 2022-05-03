using System;
using System.Linq.Expressions;
using System.Text;

namespace ObjectAssertion
{
    public static class ObjectAssertionConfigurationExtensions
    {
        public static ObjectAssertionConfiguration FailIf(this ObjectAssertionConfiguration configuration,
            bool result = true)
        {
            configuration.FailIf = result;
            configuration.Message = null;
            configuration.WithDetails = false;
            return configuration;
        }

        public static ObjectAssertionConfiguration With(this ObjectAssertionConfiguration configuration, string message,
            bool withDetails = true)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    $"{nameof(message)} cannot be null, empty, or consists only of white-spaces", message);
            }

            if (!configuration.FailIf.HasValue)
            {
                throw new InvalidOperationException("You should set a fail condition at first");
            }

            configuration.Message = message;
            configuration.WithDetails = withDetails;
            return configuration;
        }

        public static ObjectAssertionConfiguration Except<T>(this ObjectAssertionConfiguration configuration,
            Expression<Func<T, object>> getPropertyExpression)
        {
            configuration.ExceptProperty(getPropertyExpression);
            return configuration;
        }

        public static bool AreEqual(this ObjectAssertionConfiguration configuration, object expected, object actual)
        {
            var result = ObjectAssert.AreEqual(configuration, expected, actual, out var message);
            if (configuration.FailIf != result)
            {
                return result;
            }

            throw CreateException(configuration, expected, actual, message, result);
        }

        public static bool AreNotEqual(this ObjectAssertionConfiguration configuration, object expected, object actual)
        {
            var result = !ObjectAssert.AreEqual(configuration, expected, actual, out var message);
            if (configuration.FailIf != result)
            {
                return result;
            }

            throw CreateException(configuration, expected, actual, message, result);
        }

        private static ObjectAssertionException CreateException(ObjectAssertionConfiguration configuration,
            object expected, object actual, string details, bool result)
        {
            var sb = new StringBuilder();
            if (configuration.Message != null)
            {
                sb.Append(configuration.Message);
            }
            else
            {
                sb.Append("Values ");
                sb.Append(result ? "are" : "aren't");
                sb.Append(" equal: ");
                sb.Append(expected ?? "<null>");
                sb.Append(" - ");
                sb.Append(actual ?? "<null>");
            }

            if (configuration.WithDetails && !string.IsNullOrWhiteSpace(details))
            {
                sb.Append(" ");
                sb.AppendLine("details: ");
                sb.Append(details);
            }

            return new ObjectAssertionException(sb.ToString());
        }
    }
}