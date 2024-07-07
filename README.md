# ����� ���������:
1. ����� � PostgreSQL � ������� ���� ������ � ��������� `ToDoListDb` ��� ������������� `postgres` � ������� `psql`
```create database "ToDoListDb" ```
2. ���� �������� ���� ������ �� ����� ���������, ������������� � ������� � ���������� ��������, ������������ � ������ � ����� `ToDoList.Api/appsettings.json`
3. ��������� �������� � ��������� ���� ������
``` dotnet ef database update --project ToDoList.Infrastructure --startup-project ToDoList.Api ```
4. ��������� ����������
``` dotnet run --project ToDoList.Api```
5. ��������� �����
``` dotnet test```
6. ������� �������� �������� � ����� `ToDoList.postman_collection.json` ��� ����� `http://localhost:5081/swagger/index.html`