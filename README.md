# Kayra Export Backend Developer Case Study

Bu proje, Backend Developer 3. Aşama task kapsamında .NET 8, C#, SQL Server, Onion Architecture, CQRS, JWT, Redis Cache, API Gateway, Rate Limiting, RabbitMQ, Serilog ve Seq kullanılarak geliştirilmiş mikroservis tabanlı bir backend uygulamasıdır.

## İçindekiler

- Proje Özeti
- Mimari Yapı
- Kullanılan Teknolojiler
- Servisler
- Port Bilgileri
- Docker Servisleri
- Kurulum
- Migration Komutları
- Uygulamayı Çalıştırma
- API Gateway Kullanımı
- AuthService API
- ProductService API
- LogService API
- Test Senaryosu
- Redis Cache ve Cache Invalidation
- Event Driven Mimari
- Structured Logging
- API Gateway Rate Limiting
- Authorization
- 12 Factor App Uyumluluğu
- SOLID Prensipleri
- Branch ve Versiyonlama
- Proje Durumu

## Proje Özeti

Proje üç ana mikroservisten ve bir API Gateway katmanından oluşmaktadır.

- AuthService
- ProductService
- LogService
- ApiGateway

AuthService kullanıcı kayıt, giriş, JWT token ve refresh token yönetiminden sorumludur.

ProductService ürün ekleme, güncelleme ve listeleme işlemlerinden sorumludur. ProductService içinde Onion Architecture ve CQRS pattern uygulanmıştır.

LogService merkezi log kayıtlarını oluşturmak ve sorgulamak için geliştirilmiştir.

ApiGateway YARP Reverse Proxy ile tüm servislere tek giriş noktası sağlar ve rate limiting uygular.

## Mimari Yapı

Proje klasör yapısı:

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

ProductService, Onion Architecture yaklaşımıyla aşağıdaki katmanlara ayrılmıştır:

```text
ProductService.Domain
ProductService.Application
ProductService.Infrastructure
ProductService.API
```

Domain katmanı iş kurallarını ve entity yapılarını içerir.

Application katmanı CQRS command/query handler yapılarını, DTO nesnelerini ve abstraction interface'lerini içerir.

Infrastructure katmanı SQL Server, Redis, RabbitMQ ve repository implementasyonlarını içerir.

API katmanı HTTP endpointlerini ve servis konfigürasyonlarını içerir.

AuthService ve LogService de benzer şekilde Domain, Application, Infrastructure ve API katmanlarına ayrılmıştır.

## Kullanılan Teknolojiler

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

AuthService kullanıcı kimlik doğrulama işlemlerinden sorumludur.

Özellikler:

- Register
- Login
- Refresh Token
- Microsoft Identity
- JWT Token üretimi
- Role Based Authorization
- Admin, Manager, User rolleri

### ProductService

ProductService ürün işlemlerinden sorumludur.

Özellikler:

- Ürün listeleme
- Ürün detay getirme
- Ürün ekleme
- Ürün güncelleme
- JWT doğrulama
- Policy Based Authorization
- Redis cache
- Cache invalidation
- RabbitMQ event publish
- Onion Architecture
- CQRS

### LogService

LogService merkezi log yönetiminden sorumludur.

Özellikler:

- Log oluşturma
- Son logları listeleme
- Log seviyesine göre listeleme
- SQL Server üzerinde log saklama
- Information, Warning, Error, Critical seviyeleri

### ApiGateway

ApiGateway dış dünya ile mikroservisler arasında tek giriş noktasıdır.

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

Docker Compose ile aşağıdaki destek servisleri ayağa kaldırılır:

- SQL Server
- Redis
- RabbitMQ
- Seq

Docker servislerini başlatmak için:

```bash
docker compose up -d
```

Çalışan containerları görmek için:

```bash
docker ps
```

Docker servislerini durdurmak için:

```bash
docker compose down
```

## Kurulum

Projeyi klonladıktan sonra ana dizinde aşağıdaki komut çalıştırılır:

```bash
dotnet restore
```

Ardından build alınır:

```bash
dotnet build
```

Entity Framework CLI aracı yüklü değilse aşağıdaki komutla yüklenebilir:

```bash
dotnet tool install --global dotnet-ef --version 8.0.0
```

Yüklüyse güncellemek için:

```bash
dotnet tool update --global dotnet-ef --version 8.0.0
```

## Migration Komutları

Migration komutlarından önce SQL Server container çalışıyor olmalıdır.

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

## Uygulamayı Çalıştırma

Her servis ayrı terminalde çalıştırılır.

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

## API Gateway Kullanımı

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

Bu endpoint anonymous erişime açıktır.

### Product Detail

```http
GET /api/products/{id}
```

Bu endpoint anonymous erişime açıktır.

### Create Product

```http
POST /api/products
```

Bu endpoint JWT token ister. Kullanıcı `Admin` veya `Manager` rolünde olmalıdır.

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

Bu endpoint JWT token ister. Kullanıcı `Admin` veya `Manager` rolünde olmalıdır.

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

Product status değerleri:

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

Log level değerleri:

```text
1 Information
2 Warning
3 Error
4 Critical
```

## Test Senaryosu

1. Docker servislerini başlat.

```bash
docker compose up -d
```

2. Migration komutlarını çalıştır.

3. AuthService, ProductService, LogService ve ApiGateway servislerini ayrı terminallerde başlat.

4. AuthService üzerinden Admin kullanıcı oluştur.

```json
{
  "fullName": "Admin User",
  "email": "admin@kayraexport.com",
  "password": "Admin12345",
  "role": "Admin"
}
```

5. Login endpointi ile access token al.

6. ProductService Swagger üzerinde Authorize butonuna tıkla.

7. Token değerini şu formatta gir.

```text
Bearer access-token-value
```

8. ProductService üzerinden ürün oluştur.

9. ProductService üzerinden ürünleri listele.

10. Gateway üzerinden ürünleri listele.

```text
http://localhost:5000/api/products
```

11. LogService üzerinden log oluştur.

12. Gateway üzerinden logları listele.

```text
http://localhost:5000/api/logs?count=50
```

13. Seq panelinden structured logları kontrol et.

```text
http://localhost:5341
```

## Redis Cache ve Cache Invalidation

Product listeleme işlemlerinde Redis cache kullanılır.

Akış:

```text
GET /api/products
  önce Redis kontrol edilir
  cache varsa Redis üzerinden response döner
  cache yoksa SQL Server'dan veri okunur
  sonuç Redis'e yazılır
```

Ürün ekleme veya güncelleme işlemlerinden sonra cache invalidation yapılır.

Akış:

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

ProductService içinde ürün ekleme ve ürün güncelleme işlemlerinden sonra RabbitMQ üzerinden integration event publish edilir.

Eventler:

```text
ProductCreatedIntegrationEvent
ProductUpdatedIntegrationEvent
```

RabbitMQ Management UI:

```text
http://localhost:15672
```

Default kullanıcı bilgileri:

```text
username: guest
password: guest
```

## Structured Logging

Projede Serilog ve Seq kullanılmıştır.

Her servis aşağıdaki bilgilerle structured log üretir:

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

Servis bazlı filtre örnekleri:

```text
ServiceName = 'AuthService'
ServiceName = 'ProductService'
ServiceName = 'LogService'
ServiceName = 'ApiGateway'
```

## API Gateway Rate Limiting

ApiGateway üzerinde global fixed window rate limiting uygulanmıştır.

Varsayılan ayar:

```text
100 request / 1 minute
Queue limit: 20
Rejected response: 429 Too Many Requests
```

Bu yapı API isteklerini merkezi olarak kontrol altına almak için eklenmiştir.

## Authorization

Projede hem role based hem de policy based authorization uygulanmıştır.

Roller:

```text
Admin
Manager
User
```

ProductService yazma işlemleri için kullanılan policy:

```text
ProductWritePolicy
```

Bu policy `Admin` veya `Manager` rolü ister.

Korunan endpointler:

```http
POST /api/products
PUT /api/products/{id}
```

## 12 Factor App Uyumluluğu

### Codebase

Tüm proje tek bir merkezi solution altında yönetilmektedir.

### Dependencies

Tüm bağımlılıklar NuGet paketleri üzerinden tanımlanmıştır.

### Config

Connection string, JWT, Redis, RabbitMQ ve Seq ayarları appsettings üzerinden yönetilmektedir. Production ortamında bu ayarlar environment variable olarak verilebilir.

### Backing Services

SQL Server, Redis, RabbitMQ ve Seq bağımsız destek servisleri olarak Docker Compose ile yönetilmektedir.

### Build, Release, Run

Build ve runtime süreçleri ayrıdır. Proje önce restore/build edilir, ardından servisler ayrı process olarak çalıştırılır.

### Processes

Servisler stateless olacak şekilde tasarlanmıştır. State SQL Server, Redis ve RabbitMQ gibi external servislerde tutulur.

### Port Binding

Her servis kendi portunda çalışır. ApiGateway dış giriş noktasıdır.

### Concurrency

Servisler bağımsız process olarak çalıştığı için yatay ölçeklemeye uygundur.

### Disposability

ASP.NET Core lifecycle yapısı kullanılır. Servisler Ctrl+C veya process stop ile güvenli şekilde durdurulabilir.

### Dev/Prod Parity

Docker Compose ile destek servisleri local ortamda production benzeri şekilde çalıştırılır.

### Logs

Loglar Serilog ile console ve Seq üzerine structured formatta aktarılır.

### Admin Processes

Migration işlemleri dotnet ef komutları ile ayrı admin process olarak yürütülür.

## SOLID Prensipleri

### Single Responsibility Principle

Her servis ve katman tek bir sorumluluk alanına sahiptir.

### Open/Closed Principle

Application katmanındaki abstraction yapıları sayesinde yeni repository, cache veya event bus implementasyonları mevcut kodu bozmadan eklenebilir.

### Liskov Substitution Principle

Interface üzerinden çalışan servisler farklı implementasyonlarla değiştirilebilir.

### Interface Segregation Principle

Repository, cache, event bus ve unit of work sorumlulukları ayrı interface'lere bölünmüştür.

### Dependency Inversion Principle

Application katmanı Infrastructure katmanına doğrudan bağımlı değildir. Bağımlılıklar abstraction üzerinden yönetilir.

## Branch ve Versiyonlama

Task gereksinimine uygun branch yapısı:

```text
test/v1.0.0
```


## Proje Durumu

Tamamlanan başlıklar:

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

Proje çalıştırılmadan önce Docker servisleri başlatılmalı, migration komutları uygulanmalı ve ardından servisler ayrı terminal pencerelerinde çalıştırılmalıdır.

Ana giriş noktası ApiGateway servisidir:

```text
http://localhost:5000
```
