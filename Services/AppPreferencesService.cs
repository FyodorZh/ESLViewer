using ESLViewer.Models;

namespace ESLViewer.Services;

/// <summary>
/// Singleton service that holds the current application preferences.
/// Updated by PanelGrid when state is loaded; read by other services at request time.
/// </summary>
public class AppPreferencesService
{
    public AppPreferences Preferences { get; private set; } = new();

    /// <summary>Replaces the current preferences with the given snapshot value.</summary>
    public void Apply(AppPreferences? prefs)
    {
        Preferences = prefs ?? new AppPreferences();
    }
}

