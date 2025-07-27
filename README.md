# BaseApi - .NET 8 Clean Architecture API

Modern .NET 8 teknolojileri kullanılarak geliştirilmiş, Clean Architecture prensiplerine uygun, güvenli ve ölçeklenebilir bir REST API projesi.

## 🚀 Özellikler

### 🔐 Kimlik Doğrulama ve Yetkilendirme
- **JWT Token Authentication**: Güvenli token tabanlı kimlik doğrulama
- **Identity Framework**: ASP.NET Core Identity entegrasyonu
- **Role-based Authorization**: Rol tabanlı yetkilendirme sistemi
- **Bearer Token Support**: "Bearer" prefix'li JWT token desteği

### 📊 Veritabanı ve ORM
- **Entity Framework Core 8**: Modern ORM ile veritabanı yönetimi
- **SQL Server**: Güçlü veritabanı desteği
- **Code-First Migrations**: Veritabanı şema yönetimi
- **Change Tracking**: Otomatik değişiklik takibi

### 🔍 Audit ve Logging Sistemi
- **Automatic Audit Logging**: Tüm veritabanı işlemleri otomatik olarak kaydedilir
- **Dual Logging System**: 
  - `AuditLogs`: Kim neyi değiştirdi? (INSERT/UPDATE/DELETE)
  - `Logs`: Uygulama çalışırken ne oldu? (INFO/WARN/ERROR/DEBUG)
- **User Context Tracking**: Kullanıcı bilgileri otomatik kayıt
- **Request/Response Logging**: HTTP istekleri ve yanıtları loglanır
- **Structured Logging**: JSON formatında yapılandırılmış loglar
- **Log Rotation**: Otomatik log dosyası döndürme ve boyut kontrolü
- **Environment-based Logging**: Development/Production ortamlarına göre log seviyesi

### 🏗️ Mimari ve Tasarım
- **Clean Architecture**: Katmanlı mimari (Domain, Application, Infrastructure, API)
- **SOLID Principles**: Temiz kod prensipleri
- **Repository Pattern**: Veri erişim katmanı
- **Unit of Work Pattern**: İşlem yönetimi
- **Dependency Injection**: IoC container kullanımı

### 🔄 AutoMapper Entegrasyonu
- **DTO Mapping**: Entity ve DTO arası otomatik mapping
- **Profile-based Configuration**: Mapping profilleri
- **Type Safety**: Güvenli tip dönüşümleri

### 📚 OpenAPI/Swagger Geliştirmesi
- **JWT Authentication**: Swagger UI'da JWT token ile test imkanı
- **XML Documentation**: Tüm endpoint'lere açıklamalar ve response kodları
- **Custom Operation Filter**: Gelişmiş Swagger konfigürasyonu
- **Response Type Annotations**: HTTP status kodları ve response tipleri

### 🗑️ Soft Delete Desteği
- **IsDeleted Property**: Tüm entity'lerde soft delete desteği
- **Global Query Filter**: Otomatik olarak silinmemiş kayıtları getirme
- **Restore Functionality**: Silinmiş kayıtları geri yükleme
- **Audit Trail**: Silme işlemlerinin takibi

### 🛡️ Güvenlik ve Middleware
- **Exception Handling Middleware**: Merkezi hata yönetimi
- **Request Logging Middleware**: İstek takibi
- **Validation Filter**: Otomatik model doğrulama
- **SQL Injection Filter**: SQL injection saldırılarına karşı koruma
- **Security Headers Middleware**: Güvenlik başlıkları (CSP, XSS, CSRF koruması)
- **Rate Limiting**: IP ve client tabanlı istek sınırlama
- **CORS Support**: Cross-origin resource sharing

### 📈 Monitoring ve Health Checks
- **Health Check Endpoint**: `/health` endpoint'i
- **Database Connectivity Check**: Veritabanı bağlantı kontrolü

### 🗄️ Cache Sistemi
- **Redis Cache**: Yüksek performanslı cache desteği

### 📧 Email Servisi
- **Email Service**: E-posta gönderim desteği (simüle edilmiş)

## 🏛️ Proje Yapısı

```
BaseApi/
├── BaseApi.Domain/           # Domain katmanı (Entities, Value Objects)
├── BaseApi.Application/      # Application katmanı (Services, DTOs, Mappings)
├── BaseApi.Infrastructure/   # Infrastructure katmanı (External services)
├── BaseApi.Persistence/      # Persistence katmanı (DbContext, Repositories)
├── BaseApi.API/             # API katmanı (Controllers, Middleware)
└── BaseApi.Abstractions/    # Shared abstractions
```

## 🚀 Hızlı Başlangıç

### Gereksinimler
- .NET 8 SDK
- SQL Server
- Redis (opsiyonel)

### Kurulum

1. **Repository'yi klonlayın**
```bash
git clone <repository-url>
cd BaseApi
```

2. **Veritabanı bağlantısını yapılandırın**
```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=BaseApiDb;Trusted_Connection=true;TrustServerCertificate=true;",
    "Redis": "localhost:6379"
  }
}
```

3. **Migration'ları uygulayın**
```bash
dotnet ef database update --project BaseApi.Persistence --startup-project BaseApi.API
```

4. **Uygulamayı çalıştırın**
```bash
dotnet run --project BaseApi.API
```

## 📋 API Endpoints

### Authentication
- `POST /api/Auth/login` - Kullanıcı girişi
- `POST /api/Auth/register` - Kullanıcı kaydı

### Products
- `GET /api/Products` - Tüm aktif ürünleri listele
- `GET /api/Products/{id}` - Ürün detayı
- `POST /api/Products` - Yeni ürün oluştur
- `PUT /api/Products/{id}` - Ürün güncelle
- `DELETE /api/Products/{id}` - Ürünü yumuşak sil (soft delete)
- `PUT /api/Products/{id}/restore` - Silinmiş ürünü geri yükle
- `PUT /api/Products/{id}/stock` - Ürün stok güncelle
- `GET /api/Products/low-stock` - Düşük stoklu ürünleri listele

### Users
- `GET /api/Users` - Tüm kullanıcıları listele
- `GET /api/Users/{id}` - Kullanıcı detayı

## 🔍 Logging Sistemi

### Audit Logs
Veritabanı işlemlerinin otomatik kaydı:
- **INSERT**: Yeni kayıt oluşturma
- **UPDATE**: Kayıt güncelleme
- **DELETE**: Kayıt silme

### Application Logs
Sistem olaylarının kaydı:
- **INFO**: Bilgi mesajları
- **WARNING**: Uyarı mesajları
- **ERROR**: Hata mesajları
- **DEBUG**: Debug bilgileri

### 📄 File Logs
Dosya tabanlı loglar:
- **Text Logs**: `logs/log-.txt` (günlük döndürme)
- **Log Rotation**: Otomatik dosya döndürme ve boyut kontrolü

## 🛠️ Teknolojiler

- **.NET 8**: En son .NET framework
- **Entity Framework Core 8**: ORM
- **ASP.NET Core Identity**: Kimlik doğrulama
- **AutoMapper**: Object mapping
- **Serilog**: Logging
- **Redis**: Caching
- **SQL Server**: Veritabanı
- **JWT**: Token authentication
- **AspNetCoreRateLimit**: Rate limiting

## 🔧 Konfigürasyon

#### JWT Settings
```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "Issuer": "BaseApi",
    "Audience": "BaseApi",
    "ExpiryMinutes": 60
  }
}
```

#### Logging Configuration
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Warning",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information"
      }
    },
    "WriteTo": [
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 3,
          "fileSizeLimitBytes": 5242880
        }
      }
    ]
  }
}
```

#### Rate Limiting Configuration
```json
{
  "IpRateLimit": {
    "EnableEndpointRateLimiting": true,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 100
      },
      {
        "Endpoint": "POST:/api/Auth/login",
        "Period": "1m",
        "Limit": 5
      }
    ]
  }
}
```

## 📊 Veritabanı Şeması

### Ana Tablolar
- **Products**: Ürün bilgileri (soft delete desteği)
- **AspNetUsers**: Kullanıcı bilgileri
- **AspNetRoles**: Rol bilgileri
- **AuditLogs**: Denetim kayıtları
- **Logs**: Uygulama logları

## 🚀 Deployment

### Docker (Gelecek)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
COPY . .
ENTRYPOINT ["dotnet", "BaseApi.API.dll"]
```

### Azure Deployment
- Azure App Service
- Azure SQL Database

## 🤝 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📝 Lisans

Bu proje, kişisel ve akademik kullanım için ücretsizdir. Ticari kullanım için özel lisans gereklidir. Detaylar için [LICENSE](LICENSE) dosyasına göz atın.

## 📞 İletişim

Şimdilik Yok.