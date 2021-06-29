namespace CpfScores

open Database
open Npgsql
open System.Threading.Tasks
open FSharp.Control.Tasks.ContextInsensitive

open Domain 
open Utility.Composition

module Repository =

  type CpfScoreEntity = {
    id: string
    cpf: string
    value: int
    created_at: System.DateTime
  }

  type ScoreEntity = {
    value: int
    created_at: System.DateTime
  }

  module CpfScoreEntity =

    let fromDomain (score : CpfScore) : CpfScoreEntity =
      { id = score.id.ToString()
        cpf = CPF.extract score.cpf
        value = Score.extract score.value
        created_at = score.created_at }

  module ScoreInstanceEntity =

    let toDomain (entity : ScoreEntity) : Outcome<ScoreInstance, FailState> =
      ScoreInstance.create entity.value entity.created_at

    let normalize acc outcome =
      match (acc, outcome) with
      | (Success accVal, Success score) -> Success (score::accVal)
      | (_, Failure failState) -> Failure failState

  let getAllScoresByCpf (connectionString : string) (cpf : CPF) : Task<Outcome<ScoreInstance list, FailState>> =
    task {
      use connection = new NpgsqlConnection(connectionString)
      let! resultSet = query connection "SELECT value, created_at FROM CpfScores WHERE cpf=@cpf" (Some <| dict ["cpf" => CPF.extract cpf])

      match resultSet with
      | Ok seq -> return seq |> Seq.map ScoreInstanceEntity.toDomain |> Seq.fold ScoreInstanceEntity.normalize (Success [])
      | Error _ -> return Failure DatabaseError
    }

  let insert connectionString (score : CpfScore) : Task<Outcome<CpfScore, FailState>> =
    task {
      let entity = CpfScoreEntity.fromDomain score
      use connection = new NpgsqlConnection(connectionString)
      let! result = execute connection "INSERT INTO CpfScores(id, cpf, value, created_at) VALUES (@id, @cpf, @value, @created_at)" entity
      
      match result with
      | Ok _ -> return Success score
      | Error _ -> return Failure DatabaseError
    }
