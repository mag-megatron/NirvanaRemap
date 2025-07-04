using Xunit;
using Core.Services;
using Core.Events.Inputs;
using Core.Events.Outputs;

namespace Tests
{
    public class SimpleMappingTests
    {
        [Fact]
        public void Matches_ReturnsTrue_WhenInputNameMatches()
        {
            var mapping = new SimpleMapping("A", "B");
            var input = new ControllerInput("A", 1.0f);

            Assert.True(mapping.Matches(input));
        }

        [Fact]
        public void Matches_ReturnsFalse_WhenInputNameDoesNotMatch()
        {
            var mapping = new SimpleMapping("A", "B");
            var input = new ControllerInput("X", 1.0f);

            Assert.False(mapping.Matches(input));
        }

        [Fact]
        public void Map_ReturnsMappedOutput_WithCorrectNameAndValue()
        {
            var mapping = new SimpleMapping("A", "B");
            var input = new ControllerInput("A", 0.5f);

            var result = mapping.Map(input);

            Assert.Equal("B", result.OutputName);
            Assert.Equal(0.5f, result.Value);
        }
    }
}