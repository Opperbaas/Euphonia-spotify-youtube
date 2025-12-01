# 🎯 Quick Start Instructies

## Stap-voor-stap: Van Template naar Werkende Applicatie

### 📋 Checklist

- [ ] 1. Namen aanpassen
- [ ] 2. Database configureren
- [ ] 3. NuGet packages installeren
- [ ] 4. Migrations maken
- [ ] 5. Applicatie testen

---

## 1️⃣ Namen Aanpassen

Zoek en vervang de volgende placeholder namen door je eigen namen:

### Te vervangen:
- `Euphonia` → Jouw project naam
- `SpecificEntity` → Jouw entity naam (bijv. `Product`, `User`, `Order`)
- `SpecificRepository` → Jouw repository naam (bijv. `ProductRepository`)
- `SpecificService` → Jouw service naam (bijv. `ProductService`)
- `SpecificController` → Jouw controller naam (bijv. `ProductController`)
- `SpecificDto` → Jouw DTO naam (bijv. `ProductDto`)

### Bestanden om te hernoemen:
```
DataAccessLayer/Models/SpecificEntity.cs
DataAccessLayer/Interfaces/ISpecificRepository.cs
DataAccessLayer/Repositories/SpecificRepository.cs
BusinessLogicLayer/DTOs/SpecificDto.cs
BusinessLogicLayer/Interfaces/ISpecificService.cs
BusinessLogicLayer/Services/SpecificService.cs
PresentationLayer/Controllers/SpecificController.cs
```

---

## 2️⃣ Database Configureren

### Optie A: SQL Server (LocalDB)
```json
// appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=JouwDbNaam;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

### Optie B: SQL Server (Regular)
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=JouwDbNaam;User Id=sa;Password=JouwWachtwoord;TrustServerCertificate=True"
}
```

### Optie C: Azure SQL Database
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:jouwserver.database.windows.net,1433;Database=JouwDbNaam;User ID=username;Password=wachtwoord;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
}
```

---

## 3️⃣ NuGet Packages Installeren

### Voor DataAccessLayer:
```powershell
cd DataAccessLayer
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
```

### Voor PresentationLayer:
```powershell
cd PresentationLayer
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
```

### EF Core Tools installeren (globaal):
```powershell
dotnet tool install --global dotnet-ef
# Of updaten als al geïnstalleerd:
dotnet tool update --global dotnet-ef
```

---

## 4️⃣ Entity Aanpassen

### Pas SpecificEntity.cs aan naar jouw data:

```csharp
// Voorbeeld: Product entity
public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
```

### Update DbContext:
```csharp
// ApplicationDbContext.cs
public DbSet<Product> Products { get; set; }  // Pas naam aan

private void ConfigureProduct(ModelBuilder modelBuilder)  // Herneem method
{
    modelBuilder.Entity<Product>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Name);
        entity.Property(e => e.IsActive).HasDefaultValue(true);
        entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
    });
}
```

---

## 5️⃣ Migrations Maken

```powershell
# Navigeer naar de root van je project
cd c:\Users\Jamal\Documents\GitHub\Euphonia

# Maak eerste migration
dotnet ef migrations add InitialCreate --project DataAccessLayer --startup-project PresentationLayer

# Bekijk de gegenereerde migration
# Check: DataAccessLayer/Migrations/xxxxx_InitialCreate.cs

# Apply migration naar database
dotnet ef database update --project DataAccessLayer --startup-project PresentationLayer
```

### Als je een fout krijgt:
```powershell
# Verwijder migration
dotnet ef migrations remove --project DataAccessLayer --startup-project PresentationLayer

# Fix de fout in je code
# Probeer opnieuw
```

---

## 6️⃣ Applicatie Runnen

```powershell
# Navigeer naar PresentationLayer
cd PresentationLayer

# Run de applicatie
dotnet run

# Of met watch (auto-reload bij changes)
dotnet watch run
```

### Swagger UI openen:
- Open browser naar: `https://localhost:5001`
- Of: `http://localhost:5000`

---

## 7️⃣ API Testen

### Test endpoints in Swagger:

1. **GET** `/api/specific` - Haal alle items op
2. **GET** `/api/specific/{id}` - Haal specifiek item op
3. **POST** `/api/specific` - Maak nieuw item
   ```json
   {
     "name": "Test Item",
     "description": "Test beschrijving",
     "isActive": true
   }
   ```
4. **PUT** `/api/specific/{id}` - Update item
5. **DELETE** `/api/specific/{id}` - Verwijder item

---

## 8️⃣ Nieuwe Entity Toevoegen

### Quick Guide:

1. **Maak Entity Model**
   ```
   DataAccessLayer/Models/YourEntity.cs
   ```

2. **Voeg toe aan DbContext**
   ```csharp
   public DbSet<YourEntity> YourEntities { get; set; }
   ```

3. **Maak Repository Interface**
   ```
   DataAccessLayer/Interfaces/IYourRepository.cs
   ```

4. **Implementeer Repository**
   ```
   DataAccessLayer/Repositories/YourRepository.cs
   ```

5. **Update UnitOfWork**
   - Voeg property toe aan IUnitOfWork
   - Implementeer in UnitOfWork class

6. **Maak DTOs**
   ```
   BusinessLogicLayer/DTOs/YourDto.cs
   ```

7. **Maak Service Interface**
   ```
   BusinessLogicLayer/Interfaces/IYourService.cs
   ```

8. **Implementeer Service**
   ```
   BusinessLogicLayer/Services/YourService.cs
   ```

9. **Maak Controller**
   ```
   PresentationLayer/Controllers/YourController.cs
   ```

10. **Registreer in DI Container**
    ```csharp
    // Program.cs
    services.AddScoped<IYourService, YourService>();
    ```

11. **Maak nieuwe Migration**
    ```powershell
    dotnet ef migrations add Add_YourEntity --project DataAccessLayer --startup-project PresentationLayer
    dotnet ef database update --project DataAccessLayer --startup-project PresentationLayer
    ```

---

## 🐛 Troubleshooting

### "Could not find a part of the path"
- Zorg dat je in de juiste folder bent
- Check of alle bestanden correct zijn aangemaakt

### "Unable to create an object of type 'ApplicationDbContext'"
- Check connection string in appsettings.json
- Zorg dat SQL Server draait
- Test connection string

### "No migrations found"
- Run: `dotnet ef migrations add InitialCreate --project DataAccessLayer --startup-project PresentationLayer`

### "The Entity Framework tools version X is older than that of the runtime"
- Update EF tools: `dotnet tool update --global dotnet-ef`

### Build errors over missing namespaces
- Installeer alle NuGet packages
- Run: `dotnet restore`

---

## 📚 Handige Commands

```powershell
# Restore packages
dotnet restore

# Build project
dotnet build

# Clean build artifacts
dotnet clean

# List migrations
dotnet ef migrations list --project DataAccessLayer --startup-project PresentationLayer

# Remove last migration
dotnet ef migrations remove --project DataAccessLayer --startup-project PresentationLayer

# Update database to specific migration
dotnet ef database update MigrationName --project DataAccessLayer --startup-project PresentationLayer

# Drop database
dotnet ef database drop --project DataAccessLayer --startup-project PresentationLayer
```

---

## ✅ Checklist voor Productie

- [ ] Environment-specific appsettings (Development, Staging, Production)
- [ ] Error handling middleware
- [ ] Logging configureren
- [ ] API versioning
- [ ] Rate limiting
- [ ] CORS policy aanpassen
- [ ] Authentication & Authorization
- [ ] Connection string uit configuratie halen (Azure Key Vault)
- [ ] Health checks toevoegen
- [ ] API documentation verbeteren

---

**Succes met je project! 🚀**
