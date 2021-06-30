# score-api
Mock REST API for CPF scoring based on F# Saturn's Template

### Setting the application up (from scratch)

- Pull and run the latest version of Postgres ([docker installation required](https://docs.docker.com/get-docker/))
```sh
docker run --name db-pgsql -e POSTGRES_PASSWORD=passw0rd -p 5432:5432 -d postgres
```
 - Clone the repository and move to its directory ([git installation required](https://www.atlassian.com/git/tutorials/install-git))
```sh
git clone https://github.com/alsnrbernardo/score-api.git && cd score-api
```
- Build the score-api
```sh
dotnet tool restore
```
- Perform database migration (depends on Postgres container)
```sh
dotnet saturn migration
```
- Run the application (available at http://localhost:8085)
```sh
dotnet fake build -t run
```

### Missing Features and Things to Improve

* Encrypt sensitive data (e.g. CPF's are being stored as raw text)
* Externalize environment properties (e.g. DB connection string and the application port are currently hardcoded) 
* Containerize the API and database: although there's a Dockerfile and docker-compose.yml available in the repository, the database migration is not correctly configured. I couldn't come up with a reasonable solution to wait for the database to accept connections before trying to perform the migration. An alternative approach was to separate the migration step from the compose and manually execute it, but it didn't seem right to build an image with the entire dotnet sdk)
* Implement unit and integration tests
* Provide an automated to backup the database
