# SecureAssist: Enterprise Input Validation & Trust-Boundary Playbook
**Policy ID:** SA-SEC-001-EX  
**Version:** 2.0.0 (Comprehensive)  
**Status:** Mandatory  
**Scope:** .NET 8 Multi-tenant AI Workflow API (SecureAssist)

---

## 1. Governance & Trust Boundaries
All data entering SecureAssist is considered **UNTRUSTED** until it passes through a deterministic validation gate.

### 1.1 The Non-Bypassable Gate Principle
Validation logic must be decoupled from business logic and enforced at the **Peripheral (L1)** and **Application (L2)** layers.

| Boundary Level | Point of Enforcement | Technology | Objective |
| :--- | :--- | :--- | :--- |
| **Tier 0: Network** | WAF / API Gateway | Cloud-Native WAF | Block known malicious IPs, Geo-fencing, DDoS. |
| **Tier 1: Peripheral** | API Controllers / Filters | `FluentValidation`, `ActionFilter` | Structural integrity, length, type matching. |
| **Tier 2: Application** | MediatR / Service Layer | Business Rules, Domain Logic | Semantic validity, User-Tenant context matching. |
| **Tier 3: AI/External** | AI Proxy / Sanitizer | LLM Guardrails, Regex Scanners | Prompt injection, PII scrubbing, Outbound validation. |

---

## 2. Validation Standard Rules (Exhaustive)

### 2.1 String Constraints (Anti-Injection/XSS)
- **Length Whitelisting:** Every string MUST have a `MaximumLength`.
- **Character Whitelisting:** Default to strictly alphanumeric `[a-zA-Z0-9]` plus safe symbols.
- **No-HTML Policy:** All user inputs are treated as literal text. If Markdown/HTML is required, use a sanitizer (e.g., `HtmlSanitizer`) to whitelist tags.
- **Injection Patterns:** Reject strings containing SQL keywords (`SELECT`, `UNION`), Shell characters (`;`, `|`, `&`), or Javascript triggers (`<script>`, `onerror`).

### 2.2 Numerical & Range Checks
- **Overflow Prevention:** Use appropriate types (`int` vs `long`).
- **Logical Bounds:** Amounts must be positive; pagination `pageSize` limited to 100; IDs must be non-zero.

### 2.3 File Upload Security (Future-Proofing)
- **Type Whitelisting:** Check magic numbers (file signatures), NOT just extensions (`.pdf`, `.png`, `.json`).
- **Filename Sanitization:** Strip paths (`../`), regenerate filenames to GUIDs to prevent Path Traversal.
- **Size Hard-Limits:** Enforce max size (e.g., 5MB) via `RequestSizeLimit`.
- **Malware Scanning:** All files must pass through an ICAP or API-based scanner (ClamAV/Snyk) before persistence.
- **Content Inspection:** JSON/XML files must be checked for depth to prevent Billion Laughs attacks.

---

## 3. Multi-Tenant & Identity Integrity (IDOR/Tenant-Bleed)

### 3.1 Strict Tenant Context Isolation (STCI)
1. **Source of Truth:** The `tenant_id` MUST come from the JWT Claim, never the Request Body alone.
2. **The Cross-Check Rule:** If a request contains a `WorkspaceId` or `DocumentId`, the system MUST verify ownership:
   ```csharp
   // Logical Check in Service Layer
   var item = await _repo.GetByIdAsync(id);
   if (item.TenantId != _currentUser.TenantId) 
       throw new SecurityException("Cross-tenant access denied.");
   ```
3. **User-Level Permissions:** Validate that the `user_id` has specific rights to the requested resource (RBAC/ABAC).

---

## 4. AI-Specific Security: The LLM Shield

### 4.1 Inbound Safety (Prompt Sanitization)
- **Indirect Injection:** Scrub data retrieved from external sources (e.g., website crawling) before embedding in system prompts.
- **Jailbreak Detection:** Categorize and block tokens like `Ignore all instructions`, `System Override`, `as a developer`.

### 4.2 Outbound Safety (Response Validation)
- **PII Scrubbing:** Scan LLM responses for leaked API keys, emails, or internal hashes.
- **Schema Enforcement:** If the LLM generates JSON, validate it against the target schema before returning it to the UI.

---

## 5. Implementation Patterns (Production ASP.NET Core)

### A. Global Structural Guard (L1)
```csharp
public class GlobalValidationFilter : IAsyncActionFilter {
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) {
        if (!context.ModelState.IsValid) {
            var errors = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            context.Result = new BadRequestObjectResult(new { Code = "VALIDATION_ERROR", Errors = errors });
            return;
        }
        await next();
    }
}
```

### B. Comprehensive FluentValidator
```csharp
public class DocumentUploadValidator : AbstractValidator<DocumentUploadRequest> {
    public DocumentUploadValidator() {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255).Matches(@"^[\w\-. ]+$");
        RuleFor(x => x.FileContent).NotNull().Must(c => c.Length < 5 * 1024 * 1024);
        RuleFor(x => x.TenantId).Equal(ctx => _tenantService.Id); // Cross-check
    }
}
```

---

## 6. Anti-Patterns (The "Don'ts")
1. **Blacklisting:** Trying to list "bad" characters. (FAIL: Attackers find bypasses).
2. **Implicit Trust:** Assuming data from the internal DB is "safe" (FAIL: Second-order Injection).
3. **Double Encoding:** Sanitizing multiple times, leading to corrupted data like `&amp;amp;`.
4. **Client-Side Validation Only:** Using only Javascript for UX. (FAIL: Vulnerable to `curl`).

---

## 7. Review Checklist for Engineers
- [ ] **Input Constrained?** (Length, Regex, Type, Value Range)
- [ ] **Tenant Bound?** (Does identity context match the target data?)
- [ ] **Files Scanned?** (Signature check, Size limit, Path scrubbed)
- [ ] **Injection Protected?** (Parameterized queries only, No `eval()`)
- [ ] **AI Context Safe?** (System prompt protected from user input)
- [ ] **Error Messages Generic?** (No leaked stack traces or DB details)
