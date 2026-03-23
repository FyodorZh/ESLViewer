window.ESLViewer = window.ESLViewer || {};

// ─── Chart axis range ────────────────────────────────────────────────────────

/**
 * Returns the current zoom/pan axis range for the ApexCharts instance
 * rendered inside the element with the given id.
 * Returns { xMin, xMax, yMin, yMax } (nulls where not finite), or null if
 * the chart instance cannot be located.
 */
window.ESLViewer.getChartAxisRange = function (chartElementId) {
    const el = document.getElementById(chartElementId);
    if (!el) return null;

    // ApexCharts registers every instance in window.Apex._chartInstances
    // as { id, group, chart }. We find the one whose root element is inside
    // our container div.
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

/**
 * Programmatically sets the visible axis range on the ApexCharts instance
 * rendered inside the element with the given id.
 * Null values for any parameter mean "leave that axis at auto".
 */
window.ESLViewer.setChartAxisRange = function (chartElementId, xMin, xMax, yMin, yMax) {
    const el = document.getElementById(chartElementId);
    if (!el) return;

    const instances = window.Apex && window.Apex._chartInstances;
    if (!instances || instances.length === 0) return;

    const entry = instances.find(function (i) { return el.contains(i.chart.el); });
    if (!entry) return;

    const chart = entry.chart;
    try {
        if (xMin !== null && xMax !== null) {
            chart.zoomX(xMin, xMax);
        }
        if (yMin !== null && yMax !== null) {
            chart.updateOptions({ yaxis: { min: yMin, max: yMax } }, false, false);
        }
    } catch (e) { /* ignore — chart may not be fully initialised */ }
};

// ─── Cookie helpers (used by StateService) ───────────────────────────────────

/**
 * Sets a cookie. If days === 0 the cookie is a session cookie (no expiry).
 * The value is URI-encoded so it survives cookie serialisation.
 */
window.ESLViewer.setCookie = function (name, value, days) {
    let expires = '';
    if (days > 0) {
        const d = new Date();
        d.setTime(d.getTime() + days * 24 * 60 * 60 * 1000);
        expires = '; expires=' + d.toUTCString();
    }
    document.cookie = name + '=' + encodeURIComponent(value) + expires + '; path=/';
};

/**
 * Reads a cookie by name. Returns the decoded value or null if not found.
 */
window.ESLViewer.getCookie = function (name) {
    const nameEQ = name + '=';
    const cookies = document.cookie.split(';');
    for (let c of cookies) {
        c = c.trim();
        if (c.startsWith(nameEQ)) {
            return decodeURIComponent(c.substring(nameEQ.length));
        }
    }
    return null;
};

// ─── beforeunload helpers (used by PanelGrid auto-save) ──────────────────────

/**
 * Registers a beforeunload listener that invokes AutoSaveAsync on the
 * provided .NET object reference (best-effort — async may not complete).
 */
window.ESLViewer.registerBeforeUnload = function (dotNetRef) {
    window._eslViewerBeforeUnloadHandler = function () {
        dotNetRef.invokeMethodAsync('AutoSaveAsync');
    };
    window.addEventListener('beforeunload', window._eslViewerBeforeUnloadHandler);
};

/**
 * Removes the beforeunload listener previously registered by registerBeforeUnload.
 */
window.ESLViewer.unregisterBeforeUnload = function () {
    if (window._eslViewerBeforeUnloadHandler) {
        window.removeEventListener('beforeunload', window._eslViewerBeforeUnloadHandler);
        window._eslViewerBeforeUnloadHandler = null;
    }
};

// ─── Misc UI helpers ─────────────────────────────────────────────────────────

/**
 * Programmatically clicks a DOM element by id (used to trigger download links).
 */
window.ESLViewer.clickElement = function (elementId) {
    const el = document.getElementById(elementId);
    if (el) el.click();
};

