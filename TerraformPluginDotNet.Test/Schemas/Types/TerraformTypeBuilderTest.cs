using System;
using System.Collections.Generic;
using NUnit.Framework;
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
    [TestCase(typeof(List<string>), ExpectedResult = @"[""list"",""string""]")]
    [TestCase(typeof(List<int>), ExpectedResult = @"[""list"",""number""]")]
    [TestCase(typeof(Dictionary<string, string>), ExpectedResult = @"[""map"",""string""]")]
    [TestCase(typeof(Dictionary<string, int>), ExpectedResult = @"[""map"",""number""]")]
    [TestCase(typeof(TestObjectWithOptionalAttributes), ExpectedResult = """["object",{"nested_int":"number","required_string":"string"},["nested_int"]]""")]
    [TestCase(typeof(TestObjectNoOptionalAttributes), ExpectedResult = """["object",{"required_string":"string"},[]]""")]
    public string TestGetTerraformTypeAsJson(Type inputType)
    {
        return _typeBuilder.GetTerraformType(inputType).ToJson();
    }
}
