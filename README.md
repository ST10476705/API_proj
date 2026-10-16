. Introduction
The RaceDay API is a RESTful Web API built using ASP.NET Core 8 and C#. It forms Part 2 of the PROG6212 programming assignment and provides the backend logic that powers the RaceDay event management system. It handles all database operations, enforces role-based access control, and exposes a documented set of endpoints that the MVC front-end (Part 3) will consume.

This API is built to match the Entity Relationship Diagram (ERD) and SQL schema produced in Part 1, using the Code-First approach with Entity Framework Core 8. It connects to a SQL Server database and uses JWT (JSON Web Token) authentication with BCrypt password hashing to secure user accounts.

2. Purpose and Scope
The purpose of this API is to act as the central data layer of the RaceDay system. It is responsible for:

Registering users and assigning them a role (Participant or Organiser)

Authenticating users and issuing JWT tokens

Enforcing role-based restrictions on every protected endpoint

Managing events, categories, enrolments, and race results

Providing API documentation through Swagger for manual testing

The scope of Part 2 is only the API layer — no user interface is built in this part. The MVC front-end is Part 3.

3. Technologies Used
Technology	Purpose
ASP.NET Core 8 Web API	Framework for building REST endpoints
C#	Programming language
Entity Framework Core 8	Object-relational mapper (Code-First)
SQL Server (LocalDB)	Database engine
JWT Bearer Authentication	Secure API access
BCrypt.Net-Next	Password hashing
Swagger / OpenAPI	Interactive API documentation
xUnit + EF InMemory	Unit testing
GitHub Actions	Continuous integration (CI)
4. Project Structure
The project follows the standard ASP.NET Core Web API folder layout:

Controllers/ — Contains all API controllers. Each controller handles one resource type (Auth, Events, Categories, Enrolments, Results, Roles, Users).

Models/ — Contains the entity classes (User, Role, Assignment, Event, Category, Enrolment, Result) that map to database tables.

Data/ — Contains RaceDayDbContext, which manages the database connection and entity relationships.

Services/ — Contains TokenService, which generates JWT tokens for authenticated users.

Program.cs — The entry point of the application. Configures services, database, authentication, and Swagger.

appsettings.json — Contains the database connection string and JWT configuration.

This separation keeps concerns isolated: models define data, controllers handle requests, services contain business logic, and data manages persistence.

5. Database Design
The API uses Entity Framework Core (Code-First). The database is automatically created on startup using db.Database.EnsureCreated(). There is no need to run migrations manually.

The database contains the following tables:

Users — Stores all registered users with hashed passwords.

Roles — Stores the two roles: Organiser and Participant. These are seeded automatically on first run.

Assignments — Join table linking users to roles (many-to-many).

Events — Stores event details created by Organisers.

Categories — Stores categories within an event (e.g. "10km Women", "Ultra 50km").

Enrolments — Links Participants to Categories within an Event.

Results — Stores finish times, ranks, and verification status for enrolments.

Relationships are enforced at the model level:

A User has many Assignments and many Enrolments.

A Role has many Assignments.

An Event belongs to one Organiser and has many Categories.

A Category belongs to one Event and has many Enrolments.

An Enrolment belongs to one User, one Event, and one Category, and has at most one Result.

To prevent orphan records, the Assignment table has a unique constraint on (UserId, RolesId) so a user cannot be assigned the same role twice.

6. Authentication and Authorization
Authentication is handled by JWT (JSON Web Tokens).

When a user registers, their password is hashed using BCrypt before being saved to the database. Passwords are never stored in plain text.

When a user logs in, the API:

Verifies the email exists.

Uses BCrypt.Verify to compare the entered password with the stored hash.

Generates a JWT token containing the user's ID, email, first name, and role.

Returns the token to the client.

The token expires after 60 minutes (configurable in appsettings.json).

Authorization is handled by role-based access control (RBAC). Controllers use the [Authorize(Roles = "...")] attribute to restrict endpoints. For example:

[Authorize] — any logged-in user

[Authorize(Roles = "Organiser")] — only Organisers

[AllowAnonymous] — public (no token required)

If a user tries to access an endpoint their role does not permit, the API returns 403 Forbidden. If a user tries without a token, the API returns 401 Unauthorized.

7. API Endpoints
The following table summarises the main endpoints implemented:

Authentication
POST /api/auth/register — Registers a new user (auto-assigns Participant role).

POST /api/auth/login — Authenticates a user and returns a JWT token.

GET /api/auth/me — Returns the current user's profile.

Events
GET /api/events — Lists all events (with optional ?status= filter).

GET /api/events/{id} — Gets one event with its categories.

POST /api/events — Creates an event (Organiser only).

PUT /api/events/{id} — Updates an event (Organiser only).

PATCH /api/events/{id}/status — Changes event status (Organiser only).

DELETE /api/events/{id} — Cancels an event (Organiser only).

Categories
GET /api/events/{eventId}/categories — Lists categories for an event.

GET /api/categories/{id} — Gets a category by ID.

POST /api/categories — Creates a category (Organiser only).

PUT /api/categories/{id} — Updates a category (Organiser only).

Enrolments
POST /api/events/{eventId}/enrolments — Enrols a Participant in a category.

GET /api/events/{eventId}/enrolments — Lists all enrolments for an event.

GET /api/enrolments/{id} — Gets one enrolment.

PUT /api/enrolments/{id} — Updates status or payment.

DELETE /api/enrolments/{id} — Withdraws an enrolment.

Results
GET /api/events/{eventId}/results — Lists results for an event.

POST /api/results — Captures a result (Organiser only).

PUT /api/results/{id} — Updates a result (Organiser only).

PUT /api/results/{id}/verify — Marks a result as verified (Organiser only).

DELETE /api/results/{id} — Deletes a result (Organiser only).

Users and Roles
GET /api/users — Lists all users (Organiser only).

GET /api/users/{id} — Gets a user (self or Organiser).

PUT /api/users/{id} — Updates profile (self only).

PUT /api/users/{id}/role — Changes a user's role (Organiser only).

DELETE /api/users/{id} — Deactivates a user (Organiser only).

GET /api/roles — Lists all roles.

POST /api/roles — Creates a role (Organiser only).

PUT /api/roles/{id} — Updates a role (Organiser only).

DELETE /api/roles/{id} — Deletes a role (Organiser only).

8. Swagger Integration
Swagger is integrated using Swashbuckle.AspNetCore. It provides a browser-based user interface where every endpoint can be viewed and tested without any external tools.

Swagger is available at /swagger when the API is running. It displays:

Every HTTP method, route, and description

Request body requirements for POST and PUT endpoints

Response codes (200, 201, 400, 401, 403, 404, 409)

A Authorize button that accepts a JWT token

Once the token is pasted into the Authorize dialog, all protected endpoints can be tested directly from the browser.

This integration acts as live documentation and confirms that the built API matches the endpoint plan submitted in Part 1.

9. Unit Testing
Unit tests are written using xUnit with EF Core InMemory as the test database. The InMemory provider allows tests to run without hitting a real SQL Server, making them fast and repeatable.

The tests cover the following scenarios:

Password hashing and verification

Duplicate email rejection during registration

Role assignment on registration

Event creation by an Organiser

Enrolment creation and duplicate prevention

Role-based access enforcement (403 for wrong role)
