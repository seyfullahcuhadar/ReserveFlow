# ReserveFlow — Yapılacaklar

Kaynak: [PROJECT.md](PROJECT.md) · [USE_CASES.md](USE_CASES.md) · [NFR.md](NFR.md)

## Tamamlanan

- [x] Solution + Clean Architecture iskeleti
- [x] UC-ID-01 Register User
- [x] UC-ID-02 Login → JWT
- [x] UC-CAT-01 Create Organizer Profile
- [x] UC-CAT-06 Create Venue
- [x] UC-CAT-02 Create Event (Draft) + Add TicketType
- [x] UC-CAT-03 Publish Event
- [x] UC-CAT-05 Cancel Event
- [x] NFR-M01 Layered architecture (NetArchTest)
- [x] Health endpoint

---

## F1 — Domain + CRUD (şimdi buradayız)

### Catalog

- [x] UC-CAT-06 Create Venue
- [x] UC-CAT-02 Create Event (Draft) + Add TicketType
- [x] UC-CAT-03 Publish Event
- [ ] UC-CAT-04 List Events (pagination; cache F3'te)
- [x] UC-CAT-05 Cancel Event

### Scheduling

- [ ] UC-SCH-01 Provider Profile + Weekly Availability
- [ ] UC-SCH-02 List Available Slots
- [ ] UC-SCH-03 Create Appointment (overlap prevention)
- [ ] UC-SCH-04 Cancellation / Rescheduling (24h kuralı)

### Booking / Payment iskeleti

- [ ] Order + Reservation domain model
- [ ] UC-CORE-01 Event Ticket Sale (happy path, fake payment)
- [ ] UC-CORE-02 Appointment Booking (happy path)

### Kalite

- [ ] Domain + Application unit testleri (NFR-M02 ≥70%)
- [ ] API versioning `/api/v1/` (NFR-M03)
- [ ] En az 2–3 ADR yaz

---

## F2 — Security

- [ ] UC-ID-03 RBAC (role policy + 403 testleri)
- [ ] UC-ID-04 Rate limiting → 429
- [ ] Tüm public endpoint'lerde FluentValidation
- [ ] Secrets: User Secrets / env (Gitleaks)

---

## F3 — Performance

- [ ] Event/slot listelerinde pagination
- [ ] Redis cache (event list)
- [ ] DB index'ler
- [ ] p95 < 300ms ölçüm kanıtı

---

## F4 — Reliability (kritik NFR'ler)

- [ ] UC-CORE-03 Double-selling / overlap race
- [ ] UC-CORE-04 Idempotent retry (IdempotencyKey)
- [ ] UC-CORE-05 Payment fail/timeout → expire → quota/slot release
- [ ] UC-BOOK-06 Expire Pending Reservation (background)
- [ ] UC-PAY-01 Fake gateway success/fail/timeout
- [ ] UC-NOTIF-01 Outbox delivery + retry

---

## F5 — Observability

- [ ] OpenTelemetry tracing
- [ ] Metrics + structured logging
- [ ] Dashboard (latency, error rate, throughput)

---

## F6 — Scalability

- [ ] k6 load test (en az 1 kritik endpoint)
- [ ] Horizontal scale senaryosu
- [ ] Circuit breaker (gerekirse)

---

## F7 — Availability

- [ ] Gelişmiş health checks
- [ ] Backup / restore drill
- [ ] Runbook (deploy, rollback, restore)

---

## Admin

- [ ] UC-ADM-01 List Events / Appointments
- [ ] UC-ADM-02 Sales count + occupancy rate

---

## Proje bitiş kriterleri

- [ ] Bounded context + domain diyagramı
- [ ] NFR matrisi + ölçüm kanıtları
- [ ] Load test raporu
- [ ] Observability dashboard
- [ ] Runbook
- [ ] ≥ 5 ADR

---

## Sıradaki 3 madde

1. UC-CAT-04 List Events (pagination; cache F3'te)
2. UC-CORE-01 Event Ticket Sale happy path iskeleti
3. UC-SCH-01 Provider Profile + Weekly Availability
