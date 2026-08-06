#!/usr/bin/env python3
"""Expand each location's 3 LevelWaves into 12 (anchors at 1/6/12)."""
from __future__ import annotations

import copy
import math
import re
import sys
from dataclasses import dataclass, field
from typing import Dict, List, Optional, Tuple

WAVE_GUID = "005fe10a9bdd57b49a21018c954d3a7e"
LEVEL_WAVES_GUID = "5050cbd524c6ac347ab8f98b29f31a1d"

DOC_SPLIT = re.compile(r"(?=^--- !u!\d+ &)", re.M)
HEADER_RE = re.compile(r"^--- !u!(\d+) &(-?\d+)(.*)$", re.M)


@dataclass
class Doc:
    type_id: str
    file_id: str
    suffix: str
    body: str

    def raw(self) -> str:
        return f"--- !u!{self.type_id} &{self.file_id}{self.suffix}\n{self.body}"


@dataclass
class UnityFile:
    preamble: str
    docs: List[Doc]
    by_id: Dict[str, Doc] = field(default_factory=dict)

    def rebuild_index(self) -> None:
        self.by_id = {d.file_id: d for d in self.docs}


def parse_unity(path: str) -> UnityFile:
    with open(path, encoding="utf-8") as f:
        text = f.read()
    if not text.endswith("\n"):
        text += "\n"
    parts = DOC_SPLIT.split(text)
    preamble = parts[0]
    docs: List[Doc] = []
    for part in parts[1:]:
        lines = part.split("\n", 1)
        m = HEADER_RE.match(lines[0])
        if not m:
            continue
        body = lines[1] if len(lines) > 1 else ""
        if body and not body.endswith("\n"):
            body += "\n"
        docs.append(Doc(m.group(1), m.group(2), m.group(3), body))
    uf = UnityFile(preamble, docs)
    uf.rebuild_index()
    return uf


def write_unity(path: str, uf: UnityFile) -> None:
    chunks = [uf.preamble]
    for d in uf.docs:
        raw = d.raw()
        if not raw.endswith("\n"):
            raw += "\n"
        chunks.append(raw)
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write("".join(chunks))


def get_field(body: str, name: str) -> Optional[str]:
    m = re.search(rf"(?m)^\s*{re.escape(name)}:\s*(.*)$", body)
    return m.group(1).strip() if m else None


def set_field(body: str, name: str, value: str) -> str:
    pattern = rf"(?m)^(\s*{re.escape(name)}:\s*).*$"
    if re.search(pattern, body):
        return re.sub(pattern, rf"\g<1>{value}", body, count=1)
    return body


def is_gameobject(doc: Doc) -> bool:
    return doc.body.lstrip().startswith("GameObject:")


def is_transform(doc: Doc) -> bool:
    b = doc.body.lstrip()
    return b.startswith("Transform:") or b.startswith("RectTransform:")


def go_name(doc: Doc) -> str:
    return (get_field(doc.body, "m_Name") or "?").strip()


def go_components(doc: Doc) -> List[str]:
    return re.findall(r"component:\s*\{fileID:\s*(-?\d+)\}", doc.body)


def transform_go(doc: Doc) -> Optional[str]:
    return re.search(r"m_GameObject:\s*\{fileID:\s*(-?\d+)\}", doc.body).group(1) if re.search(
        r"m_GameObject:\s*\{fileID:\s*(-?\d+)\}", doc.body
    ) else None


def transform_father(doc: Doc) -> Optional[str]:
    m = re.search(r"m_Father:\s*\{fileID:\s*(-?\d+)\}", doc.body)
    return m.group(1) if m else None


def transform_children(doc: Doc) -> List[str]:
    m = re.search(r"m_Children:\n((?:  - \{fileID:.*\n)*)", doc.body)
    if not m:
        return []
    return re.findall(r"fileID:\s*(-?\d+)", m.group(1))


def set_transform_children(doc: Doc, children: List[str]) -> None:
    block = "m_Children:\n"
    if not children:
        block = "m_Children: []\n"
    else:
        for cid in children:
            block += f"  - {{fileID: {cid}}}\n"
    if re.search(r"m_Children:\n(?:  - \{fileID:.*\n)*", doc.body):
        doc.body = re.sub(r"m_Children:\n(?:  - \{fileID:.*\n)*", block, doc.body, count=1)
    elif "m_Children: []\n" in doc.body:
        doc.body = doc.body.replace("m_Children: []\n", block, 1)
    else:
        raise RuntimeError(f"Cannot set children on {doc.file_id}")


def set_transform_father(doc: Doc, father_id: str) -> None:
    doc.body = re.sub(
        r"m_Father:\s*\{fileID:\s*-?\d+\}",
        f"m_Father: {{fileID: {father_id}}}",
        doc.body,
        count=1,
    )


def build_maps(uf: UnityFile):
    gos = {d.file_id: d for d in uf.docs if is_gameobject(d)}
    tfs = {d.file_id: d for d in uf.docs if is_transform(d)}
    go_to_tf = {}
    for tid, tdoc in tfs.items():
        gid = transform_go(tdoc)
        if gid:
            go_to_tf[gid] = tid
    return gos, tfs, go_to_tf


def path_of(goid: str, gos, tfs, go_to_tf) -> str:
    names = []
    cur = goid
    for _ in range(30):
        if not cur or cur not in gos:
            break
        names.append(go_name(gos[cur]))
        tf = go_to_tf.get(cur)
        if not tf:
            break
        father = transform_father(tfs[tf])
        if not father or father == "0":
            break
        cur = transform_go(tfs[father]) if father in tfs else None
    return "/".join(reversed(names))


def collect_subtree_ids(root_go: str, uf: UnityFile, gos, tfs, go_to_tf) -> List[str]:
    """All fileIDs belonging to root_go and descendants (GOs, transforms, components)."""
    ordered: List[str] = []
    seen = set()

    def add(fid: str):
        if fid and fid != "0" and fid not in seen and fid in uf.by_id:
            seen.add(fid)
            ordered.append(fid)

    def walk_go(goid: str):
        add(goid)
        go = gos[goid]
        for cid in go_components(go):
            add(cid)
        tfid = go_to_tf.get(goid)
        if not tfid:
            return
        # transform already added via components usually, but ensure
        add(tfid)
        for child_tf in transform_children(tfs[tfid]):
            if child_tf in tfs:
                child_go = transform_go(tfs[child_tf])
                if child_go:
                    walk_go(child_go)

    walk_go(root_go)
    return ordered


class IdGen:
    def __init__(self, start: int):
        self._n = start

    def next(self) -> str:
        self._n += 1
        return str(self._n)


def clone_subtree(uf: UnityFile, root_go: str, id_gen: IdGen, new_name: Optional[str] = None) -> Tuple[str, str, Dict[str, str]]:
    """Clone subtree; returns (new_go_id, new_tf_id, old->new map). Does not attach to parent."""
    gos, tfs, go_to_tf = build_maps(uf)
    ids = collect_subtree_ids(root_go, uf, gos, tfs, go_to_tf)
    mapping = {old: id_gen.next() for old in ids}

    new_docs: List[Doc] = []
    for old_id in ids:
        src = uf.by_id[old_id]
        new_id = mapping[old_id]
        body = src.body

        def repl_fileid(m):
            old = m.group(1)
            if old in mapping:
                return m.group(0).replace(old, mapping[old], 1)
            return m.group(0)

        body = re.sub(r"\{fileID:\s*(-?\d+)\}", repl_fileid, body)
        # also component list style already covered
        doc = Doc(src.type_id, new_id, src.suffix, body)
        if is_gameobject(doc) and old_id == root_go and new_name:
            doc.body = set_field(doc.body, "m_Name", new_name)
        new_docs.append(doc)

    # Insert after original subtree's last doc
    last_idx = max(uf.docs.index(uf.by_id[i]) for i in ids)
    for i, nd in enumerate(new_docs):
        uf.docs.insert(last_idx + 1 + i, nd)
    uf.rebuild_index()

    new_go = mapping[root_go]
    new_tf = mapping[go_to_tf[root_go]]
    return new_go, new_tf, mapping


def find_level_gos(uf: UnityFile, parent_path_contains: str) -> Dict[int, str]:
    gos, tfs, go_to_tf = build_maps(uf)
    result = {}
    for gid, go in gos.items():
        name = go_name(go)
        m = re.fullmatch(r"Level \((\d+)\)", name)
        if not m:
            continue
        p = path_of(gid, gos, tfs, go_to_tf)
        if parent_path_contains not in p:
            continue
        if "Survival" in p:
            continue
        result[int(m.group(1))] = gid
    return result


def get_levelwaves_doc(uf: UnityFile, level_go: str) -> Doc:
    go = uf.by_id[level_go]
    for cid in go_components(go):
        doc = uf.by_id[cid]
        if LEVEL_WAVES_GUID in doc.body:
            return doc
    raise RuntimeError(f"No LevelWaves on {level_go}")


def get_wave_docs(uf: UnityFile, level_go: str) -> List[Doc]:
    gos, tfs, go_to_tf = build_maps(uf)
    ids = collect_subtree_ids(level_go, uf, gos, tfs, go_to_tf)
    waves = []
    for fid in ids:
        doc = uf.by_id[fid]
        if WAVE_GUID in doc.body:
            waves.append(doc)
    # sort by gameobject name for stability
    def wave_key(d: Doc):
        m = re.search(r"m_GameObject:\s*\{fileID:\s*(-?\d+)\}", d.body)
        if not m:
            return d.file_id
        return go_name(uf.by_id[m.group(1)])

    waves.sort(key=wave_key)
    return waves


def lerp(a: float, b: float, t: float) -> float:
    return a + (b - a) * t


def blend_level(uf: UnityFile, level_go: str, low_go: str, high_go: str, t: float, bonus: int) -> None:
    lw = get_levelwaves_doc(uf, level_go)
    lw.body = set_field(lw.body, "_levelBonus", str(bonus))

    src_waves = get_wave_docs(uf, level_go)
    low_waves = get_wave_docs(uf, low_go)
    high_waves = get_wave_docs(uf, high_go)

    low_total = sum(int(get_field(w.body, "ZombieCount") or "0") for w in low_waves)
    high_total = sum(int(get_field(w.body, "ZombieCount") or "0") for w in high_waves)
    target_total = max(1.0, lerp(low_total, high_total, t))

    blended_counts = []
    for i, wdoc in enumerate(src_waves):
        low = low_waves[min(i, len(low_waves) - 1)]
        high = high_waves[min(i, len(high_waves) - 1)]

        low_z = int(get_field(low.body, "ZombieCount") or "5")
        high_z = int(get_field(high.body, "ZombieCount") or "5")
        low_dbs = float(get_field(low.body, "DelayBetweenSpawn") or "1")
        high_dbs = float(get_field(high.body, "DelayBetweenSpawn") or "1")
        low_daw = float(get_field(low.body, "DelayAfterWave") or "1")
        high_daw = float(get_field(high.body, "DelayAfterWave") or "1")

        z = max(1, int(round(lerp(low_z, high_z, t))))
        blended_counts.append(z)
        dbs = round(lerp(low_dbs, high_dbs, t), 2)
        daw = round(lerp(low_daw, high_daw, t), 2)

        wdoc.body = set_field(wdoc.body, "DelayBetweenSpawn", str(dbs))
        wdoc.body = set_field(wdoc.body, "DelayAfterWave", str(daw))

        if t >= 0.5:
            high_guid = None
            high_file = None
            lines = high.body.split("\n")
            for line_i, line in enumerate(lines):
                if "EnemyTemplate:" in line:
                    chunk = "\n".join(lines[line_i : line_i + 5])
                    gm = re.search(r"guid:\s*(\w+)", chunk)
                    fm = re.search(r"fileID:\s*(-?\d+)", chunk)
                    if gm:
                        high_guid = gm.group(1)
                    if fm:
                        high_file = fm.group(1)
                    break
            if high_guid:
                wdoc.body = re.sub(
                    r"(EnemyTemplate:[\s\S]*?guid:\s*)\w+",
                    rf"\g<1>{high_guid}",
                    wdoc.body,
                    count=1,
                )
            elif high_file and high_file != "0":
                wdoc.body = re.sub(
                    r"(EnemyTemplate:\s*\{fileID:\s*)-?\d+",
                    rf"\g<1>{high_file}",
                    wdoc.body,
                    count=1,
                )

    current_total = sum(blended_counts)
    scale = target_total / current_total if current_total > 0 else 1.0
    for i, wdoc in enumerate(src_waves):
        z = max(1, int(round(blended_counts[i] * scale)))
        wdoc.body = set_field(wdoc.body, "ZombieCount", str(z))


def rename_level(uf: UnityFile, go_id: str, new_number: int) -> None:
    go = uf.by_id[go_id]
    go.body = set_field(go.body, "m_Name", f"Level ({new_number})")


def attach_child(uf: UnityFile, parent_tf: str, child_tf: str, index: int) -> None:
    parent = uf.by_id[parent_tf]
    children = transform_children(parent)
    if child_tf in children:
        children.remove(child_tf)
    index = max(0, min(index, len(children)))
    children.insert(index, child_tf)
    set_transform_children(parent, children)
    set_transform_father(uf.by_id[child_tf], parent_tf)
    # root order
    uf.by_id[child_tf].body = set_field(uf.by_id[child_tf].body, "m_RootOrder", str(index))


def reorder_level_children(uf: UnityFile, parent_tf: str, level_tf_by_number: Dict[int, str]) -> None:
    parent = uf.by_id[parent_tf]
    children = transform_children(parent)
    # keep non-level children in their relative order after levels
    level_tfs = set(level_tf_by_number.values())
    others = [c for c in children if c not in level_tfs]
    new_children = [level_tf_by_number[n] for n in range(1, 13) if n in level_tf_by_number] + others
    set_transform_children(parent, new_children)
    for i, cid in enumerate(new_children):
        if cid in uf.by_id:
            uf.by_id[cid].body = set_field(uf.by_id[cid].body, "m_RootOrder", str(i))


def expand_location(uf: UnityFile, path_key: str, id_gen: IdGen, bonuses: Tuple[int, int, int]) -> None:
    gos, tfs, go_to_tf = build_maps(uf)
    levels = find_level_gos(uf, path_key)
    if set(levels.keys()) != {1, 2, 3}:
        # maybe already expanded
        if set(levels.keys()) >= set(range(1, 13)):
            print(f"  skip {path_key}: already has 12 levels")
            return
        raise RuntimeError(f"{path_key}: expected levels 1,2,3 got {sorted(levels)}")

    print(f"  expand {path_key}: {sorted(levels)}")
    go1, go2, go3 = levels[1], levels[2], levels[3]
    tf1, tf2, tf3 = go_to_tf[go1], go_to_tf[go2], go_to_tf[go3]
    parent_tf = transform_father(tfs[tf1])

    # Rename anchors
    rename_level(uf, go2, 6)
    rename_level(uf, go3, 12)
    uf.rebuild_index()
    gos, tfs, go_to_tf = build_maps(uf)

    b1, b6, b12 = bonuses
    # Ensure anchor bonuses
    get_levelwaves_doc(uf, go1).body = set_field(get_levelwaves_doc(uf, go1).body, "_levelBonus", str(b1))
    get_levelwaves_doc(uf, go2).body = set_field(get_levelwaves_doc(uf, go2).body, "_levelBonus", str(b6))
    get_levelwaves_doc(uf, go3).body = set_field(get_levelwaves_doc(uf, go3).body, "_levelBonus", str(b12))

    level_tf = {1: tf1, 6: tf2, 12: tf3}
    level_go = {1: go1, 6: go2, 12: go3}

    # Levels 2-5 from L1 toward L6
    for n, t in zip([2, 3, 4, 5], [0.2, 0.4, 0.6, 0.8]):
        bonus = int(round(lerp(b1, b6, t)))
        new_go, new_tf, _ = clone_subtree(uf, go1, id_gen, f"Level ({n})")
        blend_level(uf, new_go, go1, go2, t, bonus)
        level_tf[n] = new_tf
        level_go[n] = new_go
        attach_child(uf, parent_tf, new_tf, n - 1)

    # Levels 7-11 from L6 toward L12
    for n, t in zip([7, 8, 9, 10, 11], [0.2, 0.4, 0.6, 0.8, 0.9]):
        bonus = int(round(lerp(b6, b12, t)))
        new_go, new_tf, _ = clone_subtree(uf, go2, id_gen, f"Level ({n})")
        blend_level(uf, new_go, go2, go3, t, bonus)
        level_tf[n] = new_tf
        level_go[n] = new_go
        attach_child(uf, parent_tf, new_tf, n - 1)

    # deactivate non-selected levels by default (inactive like originals)
    for n, gid in level_go.items():
        if n != 1:
            # keep inactive - originals 2,3 were inactive
            go = uf.by_id[gid]
            go.body = set_field(go.body, "m_IsActive", "0")

    reorder_level_children(uf, parent_tf, level_tf)
    print(f"  done {path_key}: levels {sorted(level_go)}")


def max_file_id(uf: UnityFile) -> int:
    mx = 0
    for d in uf.docs:
        try:
            v = int(d.file_id)
            if v > mx:
                mx = v
        except ValueError:
            pass
    return mx


def main():
    jobs = [
        (
            "/Users/macbookpro162019/Documents/GitHub/ZombieDefence/Assets/Prefabs/Levels.prefab",
            [("Levels/", (500, 1000, 2000))],
        ),
        (
            "/Users/macbookpro162019/Documents/GitHub/ZombieDefence/Assets/Scenes/CrossRoad +.unity",
            [
                ("LevelChoicer/Parking/", (1000, 1200, 1400)),
                ("LevelChoicer/CrossRoad T/", (1000, 1200, 1500)),
                ("LevelChoicer/TrailerPark/", (1000, 1200, 1400)),
            ],
        ),
    ]

    for path, locations in jobs:
        print("FILE", path)
        uf = parse_unity(path)
        id_gen = IdGen(max(max_file_id(uf), 9_000_000_000_000_000_000))
        for path_key, bonuses in locations:
            expand_location(uf, path_key, id_gen, bonuses)
        write_unity(path, uf)
        print("Wrote", path)


if __name__ == "__main__":
    main()
