namespace CpfScores

open Microsoft.AspNetCore.Http
open FSharp.Control.Tasks.ContextInsensitive
open System.Threading.Tasks
open Config
open Saturn

open Utility.Composition
open Domain

module Controller =

  type ResponseBody = {
    value: int
    created_at: System.DateTime
  }

  type RequestBody = {
    cpf : string
  }

  let private createCpfScore cpfInput : Outcome<CpfScore, FailState> =
    cpfInput
    |> CPF.create
    >>= ScoringService.scoreCpf
    >>= CpfScore.create

  let private toDto (inst : ScoreInstance) : ResponseBody =
    { value = Score.extract inst.value 
      created_at = inst.created_at }

  let private insert connStr outcome = TaskComp.elevate (Repository.insert connStr) outcome

  let private getAllScoresByCpf connStr outcome = TaskComp.elevate (Repository.getAllScoresByCpf connStr) outcome

  let private responseMessage (failure : FailState) = "Error while creating CPF score"

  let newCpfScore (ctx: HttpContext) : Task<HttpContext option> =
    task {
      let! input = Controller.getModel<RequestBody> ctx

      let config = Controller.getConfig ctx
 
      let! result = (createCpfScore input.cpf) |> insert config.connectionString

      match result with
      | Success _ -> return! Response.ok ctx "New CPF Score created"
      | Failure failState -> return! Response.badRequest ctx (responseMessage failState)
    }

  let listScoresByCpf (ctx : HttpContext) (cpf : string) =
    task {
      let cnf = Controller.getConfig ctx

      let! result = CPF.create cpf |> getAllScoresByCpf cnf.connectionString

      match result with
      | Success scores -> return! scores |> List.map toDto |> Response.ok ctx
      | Failure failState -> return! Response.badRequest ctx (responseMessage failState)
    }

  let resource = controller {
    show listScoresByCpf
    create newCpfScore
  }
