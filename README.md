TerraformPluginDotNet
=====================

[![Build status](https://github.com/SamuelFisher/TerraformPluginDotNet/actions/workflows/ci.yml/badge.svg?branch=master)](https://github.com/SamuelFisher/TerraformPluginDotNet/actions)

Write custom Terraform providers in C#. See samples directory for an example provider.

For more information on this project, see
[this blog post](https://samuelfisher.co.uk/2021/01/terraform-provider-csharp).

## Packages

Package                       | Version
------------------------------|---------
TerraformPluginDotNet         | [![NuGet](https://img.shields.io/nuget/vpre/TerraformPluginDotNet.svg)](https://www.nuget.org/packages/TerraformPluginDotNet)
TerraformPluginDotNet.Testing | [![NuGet](https://img.shields.io/nuget/vpre/TerraformPluginDotNet.svg)](https://www.nuget.org/packages/TerraformPluginDotNet.Testing)

## Features

Currently supports basic functionality for creating, updating and deleting
custom resources.

## Usage

This section explains how to use the `samples/SampleProvider` project with
Terraform. To define your own provider and resource types, create your own
project following the same structure as SampleProvider.

Instructions are for Linux x64 with Terraform v0.13.3. May work on other platforms
and Terraform versions.

1. In the `samples/SampleProvider/SampleProvider` directory, publish a self-contained single-file binary:

```
dotnet publish -r linux-x64 -c release -p:PublishSingleFile=true
```

2. Copy the binary built above, and `serilog.json` to your Terraform plugins directory:

```bash
# Create plugin directory
mkdir -p ~/.terraform.d/plugins/example.com/example/dotnetsample/1.0.0/linux_amd64/

# Copy binary
cp ./bin/release/net6.0/linux-x64/publish/SampleProvider ~/.terraform.d/plugins/example.com/example/dotnetsample/1.0.0/linux_amd64/terraform-provider-dotnetsample

# Copy log config
cp ./bin/release/net6.0/linux-x64/publish/serilog.json ~/.terraform.d/plugins/example.com/example/dotnetsample/1.0.0/linux_amd64/serilog.json
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

## Debugging

Terraform can attach to an already started provider by making use of debug mode,
which allows debugging in Visual Studio. To do this, start the SampleProvider
project with the `--DebugMode=true` argument. By default, this will also cause
log messages to be written to the console in addition to a file.

Once started, it will output an environment variable that can be used to
instruct Terraform to connect to this already running provider.

For more information, see
[Running Terraform With A Provider in Debug Mode](https://www.terraform.io/docs/extend/debugging.html#running-terraform-with-a-provider-in-debug-mode).

## Testing

Tests that involve running Terraform are marked with `Category=Functional`. To run
these tests, you will need to have Terraform installed and set the environment
variable `TF_PLUGIN_DOTNET_TEST_TF_BIN`.

For example:

```bash
$ TF_PLUGIN_DOTNET_TEST_TF_BIN=/usr/bin/terraform dotnet test --filter Category=Functional
```

In Visual Studio, you can create a `test.runsettings` file to do this:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
    <EnvironmentVariables>
      <TF_PLUGIN_DOTNET_TEST_TF_BIN>C:\Path\To\terraform.exe</TF_PLUGIN_DOTNET_TEST_TF_BIN>
    </EnvironmentVariables>
  </RunConfiguration>
</RunSettings>
```

The `TerraformPluginDotNet.Testing` package can be used to help with writing
tests for custom providers. See `samples/SampleProvider/SampleProvider.Test`.
