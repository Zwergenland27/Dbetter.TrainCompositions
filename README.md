# DBetter TrainCompositions
Collects and aggregats all information about train compositions, containing:
* Planned coach sequence
* Real coach sequence
  * Consists
  * Multiple Units
* Vehicles

:info: this is an extension module of DBetter, it does not work as a standalone application!

# Environment variables
Name | Description | Remarks | Default value | Development value |
--- | --- | --- | --- | --- |
PostgreSQL__ConnectionString | Connection string for the postgres instance | - | - | `Host=localhost;Database=DBetterTrainCompositions;Username=user;Password=password` |
RabbitMq__Hostname | Hostname of rabbitmq broker | - | - | `localhost` |

## Development
* `docker compose up -d`

### Create Migration and update Database
`dotnet ef migrations add --project src/DBetter.TrainCompositions.Infrastructure --startup-project src/DBetter.TrainCompositions.WebApi <name>`
`dotnet ef update database --project src/DBetter.TrainCompositions.Infrastructure --startup-project src/DBetter.TrainCompositions.WebApi`