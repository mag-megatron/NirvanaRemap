using ApplicationLayer.Services;
using Core.Events.Inputs;
using Core.Services;
using Xunit;

namespace Tests;

public class MappingServiceTests
{
    [Fact]
    public void MapInput_RemapsToOtherButton()
    {
        // Arrange
        var profile = new ProfileMapping();
        profile.AddMapping(new SimpleMapping("A Button", "B Button"));

        var input = new ControllerInput("A Button", 1.0f);

        // Act
        var outputs = profile.Apply(input).ToList();

        // Assert
        Assert.Single(outputs); // Deve haver exatamente 1 saída
        Assert.Equal("B Button", outputs[0].OutputName); // OutputName esperado
        Assert.Equal(1.0f, outputs[0].Value); // Valor preservado
    }
    
}