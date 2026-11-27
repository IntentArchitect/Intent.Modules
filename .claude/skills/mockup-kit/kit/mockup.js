// Mockup kit: theme switching, clean-screenshot mode and annotation pins for a static HTML mockup (copied to
// intent/.mockups/<app>/kit/; never edit the copy). Standalone: ?theme=dark|light, ?pins=off, Alt+T. Embedded in
// Intent's <Mockup> frame: reads window.intentMockup, takes {source:"intent-mockup",type:"theme"|"pins"|"comment-mode"|
// "comment-markers"} messages and posts pin clicks back as {type:"pin",requirement}, comment-mode clicks as {type:"point"}, Esc as {type:"escape"},
// Ctrl+Enter as {type:"submit"}, Ctrl+wheel as {type:"zoom",in,deltaY,x,y}, any other wheel as {type:"wheel"} and the hovered pin's note as {type:"pin-hover",note,rect} (no note when it leaves).
// While the viewer can pan ({type:"pan-available"}), Space goes to it as {type:"pan-key",down}, a Ctrl/Cmd or middle-button press
// as {type:"pan-drag",phase,x,y} (ended by {type:"pan-end"}), and {type:"scroll-by"} scrolls the page.
// Sets --mk-notes-space to the room the notes panel takes (0 with pins off) for the screen to leave free.
// <meta name="mockup-theme" content="html.dark"> says how dark is shown.
(function () {
    const SOURCE = "intent-mockup";
    const STORAGE_KEY = "mockup-theme";
    const embed = window.intentMockup && window.intentMockup.embedded ? window.intentMockup : null;
    const params = new URLSearchParams(location.search);

    // An opaque-origin frame throws on any storage access, so the remembered theme is best-effort.
    const remembered = {
        get() { try { return localStorage.getItem(STORAGE_KEY); } catch { return null; } },
        set(value) { try { localStorage.setItem(STORAGE_KEY, value); } catch { /* sandboxed */ } },
    };

    function themeRule() {
        const meta = document.querySelector('meta[name="mockup-theme"]');
        const spec = (meta && meta.getAttribute("content") || "body[data-theme=dark]").trim();
        const match = /^(html|body)(?:\.([\w-]+)|\[([\w-]+)(?:=["']?([\w-]+)["']?)?\])$/.exec(spec);
        if (!match) return { element: "body", attribute: "data-theme", value: "dark" };
        return { element: match[1], className: match[2], attribute: match[3], value: match[4] ?? "" };
    }

    let theme = null;
    function applyTheme(next) {
        theme = next === "dark" ? "dark" : "light";
        const rule = themeRule();
        const target = rule.element === "html" ? document.documentElement : document.body;
        if (!target) return; // body not parsed yet; init() applies it again
        if (rule.className) target.classList.toggle(rule.className, theme === "dark");
        else if (theme === "dark") target.setAttribute(rule.attribute, rule.value);
        else target.removeAttribute(rule.attribute);
        document.documentElement.setAttribute("data-mk-theme", theme);
    }

    function setPins(visible) {
        document.documentElement.classList.toggle("mk-pins-off", !visible);
        reserveNotes();
    }

    /** --mk-notes-space: how far the notes panel reaches in from the right edge, so hiding the pins gives the screen the room back. */
    function reserveNotes() {
        const panel = document.querySelector(".mk-notes-panel");
        const left = panel ? panel.getBoundingClientRect().left : 0;
        const space = panel && left > 0 ? Math.max(0, Math.round(window.innerWidth - left)) : 0;
        document.documentElement.style.setProperty("--mk-notes-space", space + "px");
    }
    window.addEventListener("resize", reserveNotes);

    function toggleTheme() {
        const next = theme === "dark" ? "light" : "dark";
        remembered.set(next);
        applyTheme(next);
    }

    /** The requirement or criterion id a pin cites: data-req, else an id leading its title ("R1.2 - Shows unread count"). */
    function citedRequirement(el) {
        const explicit = el.getAttribute("data-req");
        if (explicit) return explicit.trim();
        const match = /^\s*([A-Za-z]{1,6}-?\d+(?:\.\d+)*)\b/.exec(el.getAttribute("title") || el.getAttribute("data-mk-title") || "");
        return match ? match[1] : null;
    }

    // --- Comments (embedded only): a click places a numbered marker and is described for the agent. ---
    let commenting = false;
    let panAvailable = false;
    let hovered = null;

    function setCommenting(on) {
        commenting = !!on;
        document.documentElement.classList.toggle("mk-commenting", commenting);
        if (!commenting && hovered) { hovered.classList.remove("mk-comment-hover"); hovered = null; }
    }

    const clip = (text, max) => { const t = (text || "").replace(/\s+/g, " ").trim(); return t.length > max ? t.slice(0, max - 1) + "…" : t; };

    /** `button.btn-primary "Save"`: a tag, its id or first two classes, and what it says. */
    function describe(el) {
        const tag = el.tagName.toLowerCase();
        const id = el.id ? "#" + el.id : "";
        const classes = id ? "" : Array.from(el.classList).filter((c) => !c.startsWith("mk-")).slice(0, 2).map((c) => "." + c).join("");
        const label = el.getAttribute("aria-label") || el.getAttribute("title") || el.getAttribute("placeholder")
            || (el.children.length <= 3 ? el.textContent : "");
        return tag + id + classes + (clip(label, 60) ? ` "${clip(label, 60)}"` : "");
    }

    /** The clicked element, and the nearest ancestor with an id or class to say where it is. */
    function describeTarget(el) {
        let context = el.parentElement;
        while (context && context !== document.body && !context.id && !Array.from(context.classList).some((c) => !c.startsWith("mk-"))) {
            context = context.parentElement;
        }
        const where = context && context !== document.body ? describe(context).replace(/ ".*$/, "") : "";
        return describe(el) + (where ? ` in ${where}` : "");
    }

    /**
     * Where a click landed: the clicked element's path from the root, what it is (to know it again) and the fraction
     * across and down it, so its marker follows it. Null for an element outside the document tree (a shadow root).
     */
    function anchorOf(el, clientX, clientY) {
        const steps = [];
        for (let node = el; node !== document.documentElement; node = node.parentElement) {
            if (!node || !node.parentElement) return null;
            steps.unshift(node.tagName.toLowerCase() + ":" + Array.prototype.indexOf.call(node.parentElement.children, node));
        }
        const rect = el.getBoundingClientRect();
        const fraction = (offset, size) => (size > 0 ? Math.min(1, Math.max(0, offset / size)) : 0);
        return { path: steps.join("/"), what: describe(el), fx: fraction(clientX - rect.left, rect.width), fy: fraction(clientY - rect.top, rect.height) };
    }

    /** The anchored element, or null once that path holds something else (the mockup changed around it). */
    function elementAt(anchor) {
        let node = document.documentElement;
        for (const step of String(anchor.path).split("/")) {
            const [tag, index] = step.split(":");
            node = node && node.children[Number(index)];
            if (!node || node.tagName.toLowerCase() !== tag) return null;
        }
        return anchor.what && describe(node) !== anchor.what ? null : node;
    }

    /** The marker's point in the viewport; null while its element is hidden or scrolled out of view; undefined with no element. */
    function anchoredPoint(anchor) {
        const el = anchor && elementAt(anchor);
        if (!el) return undefined;
        const rect = el.getBoundingClientRect();
        if (!rect.width && !rect.height) return null;
        const x = rect.left + anchor.fx * rect.width;
        const y = rect.top + anchor.fy * rect.height;
        for (let clip = el.parentElement; clip && clip !== document.body && clip !== document.documentElement; clip = clip.parentElement) {
            const style = getComputedStyle(clip);
            if (style.overflowX === "visible" && style.overflowY === "visible") continue;
            if (panels) panels.observe(clip);
            const box = clip.getBoundingClientRect();
            if (x < box.left || x > box.right || y < box.top || y > box.bottom) return null;
        }
        return { x, y };
    }

    let commentMarkers = [];
    // Content resizing inside a scroll panel moves an element without resizing the body.
    const panels = window.ResizeObserver ? new ResizeObserver(() => queueLayout()) : null;
    let layoutQueued = false;
    // Page coordinates only follow the document's own scroll; a marker on an element is re-placed whenever anything scrolls or resizes.
    function layoutMarkers() {
        layoutQueued = false;
        const layer = document.getElementById("mk-comment-markers");
        if (!layer) return;
        Array.from(layer.children).forEach((marker, i) => {
            const m = commentMarkers[i];
            const point = anchoredPoint(m.anchor);
            // No anchor, or its element is gone (an old comment, a changed mockup): the page point it was placed at.
            const x = point === undefined ? Number(m.x) : point && point.x + window.scrollX;
            const y = point === undefined ? Number(m.y) : point && point.y + window.scrollY;
            marker.style.display = point === null ? "none" : "";
            if (point !== null) { marker.style.left = x + "px"; marker.style.top = y + "px"; }
        });
    }
    function queueLayout() {
        if (layoutQueued || !commentMarkers.length) return;
        layoutQueued = true;
        requestAnimationFrame(layoutMarkers);
    }

    function drawMarkers(markers) {
        let layer = document.getElementById("mk-comment-markers");
        if (!layer) {
            layer = document.createElement("div");
            layer.id = "mk-comment-markers";
            // On the root, not the body, so a positioned body can't offset page coordinates.
            document.documentElement.appendChild(layer);
            document.addEventListener("scroll", queueLayout, true);
            window.addEventListener("resize", queueLayout);
            if (panels) panels.observe(document.body);
        }
        layer.textContent = "";
        commentMarkers = Array.isArray(markers) ? markers : [];
        commentMarkers.forEach((m) => {
            const marker = document.createElement("span");
            marker.className = "mk-comment-marker" + (m.sent ? " mk-comment-marker--sent" : "");
            marker.textContent = String(m.n);
            layer.appendChild(marker);
        });
        layoutMarkers();
    }

    // --- Pin notes (embedded only): the viewer shows the hovered pin's note in its own hover card. ---
    let notedPin = null;

    /** The pin's note from the notes panel, matched by its number; else its title. */
    function pinNote(pin) {
        const number = (pin.textContent || "").trim();
        const item = number ? Array.from(document.querySelectorAll(".mk-notes li[data-pin]")).find((li) => li.getAttribute("data-pin") === number) : null;
        if (item) {
            const heading = item.querySelector("b");
            const body = item.cloneNode(true);
            const bodyHeading = body.querySelector("b");
            if (bodyHeading) bodyHeading.remove();
            return { pin: number, title: clip(heading ? heading.textContent : "", 120), text: clip(body.textContent, 600) };
        }
        const title = pin.getAttribute("data-mk-title") || "";
        return title ? { pin: number, title: "", text: clip(title, 600) } : null;
    }

    function notePin(pin) {
        if (pin === notedPin) return;
        notedPin = pin;
        if (!pin) {
            window.parent.postMessage({ source: SOURCE, type: "pin-hover" }, "*");
            return;
        }
        // The viewer's card replaces the browser's tooltip, which would say the same thing twice.
        if (pin.hasAttribute("title")) {
            pin.setAttribute("data-mk-title", pin.getAttribute("title"));
            pin.removeAttribute("title");
        }
        const r = pin.getBoundingClientRect();
        const note = pinNote(pin);
        if (note) note.color = getComputedStyle(pin).backgroundColor;
        window.parent.postMessage({ source: SOURCE, type: "pin-hover", note, rect: { x: r.left, y: r.top, width: r.width, height: r.height } }, "*");
    }

    if (embed) {
        document.addEventListener("mouseover", (event) => {
            const target = event.target instanceof Element ? event.target : null;
            notePin(!commenting && target ? target.closest(".mk-pin") : null);
        });
        document.documentElement.addEventListener("mouseleave", () => notePin(null));
        // The card is placed where the pin was; once the page moves under it, it would point at nothing.
        document.addEventListener("scroll", () => notePin(null), true);
    }

    function init() {
        applyTheme(embed ? embed.theme : params.get("theme") ?? remembered.get());
        setPins(embed ? embed.pins !== false : params.get("pins") !== "off");
        if (embed) {
            document.documentElement.classList.add("mk-embedded");
            window.parent.postMessage({ source: SOURCE, type: "ready" }, "*");
        }
    }

    // Before the body exists only an html-targeted theme can be applied; the rest waits for the DOM.
    setPins(embed ? embed.pins !== false : params.get("pins") !== "off");
    if (document.readyState === "loading") {
        applyTheme(embed ? embed.theme : params.get("theme") ?? remembered.get());
        document.addEventListener("DOMContentLoaded", init, { once: true });
    } else {
        init();
    }

    window.addEventListener("message", (event) => {
        if (!embed || event.source !== window.parent) return;
        const data = event.data;
        if (!data || data.source !== SOURCE) return;
        if (data.type === "theme") applyTheme(data.theme);
        if (data.type === "pins") setPins(!!data.visible);
        if (data.type === "comment-mode") setCommenting(data.on);
        if (data.type === "comment-markers" && document.body) drawMarkers(data.markers);
        if (data.type === "pan-available") panAvailable = !!data.on;
        if (data.type === "scroll-by" && data.at) scrollAt(data.at, Number(data.x) || 0, Number(data.y) || 0);
        if (data.type === "pan-end") endPanDrag(null);
    });

    // Capture phase, so in comment mode a click never reaches the mockup's own toggles or its pins.
    // The frame is scaled by the viewer, which zooms it; Ctrl+wheel here never reaches the viewer as a wheel.
    if (embed) {
        document.addEventListener("wheel", (event) => {
            // Told, not forwarded: the page scrolls on its own once the frame can't, but never sees the wheel.
            if (!event.ctrlKey && !event.metaKey) return window.parent.postMessage({ source: SOURCE, type: "wheel" }, "*");
            event.preventDefault();
            window.parent.postMessage({ source: SOURCE, type: "zoom", in: event.deltaY < 0, deltaY: event.deltaY, x: event.clientX, y: event.clientY }, "*");
        }, { passive: false });
    }

    document.addEventListener("click", (event) => {
        if (!embed || !commenting) return;
        const target = event.target instanceof Element ? event.target : null;
        if (!target || target.closest("#mk-comment-markers")) return;
        event.preventDefault();
        event.stopPropagation();
        window.parent.postMessage({
            source: SOURCE, type: "point", x: event.pageX, y: event.pageY, target: describeTarget(target),
            anchor: anchorOf(target, event.clientX, event.clientY),
        }, "*");
    }, true);

    document.addEventListener("mouseover", (event) => {
        if (!commenting) return;
        const target = event.target instanceof Element ? event.target : null;
        if (hovered && hovered !== target) hovered.classList.remove("mk-comment-hover");
        hovered = target && target !== document.body && target !== document.documentElement ? target : null;
        if (hovered) hovered.classList.add("mk-comment-hover");
    });

    document.addEventListener("keydown", (event) => {
        if (event.altKey && event.key.toLowerCase() === "t") toggleTheme();
        if (embed && event.key === "Escape") window.parent.postMessage({ source: SOURCE, type: "escape" }, "*");
        // Ctrl+Enter sends the reader's comments, as it does with focus anywhere else in the block.
        if (embed && event.key === "Enter" && (event.ctrlKey || event.metaKey) && !event.repeat) window.parent.postMessage({ source: SOURCE, type: "submit" }, "*");
    });

    // Maximized, holding Space pans: the viewer covers the frame while it is held. Not from a field, where it is typing.
    function panKey(event, down) {
        if (!embed || !panAvailable || event.key !== " ") return;
        const target = event.target instanceof Element ? event.target : null;
        if (target && target.closest("textarea, input, select, [contenteditable]:not([contenteditable=\"false\"])")) return;
        event.preventDefault();
        window.parent.postMessage({ source: SOURCE, type: "pan-key", down }, "*");
    }
    document.addEventListener("keydown", (event) => panKey(event, true), true);
    document.addEventListener("keyup", (event) => panKey(event, false), true);
    window.addEventListener("blur", () => {
        if (embed && panAvailable) window.parent.postMessage({ source: SOURCE, type: "pan-key", down: false }, "*");
    });

    // A Ctrl/Cmd or middle-button press pans too, with no key to shield the frame first: it goes to the viewer as
    // {type:"pan-drag",phase:"start",x,y}, which then covers the frame and takes the rest of the drag ({type:"pan-end"}
    // when it ends). A release that still lands here is {phase:"end"}; the click a pan ends in never lands.
    let panDrag = false;
    let swallowClick = false;
    function endPanDrag(event) {
        if (!panDrag) return;
        panDrag = false;
        document.documentElement.classList.remove("mk-panning");
        if (event) window.parent.postMessage({ source: SOURCE, type: "pan-drag", phase: "end", x: event.clientX, y: event.clientY }, "*");
        // The click a drag ends in comes straight after its pointerup; one that never comes mustn't eat a later click.
        setTimeout(() => { swallowClick = false; });
    }
    document.addEventListener("pointerdown", (event) => {
        if (panDrag) return;
        swallowClick = false;
        if (!embed || !panAvailable) return;
        if (event.button !== 1 && !(event.button === 0 && (event.ctrlKey || event.metaKey))) return;
        event.preventDefault();
        event.stopPropagation();
        panDrag = true;
        swallowClick = true;
        document.documentElement.classList.add("mk-panning");
        window.parent.postMessage({ source: SOURCE, type: "pan-drag", phase: "start", x: event.clientX, y: event.clientY }, "*");
    }, true);
    document.addEventListener("pointermove", (event) => { if (!event.buttons) endPanDrag(event); }, true);
    document.addEventListener("pointerup", endPanDrag, true);
    document.addEventListener("pointercancel", endPanDrag, true);
    // On the window, so it runs before comment mode's click and the mockup's own.
    for (const type of ["click", "auxclick"]) {
        window.addEventListener(type, (event) => {
            if (!swallowClick) return;
            swallowClick = false;
            event.preventDefault();
            event.stopPropagation();
        }, true);
    }

    // The part of a pan the viewer's stage couldn't take: scrolls the panels under the pointer, innermost first, then the page.
    function scrollAt(at, dx, dy) {
        const take = (el, canX, canY) => {
            const left = el.scrollLeft;
            const top = el.scrollTop;
            if (canX) el.scrollLeft = left + dx;
            if (canY) el.scrollTop = top + dy;
            dx -= el.scrollLeft - left;
            dy -= el.scrollTop - top;
        };
        const page = document.scrollingElement || document.documentElement;
        for (let el = document.elementFromPoint(at.x, at.y); el && el !== page && el !== document.body && (dx || dy); el = el.parentElement) {
            const style = getComputedStyle(el);
            take(el, /auto|scroll|overlay/.test(style.overflowX), /auto|scroll|overlay/.test(style.overflowY));
        }
        if (dx || dy) take(page, true, true);
    }

    document.addEventListener("click", (event) => {
        const target = event.target instanceof Element ? event.target : null;
        if (!target) return;
        if (target.closest("[data-toggle-theme]")) {
            toggleTheme();
            return;
        }
        const pin = target.closest(".mk-pin, [data-req]");
        const requirement = pin && citedRequirement(pin);
        if (embed && requirement) {
            event.preventDefault();
            window.parent.postMessage({ source: SOURCE, type: "pin", requirement, label: (pin.textContent || "").trim() }, "*");
        }
    });

    window.MockupKit = { setTheme: applyTheme, setPins, toggleTheme };
})();
