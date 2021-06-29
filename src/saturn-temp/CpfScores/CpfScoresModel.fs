namespace CpfScores

open System
open Utility.Composition

module Domain = 

  type FailState =
  | InvalidCpf
  | ScoreOutOfValidRange
  | UnableToScoreCpf
  | DatabaseError

  type CPF = private CPF of string
  
  type Score = private Score of int

  type CpfScore = {
    id: Guid
    cpf: CPF
    value: Score
    created_at: DateTime
  }

  type ScoredCPF = {
    cpf: CPF
    value: Score
  }

  type ScoreInstance = {
    value: Score
    created_at: DateTime
  }

  module CPF =

    let create (value : string) : Outcome<CPF, FailState> =
      if BrazilianUtils.Cpf.IsValid value
        then Success (value |> String.filter Char.IsDigit |> CPF) 
        else Failure InvalidCpf

    let extract (CPF value) = value

  module Score =
    
    let create (value : int) =
      if value > 0 && value <= 1000
        then Score value |> Success 
        else Failure ScoreOutOfValidRange

    let extract (Score value) = value

  module ScoredCPF =

    let create (cpf : CPF) (value : int) : Outcome<ScoredCPF, FailState> =
      Score.create value |> map (fun (score : Score) -> { cpf = cpf; value = score })

  module CpfScore =

    let create (scored : ScoredCPF) : Outcome<CpfScore, FailState> =
      Success { id = Guid.NewGuid(); cpf = scored.cpf; value = scored.value; created_at = DateTime.Now; }

  module ScoreInstance =

    let create (value : int) (created_at : DateTime) : Outcome<ScoreInstance, FailState> =
      Score.create value |> map (fun (score : Score) -> { value = score; created_at = created_at; })
