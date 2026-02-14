# PHASE 3 — ITEM MASTER + INVENTORY (STRICT MINIMAL MODEL)

## A) ITEM MASTER IMPORT

ItemMaster Import Headers (EXACT):
- ItemCode
- Description
- CoilRelationship

Rules:
- Upsert by (BranchId, ItemCode)
- Store CoilRelationship exactly as provided
- PPSF is nullable and editable later
- Import MUST NOT overwrite PPSF if already set

UOM Derivation Rule:
- If CoilRelationship indicates sheet → UOM = PCS
- Else → UOM = LBS
- If CoilRelationship is blank:
    - If Description contains "SHEET" or "SHT" → PCS
    - Else → LBS

No inference beyond the rules above.

---------------------------------------------------------------------

## B) INVENTORY SNAPSHOT IMPORT

Inventory Snapshot Excel Headers (EXACT, IN ORDER):

1) Item ID
2) Description
3) Size
4) Mode
5) Tag #
6) Status
7) Correctable?
8) Snapshot Loc
9) Count Loc
10) Snapshot
11) Count
12) Exception
13) [BLANK HEADER COLUMN]   <-- This column header is intentionally blank
14) Amount

IMPORTANT:
Only the following fields are imported into MetalFlow:

- Item ID
- Description
- Size
- Tag #
- Snapshot Loc
- Snapshot (quantity)
- [BLANK HEADER COLUMN] → UOM (PCS or LBS ONLY)

The following columns are IGNORED and NOT stored:
- Mode
- Status
- Correctable?
- Count Loc
- Count
- Exception
- Amount

InventorySnapshotLine must store ONLY:

- BranchId
- SnapshotId
- ItemId (from "Item ID")
- Description
- Size
- TagNumber (from "Tag #")
- Location (from "Snapshot Loc")
- Quantity (from "Snapshot")
- UOM (from blank header column; must be PCS or LBS)

Rules:

- The blank header column is the authoritative UOM source.
- UOM must be exactly PCS or LBS.
- Reject row if UOM is anything else.
- Do NOT infer UOM from ItemMaster or Description.
- Always append snapshot records (audit trail).
- Never hard delete snapshot lines.
- InventoryStock aggregate is updated for browsing only.

No other columns are stored.
No additional inference is allowed.
No transformation beyond mapping the specified fields.
