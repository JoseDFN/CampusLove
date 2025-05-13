using SGCI_app.domain.Factory;

namespace SGCI_app.infrastructure.postgres;

public class ConexDBFactory : IDbfactory
{
    private readonly string _connectionString;

    public ConexDBFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
}
