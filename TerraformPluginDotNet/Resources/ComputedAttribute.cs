using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerraformPluginDotNet.Resources
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ComputedAttribute : Attribute
    {
    }
}
