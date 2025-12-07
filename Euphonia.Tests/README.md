# Euphonia Test Project

Dit project bevat unit tests en integration tests voor de Euphonia applicatie.

## Folder Structuur

```
Euphonia.Tests/
├── UnitTests/                  # Unit tests voor individuele componenten
│   ├── Services/              # Tests voor business logic services
│   ├── Repositories/          # Tests voor data access repositories
│   └── Controllers/           # Tests voor API controllers
├── IntegrationTests/          # Integration tests voor complete flows
└── TestHelpers/               # Herbruikbare test utilities en mock data
```

## Test Framework

- **xUnit** - Modern test framework (gebruikt door Microsoft)
- **Moq** - Voor het maken van mock objects
- **EF Core InMemory** - In-memory database voor snelle repository tests
- **ASP.NET Mvc.Testing** - Voor het testen van controllers en HTTP endpoints

## Tests Uitvoeren

Alle tests uitvoeren:
```powershell
dotnet test
```

Tests voor specifieke class uitvoeren:
```powershell
dotnet test --filter FullyQualifiedName~ProfielServiceTests
```

Tests met coverage uitvoeren:
```powershell
dotnet test --collect:"XPlat Code Coverage"
```

## Test Schrijven

### AAA Pattern
Alle tests volgen het **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public async Task MethodName_ExpectedBehavior_WhenCondition()
{
    // Arrange - Test data voorbereiden
    var mockRepo = new Mock<IRepository>();
    mockRepo.Setup(r => r.GetData()).ReturnsAsync(testData);
    
    // Act - Methode uitvoeren
    var result = await service.GetData();
    
    // Assert - Resultaat controleren
    Assert.NotNull(result);
    Assert.Equal(expected, result);
}
```

### Test Naming Convention
`MethodName_ExpectedBehavior_WhenCondition`

Voorbeelden:
- `GetByIdAsync_ReturnsProfielDto_WhenProfielExists`
- `CreateAsync_ThrowsException_WhenDataIsInvalid`
- `DeleteAsync_ReturnsFalse_WhenProfielDoesNotExist`

## Mocking

Mock objects isoleren de unit onder test:

```csharp
var mockRepo = new Mock<IProfielRepository>();
mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(testProfiel);

// Controleer of methode is aangeroepen
mockRepo.Verify(r => r.GetByIdAsync(1), Times.Once);
```

## Best Practices

1. **Test één ding per test** - Elke test moet één specifiek gedrag testen
2. **Tests moeten onafhankelijk zijn** - Tests mogen elkaar niet beïnvloeden
3. **Gebruik duidelijke namen** - Test naam moet beschrijven wat er getest wordt
4. **Test ook edge cases** - Niet alleen de "happy path" testen
5. **Gebruik mocks voor dependencies** - Isoleer de unit onder test
6. **Arrange-Act-Assert** - Volg altijd dit pattern voor leesbaarheid

## Voorbeeld Tests

Zie `UnitTests/Services/ProfielServiceTests.cs` voor voorbeelden van:
- Testen met mock repositories
- Async methode tests
- Testing van verschillende scenarios (success, not found, invalid data)
- Verify dat dependencies correct worden aangeroepen
