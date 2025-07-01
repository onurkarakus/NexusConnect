using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusConnect.Core.Tests;

public class CalculatorTests
{
    [Fact]
    public void Add_ShouldReturnCorrectSum()
    {
        // 1. Arrange (Hazırlık)
        var calculator = new Calculator();
        int a = 1;
        int b = 2;
        int expected = 3;

        // 2. Act (Eylem)
        int actual = calculator.Add(a, b);

        // 3. Assert (Doğrulama)
        Assert.Equal(expected, actual);
    }
}
