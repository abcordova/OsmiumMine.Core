
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
For example, a route `/test`