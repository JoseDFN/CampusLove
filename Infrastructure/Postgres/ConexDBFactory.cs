using CampusLove.Domain.Ports;
using CampusLove.Infrastructure.Repositories;
using Npgsql;
using SGCI_app.domain.Factory;

namespace SGCI_app.infrastructure.postgres;

public class ConexDBFactory : IDbfactory
{
    private readonly string _connectionString;

    public ConexDBFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public IAppUserRepository CreateAppUserRepository()
    {
        return new ImpAppUserRepository(_connectionString);
    }
    public IUserCareerRepository CreateUserCareerRepository()
    {
        return new ImpUserCareerRepository(_connectionString);
    }
    public ICareerRepository CreateCareerRepository()
    {
        return new ImpCareerRepository(_connectionString);
    }

    public IGenderRepository CreateGenderRepository()
    {
        return new ImpGenderRepository(_connectionString);
    }
    public IUserTypeRepository CreateUserTypeRepository()
    {
        return new ImpUserTypeRepository(_connectionString);
    }
}
