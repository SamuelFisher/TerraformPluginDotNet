DataSourceProvider
==============

Basic example of a custom data source provider.

This provider defines a new resource type that provides some dummy data.

```hcl
data "datasourceprovider_data" "demo_data" {
  id = "test"
}
```
