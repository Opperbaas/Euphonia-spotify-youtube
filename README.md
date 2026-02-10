# Resonance - Three-Tier Architecture Template

Een complete C# three-tier architecture template met Entity Framework Core.

## 📁 Projectstructuur

```
Resonance/
│
├── DataAccessLayer/                    # Data Access Layer (DAL)
│   ├── Context/
│   │   └── ApplicationDbContext.cs    # EF Core DbContext
│   ├── Interfaces/
│   │   ├── IRepository.cs             # Generic repository interface
│   │   ├── ISpecificRepository.cs     # Specifieke repository interface
│   │   └── IUnitOfWork.cs             # Unit of Work interface
│   ├── Models/
│   │   └── SpecificEntity.cs          # Entity model (database tabel)
│   ├── Repositories/
│   │   ├── Repository.cs              # Generic repository implementatie
│   │   └── SpecificRepository.cs      # Specifieke repository implementatie
│   └── UnitOfWork/
│       └── UnitOfWork.cs              # Unit of Work implementatie
│
├── BusinessLogicLayer/                 # Business Logic Layer (BLL)
│   ├── DTOs/
│   │   └── SpecificDto.cs             # Data Transfer Objects
│   ├── Interfaces/
│   │   └── ISpecificService.cs        # Service interface
│   └── Services/
│       └── SpecificService.cs         # Service implementatie (business logic)
│
└── PresentationLayer/                  # Presentation Layer (UI/API)
    ├── Controllers/
    │   └── SpecificController.cs      # API Controller
    ├── Program.cs                     # Application entry point + DI setup
    └── appsettings.json               # Configuration
```

## 🎯 Architectuur Overzicht

### Data Access Layer (DAL)
- **Verantwoordelijk voor**: Database communicatie
- **Bevat**: DbContext, Entities, Repositories, Unit of Work
- **Gebruikt**: Entity Framework Core

### Business Logic Layer (BLL)
- **Verantwoordelijk voor**: Business rules, validatie, data transformatie
- **Bevat**: Services, DTOs, Business logic
- **Communiceert met**: DAL via interfaces

### Presentation Layer
- **Verantwoordelijk voor**: HTTP requests/responses, user interaction
- **Bevat**: Controllers, Views (indien MVC), API endpoints
- **Communiceert met**: BLL via service interfaces

## 🔧 Hoe te gebruiken

### 1. Nieuwe Entity Toevoegen

#### Stap 1: Maak Entity Model (DAL)
```csharp
// DataAccessLayer/Models/YourEntity.cs
public class YourEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    // Voeg properties toe
}
```

#### Stap 2: Voeg toe aan DbContext
```csharp
// DataAccessLayer/Context/ApplicationDbContext.cs
public DbSet<YourEntity> YourEntities { get; set; }
```

#### Stap 3: Maak Repository Interface (DAL)
```csharp
// DataAccessLayer/Interfaces/IYourRepository.cs
public interface IYourRepository : IRepository<YourEntity>
{
    // Voeg custom methods toe
}
```

#### Stap 4: Implementeer Repository (DAL)
```csharp
// DataAccessLayer/Repositories/YourRepository.cs
public class YourRepository : Repository<YourEntity>, IYourRepository
{
    // Implementeer custom methods
}
```

#### Stap 5: Voeg toe aan Unit of Work
```csharp
// In IUnitOfWork interface
IYourRepository YourRepository { get; }

// In UnitOfWork class
public IYourRepository YourRepository
{
    get
    {
        if (_yourRepository == null)
        {
            _yourRepository = new YourRepository(_context);
        }
        return _yourRepository;
    }
}
```

### 2. Service Toevoegen (BLL)

#### Stap 1: Maak DTOs
```csharp
// BusinessLogicLayer/DTOs/YourDto.cs
public class YourDto { /* properties */ }
public class CreateYourDto { /* properties */ }
public class UpdateYourDto { /* properties */ }
```

#### Stap 2: Maak Service Interface
```csharp
// BusinessLogicLayer/Interfaces/IYourService.cs
public interface IYourService
{
    Task<YourDto> GetByIdAsync(int id);
    // Voeg methods toe
}
```

#### Stap 3: Implementeer Service
```csharp
// BusinessLogicLayer/Services/YourService.cs
public class YourService : IYourService
{
    private readonly IUnitOfWork _unitOfWork;
    // Implementeer methods met business logic
}
```

### 3. Controller Toevoegen (Presentation)

```csharp
// PresentationLayer/Controllers/YourController.cs
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    private readonly IYourService _service;
    // Implementeer API endpoints
}
```

### 4. Dependency Injection Configureren

```csharp
// In Program.cs
services.AddScoped<IYourService, YourService>();
```

## 🗃️ Database Setup

### 1. Connection String Aanpassen
```json
// appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DB;..."
}
```

### 2. Migrations Maken
```bash
# Installeer EF Core tools
dotnet tool install --global dotnet-ef

# Navigeer naar DataAccessLayer folder
cd DataAccessLayer

# Maak eerste migration
dotnet ef migrations add InitialCreate --startup-project ../PresentationLayer

# Update database
dotnet ef database update --startup-project ../PresentationLayer
```

## 📦 Benodigde NuGet Packages

### DataAccessLayer
```bash
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
Microsoft.EntityFrameworkCore.Tools
```

### PresentationLayer
```bash
Microsoft.AspNetCore.OpenApi
Swashbuckle.AspNetCore
```

## 🚀 Applicatie Starten

```bash
# Navigeer naar PresentationLayer
cd PresentationLayer

# Run applicatie
dotnet run
```

Swagger UI is beschikbaar op: `https://localhost:5001` (of `http://localhost:5000`)

## ✅ Design Patterns Gebruikt

- **Repository Pattern**: Abstraheert data access logic
- **Unit of Work Pattern**: Beheert transacties en meerdere repositories
- **Dependency Injection**: Loose coupling tussen lagen
- **DTO Pattern**: Scheidt domain models van data transfer
- **Service Layer Pattern**: Centraliseert business logic

## 📝 Best Practices

1. **Gebruik altijd interfaces** voor communicatie tussen lagen
2. **DTOs** voor data transfer tussen lagen
3. **Validatie** in de business logic layer
4. **Exception handling** in alle lagen
5. **Async/await** voor database operaties
6. **Unit of Work** voor transactiebeheer
7. **Dependency Injection** voor loose coupling

## 🔍 Testen

### Unit Tests Structuur
```
Tests/
├── DataAccessLayer.Tests/
├── BusinessLogicLayer.Tests/
└── PresentationLayer.Tests/
```

Gebruik Moq voor het mocken van interfaces in unit tests.

## 📚 Verdere Uitbreidingen

- [ ] AutoMapper voor DTO mapping
- [ ] FluentValidation voor complexe validaties
- [ ] Logging (Serilog/NLog)
- [ ] Authentication & Authorization (JWT)
- [ ] Caching (Redis/In-Memory)
- [ ] API Versioning
- [ ] Health Checks
- [ ] Docker support

## 💡 Tips

- Vervang "Specific" overal door je eigen entity naam
- Pas namespace "Resonance" aan naar je project naam
- Voeg validatie attributen toe aan DTOs ([Required], [MaxLength], etc.)
- Gebruik betekenisvolle namen voor je entities en services
- Documenteer je code met XML comments voor IntelliSense

---

**Happy Coding! 🎵**

