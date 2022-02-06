using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace SampleProvider;

[MessagePackObject]
public class Configuration
{
    [Key("file_header")]
    [Description("Header text to prepend to every file.")]
    public string FileHeader { get; set; }
}
