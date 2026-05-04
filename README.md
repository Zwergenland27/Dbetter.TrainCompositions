# DBetter TrainCompositions
Collects and aggregats all information about train compositions, containing:
* Planned coach sequence
* Real coach sequence
  * Consists
  * Multiple Units
* Vehicles

:info: this is an extension module of DBetter, it does not work as a standalone application!

## Development
* `docker compose up -d`

### Create Migration and update Database
`dotnet ef migrations add --project src/DBetter.TrainCompositions.Infrastructure --startup-project src/DBetter.TrainCompositions.WebApi <name>`
`dotnet ef update database --project src/DBetter.TrainCompositions.Infrastructure --startup-project src/DBetter.TrainCompositions.WebApi`