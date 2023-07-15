using System.Collections.Generic;
using System.Collections.Immutable;
using NUnit.Framework;
using TerraformPluginDotNet.Schemas.Types;

namespace TerraformPluginDotNet.Test.Schemas.Types;

[TestFixture]
public class TerraformTypeTest
{
    [Test]
    public void TestTfObjectEqualityWhenEqual()
    {
        var obj1 = new TerraformType.TfObject(
            new Dictionary<string, TerraformType>()
            {
                ["a"] = new TerraformType.TfNumber(),
                ["b"] = new TerraformType.TfNumber(),
                ["c"] = new TerraformType.TfNumber(),
            }.ToImmutableDictionary(),
            ImmutableHashSet.Create("a", "b"));

        var obj2 = new TerraformType.TfObject(
            new Dictionary<string, TerraformType>()
            {
                ["c"] = new TerraformType.TfNumber(),
                ["b"] = new TerraformType.TfNumber(),
                ["a"] = new TerraformType.TfNumber(),
            }.ToImmutableDictionary(),
            ImmutableHashSet.Create("b", "a"));

        Assert.That(obj1.GetHashCode(), Is.EqualTo(obj2.GetHashCode()));
        Assert.That(obj1, Is.EqualTo(obj2));
    }

    [Test]
    public void TestTfObjectEqualityWhenNotEqual()
    {
        var obj1 = new TerraformType.TfObject(
            new Dictionary<string, TerraformType>()
            {
                ["a"] = new TerraformType.TfNumber(),
                ["b"] = new TerraformType.TfNumber(),
                ["c"] = new TerraformType.TfNumber(),
            }.ToImmutableDictionary(),
            ImmutableHashSet.Create("a", "b"));

        var obj2 = new TerraformType.TfObject(
            new Dictionary<string, TerraformType>()
            {
                ["c"] = new TerraformType.TfNumber(),
                ["b"] = new TerraformType.TfNumber(),
                ["a"] = new TerraformType.TfNumber(),
            }.ToImmutableDictionary(),
            ImmutableHashSet.Create("a"));

        Assert.That(obj1, Is.Not.EqualTo(obj2));
    }
}
