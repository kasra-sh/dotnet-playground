using Microsoft.Extensions.Options;

namespace Boiler.Web.Host.AutoApi;

// New: Configurable options for Auto API route generation
public class AutoApiOptions
{
    public bool IsEnabled { get; set; } = true;
    /// <summary>
    /// Controls how action sub-routes are generated from method names.
    /// </summary>
    public AutoApiRouteStyle RouteStyle { get; set; } = AutoApiRouteStyle.RestfulCleanRoot;

    /// <summary>
    /// If true (default), pure verb methods (e.g. GetAll with no suffix) map to the clean root route (no sub-template).
    /// Only applies when RouteStyle is RestfulCleanRoot or StripPrefixAndCleanRoot.
    /// </summary>
    public bool UseRootForPureVerbActions { get; set; } = true;

    /// <summary>
    /// If true (default), avoids duplicating the entity name in the sub-route (e.g. GetUsers on UsersAppService → root instead of /users/users).
    /// Only applies when RouteStyle is RestfulCleanRoot.
    /// </summary>
    public bool AvoidEntityDuplicateInSubRoute { get; set; } = true;
}

public enum AutoApiRouteStyle
{
    /// <summary>
    /// Always use the full method name (including verb prefix) in kebab-case as sub-template.
    /// Example: GetAllUsers → /api/users/get-all-users
    ///          GetAll     → /api/users/get-all
    /// </summary>
    FullMethodName,

    /// <summary>
    /// Strip the matched verb prefix, then use kebab-case of the remaining part.
    /// If remaining is empty and UseRootForPureVerbActions=true, maps to root.
    /// Example: GetAllUsers → /api/users/all-users
    ///          GetAll     → /api/users (if UseRootForPureVerbActions=true)
    /// </summary>
    StripPrefix,

    /// <summary>
    /// Like StripPrefix, but also avoids duplicating the entity name in sub-route (clean RESTful style).
    /// Example: GetAllUsers → /api/users/all-users
    ///          GetUsers   → /api/users (avoids /users/users)
    ///          GetAll     → /api/users
    /// </summary>
    RestfulCleanRoot
}