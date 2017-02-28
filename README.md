
# OsmiumMine.Core

AlphaOsmium Data Store Component

## Information

The core component of AlphaOsmium Database, OsmiumMine is a Redis/SSDB powered schemaless data store. The provider
allows for multiple "domains," each of which is its own independent data context.

`OsmiumMine.Core` is a portable set of projects that compose AlphaOsmium Database. However,
this project has been specially separated to allow running the core server, `AlphaOsmium.Core.Server`
as a standalone application without AlphaOsmium.

### Planned Features

- Optimization/caching to further improve query performance. Most likely this will use some form of indexing to reduce reads in a property query

### Extensions

AlphaOsmium DbSync extends OsmiumMine by providing a real-time, event-driven data synchronization API, with support for both online and offline transactions.