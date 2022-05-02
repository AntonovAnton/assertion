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
            var condition = ObjectAssert.AreEqual(expected, actual);
            Assert.True(condition);
        }

        [Theory]
        [MemberData(nameof(NotEqualObjects))]
        public void ObjectAssert_AreEqual_False(object expected, object actual)
        {
            var condition = ObjectAssert.AreEqual(expected, actual);
            Assert.False(condition);
        }

        [Theory]
        [MemberData(nameof(NotEqualObjects))]
        public void ObjectAssert_AreNotEqual_True(object expected, object actual)
        {
            var condition = ObjectAssert.AreNotEqual(expected, actual);
            Assert.True(condition);
        }

        [Theory]
        [MemberData(nameof(EqualObjects))]
        public void ObjectAssert_AreNotEqual_False(object expected, object actual)
        {
            var condition = ObjectAssert.AreNotEqual(expected, actual);
            Assert.False(condition);
        }

        [Theory]
        [MemberData(nameof(EqualObjects))]
        public void ObjectAssert_That_AreEqual_True(object expected, object actual)
        {
            var condition = ObjectAssert.That()
                .AreEqual(expected, actual);
            Assert.True(condition);
        }

        [Theory]
        [MemberData(nameof(NotEqualObjects))]
        public void ObjectAssert_That_AreEqual_False(object expected, object actual)
        {
            var condition = ObjectAssert.That()
                .AreEqual(expected, actual);
            Assert.False(condition);
        }

        [Theory]
        [MemberData(nameof(NotEqualObjects))]
        public void ObjectAssert_That_AreNotEqual_True(object expected, object actual)
        {
            var condition = ObjectAssert.That()
                .AreNotEqual(expected, actual);
            Assert.True(condition);
        }

        [Theory]
        [MemberData(nameof(EqualObjects))]
        public void ObjectAssert_That_AreNotEqual_False(object expected, object actual)
        {
            var condition = ObjectAssert.That()
                .AreNotEqual(expected, actual);
            Assert.False(condition);
        }

        public static IEnumerable<object[]> EqualObjects
        {
            get
            {
                yield return new object[] { null, null };
                yield return new[] { new object(), new object() };
                yield return new object[] { 0, -0 };
                yield return new object[] { double.NaN, double.NaN };
                yield return new object[] { double.NegativeInfinity, double.NegativeInfinity };
                yield return new object[] { double.PositiveInfinity, double.PositiveInfinity };
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
                yield return new object[] { int.MinValue, double.MinValue };
                yield return new object[] { double.NegativeInfinity, double.PositiveInfinity };
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