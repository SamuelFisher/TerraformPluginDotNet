SampleProvider
==============

Basic example of a custom resource provider.

This provider defines a new resource type that creates a file at a specified
path containing the specified text content.

```hcl
resource "sampleprovider_file" "demo_file" {
  path = "/tmp/file.txt"
  content = "this is a test"
}
```

The provider can optionally be configured to prepend a fixed header to every
file.

```hcl
provider "sampleprovider" {
  file_header = "# File Header"
}
```
