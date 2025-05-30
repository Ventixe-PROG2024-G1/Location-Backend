using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Extensions;

namespace WebApi.Tests.Extensions;

public class MappingExtensions_Tests
{
    [Fact]
    public void Maps_Properties_With_Matching_Names_And_Types()
    {
        var source = new Source();
        var result = source.MapTo<Destination>();

        result.Name.Should().Be("Test");
        result.Age.Should().Be(42);
    }

    [Fact]
    public void Ignores_Properties_Not_Present_In_Destination()
    {
        var source = new Source();
        var result = source.MapTo<Destination>();

        result.UnmappedDestinationOnly.Should().BeNull();
    }

    [Fact]
    public void Ignores_Properties_With_Mismatched_Types()
    {
        var source = new Source();
        var result = source.MapTo<Destination>();

        result.TypeMismatch.Should().BeNull();
    }

    [Fact]
    public void Throws_When_Source_Is_Null()
    {
        Source? source = null;

        var act = () => source!.MapTo<Destination>();

        act.Should().Throw<ArgumentNullException>();
    }

    private class Source
    {
        public string Name { get; set; } = "Test";
        public int Age { get; set; } = 42;
        public string UnmappedSourceOnly { get; set; } = "SourceOnly";
        public int TypeMismatch { get; set; } = 123;
    }

    private class Destination
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string UnmappedDestinationOnly { get; set; }
        public string TypeMismatch { get; set; }
    }
}
