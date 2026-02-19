# Device Library (WPF)

Desktop application for managing a catalog of electronic devices and their specifications.

## Ui

<img height="250" alt="image" src="https://github.com/user-attachments/assets/79b76349-cf3b-407e-8f77-a24515dd2597" />
<img height="250" alt="image" src="https://github.com/user-attachments/assets/bc55281e-f85d-402c-b058-22d6b136c9ac" />
<img height="250" alt="image" src="https://github.com/user-attachments/assets/4357b649-64b9-4ce7-ac2c-1f6671a688d0" />


## Overview

WPF application built using the MVVM pattern. Supports storing, viewing, editing, deleting, and searching device data.


## Features

* Add device
* Store devices
* View devices
* Edit device
* Delete device
* Search by model or manufacturer
* Multiple RAM/ROM and cameras configurations

## Tech Stack

* C# (.NET)
* WPF
* SQLite
* MVVM
* ADO.NET

## Architecture

* MVVM
* Repository pattern
* Interface-based data access
* Transaction-safe CRUD operations

## Database

Relational SQLite database with multiple related tables:

* Device info
* Display info
* Hardware info (RAM/ROM, Cameras)
* Software info

Supports multiple memory configurations per device.

## Limitations

* No sorting
* Basic search
* Manual ADO.NET implementation

## Author

Educational project for portfolio.
