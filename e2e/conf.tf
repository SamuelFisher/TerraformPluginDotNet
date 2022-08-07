provider "sampleprovider" {}

terraform {
  required_providers {
    sampleprovider = {
      source = "example.com/example/sampleprovider"
      version = "1.0.0"
    }
  }
}
