using System;
using System.Linq.Expressions;

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
            object expected, object actual, string message, bool result)
        {
            var details = message;
            message = configuration.Message ??
                      $"Values {(result ? "are" : "aren't")} equal: {expected ?? "<null>"} - {actual ?? "<null>"}";
            if (configuration.WithDetails && !string.IsNullOrEmpty(details))
            {
                message = $"{message} {Environment.NewLine}details: {details}";
            }

            return new ObjectAssertionException(message);
        }
    }
}