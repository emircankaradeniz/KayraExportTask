# Kayra Export Backend Developer Case Study

Bu proje, Backend Developer 3. A�ama task kapsam�nda .NET 8, C#, SQL Server, Onion Architecture, CQRS, JWT, Redis Cache, API Gateway, Rate Limiting, RabbitMQ, Serilog ve Seq kullan�larak geli�tirilmi� mikroservis tabanl� bir backend uygulamas�d�r.

## ��indekiler

- Proje �zeti
- Mimari Yap�
- Kullan�lan Teknolojiler
- Servisler
- Port Bilgileri
- Docker Servisleri
- Kurulum
- Migration Komutlar�
- Uygulamay� �al��t�rma
- API Gateway Kullan�m�
- AuthService API
- ProductService API
- LogService API
- Test Senaryosu
- Redis Cache ve Cache Invalidation
- Event Driven Mimari
- Structured Logging
- API Gateway Rate Limiting
- Authorization
- 12 Factor App Uyumlulu�u
- SOLID Prensipleri
- Branch ve Versiyonlama
- Proje Durumu

## Proje �zeti

Proje �� ana mikroservisten ve bir API Gateway katman�ndan olu�maktad�r.

- AuthService
- ProductService
- LogService
- ApiGateway

AuthService kullan�c� kay�t, giri�, JWT token ve refresh token y�netiminden sorumludur.

ProductService �r�n ekleme, g�ncelleme ve listeleme i�lemlerinden sorumludur. ProductService i�inde Onion Architecture ve CQRS pattern uygulanm��t�r.

LogService merkezi log kay�tlar�n� olu�turmak ve sorgulamak i�in geli�tirilmi�tir.

ApiGateway YARP Reverse Proxy ile t�m servislere tek giri� noktas� sa�lar ve rate limiting uygular.

## Mimari Yap�

Proje klas�r yap�s�:

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

ProductService, Onion Architecture yakla��m�yla a�a��daki katmanlara ayr�lm��t�r:

```text
ProductService.Domain
ProductService.Application
ProductService.Infrastructure
ProductService.API
```

Domain katman� i� kurallar�n� ve entity yap�lar�n� i�erir.

Application katman� CQRS command/query handler yap�lar�n�, DTO nesnelerini ve abstraction interface'lerini i�erir.

Infrastructure katman� SQL Server, Redis, RabbitMQ ve repository implementasyonlar�n� i�erir.

API katman� HTTP endpointlerini ve servis konfig�rasyonlar�n� i�erir.

AuthService ve LogService de benzer �ekilde Domain, Application, Infrastructure ve API katmanlar�na ayr�lm��t�r.

## Kullan�lan Teknolojiler

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

AuthService kullan�c� kimlik do�rulama i�lemlerinden sorumludur.

�zellikler:

- Register
- Login
- Refresh Token
- Microsoft Identity
- JWT Token �retimi
- Role Based Authorization
- Admin, Manager, User rolleri

### ProductService

ProductService �r�n i�lemlerinden sorumludur.

�zellikler:

- �r�n listeleme
- �r�n detay getirme
- �r�n ekleme
- �r�n g�ncelleme
- JWT do�rulama
- Policy Based Authorization
- Redis cache
- Cache invalidation
- RabbitMQ event publish
- Onion Architecture
- CQRS

### LogService

LogService merkezi log y�netiminden sorumludur.

�zellikler:

- Log olu�turma
- Son loglar� listeleme
- Log seviyesine g�re listeleme
- SQL Server �zerinde log saklama
- Information, Warning, Error, Critical seviyeleri

### ApiGateway

ApiGateway d�� d�nya ile mikroservisler aras�nda tek giri� noktas�d�r.

�zellikler:

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

Docker Compose ile a�a��daki destek servisleri aya�a kald�r�l�r:

- SQL Server
- Redis
- RabbitMQ
- Seq

Docker servislerini ba�latmak i�in:

```bash
docker compose up -d
```

�al��an containerlar� g�rmek i�in:

```bash
docker ps
```

Docker servislerini durdurmak i�in:

```bash
docker compose down
```

## Kurulum

Projeyi klonlad�ktan sonra ana dizinde a�a��daki komut �al��t�r�l�r:

```bash
dotnet restore
```

Ard�ndan build al�n�r:

```bash
dotnet build
```

Entity Framework CLI arac� y�kl� de�ilse a�a��daki komutla y�klenebilir:

```bash
dotnet tool install --global dotnet-ef --version 8.0.0
```

Y�kl�yse g�ncellemek i�in:

```bash
dotnet tool update --global dotnet-ef --version 8.0.0
```

## Migration Komutlar�

Migration komutlar�ndan �nce SQL Server container �al���yor olmal�d�r.

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

## Uygulamay� �al��t�rma

Her servis ayr� terminalde �al��t�r�l�r.

### AuthService

```powershell
cd C:\Users\rustl\OneDrive\Masa�st�\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/Services/AuthService/AuthService.API/AuthService.API.csproj
```

### ProductService

```powershell
cd C:\Users\rustl\OneDrive\Masa�st�\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/Services/ProductService/ProductService.API/ProductService.API.csproj
```

### LogService

```powershell
cd C:\Users\rustl\OneDrive\Masa�st�\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/Services/LogService/LogService.API/LogService.API.csproj
```

### ApiGateway

```powershell
cd C:\Users\rustl\OneDrive\Masa�st�\KayraExportTask
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
dotnet run --project src/ApiGateway/ApiGateway.csproj
```

## API Gateway Kullan�m�

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

PowerShell ile Gateway �zerinden login testi:

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

Response i�inde `accessToken` ve `refreshToken` d�ner.

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

Bu endpoint anonymous eri�ime a��kt�r.

### Product Detail

```http
GET /api/products/{id}
```

Bu endpoint anonymous eri�ime a��kt�r.

### Create Product

```http
POST /api/products
```

Bu endpoint JWT token ister. Kullan�c� `Admin` veya `Manager` rol�nde olmal�d�r.

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

Bu endpoint JWT token ister. Kullan�c� `Admin` veya `Manager` rol�nde olmal�d�r.

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

Product status de�erleri:

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

Log level de�erleri:

```text
1 Information
2 Warning
3 Error
4 Critical
```

## Test Senaryosu

1. Docker servislerini ba�lat.

```bash
docker compose up -d
```

2. Migration komutlar�n� �al��t�r.

3. AuthService, ProductService, LogService ve ApiGateway servislerini ayr� terminallerde ba�lat.

4. AuthService �zerinden Admin kullan�c� olu�tur.

```json
{
  "fullName": "Admin User",
  "email": "admin@kayraexport.com",
  "password": "Admin12345",
  "role": "Admin"
}
```

5. Login endpointi ile access token al.

6. ProductService Swagger �zerinde Authorize butonuna t�kla.

7. Token de�erini �u formatta gir.

```text
Bearer access-token-value
```

8. ProductService �zerinden �r�n olu�tur.

9. ProductService �zerinden �r�nleri listele.

10. Gateway �zerinden �r�nleri listele.

```text
http://localhost:5000/api/products
```

11. LogService �zerinden log olu�tur.

12. Gateway �zerinden loglar� listele.

```text
http://localhost:5000/api/logs?count=50
```

13. Seq panelinden structured loglar� kontrol et.

```text
http://localhost:5341
```

## Redis Cache ve Cache Invalidation

Product listeleme i�lemlerinde Redis cache kullan�l�r.

Ak��:

```text
GET /api/products
  �nce Redis kontrol edilir
  cache varsa Redis �zerinden response d�ner
  cache yoksa SQL Server'dan veri okunur
  sonu� Redis'e yaz�l�r
```

�r�n ekleme veya g�ncelleme i�lemlerinden sonra cache invalidation yap�l�r.

Ak��:

```text
POST /api/products
  �r�n SQL Server'a kaydedilir
  Redis product list cache temizlenir
  RabbitMQ event publish edilir
```

```text
PUT /api/products/{id}
  �r�n SQL Server'da g�ncellenir
  Redis product list cache temizlenir
  RabbitMQ event publish edilir
```

## Event Driven Mimari

ProductService i�inde �r�n ekleme ve �r�n g�ncelleme i�lemlerinden sonra RabbitMQ �zerinden integration event publish edilir.

Eventler:

```text
ProductCreatedIntegrationEvent
ProductUpdatedIntegrationEvent
```

RabbitMQ Management UI:

```text
http://localhost:15672
```

Default kullan�c� bilgileri:

```text
username: guest
password: guest
```

## Structured Logging

Projede Serilog ve Seq kullan�lm��t�r.

Her servis a�a��daki bilgilerle structured log �retir:

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

Servis bazl� filtre �rnekleri:

```text
ServiceName = 'AuthService'
ServiceName = 'ProductService'
ServiceName = 'LogService'
ServiceName = 'ApiGateway'
```

## API Gateway Rate Limiting

ApiGateway �zerinde global fixed window rate limiting uygulanm��t�r.

Varsay�lan ayar:

```text
100 request / 1 minute
Queue limit: 20
Rejected response: 429 Too Many Requests
```

Bu yap� API isteklerini merkezi olarak kontrol alt�na almak i�in eklenmi�tir.

## Authorization

Projede hem role based hem de policy based authorization uygulanm��t�r.

Roller:

```text
Admin
Manager
User
```

ProductService yazma i�lemleri i�in kullan�lan policy:

```text
ProductWritePolicy
```

Bu policy `Admin` veya `Manager` rol� ister.

Korunan endpointler:

```http
POST /api/products
PUT /api/products/{id}
```

## 12 Factor App Uyumlulu�u

### Codebase

T�m proje tek bir merkezi solution alt�nda y�netilmektedir.

### Dependencies

T�m ba��ml�l�klar NuGet paketleri �zerinden tan�mlanm��t�r.

### Config

Connection string, JWT, Redis, RabbitMQ ve Seq ayarlar� appsettings �zerinden y�netilmektedir. Production ortam�nda bu ayarlar environment variable olarak verilebilir.

### Backing Services

SQL Server, Redis, RabbitMQ ve Seq ba��ms�z destek servisleri olarak Docker Compose ile y�netilmektedir.

### Build, Release, Run

Build ve runtime s�re�leri ayr�d�r. Proje �nce restore/build edilir, ard�ndan servisler ayr� process olarak �al��t�r�l�r.

### Processes

Servisler stateless olacak �ekilde tasarlanm��t�r. State SQL Server, Redis ve RabbitMQ gibi external servislerde tutulur.

### Port Binding

Her servis kendi portunda �al���r. ApiGateway d�� giri� noktas�d�r.

### Concurrency

Servisler ba��ms�z process olarak �al��t��� i�in yatay �l�eklemeye uygundur.

### Disposability

ASP.NET Core lifecycle yap�s� kullan�l�r. Servisler Ctrl+C veya process stop ile g�venli �ekilde durdurulabilir.

### Dev/Prod Parity

Docker Compose ile destek servisleri local ortamda production benzeri �ekilde �al��t�r�l�r.

### Logs

Loglar Serilog ile console ve Seq �zerine structured formatta aktar�l�r.

### Admin Processes

Migration i�lemleri dotnet ef komutlar� ile ayr� admin process olarak y�r�t�l�r.

## SOLID Prensipleri

### Single Responsibility Principle

Her servis ve katman tek bir sorumluluk alan�na sahiptir.

### Open/Closed Principle

Application katman�ndaki abstraction yap�lar� sayesinde yeni repository, cache veya event bus implementasyonlar� mevcut kodu bozmadan eklenebilir.

### Liskov Substitution Principle

Interface �zerinden �al��an servisler farkl� implementasyonlarla de�i�tirilebilir.

### Interface Segregation Principle

Repository, cache, event bus ve unit of work sorumluluklar� ayr� interface'lere b�l�nm��t�r.

### Dependency Inversion Principle

Application katman� Infrastructure katman�na do�rudan ba��ml� de�ildir. Ba��ml�l�klar abstraction �zerinden y�netilir.

## Branch ve Versiyonlama

Task gereksinimine uygun branch yap�s�:

## Proje Durumu

Tamamlanan ba�l�klar:

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

## G�nderim Notu

Proje �al��t�r�lmadan �nce Docker servisleri ba�lat�lmal�, migration komutlar� uygulanmal� ve ard�ndan servisler ayr� terminal pencerelerinde �al��t�r�lmal�d�r.

Ana giri� noktas� ApiGateway servisidir:

```text
http://localhost:5000
```
