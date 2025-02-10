-------------------------------------------------
Testing project:
-------------------------------------------------

Run scripts found in Clean -> Domain -> Scripts
 1. CreateDatabase.sql
 2. SensitiveWord.sql

Run using visual studio

-------------------------------------------------
Production Deploy:
-------------------------------------------------

1. Set up a docker container for the application
2. Add users to the database for production environment
3. Add a production config (appsettings.production.json)
4. Publish the api
5. Deploy on hosting platform

Other details:
- Set up CI/CD
- Set up logging for production environment
- Set up monitoring for application health and performance checks
- Secure the api with HTTPS & TLS certificates
- Implement a load balancer
- Configure database backups and failover strategy

-------------------------------------------------

Future Enhancements:

API:
- Implement Authentication and authentication controller.
- Implement Role based authorization for SensitiveWordsController so that only admins can access it

Services.Sanitizer:
- Install benchmark.net
- Implement trie node algorithm
- Implement string split algorithm
- Write unit tests to check for performance across a variety of use cases and compare benchmarks to determine most effective algorithm

Services.Cache:
- Research ways to monitor performance
- Conduct load test on the service to check memory utilization based on word list size
- Introduce redis cache when performance degrades?

Clean.Application:
- Unit test

Clean.Domain:
- Implement AuditSensitiveWord entity(I want to expirement with joining entities)
- Maybe move all nuget packages here and just centralize them for ease of updating

Clean.Infrastructure:
- Fix broken methods for updating and deleting ranges
- Implement AuditSensitiveWord repositories
- Figure out transactional implementation using the unit of work pattern

Tests:
- Do test coverage analysis
- Maintain 80% plus code coverage
- Create Core test project to abstract base classes to and implement all needed nuget modules there to centralize maintenance concerns
