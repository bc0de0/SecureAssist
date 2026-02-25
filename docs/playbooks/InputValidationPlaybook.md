# SecureAssist: Input Validation & Trust-Boundary Policy (IV-POLICY)
**Policy ID:** SA-SEC-IV-001  
**Version:** 3.0.0 (Enterprise Standard)  
**Classification:** Internal Deployment / Audit Mandatory  
**Status:** Canonical Requirement

---

## 1. Governance Framework

### 1.1 Purpose
The purpose of this policy is to establish non-bypassable architectural controls for all data entering the **SecureAssist** ecosystem. This policy ensures data integrity, availability, and multi-tenant isolation by treating every entry point as a trust boundary.

### 1.2 Scope
This policy applies to:
- All Public and Internal API Endpoints.
- Asynchronous Message Bus consumers.
- File and Object storage ingestors.
- LLM/AI Proxy interfaces.

### 1.3 Normative Language (RFC 2119)
- **MUST / SHALL:** Requirement is mandatory and failure is a compliance violation.
- **SHOULD:** Recommended practice but may be ignored with documented justification.
- **MAY / OPTIONAL:** Advisory guidance.

### 1.4 Versioning Policy
Modifications to the YAML specification (`IV-SPEC`) or this policy MUST trigger a minor version bump. Breaking changes to validation logic require a major version bump.

---

## 2. Traceability Mapping
| Framework | Mapping IDs |
| :--- | :--- |
| **CWE** | CWE-20 (Improper Input Validation), CWE-89 (SQLi), CWE-639 (IDOR), CWE-79 (XSS) |
| **OWASP TOP 10** | A01:2021-Broken Access Control, A03:2021-Injection |
| **NIST SP 800-53** | SI-10 (Information Input Validation) |

---

## 3. Trust-Boundary Enforcement Tiers

| Tier | Enforcement Point | Primary Mechanism | Responsibility |
| :--- | :--- | :--- | :--- |
| **Tier 0** | Network Edge | WAF / Cloud Armor | Infrastructure Team |
| **Tier 1** | API Controller | Structural Schema / Data Annotations | API Developers |
| **Tier 2** | Application Layer | Semantic Domain Rules / FluentValidation | Domain Architects |
| **Tier 3** | Integration (AI) | LLM Proxy / Guardrails | AI Engineering |

---

## 4. Fundamental Doctrines

### 4.1 Parameterization over Filtering (IV-001)
- **Doctrine:** Security MUST NOT rely on keyword filtering (e.g., blocking `SELECT`). 
- **Mandate:** All data access SHALL use parameterized queries or ORM binding. 
- **Heuristics:** Keyword detection MAY be used for secondary logging/WAF alerting but SHALL NOT replace L1/L2 validation.

### 4.2 Strict Payload Shaping (IV-002)
- **Doctrine:** The system SHALL only accept what is explicitly modeled.
- **Mandate:** 
  - Reject unknown JSON properties (`fail-on-unknown`).
  - Strict enum enforcement; invalid enum values must result in `400 BadRequest`.
  - Disable dynamic model binding (`dynamic`, `ExpandoObject`) unless explicitly approved.

### 4.3 Anti-Automation & Availability Guards (IV-003)
- **Doctrine:** Input validation SHALL prevent resource exhaustion.
- **Mandate:**
  - Enforce strictly defined pagination ceilings (e.g., max 100 records).
  - Rate limit all tenant-scoped endpoints.
  - LLM inputs MUST be truncated to defined token limits before proxy transmission.

### 4.4 Second-Order Injection Defense (IV-004)
- **Doctrine:** Data trust decays over time.
- **Mandate:** Stored data retrieved from persistent storage MUST be re-validated/sanitized before being used in a secondary sink (e.g., embedding DB content into a System Prompt or Email Template).

---

## 5. Technical Specification

### 5.1 File & Object Security
- **Identity:** Filenames MUST be regenerated as GUIDs upon ingest.
- **Integrity:** Content MUST be validated by Magic Number (File Signature), not file extension.
- **Scanning:** All persistent files MUST be scanned for malware at the point of storage ingest.

### 5.2 Multi-Tenant Identity Boundary (STCI)
- **Rule:** Global context `tenant_id` MUST be sourced from cryptographic JWT claims.
- **Enforcement:** Every L2 handler MUST assert that the target resource's `TenantId` matches the User's `ClaimContext`.

---

## 6. Anti-Patterns
1. **Implicit Trust:** Assuming "Internal" services don't need validation.
2. **Client-First Logic:** Relying on UI validation for security.
3. **Double-Sanitization:** Corrupting data by applying escaping filters multiple times.
4. **Keyword Blacklisting:** Believing that blocking `<script>` makes an endpoint safe.

---

## 7. Review Checklist for Security Auditors
- [ ] Are all API inputs constrained by `IV-SPEC.yaml` limits?
- [ ] Is `JsonUnknownTypeHandling.Fail` enabled globally?
- [ ] Does the codebase use Parameterized Queries exclusively?
- [ ] Is second-order data re-validated before being sent to the AI?
- [ ] Do logs mask PII and tokens defined in `LOG-MASK.config`?
