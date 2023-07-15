using System.Collections.Immutable;

namespace TerraformPluginDotNet.Schemas.Types;

public abstract record TerraformType
{
    // Prevent external derived classes.
    private TerraformType()
    {
    }

    /// <summary>
    /// Returns the Terraform JSON representation of this type.
    /// </summary>
    public abstract string ToJson();

    public sealed record TfString() : TerraformType()
    {
        public override string ToJson() => "\"string\"";
    }

    public sealed record TfNumber() : TerraformType()
    {
        public override string ToJson() => "\"number\"";
    }

    public sealed record TfBool() : TerraformType()
    {
        public override string ToJson() => "\"bool\"";
    }

    public sealed record TfMap(TerraformType ValueType) : TerraformType()
    {
        public override string ToJson() => $"""["map",{ValueType.ToJson()}]""";
    }

    public sealed record TfList(TerraformType ElementType) : TerraformType()
    {
        public override string ToJson() => $"""["list",{ElementType.ToJson()}]""";
    }

    public sealed record TfSet(TerraformType ElementType) : TerraformType()
    {
        public override string ToJson() => $"""["set",{ElementType.ToJson()}]""";
    }

    public sealed record TfTuple : TerraformType
    {
        public TfTuple(ImmutableList<TerraformType> elementTypes)
        {
            ElementTypes = elementTypes;
        }

        public ImmutableList<TerraformType> ElementTypes { get; }

        public override string ToJson()
        {
            var elementTypes = string.Join(",", ElementTypes.Select(x => x.ToJson()));
            return $"[\"tuple\",[{elementTypes}]]";
        }

        public bool Equals(TfTuple? other)
        {
            if (other == null)
            {
                return false;
            }

            return ElementTypes.Count == other.ElementTypes.Count &&
                !ElementTypes.Except(other.ElementTypes).Any();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var element in ElementTypes)
                {
                    hash = hash * 31 + element.GetHashCode();
                }

                return hash;
            }
        }
    }

    public sealed record TfObject : TerraformType
    {
        public TfObject(
            ImmutableDictionary<string, TerraformType> attributes,
            ImmutableHashSet<string> optionalAttributes)
        {
            if (optionalAttributes.Except(attributes.Keys).Any())
            {
                throw new ArgumentException(
                    $"{nameof(optionalAttributes)} contain an element that is not defined in {nameof(attributes)}.",
                    nameof(optionalAttributes));
            }

            Attributes = attributes;
            OptionalAttributes = optionalAttributes;
        }

        public ImmutableDictionary<string, TerraformType> Attributes { get; }

        public ImmutableHashSet<string> OptionalAttributes { get; }

        public override string ToJson()
        {
            var attrTypes = string.Join(",", Attributes.OrderBy(x => x.Key).Select(x => $"\"{x.Key}\":{x.Value.ToJson()}"));
            var optionalAttrs = string.Join(",", OptionalAttributes.OrderBy(x => x).Select(x => $"\"{x}\""));
            return $"[\"object\",{{{attrTypes}}},[{optionalAttrs}]]";
        }

        public bool Equals(TfObject? other)
        {
            if (other == null)
            {
                return false;
            }

            return OptionalAttributes.SetEquals(other.OptionalAttributes) &&
                Attributes.Count == other.Attributes.Count &&
                !Attributes.Except(other.Attributes).Any();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                foreach (var element in Attributes)
                {
                    hash = hash * 31 + element.GetHashCode();
                }

                foreach (var element in OptionalAttributes)
                {
                    hash = hash * 31 + element.GetHashCode();
                }

                return hash;
            }
        }
    }
}
