namespace CpfScores

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.ContextInsensitive
open System.Threading.Tasks
open Config
open Saturn

open Utility.Composition
open Domain

module Controller =

  module DTO =

    type ResponseBody = {
      value: int
      created_at: string
    }

    type RequestBody = {
      cpf : string
    }

    let toDto (inst : ScoreInstance) : ResponseBody =
      { value = Score.extract inst.value 
        created_at = inst.created_at.ToString "dd/MM/yyyy HH:mm:ss" }
  
  type ErrorStatus =
  | BadRequest of string
  | ServerError of string 

  let private createCpfScore cpfInput : Outcome<CpfScore, FailState> =
    cpfInput
    |> CPF.create
    >>= ScoringService.scoreCpf
    >>= CpfScore.create

  let private insert connStr outcome = TaskComp.elevate (Repository.insert connStr) outcome

  let private getAllScoresByCpf connStr outcome = TaskComp.elevate (Repository.getAllScoresByCpf connStr) outcome

  let private failToErrorResponse (failure : FailState) = 
    match failure with
    | InvalidCpf -> BadRequest "The provided CPF is not valid."
    | ScoreOutOfValidRange -> ServerError "Unable to process request, please contact the system administrator"
    | DatabaseError _ -> ServerError "Internal error while processing the request."

  let private handleErrorResponse ctx error =
    task {
      match error with
      | BadRequest msg -> return! Response.badRequest ctx msg
      | ServerError er -> return! Response.internalError ctx er
    }

  let newCpfScore (ctx: HttpContext) : Task<HttpContext option> =
    task {
      let! input = Controller.getModel<DTO.RequestBody> ctx

      let config = Controller.getConfig ctx
 
      let! result = (createCpfScore input.cpf) |> insert config.connectionString

      match result with
      | Success _ -> return! Response.ok ctx "New score for CPF registered!"
      | Failure fail -> return! failToErrorResponse fail |> handleErrorResponse ctx
    }

  let listScoresByCpf (ctx : HttpContext) (cpf : string) =
    task {
      let cnf = Controller.getConfig ctx

      let! result = CPF.create cpf |> getAllScoresByCpf cnf.connectionString

      match result with
      | Success scores -> return! scores |> List.map DTO.toDto |> Response.ok ctx
      | Failure fail -> return! failToErrorResponse fail |> handleErrorResponse ctx
    }

  let resource = controller {
    show listScoresByCpf
    create newCpfScore
  }
