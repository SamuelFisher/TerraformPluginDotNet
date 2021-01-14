SampleProvider
==============

Basic example of a custom resource provider.

This provider defines a new resource type that creates a file at a specified
path containing the specified text content.

```hcl
resource "dotnetsample_file" "demo_file" {
  path = "/tmp/file.txt"
  content = "this is a test"
}
```
