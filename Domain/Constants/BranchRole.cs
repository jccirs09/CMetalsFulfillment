namespace CMetalsFulfillment.Domain.Constants;

public static class BranchRole
{
    public const string SystemAdmin = "SystemAdmin";
    public const string BranchAdmin = "BranchAdmin";
    public const string Supervisor = "Supervisor";
    public const string Planner = "Planner";
    public const string Operator = "Operator";
    public const string LoaderChecker = "LoaderChecker";
    public const string Driver = "Driver";
    public const string Viewer = "Viewer";

    public static readonly string[] All = { SystemAdmin, BranchAdmin, Supervisor, Planner, Operator, LoaderChecker, Driver, Viewer };
}
