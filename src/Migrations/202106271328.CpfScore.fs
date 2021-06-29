namespace Migrations
open SimpleMigrations

[<Migration(202106271328L, "Create CpfScores")>]
type CreateCpfScores() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE CpfScores(
      id TEXT NOT NULL,
      cpf TEXT NOT NULL,
      value INT NOT NULL,
      created_at TIMESTAMP NOT NULL,
      PRIMARY KEY (id)
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE CpfScores")
