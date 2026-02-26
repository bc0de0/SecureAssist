# Architecture Decision Record (ADR) - AI Response Storage
**Date:** 2026-02-26
**Status:** Decided

## Context
We need a storage solution to capture AI prompts and responses for later validation, analysis, and reinforcement learning to improve AI augmentation. This data is semi-structured (JSON) and may change schema over time.

## Decision: MongoDB (Document Storage)
We have selected **MongoDB** as the storage engine for AI interactions.

### Why not GraphQL?
- **GraphQL is not a database.** It is a query language for APIs. It defines how data is requested and transmitted between client and server, but it does not specify how data is stored on disk.
- You can use GraphQL *with* MongoDB, but it is not an alternative to it.

### Why MongoDB?
1. **Schema Flexibility:** AI responses often contain dynamic metadata, token usage details, and nested JSON structures. MongoDB's BSON format allows us to store these without predefined schemas.
2. **Vertical Scalability & Performance:** High-throughput writes for logging millions of interactions.
3. **Integration with .NET:** Mature C# driver and LINQ support.
4. **Natural Mapping:** The `Dictionary<string, object>` we currently use maps directly to MongoDB documents.

---

## Implementation Plan
1. **Abstraction:** We have already introduced the `IAiResponseStorage` interface in the Application layer.
2. **Infrastructure:** We will implement `MongoAiResponseStorage` in the Infrastructure layer using the `MongoDB.Driver` NuGet package.
3. **Migration:** As we move from In-Memory/SQLite mocks to production, we will switch the DI registration in `Program.cs`.

## Comparison
| Feature | MongoDB | GraphQL |
| :--- | :--- | :--- |
| **Type** | NoSQL Document Database | API Query Language |
| **Data Storage** | Native BSON/JSON storage | None (Depends on underlying DB) |
| **Use Case** | High-volume, flexible data | Efficient data fetching for clients |
| **Our Goal** | Store raw AI JSON for analysis | N/A (We already use REST) |
