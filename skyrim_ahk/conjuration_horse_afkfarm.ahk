; Press F6 to start / stop (displaytweaks only, if not you have to alttab [fuck you bethesda])
; This script can be used on anything magic in skyrim or outside of skyrim but idk where to put it
F6:: {
    static toggle := false
    toggle := !toggle

    while toggle {
        Click "Down"    ; hold left mouse button
        Sleep 750       ; 0.75 seconds
        Click "Up"      ; release left mouse button
        Sleep 1250      ; 1.25 seconds
    }
}
