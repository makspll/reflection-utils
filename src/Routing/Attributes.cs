using System.Text.Json.Serialization;

namespace Makspll.ReflectionUtils.Routing;

public enum RoutePropagation
{
    /// <summary>
    /// If an action has no route, will propagate to it and enable routing
    /// </summary>
    PropagateToActions,

    /// <summary>
    /// If an action has no route, this will not propagate to it, will only propagate to actions with already existing routes.
    /// </summary>
    PropagateToRoutedActions,

    /// <summary>
    /// The attribute does not propagate to actions or does not have a route
    /// </summary>
    None,
}

public enum HTTPMethod
{
    GET,
    POST,
    PUT,
    DELETE,
    PATCH,
    HEAD,
    OPTIONS
}

/// Tagged union of attributes relating to routing
public abstract class RoutingAttribute(string name)
{
    public string Name { get; set; } = name;

    /// <summary>
    /// If true seeing this attribute on a class marks it as a controller and makes it participate in route resolution.
    /// </summary>
    public virtual bool EnablesController() => false;


    /// <summary>
    /// If the attribute has a route attached to it, returns it
    /// </summary>
    public abstract string? Route();

    /// <summary>
    /// Return the route propagation strategy of the attribute
    /// </summary>
    public virtual RoutePropagation Propagation() => RoutePropagation.None;

    /// <summary>
    /// If the attribute overrides the HTTP method, return it
    /// </summary>
    public virtual HTTPMethod? HttpMethodOverride() => null;

}

public class ApiControllerAttribute : RoutingAttribute
{
    public ApiControllerAttribute() : base("ApiController") { }

    public override string? Route() => null;

    public override bool EnablesController() => true;
}

public class RouteAttribute(string? path) : RoutingAttribute("Route")
{
    public string? Path { get; init; } = path;

    public override string? Route() => Path;

    public override RoutePropagation Propagation() => RoutePropagation.PropagateToActions;
}

public class HttpAttribute(HTTPMethod method, string? route) : RoutingAttribute($"Http{char.ToUpper(method.ToString()[0])}{method.ToString()[1..].ToLower()}")
{
    public HTTPMethod Method { get; init; } = method;
    public string? Path { get; init; } = route;
    public override string? Route() => Path;

    public override HTTPMethod? HttpMethodOverride() => Method;
}
