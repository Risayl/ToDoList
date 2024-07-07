# „тобы запустить:
1. «айти в PostgreSQL и создать базу данных с названием `ToDoListDb` под пользователем `postgres` с паролем `psql`
```create database "ToDoListDb" ```
2. Ћибо создайте базу данных со своим названием, пользователем и паролем и помен€ейте название, пользовател€ и пароль в файле `ToDoList.Api/appsettings.json`
3. ѕримените миграции к созданной базе данных
``` dotnet ef database update --project ToDoList.Infrastructure --startup-project ToDoList.Api ```
4. «апустите приложение
``` dotnet run --project ToDoList.Api```
5. «апустите тесты
``` dotnet test```
6. ѕримеры запросов доступны в файле `ToDoList.postman_collection.json` или через `http://localhost:5081/swagger/index.html`