using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    [TestCase(-1, 2, true)]
    [TestCase(0, 0, false)]
    [TestCase(1, -1, false)]
    [TestCase(1, 2, false)]
    [TestCase(1, 1, false)]
    public void Constructor_WhenInvalidArgs_Throws(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);
        act.Should().Throw<ArgumentException>();
    }

    [TestCase(1, 0, true)]
    [TestCase(2, 1, false)]
    public void Constructor_WhenValidArgs_Success(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);
        act.Should().NotThrow();
    }

    [TestCase("0", 17, 2, true)]
    [TestCase("0.0", 17, 2, true)]
    [TestCase("+1.23", 4, 2, true)]
    [TestCase("-1.23", 4, 2, false)]
    [TestCase("123.4", 4, 1, true)]
    [TestCase("1.2", 4, 1, true)]
    [TestCase("12", 2, 0, true)]
    [TestCase("11.234", 5, 4, true)]
    [TestCase("11,234", 5, 4, true)]
    [TestCase("1.234", 5, 3, true)]
    public void IsValid_WhenValidNumbers_ReturnTrue(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase("", 1, 0, false)]
    [TestCase(null, 1, 0, false)]
    [TestCase("a.sd", 3, 2, true)]
    [TestCase("00.00", 3, 2, true)]
    [TestCase("+0.00", 3, 2, true)]
    [TestCase("0.000", 17, 2, true)]
    [TestCase("-1.23", 3, 2, true)]
    [TestCase("-0.00", 3, 2, true)]
    [TestCase("123", 2, 0, true)]
    [TestCase("-123", 3, 0, false)]
    [TestCase("+12.34", 4, 2, true)]
    [TestCase("1.234", 6, 2, true)]
    [TestCase("-123.4", 4, 1, false)]
    [TestCase("-123.456", 10, 2, false)]
    [TestCase("123.", 4, 2, true)]
    [TestCase("1.23a4", 10, 7, true)]
    [TestCase("t", 4, 3, true)]
    [TestCase("b.0", 4, 3, true)]
    [TestCase("1.2b34", 10, 7, true)]
    [TestCase("12.", 10, 7, true)]
    [TestCase("-123", 5, 2, true)]
    [TestCase("-1.23", 5, 4, true)]
    public void IsValid_WhenInvalidNumbers_ReturnFalse(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase(5, 2, "-123")]
    [TestCase(5, 4, "-1.23")]
    [TestCase(3, 2, "-1.23")]
    public void IsValid_WhenNegativeWhenPositiveOnly_ReturnFalse(int precision, int scale, string value)
    {
        var validator = new NumberValidator(precision, scale, true);
        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase(2, 0, "12", true)]
    [TestCase(2, 1, "123", false)]
    [TestCase(3, 0, "-12", true)]
    [TestCase(3, 1, "-123", false)]
    public void IsValid_IntegerBounds_Expected(int precision, int scale, string value, bool expected)
    {
        var validator = new NumberValidator(precision, scale, false);
        validator.IsValidNumber(value).Should().Be(expected);
    }

    [TestCase(8, 4, "12.34", true)]
    [TestCase(5, 4, "11.234", true)]
    [TestCase(7, 3, "1.234", true)]
    [TestCase(8, 4, "+123,4567", true)]
    [TestCase(4, 2, "+12.34", false)]
    [TestCase(6, 2, "1.234", false)]
    [TestCase(5, 4, "-12.3", true)]
    [TestCase(8, 4, "-1234.567", true)]
    [TestCase(12, 4, "-123.4567", true)]
    [TestCase(6, 3, "-12,345", true)]
    [TestCase(4, 1, "-123,4", false)]
    [TestCase(10, 2, "-123.456", false)]
    public void IsValid_DecimalBounds_Expected(int precision, int scale, string value, bool expected)
    {
        var validator = new NumberValidator(precision, scale, false);
        validator.IsValidNumber(value).Should().Be(expected);
    }
}