namespace CMetalsFulfillment.Features.Auth
{
    public static class AuthConstants
    {
        public const string PolicyBranchAccess = "BranchAccessPolicy";
        public const string PolicyCanAdminBranch = "CanAdminBranch";
        public const string PolicyCanPlan = "CanPlan";
        public const string PolicyCanExecuteProduction = "CanExecuteProduction";
        public const string PolicyCanVerifyLoad = "CanVerifyLoad";
        public const string PolicyCanVerifyDelivery = "CanVerifyDelivery";

        public const string RoleSystemAdmin = "SystemAdmin";
        public const string RoleBranchAdmin = "BranchAdmin";
        public const string RolePlanner = "Planner";
        public const string RoleSupervisor = "Supervisor";
        public const string RoleOperator = "Operator";
        public const string RoleLoaderChecker = "LoaderChecker";
        public const string RoleDriver = "Driver";
    }
}
