using NUnit.Framework;
using NUnit.Framework.Legacy;
using FluentAssertions;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    [Test]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();

        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Преимущества решение с FluentAssertions:
        // - Легко расширяем при добавлении новых свойств в Person 
        // - Автоматически проверяем все свойства, включая вложенные объекты
        // - При несовпадении конкретного свойства будет выдана информация какое именно свойство не совпало
        actualTsar.Should().BeEquivalentTo(expectedTsar, options => options
            .Excluding(p => p.Id) 
            .Excluding(p => p.Parent.Id)); 
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
