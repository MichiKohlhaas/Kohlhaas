# Kohlhaas Project

#### Table of Contents
- [Introduction](#introduction)
- [Installation](#installation)
- [Project structure](#project-structure)
- [Presentation layer project](#presentation-layer)
- [Application layer project](#application-layer)
- [Domain layer project](#domain-layer)
- [Infrastructure layer project](#infrastructure-layer)
- [Database application](#database-application)
- [Database engine](#database-engine)
- [Graph query language](#parser)

---

## Introduction
WIP C# project.

This project consists of two parts:
- The first is a Document Management System (DMS) web application created using ASP.NET Core. 
- The second is graph-based database engine.

The architecture of the DMS project is that of Domain-driven design (DDD). The idea is to eventually have a DMS web app with users, projects, and documents, and full CRUD capabilities. The project focuses on creating a DMS suitable for use in the V-Model lifecycle, specifically for the development of medical devices.

The database engine is my attempt to create a DBMS. I took some inspiration from how traditional RDBMS work, and from Neo4j for the graph-based part. The graph part is to create traceability matrices for requirements' validations and verifications.

---

## Installation
TBD

---

### Project structure
#### The DMS project is structured like:

**Presentation Layer (Kohlhaas.Presentation)**

↓ depends on

**Application Layer (Kohlhaas.Application)**

↓ depends on

**Domain Layer (Kohlhaas.Domain)**

↑ implemented by

**Infrastructure Layer (Kohlhaas.Infrastructure)**

#### The graph database engine is structured like:

**Database application (Kohlhaas.DB)**

↓ depends on

**Database engine (Kohlhaas.Engine)**

↓↑ TBD

**Lexical analyzer and syntax parser (Kohlhaas.Parser)**
            
## Domain Layer
The first part of the DDD architecture where the project's business logic, entities, and value objects reside. Defines the repository and unit-of-work interfaces.

---

## Application Layer
This layer contains all the data transfer objects (DTO) and services that the presentation layer calls.

---

## Infrastructure Layer
This layer talks to the database, which is currently Postgres, using Entity Framework (EF) Core.

---

## Presentation Layer
ASP.NET Core project part. Holds the controllers (and eventually the frontend as well). The AuthController is for creating a user, signing in, and doing some user profile updates. 

Uses JSON Web Tokens (JWT) for authentication.

Using a tool like Postman, one can create a user by passing some JSON payload in the body:
```json
{
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePassword123!",
  "confirmPassword": "SecurePassword123!",
  "role": 1,
  "department": "Engineering"
}
```

The other controllers are not implemented yet.

---

## Database Application
This is the Kohlhaas.DB application (inspired by InnoDB for the name). It's a worker service with a TCP listener service and a query executor service. The TCP listener service is for connecting with TCP clients and reading the bytes sent over the stream, then passing the data to the query executor service to execute the command (if everything is valid). Still WIP.

Right now the project can start and if there is a TCP client, it can accept a very basic create statement based on the query language being developed along side it. Doesn't do anything with the statement yet. See [Parser](#lexical-analyzer-and-syntax-parser) for more info about the custom query "language".

---

## Database Engine
A library that is to be used by the DBMS to access the database files. Takes inspiration from Neo4j for having nodes, labels, properties, and relationships. These file types are defined here. Still WIP.

---

## Lexical Analyzer and Syntax Parser
This is the custom "language" I'm creating to be used by the database engine. I only have the 'create' statement created right now. From the `Kohlhaas.Parser.Parser.cs` class, the grammar is:
```csharp
/// <summary>
/// statement        ::= create-statement | empty-statement
/// create-statement ::= 'CREATE' '{' node-content '}' ';'
/// node-content     ::= label-array [ ',' property-list ]
/// label-array      ::= '[' [STRING {',' STRING}* '}'
/// property-list    ::= property {',' property}*
/// property         ::= NAME '=' value
/// value            ::= STRING | NUMBER
/// empty-statement  ::= ';'
/// </summary>
```
An example to create a node looks like:

`CREATE{["label 1"], property="value", number=23};`

It takes some inspiration from how Neo4j creates its nodes. The whole statement is wrapped in curly braces, labels are surrounded by square brackets, and properties have a Name=Value structure--separated by commas.

More functionality will be added at some point.

### Kohlhaas
[Kohlhaas](https://de.wikipedia.org/wiki/Michael_Kohlhaas)