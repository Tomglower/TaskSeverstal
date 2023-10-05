
## Ерофеев Роман. Задание 3.
---
### 1. Запуск
- Для запуска необходимо скачать и поставить решение.
- После в `appsettings.json` поменять логин и пароль от своей базы данных.
- В решении находится 2 проекта, необходимо запустить `Task3`. 
- После запуска проекта `Task3` , он автоматически создаст базу данных и необходимые миграции для неё.
### 2. Тесты 
- В решении находится 2 проекта `Task3` и `TestingProject`.
- Для того, чтобы протестировать контроллеры, необходимо кликнуть по проекту `TestingProject` правой кнопкой мыши и выбрать функцию "Выполнить тесты".
---
## Функциональность проекта:
### 1. Создание заметок:
Контроллер позволяет создавать новые заметки. Запросы POST на AddNote позволяют добавлять новую заметку в базу данных.

### 2. Получение списка заметок:
Контроллер предоставляет метод GetNotes для получения списка всех заметок из базы данных. Этот метод выполняет запрос GET.

### 3. Изменение заметок:
Контроллер позволяет обновлять существующие заметки. Метод ChangeNote принимает id заметки и объект Note с обновленными данными. Запросы PUT обновляют данные заметки в базе данных.

### 4. Удаление заметок:
С помощью метода DeleteNote можно удалять заметки по их id. Этот метод выполняет запрос DELETE.

### 4. Получение одной заметки по id:
Контроллер предоставляет метод GetNote, который позволяет получить информацию о заметке по её id. Этот метод также выполняет запрос GET.

### 6. Логирование:
Весь контроллер имеет встроенную систему логирования с использованием Log4net. Различные действия, такие как добавление, обновление и удаление заметок, а также возможные ошибки, записываются в журнал.

### 7. Использование кэширования (закомментировано):
В проекте также предусмотрено использование кэширования заметок в памяти с помощью IMemoryCache. Код содержит закомментированный метод GetNotes, который показывает пример использования кэширования.

---
## Выполненные требования:
 - [x] Принципы REST;
 - [x] Документирование API;
 - [x] Интуитивно понятный API;
 - [x] Обработка ошибок;
 - [x] Кеширование;
 - [x] Покрытие тестами;
 - [x] Контейнеризация;
 - [x] Масштабирование.
 
