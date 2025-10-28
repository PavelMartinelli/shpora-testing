using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    public void GetCurrentTsar_WhenComparingWithExpected_ShouldBeEquivalent()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Преимущества решение с FluentAssertions:
        // - Легко расширяем при добавлении новых свойств в Person 
        // - Автоматически проверяем все свойства, включая вложенные объекты до 10 уровней вложенности (по умолчанию)
        // - При несовпадении конкретного свойства будет выдана информация какое именно свойство не совпало
        actualTsar.ShouldBeEquivalentToPerson(expectedTsar);	 
    }

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Недостатки подхода с CustomEquality:
        // - Нет детальной информации о том, какое именно свойство не совпало
        // - При добавлении новых свойств в Person нужно менять метод AreEqual
        // - Риск переполнения стека при большом уровне вложенности
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }
    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}

public static class PersonAssertions
{
    public static void ShouldBeEquivalentToPerson(this Person actual, Person expected)
    {
        actual.Should().BeEquivalentTo(expected, options => options
            .Excluding(field => 
                field.DeclaringType == typeof(Person) &&
                field.Name == nameof(Person.Id))
            .AllowingInfiniteRecursion());
    }
}