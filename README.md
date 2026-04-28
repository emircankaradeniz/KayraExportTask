# Kayra Export Backend Developer Case Study

Bu proje, Backend Developer 3. Aþama task kapsamýnda .NET 8, C#, SQL Server, Onion Architecture, CQRS, JWT, Redis Cache, API Gateway, Rate Limiting, RabbitMQ, Serilog ve Seq kullanýlarak geliþtirilmiþ mikroservis tabanlý bir backend uygulamasýdýr.

## Ýçindekiler

- Proje Özeti
- Mimari Yapý
- Kullanýlan Teknolojiler
- Servisler
- Port Bilgileri
- Docker Servisleri
- Kurulum
- Migration Komutlarý
- Uygulamayý Çalýþtýrma
- API Gateway Kullanýmý
- AuthService API
- ProductService API
- LogService API
- Test Senaryosu
- Redis Cache ve Cache Invalidation
- Event Driven Mimari
- Structured Logging
- API Gateway Rate Limiting
- Authorization
- 12 Factor App Uyumluluðu
- SOLID Prensipleri
- Branch ve Versiyonlama
- Proje Durumu

## Proje Özeti

Proje üç ana mikroservisten ve bir API Gateway katmanýndan oluþmaktadýr.

- AuthService
- ProductService
- LogService
- ApiGateway

AuthService kullanýcý kayýt, giriþ, JWT token ve refresh token yönetiminden sorumludur.

ProductService ürün ekleme, güncelleme ve listeleme iþlemlerinden sorumludur. ProductService içinde Onion Architecture ve CQRS pattern uygulanmýþtýr.

LogService merkezi log kayýtlarýný oluþturmak ve sorgulamak için geliþtirilmiþtir.

ApiGateway YARP Reverse Proxy ile tüm servislere tek giriþ noktasý saðlar ve rate limiting uygular.

## Mimari Yapý

Proje klasör yapýsý:

```text
KayraExportTask
  src
    ApiGateway
    Services
      AuthService
        AuthService.API
        AuthService.Application
        AuthService.Domain
        AuthService.Infrastructure
      ProductService
        ProductService.API
        ProductService.Application
        ProductService.Domain
        ProductService.Infrastructure
      LogService
        LogService.API
        LogService.Application
        LogService.Domain
        LogService.Infrastructure
    Shared
      SharedKernel
  docker-compose.yml
  KayraExportTask.sln
  README.md
```

ProductService, Onion Architecture yaklaþýmýyla aþaðýdaki katmanlara ayrýlmýþtýr:

```text
ProductService.Domain
ProductService.Application
ProductService.Infrastructure
ProductService.API
```

Domain katmaný iþ kurallarýný ve entity yapýlarýný içerir.

Application katmaný CQRS command/query handler yapýlarýný, DTO nesnelerini ve abstraction interface'lerini içerir.

Infrastructure katmaný SQL Server, Redis, RabbitMQ ve repository implementasyonlarýný içerir.

API katmaný HTTP endpointlerini ve servis konfigürasyonlarýný içerir.

AuthService ve LogService de benzer þekilde Domain, Application, Infrastructure ve API katmanlarýna ayrýlmýþtýr.

## Kullanýlan Teknolojiler

- .NET 8
- C#
- ASP.NET Core Web API
- SQL Server
- Entity Framework Core
- Microsoft Identity
- JWT Bearer Authentication
- Refresh Token
- MediatR
- CQRS Pattern
- Onion Architecture
- Redis
- RabbitMQ
- YARP Reverse Proxy
- Rate Limiting
- Serilog
- Seq
- Docker Compose
- Swagger / OpenAPI

## Servisler

### AuthService

AuthService kullanýcý kimlik doðrulama iþlemlerinden sorumludur.

Özellikler:

- Register
- Login
- Refresh Token
- Microsoft Identity
- JWT Token üretimi
- Role Based Authorization
- Admin, Manager, User rolleri

### ProductService

ProductService ürün iþlemlerinden sorumludur.

Özellikler:

- Ürün listeleme
- Ürün detay getirme
- Ürün ekleme
- Ürün güncelleme
- JWT doðrulama
- Policy Based Authorization
- Redis cache
- Cache invalidation
- RabbitMQ event publish
- Onion Architecture
- CQRS

### LogService

LogService merkezi log yönetiminden sorumludur.

Özellikler:

- Log oluþturma
- Son loglarý listeleme
- Log seviyesine göre listeleme
- SQL Server üzerinde log saklama
- Information, Warning, Error, Critical seviyeleri

### ApiGateway

ApiGateway dýþ dünya ile mikroservisler arasýnda tek giriþ noktasýdýr.

Özellikler:

- YARP Reverse Proxy
- Auth route
- Product route
- Log route
- Rate Limiting
- CORS

## Port Bilgileri

```text
ApiGateway      http://localhost:5000
AuthService     http://localhost:5101
ProductService  http://localhost:5053
LogService      http://localhost:5103
Seq             http://localhost:5341
RabbitMQ UI     http://localhost:15672
SQL Server      localhost,1433
Redis           localhost:6379
```

## Docker Servisleri

Docker Compose ile aþaðýdaki destek servisleri ayaða kaldýrýlýr:

- SQL Server
- Redis
- RabbitMQ
- Seq

Docker servislerini baþlatmak için:

```bash
docker compose up -d
```

Çalýþan containerlarý görmek için:

```bash
docker ps
```

Docker servislerini durdurmak için:

```bash
docker compose down
```

## Kurulum

Projeyi klonladýktan sonra ana dizinde aþaðýdaki komut çalýþtýrýlýr:

```bash
dotnet restore
```

Ardýndan build alýnýr:

```bash
dotnet build
```

Entity Framework CLI aracý yüklü deðilse aþaðýdaki komutla yüklenebilir:

```bash
dotnet tool install --global dotnet-ef --version 8.0.0
```

Yüklüyse güncellemek için:

```bash
dotnet tool update --global dotnet-ef --version 8.0.0
```

## Migration Komutlarý

Migration komutlarýndan önce SQL Server container çalýþýyor olmalýdýr.

```bash
docker compose up -d
```

### ProductService Migration

```bash
dotnet ef migrations add InitialCreate --project src/Services/ProductService/ProductService.Infrastructure --startup-project src/Services/ProductService/ProductService.API --context ProductDbContext --output-dir Data/Migrations
```

```bash
dotnet ef database update --project src/Services/ProductService/ProductService.Infrastructure --startup-project src/Services/ProductService/ProductService.API --context ProductDbContext
```

### AuthService Migration

```bash
dotnet ef migrations add InitialCreate --project src/Services/AuthService/AuthService.Infrastructure --startup-project src/Services/AuthService/AuthService.API --context AuthDbContext --output-dir Data/Migrations
```

```bash
dotnet ef database update --project src/Services/AuthService/AuthService.Infrastructure --startup-project src/Services/AuthService/AuthService.API --context AuthDbContext
```

### LogService Migration

```bash
dotnet ef migrations add InitialCreate --project src/Services/LogService/LogService.Infrastructure --startup-project src/Services/LogService/LogService.API --context LogDbContext --output-dir Data/Migrations
```

```bash
dotnet ef database update --project src/Services/LogService/LogService.Infrastructure --startup-project src/Services/LogService/LogService.API --context LogDbContext
```

## Uygulamayý Çalýþtýrma

Her servis ayrý terminalde çalýþtýrýlýr.

### AuthService

```powershell
cd C:\Users\rustl\OneDrive\Masaüstü\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/Services/AuthService/AuthService.API/AuthService.API.csproj
```

### ProductService

```powershell
cd C:\Users\rustl\OneDrive\Masaüstü\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/Services/ProductService/ProductService.API/ProductService.API.csproj
```

### LogService

```powershell
cd C:\Users\rustl\OneDrive\Masaüstü\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/Services/LogService/LogService.API/LogService.API.csproj
```

### ApiGateway

```powershell
cd C:\Users\rustl\OneDrive\Masaüstü\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/ApiGateway/ApiGateway.csproj
```

## API Gateway Kullanýmý

Gateway ana endpoint:

```text
http://localhost:5000
```

Gateway route bilgileri:

```text
/api/auth/**      -> AuthService
/api/products/**  -> ProductService
/api/logs/**      -> LogService
```

Gateway health test:

```http
GET http://localhost:5000/
```

Product listeleme:

```http
GET http://localhost:5000/api/products
```

Log listeleme:

```http
GET http://localhost:5000/api/logs?count=50
```

Auth login:

```http
POST http://localhost:5000/api/auth/login
```

PowerShell ile Gateway üzerinden login testi:

```powershell
Invoke-RestMethod -Method Post `
  -Uri "http://localhost:5000/api/auth/login" `
  -ContentType "application/json" `
  -Body '{"email":"admin@kayraexport.com","password":"Admin12345"}'
```

## AuthService API

Swagger:

```text
http://localhost:5101/swagger
```

### Register

```http
POST /api/auth/register
```

Request:

```json
{
  "fullName": "Admin User",
  "email": "admin@kayraexport.com",
  "password": "Admin12345",
  "role": "Admin"
}
```

### Login

```http
POST /api/auth/login
```

Request:

```json
{
  "email": "admin@kayraexport.com",
  "password": "Admin12345"
}
```

Response içinde `accessToken` ve `refreshToken` döner.

### Refresh Token

```http
POST /api/auth/refresh-token
```

Request:

```json
{
  "accessToken": "access-token-value",
  "refreshToken": "refresh-token-value"
}
```

## ProductService API

Swagger:

```text
http://localhost:5053/swagger
```

### Product List

```http
GET /api/products
```

Bu endpoint anonymous eriþime açýktýr.

### Product Detail

```http
GET /api/products/{id}
```

Bu endpoint anonymous eriþime açýktýr.

### Create Product

```http
POST /api/products
```

Bu endpoint JWT token ister. Kullanýcý `Admin` veya `Manager` rolünde olmalýdýr.

Request:

```json
{
  "name": "Sample Product",
  "description": "Professional test product for Kayra Export task.",
  "sku": "SKU-001",
  "price": 149.90,
  "stockQuantity": 25
}
```

### Update Product

```http
PUT /api/products/{id}
```

Bu endpoint JWT token ister. Kullanýcý `Admin` veya `Manager` rolünde olmalýdýr.

Request:

```json
{
  "name": "Updated Product",
  "description": "Updated product description.",
  "price": 199.90,
  "stockQuantity": 50,
  "status": 2
}
```

Product status deðerleri:

```text
1 Draft
2 Active
3 Passive
4 Deleted
```

## LogService API

Swagger:

```text
http://localhost:5103/swagger
```

### Create Log

```http
POST /api/logs
```

Request:

```json
{
  "serviceName": "ProductService",
  "level": 1,
  "message": "Product list endpoint called successfully.",
  "exception": null,
  "traceId": "manual-test-trace-id",
  "path": "/api/products",
  "method": "GET"
}
```

### Get Latest Logs

```http
GET /api/logs?count=50
```

### Get Logs By Level

```http
GET /api/logs/level/1?count=50
```

Log level deðerleri:

```text
1 Information
2 Warning
3 Error
4 Critical
```

## Test Senaryosu

1. Docker servislerini baþlat.

```bash
docker compose up -d
```

2. Migration komutlarýný çalýþtýr.

3. AuthService, ProductService, LogService ve ApiGateway servislerini ayrý terminallerde baþlat.

4. AuthService üzerinden Admin kullanýcý oluþtur.

```json
{
  "fullName": "Admin User",
  "email": "admin@kayraexport.com",
  "password": "Admin12345",
  "role": "Admin"
}
```

5. Login endpointi ile access token al.

6. ProductService Swagger üzerinde Authorize butonuna týkla.

7. Token deðerini þu formatta gir.

```text
Bearer access-token-value
```

8. ProductService üzerinden ürün oluþtur.

9. ProductService üzerinden ürünleri listele.

10. Gateway üzerinden ürünleri listele.

```text
http://localhost:5000/api/products
```

11. LogService üzerinden log oluþtur.

12. Gateway üzerinden loglarý listele.

```text
http://localhost:5000/api/logs?count=50
```

13. Seq panelinden structured loglarý kontrol et.

```text
http://localhost:5341
```

## Redis Cache ve Cache Invalidation

Product listeleme iþlemlerinde Redis cache kullanýlýr.

Akýþ:

```text
GET /api/products
  önce Redis kontrol edilir
  cache varsa Redis üzerinden response döner
  cache yoksa SQL Server'dan veri okunur
  sonuç Redis'e yazýlýr
```

Ürün ekleme veya güncelleme iþlemlerinden sonra cache invalidation yapýlýr.

Akýþ:

```text
POST /api/products
  ürün SQL Server'a kaydedilir
  Redis product list cache temizlenir
  RabbitMQ event publish edilir
```

```text
PUT /api/products/{id}
  ürün SQL Server'da güncellenir
  Redis product list cache temizlenir
  RabbitMQ event publish edilir
```

## Event Driven Mimari

ProductService içinde ürün ekleme ve ürün güncelleme iþlemlerinden sonra RabbitMQ üzerinden integration event publish edilir.

Eventler:

```text
ProductCreatedIntegrationEvent
ProductUpdatedIntegrationEvent
```

RabbitMQ Management UI:

```text
http://localhost:15672
```

Default kullanýcý bilgileri:

```text
username: guest
password: guest
```

## Structured Logging

Projede Serilog ve Seq kullanýlmýþtýr.

Her servis aþaðýdaki bilgilerle structured log üretir:

```text
Timestamp
Level
Message
ServiceName
MachineName
ProcessId
ThreadId
Request Path
HTTP Method
Elapsed Time
Exception
```

Seq paneli:

```text
http://localhost:5341
```

Servis bazlý filtre örnekleri:

```text
ServiceName = 'AuthService'
ServiceName = 'ProductService'
ServiceName = 'LogService'
ServiceName = 'ApiGateway'
```

## API Gateway Rate Limiting

ApiGateway üzerinde global fixed window rate limiting uygulanmýþtýr.

Varsayýlan ayar:

```text
100 request / 1 minute
Queue limit: 20
Rejected response: 429 Too Many Requests
```

Bu yapý API isteklerini merkezi olarak kontrol altýna almak için eklenmiþtir.

## Authorization

Projede hem role based hem de policy based authorization uygulanmýþtýr.

Roller:

```text
Admin
Manager
User
```

ProductService yazma iþlemleri için kullanýlan policy:

```text
ProductWritePolicy
```

Bu policy `Admin` veya `Manager` rolü ister.

Korunan endpointler:

```http
POST /api/products
PUT /api/products/{id}
```

## 12 Factor App Uyumluluðu

### Codebase

Tüm proje tek bir merkezi solution altýnda yönetilmektedir.

### Dependencies

Tüm baðýmlýlýklar NuGet paketleri üzerinden tanýmlanmýþtýr.

### Config

Connection string, JWT, Redis, RabbitMQ ve Seq ayarlarý appsettings üzerinden yönetilmektedir. Production ortamýnda bu ayarlar environment variable olarak verilebilir.

### Backing Services

SQL Server, Redis, RabbitMQ ve Seq baðýmsýz destek servisleri olarak Docker Compose ile yönetilmektedir.

### Build, Release, Run

Build ve runtime süreçleri ayrýdýr. Proje önce restore/build edilir, ardýndan servisler ayrý process olarak çalýþtýrýlýr.

### Processes

Servisler stateless olacak þekilde tasarlanmýþtýr. State SQL Server, Redis ve RabbitMQ gibi external servislerde tutulur.

### Port Binding

Her servis kendi portunda çalýþýr. ApiGateway dýþ giriþ noktasýdýr.

### Concurrency

Servisler baðýmsýz process olarak çalýþtýðý için yatay ölçeklemeye uygundur.

### Disposability

ASP.NET Core lifecycle yapýsý kullanýlýr. Servisler Ctrl+C veya process stop ile güvenli þekilde durdurulabilir.

### Dev/Prod Parity

Docker Compose ile destek servisleri local ortamda production benzeri þekilde çalýþtýrýlýr.

### Logs

Loglar Serilog ile console ve Seq üzerine structured formatta aktarýlýr.

### Admin Processes

Migration iþlemleri dotnet ef komutlarý ile ayrý admin process olarak yürütülür.

## SOLID Prensipleri

### Single Responsibility Principle

Her servis ve katman tek bir sorumluluk alanýna sahiptir.

### Open/Closed Principle

Application katmanýndaki abstraction yapýlarý sayesinde yeni repository, cache veya event bus implementasyonlarý mevcut kodu bozmadan eklenebilir.

### Liskov Substitution Principle

Interface üzerinden çalýþan servisler farklý implementasyonlarla deðiþtirilebilir.

### Interface Segregation Principle

Repository, cache, event bus ve unit of work sorumluluklarý ayrý interface'lere bölünmüþtür.

### Dependency Inversion Principle

Application katmaný Infrastructure katmanýna doðrudan baðýmlý deðildir. Baðýmlýlýklar abstraction üzerinden yönetilir.

## Branch ve Versiyonlama

Task gereksinimine uygun branch yapýsý:

## Proje Durumu

Tamamlanan baþlýklar:

```text
Auth Microservice
Product Microservice
Log Microservice
API Gateway
JWT Authentication
Refresh Token
Microsoft Identity
Role-Based Authorization
Policy-Based Authorization
Onion Architecture
CQRS
Redis Cache
Cache Invalidation
RabbitMQ Event Publish
SQL Server Persistence
Structured Logging
Serilog
Seq
Docker Compose
Swagger Documentation
Rate Limiting
```

## Gönderim Notu

Proje çalýþtýrýlmadan önce Docker servisleri baþlatýlmalý, migration komutlarý uygulanmalý ve ardýndan servisler ayrý terminal pencerelerinde çalýþtýrýlmalýdýr.

Ana giriþ noktasý ApiGateway servisidir:

```text
http://localhost:5000
```
