# Чтобы запустить:
1. Зайти в PostgreSQL и создать базу данных с названием `ToDoListDb` под пользователем `postgres` с паролем `psql`

```create database "ToDoListDb" ```
2. Либо создайте базу данных со своим названием, пользователем и паролем и поменяейте название, пользователя и пароль в файле `ToDoList.Api/appsettings.json`
3. Примените миграции к созданной базе данных

``` dotnet ef database update --project ToDoList.Infrastructure --startup-project ToDoList.Api ```
4. Запустите приложение

``` dotnet run --project ToDoList.Api```
5. Запустите тесты

``` dotnet test```
6. Примеры запросов доступны в файле `ToDoList.postman_collection.json` или через `http://localhost:5081/swagger/index.html`
