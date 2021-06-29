module Program

open System.Reflection
open SimpleMigrations
open Npgsql
open SimpleMigrations.DatabaseProvider
open SimpleMigrations.Console


[<EntryPoint>]
let main argv =
    let assembly = Assembly.GetExecutingAssembly()
    use db = new NpgsqlConnection "Host=localhost;Username=postgres;Password=passw0rd;Database=postgres"
    let provider = PostgresqlDatabaseProvider(db)
    let migrator = SimpleMigrator(assembly, provider)
    let consoleRunner = ConsoleRunner(migrator)
    consoleRunner.Run(argv) |> ignore
    0