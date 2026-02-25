# SecureAssist Security Architecture: Input Validation Integration Guide

## 1. Overview
SecureAssist uses a **Policy-as-Code** approach for input validation. This ensures that security standards are not just documented in static playbooks but are deterministic, testable, and capable of being integrated into the CI/CD pipeline and runtime middleware.

This guide explains how the `InputValidationPlaybook.yaml` (IV-SPEC) is utilized throughout the system.

---

## 2. The 360° Comprehensiveness Model
The validation system is designed to provide 100% coverage across the following vectors:

- **Structural Integrity:** Enforced via Tier 1 (Peripheral) checks to prevent mass-assignment, over-posting, and payload-size attacks.
- **Data Integrity:** Enforced via Tier 2 (Application) checks using parameterized doctrines and secondary re-validation.
- **Identity Integrity:** Enforced via **STCI (Strict Tenant Context Isolation)**, ensuring no user can ever access data belonging to another tenant via ID tampering.
- **AI Safety:** Enforced via Tier 3 (AI Proxy) layers that protect both the Prompt (Inbound) and the Response (Outbound).
- **Availability:** Enforced via rate limits and pagination ceilings to protect the API from resource exhaustion.

---

## 3. How to Use the YAML (IV-SPEC) in the Project

The `InputValidationPlaybook.yaml` is the **Source of Truth**. It should be integrated as follows:

### 3.1 C#/.NET Core Middleware Integration
Architects should use the YAML values to configure the application during startup:

```csharp
// Example: Converting IV-101 into Runtime Config
builder.Services.AddControllers()
    .AddJsonOptions(options => {
        // Mapped from IV-101
        options.JsonSerializerOptions.UnknownTypeHandling = JsonUnknownTypeHandling.Fail;
        options.JsonSerializerOptions.MaxDepth = 32; 
    });
```

### 3.2 Automated Validation Testing
Integration tests should parse the YAML and dynamically generate test cases:
- For every `regex_catalog` entry, the test suite must verify both positive and negative matches.
- For every `max_request_size_kb`, the test suite must attempt a payload that exceeds the limit and expect a `413 Payload Too Large`.

### 3.3 CI/CD Policy Enforcement
A pre-deployment script can verify if new code follows the `IV-SPEC`:
- Search for dynamic SQL or string concatenation (violates `IV-201`).
- Verify that every DTO has a matching `FluentValidator`.

### 3.4 AI Proxy Middleware
The AI service module should ingest `IV-601` (Inbound Tokens) and `IV-602` (PII Entities) to dynamically update its regex scanners/guardrail layers without needing a code change.

---

## 4. Operationalization
Any deviation from the YAML spec found in production logs should trigger a **Critical Security Incident (P1)**, as the system is designed to "Fail-Fast" and block these attempts purely at the architectural level.

---
**Document Info:**
- **Primary Source:** [InputValidationPlaybook.yaml](./playbooks/InputValidationPlaybook.yaml)
- **Policy Standard:** [InputValidationPlaybook.md](./playbooks/InputValidationPlaybook.md)
