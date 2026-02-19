# Device Library (WPF)

Desktop application for managing a catalog of electronic devices and their specifications.

---

## Overview

WPF application built using the MVVM pattern. Supports storing, viewing, editing, deleting, and searching device data.

---

## Features

* Add device
* Store devices
* View devices
* Edit device
* Delete device
* Search by model or manufacturer
* Multiple RAM/ROM and cameras configurations

---

## Tech Stack

* C# (.NET)
* WPF
* SQLite
* MVVM
* ADO.NET

---

## Architecture

* MVVM
* Repository pattern
* Interface-based data access
* Transaction-safe CRUD operations

---

## Database

Relational SQLite database with multiple related tables:

* Device info
* Display info
* Hardware info (RAM/ROM, Cameras)
* Software info

Supports multiple memory configurations per device.

---

## Limitations

* No sorting
* Basic search
* Manual ADO.NET implementation

---

## Author

Educational project for portfolio.
