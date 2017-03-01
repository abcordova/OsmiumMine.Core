
# OsmiumMine Core REST API Documentation

Version 0.1.1

Copyright &copy; 2016-2017 Nihal Talur (0xFireball), IridiumIon Software. All Rights Reserved.

Except as otherwise noted, the content of this page is licensed under the Creative Commons Attribution 3.0 License, and code samples are licensed under the Apache 2.0 License.

## About

OsmiumMine provides a REST API that can dynamically
update internal configuration and use the dynamic database
service.

The REST API is divided into **modules** according to groups
of common functionality.

### Modules

OsmiumMine contains the following modules:

- **Remote IO** - For accessing (read, write, update) the dynamic database
- **Remote Security** - For managing (adding, enumerating, removing) security rules

## Module documentation

### Application Route Prefix

All listed routes should be prefixed by `/om`.
For example, a listed route `/test` would correspond
to `/om/test`.

### Remote IO

All **Remote IO** routes should additionally be prefixed by `/io`. For example,
`/a/b` would be `/om/io/a/b`.

All **Remote IO** routes are of the following format:

Path:

`/{databaseId}/{path}.json`

With one of the following verbs:

- `PUT` - Performs a PUT operation in the database
- `PATCH` - Performs an UPDATE operation in the database
- `POST` - Performs a PUSH operation in the database
- `DELETE` - Performs a DELETE operation in the database
- `GET` - Performs a RETRIEVE operation in the database

Addtionally, `PUT`, `PATCH`, and `POST` all require
a request body containing a JSON string containing the request
payload sent with the MIME type `application/json`.

#### Database path

The `path` parameter specifies the path in the database.
For example, assume the following data in the database is stored at the
root:

```json
{
    "things": {
        "cookie": {
            "name": "Chocolatey"
        },
        "table": {
            "name": "Tabley McTableFace"
        }
    }
}
```

Using RETRIEVE on `/.json` would return the following data.
Using RETRIEVE on `/things.json` would return the value
of the `things` object. Using RETRIEVE on `/things/cookie.json`
would return the value of the `cookie` object.

This powerful JSON path-based interface provides strong
support for dynamic and extensible data storage.

#### Database operations

##### PUT

Write data to the database. Replaces any existing data.

##### UPDATE

Write data to the database. New data is merged with existing data.

##### PUSH

Pushes data to a time-keyed list in the database. This
is especially useful for sequential data such as a message thread.

##### DELETE

Deletes all data under the specified path.

##### RETRIEVE

Retrieves all data under the specified path.
Add `shallow=1` to the query string to do a shallow
retrieve. A shallow retrieve replaces all child
value objects with `true`, but retains primitive values.

##### Additional parameters

Add `print=silent` to the query string to do a silent operation.
This will suppress any returned data, and instead provide
the `204 No Content` status code on a successful operation.

#### Manipulating data

Using a combination of the above operations,
an application can manage its remotely stored data.

#### Database Realms

OsmiumMine allows for an unlimited number of databases,
Simply by specifying a different string as the `databaseId`,
a different database with its own independent data store can be accessed.
The default security rules, however, completely deny
access to a database, for security reasons. The **Remote Security**
module can be used to dynamically update database rules.

#### Security Rules

Security rules are essential for ensuring application data
integrity and managing access of unauthenticated IO
requests.

The security rule pipeline is run on every request to
determine whether it should be allowed. It works
like this:

- Set the request DENIED (by default)
- Check the corresponding security rule table for the given database
  - (Rules are sorted by priority when created)
- Retrieve all rules matching the given path
- Check each rule in order until one specifies that the request should be allowed
  - The request is marked as GRANTED
- If no rules match, the request is denied. If GRANTED, the request is processed by the module.

Each rule can specify one or more database action to allow or deny:

##### Security Rule Database Actions

```csharp
[Flags]
public enum DatabaseAction
{
    Retrieve = 1 << 0,
    Delete = 1 << 1,
    Push = 1 << 2,
    Update = 1 << 3,
    Put = 1 << 4,
    Read = Retrieve,
    Write = Push | Update | Put,
    All = Retrieve | Delete | Push | Update | Put
}
```

### Remote Security

#### Authentication

The **Remote Security** API requires stateless authentication through
an API key. This is important because the API provides complete access
over all the databases.

API keys can be specified in the application configuration. Please
refer to the Server Configuration document for more information.

API keys should be added to all requests to this module as a query
parameter in the form `apikey={your_api_key}`.

#### Path patterns

Rules match paths through path patterns, which can be specified
as wildcards or as regular expressions. If using a wildcard as part
of the path, pass the query string parameter `w=1` to indicate that
the path pattern should be parsed as a wildcard.

#### Remote Security Routes

All **Remote Security** routes should additionally be prefixed
by `/rsec`.

##### Creating Rules

Path:

`/rules/create/{dbid}`

Parameters:

URL:

- `dbid` - the database realm to access

Query string:

- `path` - the path pattern to apply the rule to