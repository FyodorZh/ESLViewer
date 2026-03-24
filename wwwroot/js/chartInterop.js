window.ESLViewer = window.ESLViewer || {};

// ─── Chart axis range ────────────────────────────────────────────────────────

window.ESLViewer.getChartAxisRange = function (chartElementId) {
    const el = document.getElementById(chartElementId);
    if (!el) return null;
    const instances = window.Apex && window.Apex._chartInstances;
    if (!instances || instances.length === 0) return null;
    const entry = instances.find(function (i) { return el.contains(i.chart.el); });
    if (!entry) return null;
    const globals = entry.chart.w && entry.chart.w.globals;
    if (!globals) return null;
    return {
        xMin: isFinite(globals.minX) ? globals.minX : null,
        xMax: isFinite(globals.maxX) ? globals.maxX : null,
        yMin: isFinite(globals.minY) ? globals.minY : null,
        yMax: isFinite(globals.maxY) ? globals.maxY : null
    };
};

window.ESLViewer.setChartAxisRange = function (chartElementId, xMin, xMax, yMin, yMax) {
    const el = document.getElementById(chartElementId);
    if (!el) return;
    const instances = window.Apex && window.Apex._chartInstances;
    if (!instances || instances.length === 0) return;
    const entry = instances.find(function (i) { return el.contains(i.chart.el); });
    if (!entry) return;
    const chart = entry.chart;
    try {
        if (xMin !== null && xMax !== null) chart.zoomX(xMin, xMax);
        if (yMin !== null && yMax !== null) chart.updateOptions({ yaxis: { min: yMin, max: yMax } }, false, false);
    } catch (e) { }
};

// ─── Storage (localStorage replaces cookies for better capacity & persistence) ─

/**
 * Saves a value to localStorage. The `days` parameter is kept for API
 * compatibility but is ignored (localStorage persists until cleared).
 */
window.ESLViewer.setCookie = function (name, value, days) {
    try {
        localStorage.setItem(name, value);
        // Keep _stateKey in sync so the beforeunload handler always uses the right key (#11)
        window.ESLViewer._stateKey = name;
        // Also update the unload cache so beforeunload can save synchronously
        window.ESLViewer._stateCache = value;
    } catch (e) { /* storage full or private mode */ }
};

/**
 * Reads a value from localStorage. Returns null if absent.
 */
window.ESLViewer.getCookie = function (name) {
    try {
        return localStorage.getItem(name);
    } catch (e) { return null; }
};

// ─── State cache for synchronous beforeunload save ────────────────────────────

window.ESLViewer._stateCache = null;
window.ESLViewer._stateKey = 'eslviewer_state';

/**
 * Updates the in-memory state cache without going through .NET.
 * Called by C# after every explicit save so beforeunload can persist it synchronously.
 */
window.ESLViewer.updateStateCache = function (json) {
    window.ESLViewer._stateCache = json;
};

// ─── beforeunload helpers ─────────────────────────────────────────────────────

window.ESLViewer.registerBeforeUnload = function (dotNetRef) {
    window._eslViewerBeforeUnloadHandler = function () {
        // Synchronously persist whatever state was last saved explicitly
        if (window.ESLViewer._stateCache) {
            try { localStorage.setItem(window.ESLViewer._stateKey, window.ESLViewer._stateCache); } catch (e) { }
        }
        // Also fire the async .NET auto-save for the most up-to-date state
        // (may not complete before unload, but often does in WASM)
        dotNetRef.invokeMethodAsync('AutoSaveAsync');
    };
    window.addEventListener('beforeunload', window._eslViewerBeforeUnloadHandler);
};

window.ESLViewer.unregisterBeforeUnload = function () {
    if (window._eslViewerBeforeUnloadHandler) {
        window.removeEventListener('beforeunload', window._eslViewerBeforeUnloadHandler);
        window._eslViewerBeforeUnloadHandler = null;
    }
};

// ─── Misc UI helpers ─────────────────────────────────────────────────────────

window.ESLViewer.clickElement = function (elementId) {
    const el = document.getElementById(elementId);
    if (el) el.click();
};

// ─── Grid cell resizers (Stage 4) ────────────────────────────────────────────

/**
 * Initialises (or re-initialises) column and row resizer handles for the
 * CSS-Grid element identified by `containerId`.
 *
 * @param {string}  containerId  id of the grid container div
 * @param {number}  minCellPx    minimum cell width/height in pixels
 * @param {object}  dotNetRef    DotNetObjectReference with OnResizeComplete(colFracs, rowFracs)
 */
window.ESLViewer.initGridResizers = function (containerId, minCellPx, dotNetRef) {
    const el = document.getElementById(containerId);
    if (!el) return;

    // Remove stale resizers and disconnect old ResizeObserver
    el.querySelectorAll('.esl-resizer').forEach(function (r) { r.remove(); });
    if (el._eslResizeObs) { el._eslResizeObs.disconnect(); el._eslResizeObs = null; }

    // Read grid track counts from computed style
    const cs = window.getComputedStyle(el);
    const colWidths  = cs.gridTemplateColumns.split(' ').map(parseFloat).filter(isFinite);
    const rowHeights = cs.gridTemplateRows.split(' ').map(parseFloat).filter(isFinite);
    const xSize = colWidths.length;
    const ySize = rowHeights.length;
    if (xSize < 1 || ySize < 1) return;

    // Store config on the element for use in drag handlers
    el._eslGrid = { dotNetRef, minCellPx, xSize, ySize };

    // Create column resizers (one per internal column boundary)
    for (let i = 0; i < xSize - 1; i++) {
        const r = document.createElement('div');
        r.className = 'esl-resizer esl-col-resizer';
        r.dataset.idx = i;
        el.appendChild(r);
        r.addEventListener('pointerdown', window.ESLViewer._startResize);
    }

    // Create row resizers (one per internal row boundary)
    for (let j = 0; j < ySize - 1; j++) {
        const r = document.createElement('div');
        r.className = 'esl-resizer esl-row-resizer';
        r.dataset.idx = j;
        el.appendChild(r);
        r.addEventListener('pointerdown', window.ESLViewer._startResize);
    }

    window.ESLViewer._positionResizers(el);

    // Reposition resizers when the container is resized (e.g. window resize)
    el._eslResizeObs = new ResizeObserver(function () {
        window.ESLViewer._positionResizers(el);
    });
    el._eslResizeObs.observe(el);
};

/** Repositions all resizer handles according to the current computed grid track sizes. */
window.ESLViewer._positionResizers = function (el) {
    const cs = window.getComputedStyle(el);
    const colWidths  = cs.gridTemplateColumns.split(' ').map(parseFloat).filter(isFinite);
    const rowHeights = cs.gridTemplateRows.split(' ').map(parseFloat).filter(isFinite);
    const gap = parseFloat(cs.gap) || parseFloat(cs.columnGap) || 0;
    const rowGap = parseFloat(cs.rowGap) || gap;

    const h = el.clientHeight;
    const w = el.clientWidth;

    let x = 0;
    el.querySelectorAll('.esl-col-resizer').forEach(function (r, i) {
        x += colWidths[i] || 0;
        r.style.left   = (x + gap / 2 - 4) + 'px';
        r.style.top    = '0';
        r.style.width  = '8px';
        r.style.height = h + 'px';
        x += gap;
    });

    let y = 0;
    el.querySelectorAll('.esl-row-resizer').forEach(function (r, j) {
        y += rowHeights[j] || 0;
        r.style.left   = '0';
        r.style.top    = (y + rowGap / 2 - 4) + 'px';
        r.style.width  = w + 'px';
        r.style.height = '8px';
        y += rowGap;
    });
};

/** Handles pointerdown on a resizer element — starts the resize drag. */
window.ESLViewer._startResize = function (e) {
    e.preventDefault();
    e.stopPropagation();

    const resizer = e.currentTarget;
    const el = resizer.parentElement;
    const cfg = el._eslGrid;
    if (!cfg) return;

    const isCol = resizer.classList.contains('esl-col-resizer');
    const idx   = parseInt(resizer.dataset.idx);
    const minPx = cfg.minCellPx;

    // Capture pointer so we receive move/up outside the resizer element
    resizer.setPointerCapture(e.pointerId);

    // Snapshot track sizes at drag start
    const cs0 = window.getComputedStyle(el);
    const startCols = cs0.gridTemplateColumns.split(' ').map(parseFloat).filter(isFinite);
    const startRows = cs0.gridTemplateRows.split(' ').map(parseFloat).filter(isFinite);
    const startX = e.clientX;
    const startY = e.clientY;

    // Keep track of current sizes for the final report
    let curCols = startCols.slice();
    let curRows = startRows.slice();

    function onMove(ev) {
        if (isCol) {
            const dx = ev.clientX - startX;
            const cols = startCols.slice();
            let l = cols[idx] + dx;
            let r = cols[idx + 1] - dx;
            // Enforce minimum
            if (l < minPx) { r += l - minPx; l = minPx; }
            if (r < minPx) { l += r - minPx; r = minPx; }
            cols[idx] = l; cols[idx + 1] = r;
            curCols = cols;
            el.style.gridTemplateColumns = cols.map(function (v) { return v + 'fr'; }).join(' ');
        } else {
            const dy = ev.clientY - startY;
            const rows = startRows.slice();
            let t = rows[idx] + dy;
            let b = rows[idx + 1] - dy;
            if (t < minPx) { b += t - minPx; t = minPx; }
            if (b < minPx) { t += b - minPx; b = minPx; }
            rows[idx] = t; rows[idx + 1] = b;
            curRows = rows;
            el.style.gridTemplateRows = rows.map(function (v) { return v + 'fr'; }).join(' ');
        }
        window.ESLViewer._positionResizers(el);
    }

    function onUp() {
        resizer.removeEventListener('pointermove', onMove);
        resizer.removeEventListener('pointerup', onUp);

        // Normalize to fractions that sum to 1
        const colSum = curCols.reduce(function (a, b) { return a + b; }, 0) || 1;
        const rowSum = curRows.reduce(function (a, b) { return a + b; }, 0) || 1;
        const colFracs = curCols.map(function (v) { return v / colSum; });
        const rowFracs = curRows.map(function (v) { return v / rowSum; });

        cfg.dotNetRef.invokeMethodAsync('OnResizeComplete', colFracs, rowFracs)
            .catch(function () { /* component may have been disposed before drag ended */ });
    }

    resizer.addEventListener('pointermove', onMove);
    resizer.addEventListener('pointerup', onUp);
};

/** Cleans up resizer handles and ResizeObserver for a grid container. */
window.ESLViewer.cleanupGridResizers = function (containerId) {
    const el = document.getElementById(containerId);
    if (!el) return;
    // Release pointer capture if a resize drag is in progress (#15)
    el.querySelectorAll('.esl-resizer').forEach(function (r) {
        try { r.releasePointerCapture && r.releasePointerCapture(0); } catch (e) { }
        r.remove();
    });
    if (el._eslResizeObs) { el._eslResizeObs.disconnect(); el._eslResizeObs = null; }
    el._eslGrid = null;
};

// ─── Bootstrap dropdown with fixed positioning (escapes overflow:hidden) ──────

/**
 * Re-initialises a Bootstrap dropdown toggle to use position:fixed via Popper.js,
 * so the dropdown menu is never clipped by an ancestor with overflow:hidden.
 * @param {string} elementId  id of the dropdown toggle button
 */
window.ESLViewer.initFixedDropdown = function (elementId) {
    const el = document.getElementById(elementId);
    if (!el || !window.bootstrap) return;
    const existing = bootstrap.Dropdown.getInstance(el);
    if (existing) existing.dispose();
    new bootstrap.Dropdown(el, {
        popperConfig: function (defaultConfig) {
            return Object.assign({}, defaultConfig, { strategy: 'fixed' });
        }
    });
};

