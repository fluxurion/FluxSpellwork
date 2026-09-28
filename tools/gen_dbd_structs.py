#!/usr/bin/env python3
"""Generate version-specific DB2 struct classes for SpellWork from WoWDBDefs DBD files.

Emits <Table>EntryV115 / <Table>EntryV160 structs into Structures/V115 and V160,
each with a ToCanonical() method converting to the canonical (10.x-shaped) entry type.
"""
import os, re, sys

DEFS = sys.argv[1] if len(sys.argv) > 1 else os.path.join(os.environ["TEMP"], "WoWDBDefs", "definitions")
SRC = sys.argv[2] if len(sys.argv) > 2 else r"C:\FluxSpellwork\SpellWork\DBC\Structures"

BUILD_RE = re.compile(r"^BUILD\s+(.+)$")
LAYOUT_RE = re.compile(r"^LAYOUT\s+(.+)$")
VER_RE = re.compile(r"^(\d+)\.(\d+)\.(\d+)\.(\d+)$")
COL_RE = re.compile(r"^(\$[^\$]*\$)?([A-Za-z0-9_]+)(?:<([^>]+)>)?(?:\[(\d+)\])?\s*(?://.*)?$")

def parse_version(s):
    m = VER_RE.match(s.strip())
    return tuple(int(x) for x in m.groups()) if m else None

def build_covers(entry, build):
    if "-" in entry:
        lo, hi = parse_version(entry.split("-", 1)[0]), parse_version(entry.split("-", 1)[1])
        return lo is not None and hi is not None and lo <= build <= hi
    v = parse_version(entry)
    return v is not None and v == build

def parse_dbd(path):
    lines = open(path, encoding="utf-8", errors="replace").read().splitlines()
    columns, blocks, cur, in_columns = {}, [], None, False
    def flush():
        nonlocal cur
        if cur and (cur["layouts"] or cur["builds"]):
            blocks.append(cur)
        cur = None
    for raw in lines:
        s = raw.strip()
        if s == "COLUMNS":
            in_columns = True; continue
        if s == "":
            if in_columns: in_columns = False
            else: flush()
            continue
        if in_columns:
            s2 = s.split("//", 1)[0].strip()
            if not s2: continue
            parts = s2.split()
            if len(parts) >= 2:
                columns[parts[1].rstrip("?")] = parts[0].split("<", 1)[0]
            continue
        if cur is None:
            cur = {"layouts": [], "builds": [], "cols": []}
        m = LAYOUT_RE.match(s)
        if m:
            cur["layouts"] += [h.strip() for h in m.group(1).split(",")]; continue
        m = BUILD_RE.match(s)
        if m:
            cur["builds"] += [e.strip() for e in m.group(1).split(",")]; continue
        if s.startswith("COMMENT"):
            continue
        cm = COL_RE.match(s)
        if cm:
            flags = (cm.group(1) or "").strip("$")
            parts = flags.split(",") if flags else []
            cur["cols"].append({"name": cm.group(2), "size": cm.group(3),
                                "arr": cm.group(4), "noninline": "noninline" in parts,
                                "id": "id" in parts, "relation": "relation" in parts})
    flush()
    return columns, blocks

def layout_for(columns, blocks, build_str):
    bv = parse_version(build_str)
    for blk in blocks:
        for e in blk["builds"]:
            if build_covers(e, bv):
                return blk["layouts"], [{**c, "type": columns.get(c["name"], "int")} for c in blk["cols"]]
    return None, None

def parse_cs_struct(path):
    """Return (classname, fields) for canonical struct."""
    fields, card, index = [], None, None
    clsname = None
    for line in open(path, encoding="utf-8", errors="replace").read().splitlines():
        s = line.strip()
        m = re.match(r"public\s+(?:sealed\s+)?class\s+(\w+)", s)
        if m:
            clsname = m.group(1)
        m = re.match(r"\[Index\((\w+)\)\]", s)
        if m:
            index = m.group(1) == "true"; continue
        m = re.match(r"\[Cardinality\((\d+)\)\]", s)
        if m:
            card = int(m.group(1)); continue
        m = re.match(r"public\s+(?!sealed|class|static)([A-Za-z0-9_<>\[\]]+)\s+([A-Za-z0-9_]+)\s*(?:=[^;]*)?;", s)
        if m and "(" not in s:
            fields.append({"type": m.group(1), "name": m.group(2), "card": card, "index": index})
            card, index = None, None
    return clsname, fields

def dbd_cstype(c):
    t = c["type"]
    if t in ("locstring", "string"): return "string"
    if t == "float": return "float"
    if t == "double": return "double"
    unsigned = (t == "uint")
    bits = 32
    if c["size"]:
        if c["size"].startswith("u"): unsigned, bits = True, int(c["size"][1:])
        else: bits = int(c["size"])
    return {(False,8):"sbyte",(True,8):"byte",(False,16):"short",(True,16):"ushort",
            (False,32):"int",(True,32):"uint",(False,64):"long",(True,64):"ulong"}[(unsigned,bits)]

def cs_base(t):
    return t[:-2] if t.endswith("[]") else t

# name overrides: (table, vers col) -> canonical field. None = drop the column entirely.
OVERRIDES = {
    "AreaTable": {"Ambient_multiplier": "AmbientMultiplier"},
    "SpellEffect": {"EffectPos_facing": "EffectPosFacing", "EffectBasePointsF": "EffectBasePoints",
                    "EffectBasePoints": None, "Field_5_5_4_67090_025": "ScalingClass"},
    "SpellRadius": {"RadiusMax": "MaxRadius"},
    "SpellRange": {"RangeMin": "MinRange", "RangeMax": "MaxRange"},
    "SpellXSpellVisual": {"Flags2": "Flags"},
    "SkillLine": {"NeutralDisplayName": "OverrideSourceInfoDisplayName"},
    "SkillLineAbility": {"RaceMasks": "RaceMask", "Field_5_5_4_67090_013": "SkillupSkillLineID"},
    "ItemEffect": {"ParentItemID": "ItemID"},
    "ItemSparse": {"Gem_properties": "GemProperties", "Socket_match_enchantment_ID": "SocketMatchEnchantmentId",
                   "DamageType": "DamageDamageType", "OppositeFactionItemID": "FactionRelated",
                   "StatModifier_bonusStat": "StatModifierBonusStat"},
    "SpellReagents": {"ReagentReCraftCount": "ReagentRecraftCount"},
}

def canon_name(table, col):
    o = OVERRIDES.get(table, {})
    if col in o:
        return o[col]
    if col.endswith("_lang") and col[:-5]:
        return col[:-5]
    return col

def canon_file(table):
    for cand in (f"{table}Entry.cs", f"{table}.cs"):
        p = os.path.join(SRC, cand)
        if os.path.exists(p): return p
    return None

def gen_struct(table, ver, cols, canon_cls, canon_fields, outdir):
    lines = ["using DBFileReaderLib.Attributes;", "",
             f"namespace SpellWork.DBC.Structures.{ver}", "{",
             f"    public sealed class {table}Entry{ver} : IConvertsTo<{canon_cls}>", "    {"]
    for c in cols:
        t = dbd_cstype(c)
        if c["id"]:
            lines.append(f"        [Index({str(c['noninline']).lower()})]")
        if c["arr"]:
            lines.append(f"        [Cardinality({c['arr']})]")
            lines.append(f"        public {t}[] {c['name']} = new {t}[{c['arr']}];")
        else:
            lines.append(f"        public {t} {c['name']};")
    lines.append("")
    # ----- ToCanonical -----
    lines.append(f"        public {canon_cls} ToCanonical()")
    lines.append("        {")
    lines.append(f"            var e = new {canon_cls}")
    lines.append("            {")
    copys = []
    canon_by_name = {f["name"]: f for f in canon_fields}
    for c in cols:
        cname = canon_name(table, c["name"])
        if cname is None or cname not in canon_by_name:
            continue
        cf = canon_by_name[cname]
        vt = dbd_cstype(c)
        ct = cs_base(cf["type"])
        is_arr = cf["type"].endswith("[]")
        # ID conversions
        if is_arr and not c["arr"]:
            continue
        if c["arr"] and not is_arr:
            # special: combine int[2] -> long (RaceMask / AllowableRace)
            if ct == "long":
                lines.append(f"                {cname} = (long)(({c['name']}[0] & 0xFFFFFFFFL) | ((long){c['name']}[1] << 32)),")
            continue
        if c["arr"] and is_arr:
            clen = int(c["arr"])
            tlen = cf["card"] or clen
            if vt == ct and clen == tlen:
                lines.append(f"                {cname} = {c['name']},")
            elif clen == tlen:
                elems = ", ".join(f"({ct}){c['name']}[{i}]" for i in range(tlen))
                lines.append(f"                {cname} = new {ct}[] {{ {elems} }},")
            else:
                copys.append((c["name"], cname, min(clen, tlen)))
            continue
        # scalars
        if vt == ct:
            lines.append(f"                {cname} = {c['name']},")
        else:
            lines.append(f"                {cname} = ({ct}){c['name']},")
    lines.append("            };")
    for src, dst, n in copys:
        lines.append(f"            System.Array.Copy({src}, e.{dst}, {n});")
    lines.append("            return e;")
    lines.append("        }")
    lines.append("    }\n}\n")
    path = os.path.join(outdir, f"{table}Entry{ver}.cs")
    open(path, "w", newline="", encoding="utf-8").write("\r\n".join(lines))

TABLES = """AreaGroupMember AreaTable ContentTuning ContentTuningXExpected CraftingData
Difficulty ExpectedStat ExpectedStatMod Map MapDifficulty OverrideSpellData ScreenEffect
SpellCastTimes SpellCategory SpellDuration SpellRadius SpellRange RandPropPoints
SkillLineAbility SkillLine Spell SpellName SpellMisc SpellEffect SpellTargetRestrictions
SpellXSpellVisual SpellScaling SpellAuraOptions SpellProcsPerMinute SpellAuraRestrictions
SpellCategories SpellCastingRequirements SpellClassOptions SpellCooldowns SpellInterrupts
SpellEquippedItems SpellLabel SpellLevels SpellPower SpellReagents SpellReagentsCurrency
SpellShapeshift SpellTotems SpellXDescriptionVariables SpellDescriptionVariables
ItemEffect ItemSparse ItemXItemEffect""".split()

VERSIONS = {"V115": "1.15.9.69722", "V160": "1.60.1.70009"}

def main():
    for ver, build in VERSIONS.items():
        outdir = os.path.join(SRC, ver)
        os.makedirs(outdir, exist_ok=True)
        for t in TABLES:
            p = os.path.join(DEFS, t + ".dbd")
            if not os.path.exists(p):
                continue
            cols, blocks = parse_dbd(p)
            h, cl = layout_for(cols, blocks, build)
            if cl is None:
                fp = os.path.join(outdir, f"{t}Entry{ver}.cs")
                if os.path.exists(fp): os.remove(fp)
                print(f"{ver} {t}: no layout"); continue
            cf = canon_file(t)
            cls, cflds = parse_cs_struct(cf)
            gen_struct(t, ver, cl, cls, cflds, outdir)
            print(f"{ver} {t}: generated ({len(cl)} cols)")

if __name__ == "__main__":
    main()
