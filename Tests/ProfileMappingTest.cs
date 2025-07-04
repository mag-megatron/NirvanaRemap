using Xunit;
using Core.Events.Inputs;
using Core.Events.Outputs;
using Core.Interfaces;
using Core.Services;
using System.Linq;

namespace Tests
{
    public class ProfileMappingTests
    {
        [Fact]
        public void Apply_ReturnsMappedOutputs_WhenMatchingInput()
        {
            var profile = new ProfileMapping();
            profile.AddMapping(new SimpleMapping("A", "B"));
            var input = new ControllerInput("A", 1.0f);

            var result = profile.Apply(input).ToList();

            Assert.Single(result);
            Assert.Equal("B", result[0].OutputName);
            Assert.Equal(1.0f, result[0].Value);
        }

        [Fact]
        public void Apply_ReturnsEmpty_WhenNoMatchingInput()
        {
            var profile = new ProfileMapping();
            profile.AddMapping(new SimpleMapping("A", "B"));
            var input = new ControllerInput("X", 1.0f);

            var result = profile.Apply(input).ToList();

            Assert.Empty(result);
        }

        [Fact]
        public void GetMappings_ReturnsAllAddedMappings()
        {
            var profile = new ProfileMapping();
            var mapping = new SimpleMapping("A", "B");
            profile.AddMapping(mapping);

            var mappings = profile.GetMappings();

            Assert.Contains(mapping, mappings);
        }
    }
}