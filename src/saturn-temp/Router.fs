module Router

open Saturn
open Giraffe.Core
open Giraffe.ResponseWriters

let api = pipeline {
    plug acceptJson
    set_header "x-pipeline-type" "Api"
}

let appRouter = router {
    not_found_handler (text "Resource not found.")
    pipe_through api

    forward "/score" CpfScores.Controller.resource
}
