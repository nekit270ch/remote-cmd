# remote-cmd
Инструмент для удаленного выполнения команд
```
remotecmd [-n <host>] [-c <id> <host>]
    -n <host>
        Создать новый клиент.
            host: URL для RemoteCmd API

    -c <id> <host>
        Подключиться к запущенному клиенту.
            id: Уникальный ID клиента
            host: URL для RemoteCmd API
```
Доступные сервера:
- https://nekit270.ch/rcapi

Вы можете создать собственный сервер RemoteCmd API и использовать его. Для этого скопируйте папку `serv` на свой HTTP-сервер.
