namespace CpfScores

open Domain
open Utility.Composition

module ScoringService =

    let private rng = System.Random()

    let scoreCpf (cpf : CPF) : Outcome<ScoredCPF, FailState> =
        ScoredCPF.create cpf (rng.Next(1, 1001))