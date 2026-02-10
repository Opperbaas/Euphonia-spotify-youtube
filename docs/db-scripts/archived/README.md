Deze map bevat legacy SQL-scripts die zijn gearchiveerd uit de hoofdrepository.

Reden:
- Scripts waren achtergrond fixes/migraties of eenmalige correcties en mogen niet meer in de actieve codebase zichtbaar zijn.
- Voor audit/traceability en mogelijke rollbacks bewaren we de originele scripts, maar verplaatsen ze uit het hoofdpad.

Belangrijke instructies:
- Voer nooit deze scripts zomaar in productie uit zonder review.
- Als een script belangrijke schema-wijzigingen bevat die nog niet in EF-migraties staan, overweeg dan om die wijzigingen in een EF-migration vast te leggen.

Wij verplaatten op 2025-12-21 (branch: gebruikersvriendelijkheid, commit: d7b5403) de volgende bestanden naar deze map:
- FixUserIDIdentity_Simple.sql
- FixUserIDIdentity.sql
- FixHistoriekIDIdentity.sql
- FixMuziekAnalyseIDIdentity.sql
- FixMuziekIDIdentity.sql
- FixNotificatieIDIdentity.sql
- FixProfielIDIdentity.sql
- FixStatistiekIDIdentity.sql
- FixStemmingIDIdentity.sql
- FixStemmingMuziekPKIdentity.sql
- FixStemmingTypeIDIdentity.sql
- SeedStemmingTypes.sql
- SeedTestNotificaties.sql
- UpdateNotificatieTable.sql
- CreateNotificatieTable.sql
- migration_script.sql

Als je wilt dat ik dit archiveer of herstel in een andere vorm (bijv. in een `archive`-branch of extern), laat het weten.

