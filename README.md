-------------------------------------------------
Testing project:
-------------------------------------------------

Run scripts found in Clean -> Domain -> Scripts
 1. CreateDatabase.sql
 2. SensitiveWord.sql

Run using visual studio

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

Clean.Infrastructure:
- Fix broken methods for updating and deleting ranges
- Implement AuditSensitiveWord repositories
- Figure out transactional implementation using the unit of work pattern
