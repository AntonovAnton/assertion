using System.Collections.Generic;
using Xunit;

namespace ObjectAssertion.Tests
{
    public class ObjectAssertTests
    {
        [Theory]
        [MemberData(nameof(EqualObjects))]
        public void ObjectAssert_AreEqual_True(object expected, object actual)
        {
            Assert.True(ObjectAssert.AreEqual(expected, actual));
        }

        [Theory]
        [MemberData(nameof(NotEqualObjects))]
        public void ObjectAssert_AreEqual_False(object expected, object actual)
        {
            Assert.False(ObjectAssert.AreEqual(expected, actual));
        }

        [Theory]
        [MemberData(nameof(NotEqualObjects))]
        public void ObjectAssert_AreNotEqual_True(object expected, object actual)
        {
            Assert.True(ObjectAssert.AreNotEqual(expected, actual));
        }

        [Theory]
        [MemberData(nameof(EqualObjects))]
        public void ObjectAssert_AreNotEqual_False(object expected, object actual)
        {
            Assert.False(ObjectAssert.AreNotEqual(expected, actual));
        }

        public static IEnumerable<object[]> EqualObjects
        {
            get
            {
                yield return new object[] { null, null };
                yield return new[] { new object(), new object() };
                yield return new object[] { 123, 123 };
                yield return new object[] { "test string", "test string" };
                yield return new object[] { new object[] { 123, "test string" }, new object[] { 123, "test string" } };
                yield return new object[]
                {
                    new { number = 123, testString = "test string" }, new { number = 123, testString = "test string" }
                };
            }
        }

        public static IEnumerable<object[]> NotEqualObjects
        {
            get
            {
                yield return new[] { null, new object() };
                yield return new[] { new object(), null };
                yield return new object[] { 123, -123 };
                yield return new object[] { "test string", " " };
                yield return new object[] { new object[] { 123, "test string" }, new object[] { "test string", 123 } };
                yield return new object[]
                {
                    new { number = 123, testString = "test string" }, new { testString = "test string", number = 123 }
                };
            }
        }
    }
}