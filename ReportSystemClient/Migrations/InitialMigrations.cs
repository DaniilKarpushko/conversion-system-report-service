using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace ApiGateway.Migrations;

[Migration(version: 1727972999, description: "Initial migration")]
public class InitialMigrations: IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = $"""
                            create table request (
                                product_id integer not null,
                                request_id text not null,
                                start timestamp not null,
                                "end" timestamp not null,
                                primary key (request_id)
                            );
                            """
        });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = $"drop table if exists request;"
        });
    }

    public string ConnectionString => throw new NotSupportedException();
}