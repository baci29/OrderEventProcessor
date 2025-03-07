# OrderEventProcessor

OrderEventProcessor je .NET 8 konzolová aplikace vytvořená ve Visual Studio 2022, která zpracovává zprávy ze dvou typů RabbitMQ front:
- **OrderEvent**: Vytváří záznam objednávky, který je uložen v PostgreSQL DB.
- **PaymentEvent**: Kontroluje, zda jsou související objednávky plně zaplaceny, a vypisuje zprávu do konzole.

## Technologie a závislosti

- **.NET 8**: Vývojové prostředí pro aplikaci.
- **PostgreSQL**: Databáze pro ukládání záznamů objednávek.
- **RabbitMQ)**: Fronta zpráv pro zpracování objednávek a plateb.
- **Docker**: Virtualizace a nasazení aplikace.

## Struktura projektu

OrderEventProcessor/ ├── Database/ │ └── init.sql├── Models/ │ ├── OrderEvent.cs│ └── PaymentEvent.cs├── Services/ │ ├── IOrderService.cs│ ├── IPaymentService.cs│ ├── OrderService.cs│ ├── PaymentService.cs│ └── RabbitMqListener.cs├── Tests/ │ ├── Services/ │ │ ├── OrderServiceTests.cs│ │ └── PaymentServiceTests.cs├── appsettings.json├── Dockerfile ├── docker-compose.yml└── Program.cs
