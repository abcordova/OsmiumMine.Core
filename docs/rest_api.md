
# OsmiumMine Core Documentation

Version 0.1

Copyright &copy; 2016-2017 Nihal Talur (0xFireball), IridiumIon Software. All Rights Reserved.

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

## Routes

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
            name: "Tabley McTableFace"
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

TODO