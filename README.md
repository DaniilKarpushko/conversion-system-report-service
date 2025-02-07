# Тестовое задание для стажера-разработчика

## Задача 1
Реализовать сервис работы с системой отчетов по конверсии просмотров/оплат определенных товаров.

Пользователи за рабочий день могут запросить несколько отчетов. 
Запрос одного отчета стоит столько же денег, сколько запрос нескольких отчетов.

### Запрос отчета содержит:
- Период проверки конверсии
- Идентификатор товара
- Идентификатор оформления

### Ответ содержит:
- Рацио просмотров/оплат
- Количество оплат

### Требования:
1. Пользователь сервиса может ждать отчета сутки.
2. О запросе сервис узнает через шину сообщений.
3. Пользователь получает результат через HTTP (или GRPC) интерфейс.
4. В ожидании отчета пользователи могут часто вызывать эндпоинт, что может давать сбой в работе базы сервиса.

## Задача 2
С помощью сервиса уменьшить траты на составление отчетов.

## Технические данные
- Сервис должен быть написан на **C#.NET**.
- Шина сообщений **любая** (например, Kafka, RabbitMQ, ActiveMQ).
- База данных: преимущественно **PostgreSQL**, но может использоваться любая реляционная БД.
- Преимуществом будет использование **GRPC** интерфейса для получения результатов отчета.
- Сервис будет распространяться в виде **Docker-образа**.
- Обязательное покрытие **Unit-тестами**.

## Результат
Предоставить в виде публичного репозитория на **GitHub**.

## Оценка решения
Оценивается:
- Архитектура сервиса.
- Работа с ООП и бизнес-объектами.
- Оптимальность решения.
- Обработка ошибок в различных сценариях использования.
- Удобство и логичность сервиса в использовании.
- Общая чистота кода.

