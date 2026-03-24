using System.Text;
using System.Text.Json;
using ESLViewer.Models.State;
using Microsoft.JSInterop;

namespace ESLViewer.Services;

/// <summary>
/// Single owner of all save/restore/serialization logic.
/// Has no dependency on Blazor components or JS interop directly —
/// JS interop is performed by calling methods that accept an <see cref="IJSRuntime"/>
/// parameter, keeping this service testable and component-agnostic.
/// </summary>
public class StateService
{
    private const string CookieName = "eslviewer_state";

    /// <summary>
    /// Known supported versions. Deserialize returns null for truly unknown versions
    /// while still tolerating extra/unknown JSON fields for forward compatibility.
    /// </summary>
    private static readonly HashSet<string> KnownVersions = new() { "1.0", "1.1" };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        // Unknown properties are silently ignored (default in STJ), which
        // gives forward compatibility when newer clients read older JSON.
    };

    // ── Serialization ─────────────────────────────────────────────────────────

    /// <summary>Serializes <paramref name="snapshot"/> to an indented JSON string.</summary>
    public string Serialize(AppSnapshot snapshot)
        => JsonSerializer.Serialize(snapshot, JsonOptions);

    /// <summary>
    /// Deserializes a JSON string into an <see cref="AppSnapshot"/>.
    /// Returns null on malformed JSON or on a completely unrecognised version string.
    /// Unknown extra fields are silently ignored (forward-compatible reads).
    /// </summary>
    public AppSnapshot? Deserialize(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        try
        {
            var snapshot = JsonSerializer.Deserialize<AppSnapshot>(json, JsonOptions);
            if (snapshot is null) return null;

            // Unknown version: return null so the caller can decide how to handle it.
            // Known-version snapshots with extra fields are accepted as-is.
            if (!KnownVersions.Contains(snapshot.Version))
                return null;

            return snapshot;
        }
        catch
        {
            return null;
        }
    }

    // ── Cookie ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Serializes <paramref name="snapshot"/> and writes it to the
    /// <c>eslviewer_state</c> browser cookie (session cookie by default).
    /// The value is URI-encoded by the JS helper.
    /// Pass <paramref name="days"/> &gt; 0 for a persistent cookie.
    /// </summary>
    public async Task SaveToCookieAsync(AppSnapshot snapshot, IJSRuntime js, int days = 0)
    {
        var json = Serialize(snapshot);
        await js.InvokeVoidAsync("ESLViewer.setCookie", CookieName, json, days);
        // Update the synchronous state cache so beforeunload can persist it without async round-trip
        try { await js.InvokeVoidAsync("ESLViewer.updateStateCache", json); } catch { }
    }

    /// <summary>
    /// Reads the <c>eslviewer_state</c> cookie and deserializes it.
    /// Returns null if the cookie is absent or the content is invalid.
    /// </summary>
    public async Task<AppSnapshot?> LoadFromCookieAsync(IJSRuntime js)
    {
        var value = await js.InvokeAsync<string?>("ESLViewer.getCookie", CookieName);
        return string.IsNullOrEmpty(value) ? null : Deserialize(value);
    }

    // ── Clipboard ─────────────────────────────────────────────────────────────

    /// <summary>Serializes <paramref name="snapshot"/> and writes it to the system clipboard.</summary>
    public async Task SaveToClipboardAsync(AppSnapshot snapshot, IJSRuntime js)
    {
        var json = Serialize(snapshot);
        await js.InvokeVoidAsync("navigator.clipboard.writeText", json);
    }

    /// <summary>
    /// Reads the clipboard text and deserializes it as an <see cref="AppSnapshot"/>.
    /// Returns null if the clipboard is empty or does not contain valid snapshot JSON.
    /// </summary>
    public async Task<AppSnapshot?> LoadFromClipboardAsync(IJSRuntime js)
    {
        var json = await js.InvokeAsync<string?>("navigator.clipboard.readText");
        return string.IsNullOrEmpty(json) ? null : Deserialize(json);
    }

    // ── File (download / upload) ──────────────────────────────────────────────

    /// <summary>
    /// Serializes <paramref name="snapshot"/> and returns a
    /// <c>data:application/json;base64,…</c> URL suitable for use as the
    /// <c>href</c> of a download anchor element.
    /// This method is synchronous — no JS interop needed for URL generation.
    /// </summary>
    public string GetDownloadDataUrl(AppSnapshot snapshot)
    {
        var json = Serialize(snapshot);
        var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        return $"data:application/json;base64,{base64}";
    }

    /// <summary>
    /// Deserializes the provided file text as an <see cref="AppSnapshot"/>.
    /// File reading is handled in the UI layer; this method only parses.
    /// </summary>
    public AppSnapshot? LoadFromFileContent(string fileContent)
        => Deserialize(fileContent);
}

