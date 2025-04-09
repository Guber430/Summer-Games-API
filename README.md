# Summer Games Web API

This repository contains both a **Web API** and a **Client Application** for managing a **Summer Games** event, similar to the Canada Summer Games held in the Niagara Region. The API handles data about athletes, sports, and contingents (provinces/territories), and the client application interacts with the API to allow for adding, updating, and viewing athletes, filtering by **Sport** or **Contingent**.

---

## Table of Contents
- [Project Overview](#project-overview)
  - [Web API](#web-api)
  - [Client Application](#client-application)
- [Technologies Used](#technologies-used)
- [Setup Instructions](#setup-instructions)
  - [Prerequisites](#prerequisites)
  - [Clone the Repository](#clone-the-repository)
- [API Endpoints](#api-endpoints)
  - [Sport](#sport)
  - [Contingent](#contingent)
  - [Athlete](#athlete)

---


## Project Overview

### Web API
The **Web API** is built using **C#**, **.NET 8**, and **SQLite**. It includes endpoints for managing the entities of **Sport**, **Contingent**, and **Athlete**. The API enforces several validation rules, including age range, BMI, and gender restrictions for athletes. Additionally, it supports filtering athletes by sport or contingent.

Key features include:
- **CRUD** operations for athletes, sports, and contingents.
- **Validation** for age, BMI, and gender for athletes.
- **Unique athlete code generation** with the prefix `A:`.
- **Filtering** athletes by **Sport** or **Contingent**.
- **Error handling** for validation rules, database errors, and concurrency issues.

### Client Application
The **Client Application** interacts with the Web API to:
- View a list of athletes filtered by either **Sport** or **Contingent**.
- Add, update, or delete athletes, including reassignment to different sports or contingents.
- Handle and display error information provided by the Web API.

The client is built using **MAUI** and provides a smooth interface for interacting with the API.

---

## Technologies Used
- **Backend**: C#, .NET 8
- **Database**: SQLite
- **Client**: MAUI
- **API Documentation**: Swagger
- **Authentication**: Not included (can be added as needed)

## Setup Instructions

### Prerequisites
- **.NET 8 SDK**
- **SQLite**
- **Visual Studio** (or another IDE)
- **MAUI** (for the Client Application)

### Clone the Repository
To get started, clone the repository:
```bash
git clone https://github.com/Guber430/Summer-Games-API.git
cd Summer-Games-API
