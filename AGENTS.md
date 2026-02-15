# METALFLOW SYSTEM AGENT INSTRUCTIONS

## SYSTEM-WIDE UI/UX & COMPONENT DIRECTIVE
- **Reference Material:** Read the Stich MetalFlow Main Layout as the absolute reference for UI/UX.
- **Component Library:** STRICTLY use MudBlazor components throughout the ENTIRE system.
- **Scope:** This strict MudBlazor requirement explicitly includes the login pages, profile pages, and all Identity-related UI. Do not use standard HTML, Bootstrap, or default Blazor templates for any UI elements.

---

## PHASE 1 — FOUNDATION (COMPLETE, LOCKED, UX-READY, NO GUESSWORK)

### PURPOSE (REAL LIFE)
- MetalFlow is a single Blazor Server app used daily to run branch operations while ERP stays system of record.
- Phase 1 guarantees: secure login, deterministic branch context, DB-backed branch roles, setup gating, audit trail base, and “install on empty DB” readiness.
- No operational module can be used until branch setup is complete (Phase 2 minimum config).

### NON-NEGOTIABLES
- Single Blazor Server project (.NET 10)
- MudBlazor for ALL UI
- EF Core with SQLite for DEV (still use migrations)
- EF Core Migrations REQUIRED; EnsureCreated forbidden
- NO HTTP APIs / NO /api/*
- ALL business logic in Services (UI → Services → DbContext)
- All operational entities include BranchId (from Phase 1 onward)
- All service queries MUST filter by active BranchId
- No hard deletes for operational data
- Override actions require reason + AuditEvent
- All times stored UTC; UI displays branch-local time
- Single active branch per session

### ENTITIES + REQUIRED FIELDS (EXACT)

**1) ApplicationUser (Identity)**
- Id (string)
- UserName (required)
- Email (required)
- FullName (required) // used everywhere (audit/concurrency attribution/UI)
- IsActive (bool required default true)
- CreatedAtUtc (required)
- LastLoginAtUtc (nullable)

**2) Branch (seeded; SystemAdmin only can edit)**
- Id
- Code (string required unique) // deterministic codes (see seed list)
- Name (required)
- Country (required fixed "Canada")
- Province (required)
- City (required)
- Address1 (required)
- PostalCode (required)
- Phone (required)
- TimeZoneId (required) // deterministic mapping list
- IsActive (bool required default true)
- CreatedAtUtc (required)
- UpdatedAtUtc (required)

**3) UserBranchMembership**
- Id
- UserId (required)
- BranchId (required)
- IsDefaultForUser (bool required)
- IsActive (bool required default true)
- AddedAtUtc (required)
Constraints:
- Unique(UserId, BranchId)
- At most one IsDefaultForUser=true per UserId (enforced in service)

**4) UserBranchRole (branch roles stored ONLY here)**
- Id
- UserId (required)
- BranchId (required)
- RoleName (required string)
- AssignedAtUtc (required)
- AssignedByUserId (required)
Constraints:
- Unique(UserId, BranchId, RoleName)

**5) AuditEvent (append-only)**
- Id
- BranchId (required)
- EventType (required)
- EntityType (required)
- EntityId (required)
- OccurredAtUtc (required)
- ActorUserId (required)
- Reason (required ONLY for override events)
- PayloadJson (required)
Rules:
- Never update/delete AuditEvent.

**6) Concurrency tokens required NOW (columns + EF configuration)**
Add long Version (optimistic concurrency) to:
- PickingListHeader
- PickingListLine
- WorkOrder
- PickPackTask
- LoadPlan
- ErpShipmentRef
- Tag

### CONCURRENCY UX RULE
On conflict: show blocking dialog:
"Updated by {User} at {Time}. Refresh required."

### ROLES (EXACT)
SystemAdmin
BranchAdmin
Supervisor
Planner
Operator
LoaderChecker
Driver
Viewer

### CLAIMS (ONLY THESE; via IClaimsTransformation)
- mf:userId
- mf:isSystemAdmin
- mf:defaultBranchId
Do NOT store branch roles in claims.

### AUTHORIZATION (NO GUESSWORK)
- All role checks use: IRoleResolver.HasRole(userId, branchId, roleName)
- RoleResolver: Scoped; loads roles for (userId, branchId) and caches per request.

### BRANCH RESOLUTION ORDER (EXACT)
1) Query string ?branchId= (only if valid active membership)
2) Encrypted cookie mf.branch (only if membership valid)
3) Membership where IsDefaultForUser=true
4) Else: block access with “No branch membership” screen

Invalid branch requested:
- Do NOT switch
- Fallback to valid branch
- Show snackbar warning: "Branch access denied. Stayed on {BranchCode}."

### SINGLE ACTIVE BRANCH PER SESSION
- BranchContext is Scoped and holds ActiveBranchId/Code/TimeZoneId.
- Every service method must require BranchContext and filter by BranchId automatically.

### SETUP GATES (PHASE 1 must implement evaluation + blocking)
SetupComplete for active branch requires ALL:
- At least one BranchAdmin exists for the branch
- ≥1 active Machine (Phase 2)
- ≥1 active PickPackStation (Phase 2)
- ≥1 active ShiftTemplate (Phase 2)
- ≥1 active Truck (Phase 2)
- ≥1 active ShippingFsaRule (Phase 2)
- ≥1 active ShippingFobMapping (Phase 2)
If not complete:
- Only Setup module accessible (SystemAdmin may still view but sees gate failures)

### UI/UX REQUIRED (MudBlazor)
- MainLayout shell:
  - Top bar: active branch badge (code+name), local time (branch TZ), user FullName
  - Switch Branch button (opens BranchPicker)
  - Snackbar provider always enabled
- BranchPicker dialog/page:
  - Shows membership branches: Code, Name, City, Province
  - Select branch; optional “set as default”
- SetupDashboard:
  - checklist of gates with red/green states and “Fix” links
  - blocks operational nav
- UserAdmin page:
  - Create user: Email, UserName, FullName, TempPassword, IsActive
  - Assign memberships: branch + default toggle + active toggle
  - Assign roles per branch (DB-backed)
  - SystemAdmin can grant SystemAdmin/BranchAdmin; BranchAdmin cannot grant SystemAdmin

### SEEDING (MANDATORY; runs after migrations, idempotent)
- Call db.Database.Migrate() at startup.
- Seed Identity roles: SystemAdmin..Viewer.
- Seed Cascadia Metals Canada branches (insert if missing; update address/phone/timezone if changed).
Branch seed dataset (LOCKED):
- DLT | Delta    | BC | Delta     | 7630 Berg Road               | V4G 1G4 | 604-946-3890 | America/Vancouver
- SRY | Surrey   | BC | Surrey    | #104 – 19433 96th Avenue     | V4N 4C4 | 604-881-4500 | America/Vancouver
- CGY | Calgary  | AB | Calgary   | 5566 54 Ave SE               | T2C 3A5 | 403-279-4995 | America/Edmonton
- YEG | Edmonton | AB | Edmonton  | 9525 60 Avenue NW            | T6E 0C3 | 780-962-9006 | America/Edmonton
- YXE | Saskatoon| SK | Saskatoon | 3062 Millar Ave              | S7K 5X9 | 306-652-2210 | America/Regina
- YBR | Brandon  | MB | Brandon   | 33rd St East & Hwy 10        | R7A 5Y4 | 204-728-9484 | America/Winnipeg
- YWG | Winnipeg | MB | Winnipeg  | 1540 Seel Ave                | R3T 4Z6 | 204-477-8748 | America/Winnipeg
- YHM | Hamilton | ON | Hamilton  | 1632 Burlington St E         | L8H 3L3 | 905-795-1880 | America/Toronto
- YUL | Dorval   | QC | Dorval    | 1535 Hymus Blvd              | H9P 1J5 | 514-532-0290 | America/Toronto

SystemAdmin seed user (required; from config keys):
- Seed:SystemAdmin:Email
- Seed:SystemAdmin:UserName
- Seed:SystemAdmin:FullName
- Seed:SystemAdmin:Password
Optional:
- Seed:SystemAdmin:DefaultBranchCode (e.g., SRY)
- Seed:SystemAdmin:MembershipBranchCodes (comma list)

Seeder behavior:
- Create user if missing; ensure IsActive=true.
- Add Identity role SystemAdmin.
- If DefaultBranchCode provided and valid, create membership + set default.

---

## PHASE 2 — CONFIGURATION MODULES (COMPLETE, LOCKED, UX-READY, NO GUESSWORK)

### PURPOSE (REAL LIFE)
- Configure the physical world and dispatch rules per branch so later scheduling/execution is deterministic:
  machines, stations, shifts, trucks, non-working/OT rules, FSA geography, FOB shipping method mapping.

### ACCESS CONTROL (LOCK IT HERE)
- SystemAdmin: view/edit any branch
- BranchAdmin: view/edit own branch only
- Planner: view all; edit Shifts + NonWorkingDays only
- Supervisor: same as Planner + approve OT overrides
- Others: view-only or hidden

### ENTITIES + REQUIRED FIELDS

**Machine**
- Id, BranchId
- MachineCode (required unique per branch)
- MachineName (required)
- MachineType (CTL|Slitter required)
- IsActive (bool required default true)
- DefaultDurationMinutes (int required default 60)
- Notes (nullable)
- CreatedAtUtc, UpdatedAtUtc
Unique(BranchId, MachineCode)

**MachineOperatorAssignment**
- Id, BranchId
- MachineId (required)
- UserId (required)
- IsQualified (bool required default true)
- QualifiedAtUtc (required)
- QualifiedByUserId (required)
Unique(BranchId, MachineId, UserId)

**PickPackStation**
- Id, BranchId
- StationCode (required unique per branch)
- StationName (required)
- IsActive (bool required default true)
- Notes (nullable)
- CreatedAtUtc, UpdatedAtUtc
Unique(BranchId, StationCode)

**ShiftTemplate (overnight supported)**
- Id, BranchId
- ShiftCode (required unique per branch)
- Name (required)
- StartLocalTime (TimeOnly required)
- EndLocalTime (TimeOnly required)
- BreakMinutes (int required default 0)
- IsActive (bool required default true)
- CreatedAtUtc, UpdatedAtUtc
Computed:
- IsOvernight = EndLocalTime < StartLocalTime
Unique(BranchId, ShiftCode)

**Truck**
- Id, BranchId
- TruckCode (required unique per branch)
- Description (required)
- MaxWeightLbs (decimal required > 0)
- IsActive (bool required default true)
- CreatedAtUtc, UpdatedAtUtc
Unique(BranchId, TruckCode)

**ShippingRegion**
- Id, BranchId
- RegionCode (required unique per branch)
- RegionName (required)
- IsActive (required)
- CreatedAtUtc, UpdatedAtUtc

**ShippingGroup**
- Id, BranchId
- GroupCode (required unique per branch)
- GroupName (required)
- IsActive (required)
- CreatedAtUtc, UpdatedAtUtc

**ShippingFsaRule (deterministic)**
- Id, BranchId
- FsaPrefix (length 3 required uppercase)
- RegionId (required)
- GroupId (required)
- Priority (int required default 1; lower wins)
- IsActive (required)
- CreatedAtUtc, UpdatedAtUtc
Unique(BranchId, FsaPrefix, Priority)
Match rule:
- Normalize postal: uppercase, remove spaces; take first 3 chars.
- Find active rules for FsaPrefix; pick smallest Priority.
- If none: HARD FAIL in Phase 4 import.

**ShippingFobMapping (strict FOB → ShippingMethod)**
- Id, BranchId
- FobToken (string required uppercase; exact ERP FOB_POINT)
- ShippingMethod (enum required)
- IsActive (required)
- CreatedAtUtc, UpdatedAtUtc
Unique(BranchId, FobToken)
Allowed ShippingMethod enum (LOCKED):
- Delivery
- CustomerPickup
- Transfer
Match rule:
- Normalize FOB token uppercase trimmed; exact match only.
- If none: HARD FAIL in Phase 4 import.

**NonWorkingDay**
- Id, BranchId
- DateLocal (DateOnly required)
- Reason (required)
- IsActive (required)
- CreatedAtUtc, UpdatedAtUtc
Unique(BranchId, DateLocal)

**NonWorkingDayOverride (OT enable approval)**
- Id, BranchId
- DateLocal (DateOnly required)
- OverrideType (OvertimeEnabled)
- Reason (required)
- ApprovedByUserId (required)
- ApprovedAtUtc (required)
- AuditEventId (required)
Unique(BranchId, DateLocal, OverrideType)

### RULES (SERVICE-ENFORCED)
- All config is branch-scoped and filtered.
- OT override: Supervisor only + reason modal + AuditEvent + NonWorkingDayOverride insert (no delete).
- Shifts: overnight computed; UI must label overnight.
- No inference/fallback for FOB or FSA matching.

### UX/UI SCREENS (MudBlazor)
- Configuration Home (tile counts + Setup Gate checklist)
- Machines (list + detail + qualified operator management)
- Stations (CRUD)
- Shifts (builder + overnight preview)
- Trucks (CRUD)
- Shipping:
  - Regions/Groups CRUD
  - FSA rules editor + Test Postal tool (match/no match)
  - FOB mapping editor + Test FOB tool (match/no match)
- Non-working Calendar:
  - calendar view
  - add non-working day
  - enable OT override (Supervisor + reason + audit)

### SETUP GATES (Phase 1 depends on Phase 2)
SetupComplete ONLY if:
- ≥1 active Machine
- ≥1 active PickPackStation
- ≥1 active ShiftTemplate
- ≥1 active Truck
- ≥1 active ShippingFsaRule
- ≥1 active ShippingFobMapping

---

## PHASE 3 — ITEM MASTER + INVENTORY (COMPLETE, LOCKED, NO INFERENCE)

### PURPOSE (REAL LIFE)
- Provide item identity and ERP snapshot visibility for planning and validation.
- MetalFlow does NOT run transactional inventory; ERP is authoritative.
- Snapshots are append-only history; InventoryStock is a deterministic mirror of latest snapshot values.

### ACCESS
- BranchAdmin/SystemAdmin: import + manage
- Planner/Supervisor: view
- Others: optional read-only

### ENTITIES

**ItemMaster**
- Id, BranchId
- ItemCode (required unique per branch)
- Description (required)
- CoilRelationship (nullable; exact text)
- IsActive (required default true)
- CreatedAtUtc, UpdatedAtUtc
Unique(BranchId, ItemCode)

**InventorySnapshotHeader (append-only)**
- Id, BranchId
- SnapshotDateUtc (required)
- SnapshotReference (required)
- SourceSystem (required must equal "ERP")
- ImportedByUserId (required)
- ImportedAtUtc (required)

**InventorySnapshotLine (append-only)**
- Id, BranchId
- SnapshotHeaderId (required)
- ItemCode (required; must exist in ItemMaster)
- Description (required; keep as provided)
- UomRaw (required; PCS or LBS only)
- OnHandQty (decimal required >=0)
- OnHandWeightLbs (decimal required >=0)

**InventoryStock (latest mirror)**
- Id, BranchId
- ItemCode (required)
- UomRaw (PCS|LBS required)
- CurrentQty (required)
- CurrentWeightLbs (required)
- LastSnapshotDateUtc (required)
- Version (concurrency)
Unique(BranchId, ItemCode, UomRaw)

### IMPORT FORMAT (LOCKED)

ItemMaster import table headers EXACT:
ItemCode | Description | CoilRelationship
Rules:
- Re-import updates Description + CoilRelationship only
- No deletion

Inventory Snapshot import:
Header block Key:Value required:
SNAPSHOT_DATE_UTC:
SNAPSHOT_REFERENCE:
SOURCE_SYSTEM: ERP

Then table headers EXACT:
ItemCode | Description | [blank header] | OnHandQty | OnHandWeightLbs
- blank header column must exist; values are PCS or LBS only.

### HARD FAIL (transaction; no partial commit)
- Missing required header fields
- SOURCE_SYSTEM != ERP
- Missing table headers (including blank header)
- UOM not PCS or LBS
- ItemCode not found in ItemMaster
- Negative qty/weight
- Duplicate ItemCode+UOM within a snapshot

### AGGREGATE UPDATE (NO DELTAS)
For each snapshot line, upsert InventoryStock and set:
- CurrentQty = OnHandQty
- CurrentWeightLbs = OnHandWeightLbs
- LastSnapshotDateUtc = SnapshotDateUtc

### UX/UI
- ItemMaster: import wizard + list/search
- Snapshots: import wizard (validate→preview→commit) + history + detail read-only
- Current Stock: list/search, filter by UOM, banner showing last snapshot import time/user

---

## PHASE 4 — PICKING LIST IMPORT (COMPLETE, LOCKED, ERP SOURCE OF RECORD)

### PURPOSE (REAL LIFE)
- ERP issues Picking Lists; MetalFlow consumes them to drive:
  production scheduling/execution, pick/pack, load planning, scanning, delivery proof, reporting.
- Re-import handles ERP changes deterministically without deleting history.

### ACCESS
- Planner: import/reimport; confirm cancellations
- Supervisor: approve overrides
- Others: execute downstream

### ENTITIES

**PickingListHeader**
- Id, BranchId
- PickingListNumber (required unique per branch)
- PrintDateUtc (required)
- ShipDateUtc (required)
- Buyer (required)
- SalesRep (required)
- ShipVia (required)
- FobPoint (required)
- SoldTo (required)
- ShipToMultiline (required exact preserved)
- TotalWeightLbs (required)
- OrderInstructions (nullable exact)
- ShippingMethod (required; derived strictly from FOB mapping Phase 2)
- ShippingRegionId (required; derived strictly from FSA mapping Phase 2)
- ShippingGroupId (required; derived strictly from FSA mapping Phase 2)
- Status (New|Planned|InProgress|Completed)
- Version (concurrency)
Unique(BranchId, PickingListNumber)

**PickingListLine**
- Id, BranchId
- PickingListId
- LineNumber (required)
- ItemCode (required)
- Description (required)
- UomRaw (PCS|LBS required)
- MaterialType (required; derived ONLY: PCS→SHEET, LBS→COIL)
- QtyOrdered (required)
- WeightOrderedLbs (required)
- WidthIn (nullable)
- LengthIn (nullable)
- ReservedMaterialsJson (nullable exact)
- LineInstructions (nullable exact)
- LineStatus (Active|PendingCancel|Cancelled)
- Version (concurrency)
Unique(PickingListId, LineNumber)

### IMPORT FORMAT (STRICT plain-text; keys EXACT from your master spec)
HEADER keys required:
PICKING_LIST_NO, PRINT_DATE, SHIP_DATE, BUYER, SALES_REP, SHIP_VIA, FOB_POINT, SOLD_TO, SHIP_TO, TOTAL_WEIGHT_LBS, ORDER_INSTRUCTIONS

LINE keys required:
LINE_NUMBER, ITEM_CODE, DESCRIPTION, UOM, QTY_ORDERED, WEIGHT_ORDERED_LBS, WIDTH_IN, LENGTH_IN, RESERVED_MATERIALS[], LINE_INSTRUCTIONS

Rules:
- ShippingMethod: derived via FOB mapping (no match → FAIL import)
- Region/Group: derived via FSA from SHIP_TO postal code (no match → FAIL import)
- ShipTo multi-line preserved exactly.

### REIMPORT RULES (NO GUESSWORK)
- Match existing lines by (LineNumber + ItemCode)
- Missing existing line on reimport → set LineStatus=PendingCancel (do NOT cancel)
- Planner confirms cancel → LineStatus=Cancelled and writes AuditEvent
- Never hard delete headers/lines

### UX/UI
- Import: paste → validate → preview (header card + lines grid) → commit
- Picking List detail: ship-to preserved; derived shipping fields read-only; pending cancel banner + confirm action
- Links to downstream: pick/pack tasks, work orders, load reservations, shipment refs

---

## PHASE 5 — PICK & PACK (COMPLETE, LOCKED, HARD STOPS)

### PURPOSE (REAL LIFE)
- Warehouse picks and stages product, scanning tags to prevent duplicates and ensure correct quantities/weight tolerance.
- Produces execution trace used by load scanning and shipment linking.

### ACCESS
- Operator: claim/execute
- Planner/Supervisor: manual done + overrides (reason + audit)
- LoaderChecker/Driver/Viewer: view

### ENTITIES

**PickPackTask**
- Id, BranchId
- PickingListId
- StationId
- Status (New|Claimed|InProgress|Done)
- ClaimedByUserId (nullable)
- ClaimedAtUtc (nullable)
- CompletedAtUtc (nullable)
- Version (concurrency)

**PickPackTaskLine**
- Id, BranchId
- PickPackTaskId
- PickingListLineId
- QtyPicked (decimal required default 0)
- WeightPickedLbs (decimal required default 0)
- IsSatisfied (bool required default false)

**PickPackScan**
- Id, BranchId
- PickPackTaskId
- TagNumber (required)
- Qty (required)
- ScannedAtUtc (required)
- ScannedByUserId (required)

### RULES
- Operators can claim any station task
- Duplicate source tag = HARD STOP (never overridable)
- Auto Done when all task lines satisfied
- Manual Done requires Planner/Supervisor + reason + AuditEvent
Weight rules:
- WeightPerPiece = round(WeightOrderedLbs / QtyOrdered, 4)
- StagedWeight = round(Qty * WeightPerPiece, 2)
Tolerance rule:
- abs(ExpectedWeight - TotalShippedWeight) <= 10 lbs per PickingListLine overall

### UX/UI
- Station Board: choose station → claim task
- Execution screen: scan input always focused; live qty/weight; hard stop dialogs
- Completion: auto or manual approval modal (reason required)

---

## PHASE 6 — WORK ORDER + SCHEDULER (COMPLETE, LOCKED, SHIFT-AWARE)

### PURPOSE (REAL LIFE)
- Planners schedule CTL/Slitter production without overlaps and respecting working time.
- Operators execute WOs and record ERP-issued tags (8-digit numeric; never generated).

### ACCESS
- Planner: schedule/drag/drop, override duration
- Operator: execute
- Supervisor: cancellations/overrides requiring reason + audit

### ENTITIES

**WorkOrder**
- Id, BranchId
- MachineId
- Status (New|Planned|InProgress|Completed|Cancelled)
- ScheduledStartUtc (required)
- ScheduledEndUtc (required)
- DurationMinutes (required; default 60; planner can override)
- IsOvertimeEnabled (bool required default false)
- CreatedAtUtc, CreatedByUserId
- Version (concurrency)

**WorkOrderLink**
- Id, BranchId
- WorkOrderId
- PickingListLineId

**Tag**
- Id, BranchId
- TagNumber (required; 8-digit numeric)
- Status (Active|SoftDeleted|Shipped)
- CreatedAtUtc, CreatedByUserId
- Version (concurrency)

**WorkOrderTag**
- Id, BranchId
- WorkOrderId
- TagId
- RecordedAtUtc
- RecordedByUserId

### SCHEDULING RULES
- Auto schedule next available working slot:
  uses ShiftTemplates + NonWorkingDay; excludes weekends + NonWorkingDays unless OT override exists
- Hard block machine overlap always
- Drag/drop allowed only for Planned & InProgress
- Snap to shift start unless OT enabled
- Maintenance blocks: if not implemented yet, do NOT invent behavior; add placeholder entity only if you plan it soon

### TAG RULES
- Must be 8-digit numeric
- Issued by ERP: user enters ERP tag number; system never generates or edits
- SoftDelete only if not shipped; requires Supervisor + reason + AuditEvent

### UX/UI
- Scheduler per machine timeline with drag/drop, snapping, non-working shading
- WorkOrder detail + operator execution controls
- Tag entry with validation

---

## PHASE 7 — FORWARD LOAD PLANNING (COMPLETE, LOCKED)

### PURPOSE (REAL LIFE)
- Planner creates future loads before ERP packing lists exist.
- Assign truck/driver, reserve picking lists, sequence stops, enforce capacity.

### ACCESS
- Planner: create/edit reservations and sequencing
- Supervisor: overrides if you add them (reason + audit)
- LoaderChecker/Driver: view assigned loads

### ENTITIES

**LoadPlan**
- Id, BranchId
- PlannedDateLocal (required)
- TruckId (required)
- DriverUserId (nullable until assigned)
- Status (Draft|Active|InTransit|Delivered|Closed)
- CreatedAtUtc, CreatedByUserId
- Version (concurrency)

**LoadPlanReservation**
- Id, BranchId
- LoadPlanId
- PickingListId
- StopSequence (required)
- ReservationStatus (Reserved|Removed|ConvertedToErp)
- ReservedAtUtc, ReservedByUserId
Unique(LoadPlanId, StopSequence)

### RULES
- MetalFlow does NOT create ERP packing lists
- Capacity enforced using ordered weight:
  sum(PickingListHeader.TotalWeightLbs for reservations) <= Truck.MaxWeightLbs
- Stop sequencing is required and editable
- Reservation removal should write AuditEvent (reason optional unless you classify it as override)

### UX/UI
- Load Planning Board:
  - available picking lists list (filters)
  - load plan panel with reservations and stop reorder
  - capacity meter
  - lifecycle actions

---

## PHASE 8 — ERP SHIPMENT LINKING (COMPLETE, LOCKED)

### PURPOSE (REAL LIFE)
- Once ERP produces the official packing list number, link it.
- Load verification cannot proceed until linked.

### ACCESS
- Planner/BranchAdmin: link ERP packing list
- LoaderChecker: blocked until link exists

### ENTITIES

**ErpShipmentRef**
- Id, BranchId
- PickingListId
- ErpPackingListNumber (required; unique per branch)
- ShipDateLocal (required)
- Status (Created|Linked|LoadedVerified|Shipped|Delivered)
- Version (concurrency)

**ShipmentTagAllocation**
- Id, BranchId
- ErpPackingListNumber (required)
- TagNumber (required)
- AllocatedAtUtc
- AllocatedByUserId

### RULES
- Reservation converts to ErpShipmentRef (ReservationStatus → ConvertedToErp)
- LoadedVerified not allowed until ErpShipmentRef exists
- Uniqueness enforced on (BranchId, ErpPackingListNumber)

### UX/UI
- Link screen/action from LoadPlan/Reservation:
  - enter ERP packing list number
  - validate uniqueness and branch
  - confirm link
- Post-link display: ERP packing list number visible in load and shipment views

---

## PHASE 9 — LOAD SCANNING (COMPLETE, HARD STOP, NO OVERRIDES EXCEPT WEIGHT)

### PURPOSE (REAL LIFE)
- LoaderChecker verifies loading via scanning.
- Prevent wrong tags, duplicates, cross-load mistakes. Hard stops enforce safety.

### ACCESS
- LoaderChecker: scan and verify
- Supervisor: weight override approval only
- Planner: view

### ENTITIES

**LoadScanSession**
- Id, BranchId
- LoadPlanId (required)
- ErpPackingListNumber (required)      // must exist; link required before scanning
- Status (Open|Verified)
- OpenedAtUtc, OpenedByUserId
- VerifiedAtUtc (nullable), VerifiedByUserId (nullable)

**LoadScan**
- Id, BranchId
- SessionId
- TagNumber
- ScannedAtUtc
- ScannedByUserId
Unique(SessionId, TagNumber)

### HARD STOP RULES (NEVER OVERRIDABLE)
- Tag already scanned (this session)
- Tag not found (Tag table missing)
- Tag belongs to another load (already tied to another active session/shipment)
- Tag assigned to another shipment (ShipmentTagAllocation mismatch)
- Tag already shipped

### OVERRIDE ALLOWED (ONLY)
- Missing weight → Supervisor + reason + AuditEvent

LoadedVerified only if:
- All expected tags scanned
- All required weights present

### UX/UI
- Scan-first screen: big input, always focused
- Progress indicator expected vs scanned
- Hard-stop modal with exact reason
- Verify button disabled until criteria met
- Weight override modal requires Supervisor + reason (creates AuditEvent)

---

## PHASE 10 — DELIVERY PROOF (COMPLETE, LOCKED)

### PURPOSE (REAL LIFE)
- Driver uploads proof of delivery (signed docs/photos) for ERP shipment.
- Stored securely with retention and protected viewing.

### ACCESS
- Driver: upload for assigned shipment/load
- Planner/Supervisor/BranchAdmin: view
- Others: per your policy

### ENTITIES

**DeliveryProof**
- Id, BranchId
- ErpPackingListNumber (required)
- FileName (required)
- ContentType (required)
- FileSizeBytes (required)
- StoragePath (required)
- UploadedAtUtc (required)
- UploadedByUserId (required)
- RetentionDays (required default 365)
- ExpiresAtUtc (required)

### RULES
- Store files: App_Data/DeliveryProof/{BranchId}/{yyyy}/{MM}/
- DB stores metadata only
- Serve via protected Razor route: /proof/{proofId}
- Max file size 10MB hard fail
- Retention default 365 configurable

### UX/UI
- Driver upload page: attach file(s) + list existing proofs
- Office view: list proofs by shipment; open via protected route

---

## PHASE 11 — REPORTING (COMPLETE, LIVE ONLY, CSV EXPORT)

### PURPOSE (REAL LIFE)
- Supervisors and planners need operational visibility from live data only.
- No snapshot reporting tables.

### ACCESS
- Supervisor/Planner: view + export
- Viewer: view-only
- Others: per your policy

### REQUIRED REPORTS (MINIMUM)
Production (per shift)
- output weight
- # coils processed
- man-hours (from ShiftTemplate + execution spans if captured)

Logistics
- load weight
- # stops
- driver hours from DriverShiftLog (must exist; add entity if not yet present)

Exceptions
- missing tags
- weight overrides
- cancellations after start
- coil substitutions

### EXPORT
- CSV per branch + date range (date range required)
- Active branch fixed by BranchContext

### UX/UI
- Reports landing tiles: Production / Logistics / Exceptions
- Filter bar: date range required; optional machine/driver
- Table-first pages; charts optional
- Export button always visible and produces deterministic CSV