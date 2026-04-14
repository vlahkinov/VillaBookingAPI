# VillaBookingAPI

REST API за управление на резервации на вили, проектирано за синхронизация с .NET MAUI мобилно приложение.

## Технологии

- ASP.NET Core 8.0 Web API (Controllers)
- Entity Framework Core 8.0 с SQL Server (LocalDB)
- Swagger / OpenAPI документация
- Clean Architecture (Controller → Service → DbContext)

## Структура на проекта

```
VillaBookingAPI/
├── Controllers/
│   └── BookingsController.cs      # API endpoints (CRUD)
├── Models/
│   ├── Booking.cs                 # Entity model
│   ├── ApiResponse.cs             # Standard response wrapper
│   └── Dto/
│       ├── BookingCreateDto.cs    # DTO за създаване
│       └── BookingUpdateDto.cs    # DTO за редактиране
├── Data/
│   └── AppDbContext.cs            # EF Core DbContext + seed data
├── Services/
│   ├── IBookingService.cs         # Интерфейс
│   └── BookingService.cs          # Имплементация + бизнес логика
├── Program.cs                     # DI, middleware, конфигурация
├── appsettings.json               # Connection string
└── VillaBookingAPI.csproj         # NuGet зависимости
```

## Как да стартирате

### Предварителни изисквания

- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0
- SQL Server LocalDB (идва с Visual Studio) или друг SQL Server инстанция

### Стъпки

1. **Клонирайте / копирайте** папката `VillaBookingAPI`

2. **Възстановете пакетите:**
   ```bash
   cd VillaBookingAPI
   dotnet restore
   ```

3. **Създайте миграция и база данни:**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   > Ако `dotnet ef` не е инсталиран: `dotnet tool install --global dotnet-ef`

4. **Стартирайте API-то:**
   ```bash
   dotnet run
   ```

5. **Отворете Swagger UI:**
   ```
   https://localhost:7152/swagger
   ```

### Ако използвате друг SQL Server

Променете connection string-а в `appsettings.json`:
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=VillaBookingDb;Trusted_Connection=True;TrustServerCertificate=True"
```

## API Endpoints

| Метод  | Route               | Описание                      |
|--------|---------------------|-------------------------------|
| GET    | /api/bookings       | Всички резервации (по дата)   |
| GET    | /api/bookings/{id}  | Резервация по Id              |
| POST   | /api/bookings       | Създаване на резервация       |
| PUT    | /api/bookings/{id}  | Редактиране на резервация     |
| DELETE | /api/bookings/{id}  | Изтриване на резервация       |

## Бизнес правила

- **GuestsCount** трябва да е между 1 и 4
- **StartDate** трябва да е преди **EndDate**
- **Няма припокриващи се резервации** за една и съща къща (HouseId)
- Стандартни HTTP status кодове: 200, 201, 400, 404, 500

## Seed Data

Базата се зарежда с 5 примерни резервации при първата миграция.

## Пример за POST заявка

```json
{
  "clientName": "Нов Клиент",
  "guestsCount": 2,
  "startDate": "2025-09-01T00:00:00",
  "endDate": "2025-09-05T00:00:00",
  "houseId": 1,
  "isDepositPaid": false,
  "createdBy": "maui_app"
}
```
