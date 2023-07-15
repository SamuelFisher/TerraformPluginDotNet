using System;
using System.Collections.Generic;
using MessagePack;
using NUnit.Framework;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Schemas.Types;

namespace TerraformPluginDotNet.Test.Schemas.Types;

[TestFixture]
public class TerraformTypeBuilderTest
{
    private TerraformTypeBuilder _typeBuilder;

    [SetUp]
    public void Setup()
    {
        _typeBuilder = new TerraformTypeBuilder();
    }

    [TestCase(typeof(int), ExpectedResult = @"""number""")]
    [TestCase(typeof(bool), ExpectedResult = @"""bool""")]
    [TestCase(typeof(float), ExpectedResult = @"""number""")]
    [TestCase(typeof(double), ExpectedResult = @"""number""")]
    [TestCase(typeof(Tuple<string>), ExpectedResult = """["tuple",["string"]]""")]
    [TestCase(typeof(Tuple<int, int>), ExpectedResult = """["tuple",["number","number"]]""")]
    [TestCase(typeof(Tuple<bool, bool, bool>), ExpectedResult = """["tuple",["bool","bool","bool"]]""")]
    [TestCase(typeof(Tuple<string, string, string, string>), ExpectedResult = """["tuple",["string","string","string","string"]]""")]
    [TestCase(typeof(Tuple<string, string, string, string, string>), ExpectedResult = """["tuple",["string","string","string","string","string"]]""")]
    [TestCase(typeof(Tuple<string, string, string, string, string, string>), ExpectedResult = """["tuple",["string","string","string","string","string","string"]]""")]
    [TestCase(typeof(Tuple<string, int, bool, string, int, bool, float>), ExpectedResult = """["tuple",["string","number","bool","string","number","bool","number"]]""")]
    [TestCase(typeof(Tuple<string, string, string, string, string, string, string, string>), ExpectedResult = """["tuple",["string","string","string","string","string","string","string","string"]]""")]
    [TestCase(typeof(List<string>), ExpectedResult = """["list","string"]""")]
    [TestCase(typeof(List<int>), ExpectedResult = """["list","number"]""")]
    [TestCase(typeof(Dictionary<string, string>), ExpectedResult = """["map","string"]""")]
    [TestCase(typeof(Dictionary<string, int>), ExpectedResult = """["map","number"]""")]
    [TestCase(typeof(TestObjectWithOptionalAttributes), ExpectedResult = """["object",{"nested_int":"number","required_string":"string"},["nested_int"]]""")]
    [TestCase(typeof(TestObjectNoOptionalAttributes), ExpectedResult = """["object",{"required_string":"string"},[]]""")]
    public string TestGetTerraformTypeAsJson(Type inputType)
    {
        return _typeBuilder.GetTerraformType(inputType).ToJson();
    }

    [Test]
    public void TestObjectMissingMessagePackAttribute()
    {
        Assert.Throws<InvalidOperationException>(() => _typeBuilder.GetTerraformType(typeof(TestMissingAttrObjectResource)));
    }

    [SchemaVersion(1)]
    [MessagePackObject]
    class TestMissingAttrObjectResource
    {
        [Key("object_attribute")]
        [Required]
        public TestMissingAttrObject ObjectAttribute { get; set; }
    }

    // Missing [MessagePackObject]
    class TestMissingAttrObject
    {
        [Key("some_attribute")]
        [Required]
        public string SomeAttribute { get; set; }
    }
}
