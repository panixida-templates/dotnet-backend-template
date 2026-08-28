namespace Organization.Product.Host;

internal static class SonarQualityGateProbe
{
    // TODO: Remove after verifying that SonarQube blocks the pull request.
    internal static void Execute()
    {
    }

    internal static void Fail()
    {
        throw new Exception("SonarQube quality gate probe");
    }
}
