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

### Testing

To interact with the api, use a REST testing tool like [Postman](https://www.postman.com/downloads/).

API endpoints:

* `GET /score/<CPF>` fetches the values and creation dates of scores attributed to the given [CPF](https://theonegenerator.com/generators/documents/cpf-generator/).
 ```
 #Concrete example
 http://localhost:8085/score/60522679684
 ```

* `POST /score` scores and registers the provided CPF
```
// Request Body example 
{
    "cpf" : "605.226.796-84"
}
```

### Missing Features and Things to Improve

* Encrypt sensitive data (e.g. CPF's are being stored as raw text)
* Externalize environment properties (e.g. DB connection string and the application port are currently hardcoded) 
* Containerize the API and database: although there's a Dockerfile and docker-compose.yml available in the repository, the database migration is not correctly configured. I couldn't come up with a reasonable solution to wait for the database to accept connections before trying to perform the migration. An alternative approach was to separate the migration step from the compose and manually execute it, but it didn't seem right to build an image with the entire dotnet sdk)
* Implement unit and integration tests
* Provide an automated to backup the database

### Implementation Approach and Design Choices

In order to (partially) solve the proposed problem, I:
- Started by researching the problem domain and looking for use cases
- Learned a subset of F# through the official documentation
- Spent some time understanding the dotnet and F# tooling (e.g. how to build and run an app, how to add and manage dependencies)
- Learned the Saturn Framework by exploring the template creation tooling 

Once I had a good grasp of the chosen technologies, I:
- Created the project from a Saturn template (taking advantage of the generated structure, application configuration, and features such as database interaction and migration)
- Mapped the domain to F# (data types, interactions, possible errors)
- Implemented DTO's for the api and database layer and the necessary conversions between the domain types and the DTO's
- Built custom types and functions to deal with the composition of partial operations
- Revamped the generated repository and controller to suit my implementation approach (heavily influenced by Scott Wlaschin's [ROP](https://fsharpforfunandprofit.com/rop/))

Personal Thoughts
- I've extensively relied on F#'s type system to guide my implementation (which provided a great experience). Especially since I left the tests as an afterthought (and didn't manage to include them in time)
- Unlike F# and the dotnet tooling, Saturn has an anemic documentation.
- Saturn helped me a lot in getting things started
- Great articles about Functional Programming applied to Real World apps provided by the F# community
