TerraformPluginDotNet
=====================

Write custom Terraform providers in C#. See samples directory for an example provider.

For more information on this project, see
[this blog post](https://samuelfisher.co.uk/2021/01/terraform-provider-csharp).

## Features

Currently supports basic functionality for creating, updating and deleting
custom resources.

## Usage

This section explains how to use the `samples/SampleProvider` project with
Terraform. To define your own provider and resource types, create your own
project following the same structure as SampleProvider.

Instructions are for Linux x64 with Terraform v0.13.3. May work on other platforms
and Terraform versions.

1. In the `samples/SampleProvider` directory, publish a self-contained single-file binary:

```
dotnet publish -r linux-x64 -c release -p:PublishSingleFile=true
```

2. Copy the binary built above, and `serilog.json` to your Terraform plugins directory:

```bash
# Create plugin directory
mkdir -p ~/.terraform.d/plugins/example.com/example/dotnetsample/1.0.0/linux_amd64/

# Copy binary
cp ./bin/release/net5.0/linux-x64/publish/SampleProvider ~/.terraform.d/plugins/example.com/example/dotnetsample/1.0.0/linux_amd64/terraform-provider-dotnetsample

# Copy log config
cp ./bin/release/net5.0/linux-x64/publish/serilog.json ~/.terraform.d/plugins/example.com/example/dotnetsample/1.0.0/linux_amd64/serilog.json
```

3. Create a new Terraform project or open an existing one. The remaining commands
are run in this directory.

3. Add to `versions.tf`:

```hcl
terraform {
  required_providers {
    dotnetsample = {
      source = "example.com/example/dotnetsample"
      version = "1.0.0"
    }
  }
}
```

3. Add to `providers.tf`:

```hcl
provider "dotnetsample" {}
```

4. Define a resource in `sample.tf`

```hcl
resource "dotnetsample_file" "demo_file" {
  path = "/tmp/file.txt"
  content = "this is a test"
}
```

5. Initialize Terraform with the new plugin: `terraform init`
6. Run `terraform apply`
7. File at `/tmp/file.txt` should contain the contents `this is a test`

See `log.txt` in your working directory for troubleshooting.
