#!/usr/bin/env python3
"""Author a static 12-button LevelsGrid under Difficulty in MainMenu.unity."""
from __future__ import annotations

import re
from dataclasses import dataclass
from typing import Dict, List, Optional, Tuple

SCENE = "/Users/macbookpro162019/Documents/GitHub/ZombieDefence/Assets/Scenes/MainMenu.unity"
EASY_GO = "1890642625"
DIFFICULTY_GO = "147932804"
DIFFICULTY_RT = "147932805"
DIFFICULTY_PANEL = "147932808"
OLD_LEVEL_BUTTON_TFS = ["1890642626", "1500365723", "26365521"]  # Easy, Mid, Hard — deactivate
SURVIVE_TF = "1450974391"
GRID_LAYOUT_GUID = "8a8695521f0d02e499659fee002a26c2"
LEAN_GUID = "dbf51be86e70d7941bd40914206e4ddc"
TMP_GUID = "f4688fdb7df04437aeb418b961361dc5"
DIFF_BTN_GUID = "ce13d9f1b237ef34ba39cdb792e413e4"

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
    by_id: Dict[str, Doc]

    def rebuild(self) -> None:
        self.by_id = {d.file_id: d for d in self.docs}


def parse(path: str) -> UnityFile:
    text = open(path, encoding="utf-8").read()
    if not text.endswith("\n"):
        text += "\n"
    parts = DOC_SPLIT.split(text)
    docs = []
    for part in parts[1:]:
        lines = part.split("\n", 1)
        m = HEADER_RE.match(lines[0])
        if not m:
            continue
        body = lines[1] if len(lines) > 1 else ""
        if body and not body.endswith("\n"):
            body += "\n"
        docs.append(Doc(m.group(1), m.group(2), m.group(3), body))
    uf = UnityFile(parts[0], docs, {})
    uf.rebuild()
    return uf


def write(path: str, uf: UnityFile) -> None:
    with open(path, "w", encoding="utf-8", newline="\n") as f:
        f.write(uf.preamble)
        for d in uf.docs:
            raw = d.raw()
            if not raw.endswith("\n"):
                raw += "\n"
            f.write(raw)


def get_field(body: str, name: str) -> Optional[str]:
    m = re.search(rf"(?m)^\s*{re.escape(name)}:\s*(.*)$", body)
    return m.group(1).strip() if m else None


def set_field(body: str, name: str, value: str) -> str:
    pattern = rf"(?m)^(\s*{re.escape(name)}:\s*).*$"
    if re.search(pattern, body):
        return re.sub(pattern, rf"\g<1>{value}", body, count=1)
    return body


def is_go(doc: Doc) -> bool:
    return doc.body.lstrip().startswith("GameObject:")


def is_tf(doc: Doc) -> bool:
    b = doc.body.lstrip()
    return b.startswith("Transform:") or b.startswith("RectTransform:")


def go_comps(doc: Doc) -> List[str]:
    return re.findall(r"component:\s*\{fileID:\s*(-?\d+)\}", doc.body)


def tf_go(doc: Doc) -> Optional[str]:
    m = re.search(r"m_GameObject:\s*\{fileID:\s*(-?\d+)\}", doc.body)
    return m.group(1) if m else None


def tf_father(doc: Doc) -> Optional[str]:
    m = re.search(r"m_Father:\s*\{fileID:\s*(-?\d+)\}", doc.body)
    return m.group(1) if m else None


def tf_children(doc: Doc) -> List[str]:
    m = re.search(r"m_Children:\n((?:  - \{fileID:.*\n)*)", doc.body)
    if not m:
        return []
    return re.findall(r"fileID:\s*(-?\d+)", m.group(1))


def set_children(doc: Doc, children: List[str]) -> None:
    if not children:
        block = "m_Children: []\n"
    else:
        block = "m_Children:\n" + "".join(f"  - {{fileID: {c}}}\n" for c in children)
    if re.search(r"m_Children:\n(?:  - \{fileID:.*\n)*", doc.body):
        doc.body = re.sub(r"m_Children:\n(?:  - \{fileID:.*\n)*", block, doc.body, count=1)
    elif "m_Children: []\n" in doc.body:
        doc.body = doc.body.replace("m_Children: []\n", block, 1)
    else:
        raise RuntimeError("no m_Children")


def maps(uf: UnityFile):
    gos = {d.file_id: d for d in uf.docs if is_go(d)}
    tfs = {d.file_id: d for d in uf.docs if is_tf(d)}
    go_to_tf = {tf_go(t): tid for tid, t in tfs.items() if tf_go(t)}
    return gos, tfs, go_to_tf


def collect_subtree(uf: UnityFile, root_go: str) -> List[str]:
    gos, tfs, go_to_tf = maps(uf)
    ordered: List[str] = []
    seen = set()

    def add(fid: str):
        if fid and fid != "0" and fid not in seen and fid in uf.by_id:
            seen.add(fid)
            ordered.append(fid)

    def walk(goid: str):
        add(goid)
        for cid in go_comps(gos[goid]):
            add(cid)
        tfid = go_to_tf.get(goid)
        if not tfid:
            return
        add(tfid)
        for child_tf in tf_children(tfs[tfid]):
            if child_tf in tfs:
                child_go = tf_go(tfs[child_tf])
                if child_go:
                    walk(child_go)

    walk(root_go)
    return ordered


class IdGen:
    def __init__(self, start: int):
        self.n = start

    def next(self) -> str:
        self.n += 1
        return str(self.n)


def clone_subtree(uf: UnityFile, root_go: str, id_gen: IdGen, new_name: str) -> Tuple[str, str, Dict[str, str]]:
    ids = collect_subtree(uf, root_go)
    mapping = {old: id_gen.next() for old in ids}
    gos, tfs, go_to_tf = maps(uf)
    new_docs = []
    for old_id in ids:
        src = uf.by_id[old_id]
        body = src.body

        def repl(m):
            old = m.group(1)
            return m.group(0).replace(old, mapping[old], 1) if old in mapping else m.group(0)

        body = re.sub(r"\{fileID:\s*(-?\d+)\}", repl, body)
        doc = Doc(src.type_id, mapping[old_id], src.suffix, body)
        if is_go(doc) and old_id == root_go:
            doc.body = set_field(doc.body, "m_Name", new_name)
        new_docs.append(doc)

    last_idx = max(uf.docs.index(uf.by_id[i]) for i in ids)
    for i, nd in enumerate(new_docs):
        uf.docs.insert(last_idx + 1 + i, nd)
    uf.rebuild()
    return mapping[root_go], mapping[go_to_tf[root_go]], mapping


def find_diff_btn(uf: UnityFile, go_id: str) -> Doc:
    for cid in go_comps(uf.by_id[go_id]):
        doc = uf.by_id[cid]
        if DIFF_BTN_GUID in doc.body:
            return doc
    raise RuntimeError("no DifficultyButton")


def find_button(uf: UnityFile, go_id: str) -> Doc:
    for cid in go_comps(uf.by_id[go_id]):
        doc = uf.by_id[cid]
        if "m_OnClick:" in doc.body and "SelectDifficulty" in doc.body or (
            "m_OnClick:" in doc.body and "m_MethodName:" in doc.body
        ):
            if "m_OnClick:" in doc.body:
                return doc
    for cid in go_comps(uf.by_id[go_id]):
        doc = uf.by_id[cid]
        if "m_OnClick:" in doc.body:
            return doc
    raise RuntimeError("no Button")


def set_select_difficulty(button_doc: Doc, level_index: int) -> None:
    button_doc.body = re.sub(
        r"m_MethodName:.*",
        "m_MethodName: SelectDifficulty",
        button_doc.body,
        count=1,
    )
    button_doc.body = re.sub(
        r"m_IntArgument:\s*-?\d+",
        f"m_IntArgument: {level_index}",
        button_doc.body,
        count=1,
    )
    # Ensure target is DifficultyPanel
    button_doc.body = re.sub(
        r"(m_Target:\s*\{fileID:\s*)-?\d+",
        rf"\g<1>{DIFFICULTY_PANEL}",
        button_doc.body,
        count=1,
    )


def disable_lean_and_set_text(uf: UnityFile, go_id: str, label: str) -> None:
    gos, tfs, go_to_tf = maps(uf)
    ids = collect_subtree(uf, go_id)
    for fid in ids:
        doc = uf.by_id[fid]
        if LEAN_GUID in doc.body:
            doc.body = set_field(doc.body, "m_Enabled", "0")
        if TMP_GUID in doc.body and "m_text:" in doc.body:
            # only the main label TMP under this button tree — set all TMP texts that look like labels
            doc.body = set_field(doc.body, "m_text", label)
            # also update m_text without quotes issues
            doc.body = re.sub(r'(?m)^(\s*m_text:\s*).*$', rf'\g<1>{label}', doc.body, count=1)


def next_free_id_start(uf: UnityFile) -> int:
    used = {int(d.file_id) for d in uf.docs if d.file_id.lstrip("-").isdigit()}
    # Prefer a compact positive range unused by typical scene IDs.
    start = 910_000_001
    while start in used:
        start += 1
    return start - 1  # IdGen.next() increments first


def main():
    uf = parse(SCENE)
    for d in uf.docs:
        if is_go(d) and get_field(d.body, "m_Name") == "LevelsGrid":
            print("LevelsGrid already exists — abort")
            return

    id_gen = IdGen(next_free_id_start(uf))

    # Create LevelsGrid GO + RectTransform + GridLayoutGroup
    grid_go = id_gen.next()
    grid_rt = id_gen.next()
    grid_layout = id_gen.next()

    grid_go_doc = Doc(
        "1",
        grid_go,
        "",
        f"""GameObject:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  serializedVersion: 6
  m_Component:
  - component: {{fileID: {grid_rt}}}
  - component: {{fileID: {grid_layout}}}
  m_Layer: 5
  m_Name: LevelsGrid
  m_TagString: Untagged
  m_Icon: {{fileID: 0}}
  m_NavMeshLayer: 0
  m_StaticEditorFlags: 0
  m_IsActive: 1
""",
    )
    grid_rt_doc = Doc(
        "224",
        grid_rt,
        "",
        f"""RectTransform:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {grid_go}}}
  m_LocalRotation: {{x: 0, y: 0, z: 0, w: 1}}
  m_LocalPosition: {{x: 0, y: 0, z: 0}}
  m_LocalScale: {{x: 1, y: 1, z: 1}}
  m_ConstrainProportionsScale: 0
  m_Children: []
  m_Father: {{fileID: {DIFFICULTY_RT}}}
  m_RootOrder: 1
  m_LocalEulerAnglesHint: {{x: 0, y: 0, z: 0}}
  m_AnchorMin: {{x: 0.5, y: 0.5}}
  m_AnchorMax: {{x: 0.5, y: 0.5}}
  m_AnchoredPosition: {{x: 0, y: 40}}
  m_SizeDelta: {{x: 900, y: 360}}
  m_Pivot: {{x: 0.5, y: 0.5}}
""",
    )
    grid_layout_doc = Doc(
        "114",
        grid_layout,
        "",
        f"""MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: {grid_go}}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: {GRID_LAYOUT_GUID}, type: 3}}
  m_Name: 
  m_EditorClassIdentifier: 
  m_Padding:
    m_Left: 0
    m_Right: 0
    m_Top: 0
    m_Bottom: 0
  m_ChildAlignment: 4
  m_StartCorner: 0
  m_StartAxis: 0
  m_CellSize: {{x: 200, y: 90}}
  m_Spacing: {{x: 16, y: 16}}
  m_Constraint: 1
  m_ConstraintCount: 4
""",
    )

    # Insert after Difficulty panel docs
    insert_at = uf.docs.index(uf.by_id[DIFFICULTY_PANEL]) + 1
    for i, doc in enumerate([grid_go_doc, grid_rt_doc, grid_layout_doc]):
        uf.docs.insert(insert_at + i, doc)
    uf.rebuild()

    # Attach LevelsGrid under Difficulty (after BackGround)
    diff_rt = uf.by_id[DIFFICULTY_RT]
    children = tf_children(diff_rt)
    # insert grid after background if present
    if children and children[0]:
        children = [children[0], grid_rt] + [c for c in children[1:] if c != grid_rt]
    else:
        children = [grid_rt] + children
    set_children(diff_rt, children)

    # Deactivate old Easy/Mid/Hard
    gos, tfs, go_to_tf = maps(uf)
    for tfid in OLD_LEVEL_BUTTON_TFS:
        goid = tf_go(uf.by_id[tfid])
        uf.by_id[goid].body = set_field(uf.by_id[goid].body, "m_IsActive", "0")

    # Clone Easy into 12 level buttons
    level_btn_components: List[str] = []
    level_tfs: List[str] = []
    for level in range(1, 13):
        new_go, new_tf, mapping = clone_subtree(uf, EASY_GO, id_gen, f"Level ({level})")
        uf.by_id[new_go].body = set_field(uf.by_id[new_go].body, "m_IsActive", "1")
        # parent under grid
        uf.by_id[new_tf].body = set_field(uf.by_id[new_tf].body, "m_Father", f"{{fileID: {grid_rt}}}")
        uf.by_id[new_tf].body = set_field(uf.by_id[new_tf].body, "m_RootOrder", str(level - 1))
        # size hint for layout element (grid drives size)
        uf.by_id[new_tf].body = set_field(uf.by_id[new_tf].body, "m_AnchoredPosition", "{x: 0, y: 0}")
        uf.by_id[new_tf].body = set_field(uf.by_id[new_tf].body, "m_SizeDelta", "{x: 200, y: 90}")

        btn = find_button(uf, new_go)
        set_select_difficulty(btn, level - 1)
        disable_lean_and_set_text(uf, new_go, str(level))

        diff_btn = find_diff_btn(uf, new_go)
        level_btn_components.append(diff_btn.file_id)
        level_tfs.append(new_tf)
        print(f"Created Level ({level}) go={new_go} diffBtn={diff_btn.file_id}")

    set_children(uf.by_id[grid_rt], level_tfs)

    # Update DifficultyPanel._difficultButtons list; drop template fields if present
    panel = uf.by_id[DIFFICULTY_PANEL]
    buttons_block = "_difficultButtons:\n" + "".join(
        f"  - {{fileID: {fid}}}\n" for fid in level_btn_components
    )
    if re.search(r"_difficultButtons:\n(?:  - \{fileID:.*\n)*", panel.body):
        panel.body = re.sub(
            r"_difficultButtons:\n(?:  - \{fileID:.*\n)*",
            buttons_block,
            panel.body,
            count=1,
        )
    else:
        panel.body += buttons_block

    # Remove obsolete runtime fields if they were serialized
    for obsolete in ("_buttonTemplate:", "_buttonsRoot:", "_levelsCount:", "_buttonsBuilt:"):
        panel.body = re.sub(rf"(?m)^\s*{re.escape(obsolete)}.*\n", "", panel.body)

    write(SCENE, uf)
    print("Wrote", SCENE)
    print("LevelsGrid children:", len(level_tfs))
    print("DifficultyPanel buttons:", len(level_btn_components))


if __name__ == "__main__":
    main()
