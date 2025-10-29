using FluentAssertions;
using NUnit.Framework;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    public static IEnumerable<TestCaseData> InvalidConstructorArgsTestCases()
    {
        foreach (var onlyPositive in new[] { true, false })
        {
            yield return new TestCaseData(0, 1, onlyPositive)
                .SetName($"precision is zero onlyPositive={onlyPositive}");
            yield return new TestCaseData(-1, 2, onlyPositive)
                .SetName($"precision is negative onlyPositive={onlyPositive}");
            yield return new TestCaseData(1, -1, onlyPositive)
                .SetName($"scale is negative onlyPositive={onlyPositive}");
            yield return new TestCaseData(1, 2, onlyPositive)
                .SetName($"scale greater than precision onlyPositive={onlyPositive}");
            yield return new TestCaseData(1, 1, onlyPositive)
                .SetName($"scale equals precision onlyPositive={onlyPositive}");
        }
    }
    
    [Test]
    [TestCaseSource(nameof(InvalidConstructorArgsTestCases))]
    public void Constructor_WhenInvalidArgs_ThrowsArgumentException(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);
        act.Should().Throw<ArgumentException>();
    }

    [TestCase(1, 0, true, TestName = "valid precision and scale")]
    [TestCase(2, 1, false, TestName = "scale less than precision")]
    public void Constructor_WhenValidArgs_Success(int precision, int scale, bool onlyPositive)
    {
        Action act = () => new NumberValidator(precision, scale, onlyPositive);
        act.Should().NotThrow();
    }

    [TestCase("0", 4, 2, true, TestName = "integer zero")]
    [TestCase("0.0", 4, 2, true, TestName = "fractional zero")]
    [TestCase("+0.0", 3, 2, true, TestName = "sign with zero")]
    [TestCase("01.23", 4, 2, true, TestName = "leading zeros")]
    [TestCase("+1.23", 4, 2, true, TestName = "positive with sign")]
    [TestCase("-1.23", 4, 2, false, TestName = "negative when allowed")]
    [TestCase("123.4", 4, 1, true, TestName = "precision equals number length")]
    [TestCase("11,234", 5, 4, true, TestName = "comma separator")]
    public void IsValidNumber_WhenValidNumbers_ReturnTrue(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase("", 1, 0, false, TestName = "empty string")]
    [TestCase(" 123", 4, 0, true, TestName = "leading space before number")]
    [TestCase("123 ", 4, 0, true, TestName = "trailing space after number")]
    [TestCase(" 123 ", 5, 0, true, TestName = "spaces both sides")]
    [TestCase("1.2\n3", 4, 2, true, TestName = "special character inside")]
    [TestCase("\n1.23", 4, 2, true, TestName = "special character before")]
    [TestCase(null, 1, 0, false, TestName = "null value")]
    [TestCase("abc", 3, 0, true, TestName = "non-digit string")]
    [TestCase("1a.2", 3, 2, true, TestName = "invalid character in integer part")]
    [TestCase("1.2a", 3, 2, true, TestName = "invalid character in fraction part")]
    [TestCase("-1.23", 4, 2, true, TestName = "negative fraction when positive only")]
    [TestCase("-123", 4, 0, true, TestName = "negative integer when positive only")]
    [TestCase("123.", 3, 0, true, TestName = "missing fractional part")]
    [TestCase(".123", 4, 3, true, TestName = "missing integer part")]
    [TestCase("++1.23", 5, 2, true, TestName = "multiple signs")]
    [TestCase("1.2.3", 3, 2, true, TestName = "multiple separators")]
    [TestCase("1/23", 3, 2, true, TestName = "invalid separator")]
    public void IsValidNumber_WhenInvalidNumbers_ReturnFalse(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("12", 2, 0, false, TestName = "integer fits precision")]
    [TestCase("-1", 2, 0, false, TestName = "negative integer fits precision")]
    public void IsValidNumber_WhenIntegerBoundsValid_ReturnsTrue(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase("123", 2, 0, false, TestName = "integer exceeds precision")]
    [TestCase("-12", 2, 0, false, TestName = "negative integer exceeds precision")]
    public void IsValidNumber_WhenIntegerBoundsInvalid_ReturnsFalse(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeFalse();
    }

    [TestCase("12.34", 8, 4, false, TestName = "fraction fits scale")]
    [TestCase("11.234", 5, 4, false, TestName = "fraction fits precision and scale")]
    [TestCase("-12.3", 5, 4, false, TestName = "negative fraction fits bounds")]
    public void IsValidNumber_WhenFractionalBoundsValid_ReturnsTrue(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeTrue();
    }

    [TestCase("+12.34", 4, 2, false, TestName = "signed number exceeds precision")]
    [TestCase("1.234", 6, 2, false, TestName = "fraction exceeds scale")]
    [TestCase("-123,4", 4, 1, false, TestName = "negative number exceeds precision")]
    public void IsValidNumber_WhenFractionalBoundsInvalid_ReturnsFalse(string value, int precision, int scale, bool onlyPositive)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);
        validator.IsValidNumber(value).Should().BeFalse();
    }
}