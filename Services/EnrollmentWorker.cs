namespace EnrollmentLab.Services;

public class EnrollmentWorker
{
    private readonly IServiceScopeFactory _scopeFactory;

    public EnrollmentWorker(
        IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task RunAsync()
    {
        //Create temporary sc0ope
        using var scope =
            _scopeFactory.CreateScope();

        var service =
            scope.ServiceProvider
                 .GetRequiredService<IEnrollmentService>();

        await service.EnrollAsync(
            "S001",
            "CS101");
    }
}