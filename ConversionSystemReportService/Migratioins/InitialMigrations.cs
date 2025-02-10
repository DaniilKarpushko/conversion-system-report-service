using FluentMigrator;
using FluentMigrator.Expressions;
using FluentMigrator.Infrastructure;

namespace ConversionSystemReportService.Migratioins;

[Migration(version: 1727972999, description: "Initial migration")]
public class InitialMigrations : IMigration
{
    public void GetUpExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = $"""
                            create table "user" (
                                "user_id" serial primary key,
                                "name" text not null,
                                "email" text not null unique
                            );
                            
                            create table "product" (
                                "product_id" serial primary key,
                                "product_name" text not null,
                                "price" decimal not null
                            );
                            
                            create table "purchased" (
                                "purchase_id" serial primary key,
                                "user_id" int not null,
                                "product_id" int not null,
                                "purchased_at" timestamp not null,
                                foreign key ("user_id") references "user" ("user_id") on delete cascade,
                                foreign key ("product_id") references "product" ("product_id") on delete cascade
                            );
                            
                            create table "report" (
                                "report_id" text primary key,
                                "ratio" double precision not null,
                                "purchase_amount" int not null,
                                "state" int not null
                            );
                            
                            create table "viewed" (
                                "view_id" serial primary key,
                                "user_id" int not null,
                                "product_id" int not null,
                                "viewed_at" timestamp not null,
                                foreign key ("user_id") references "user" ("user_id") on delete cascade,
                                foreign key ("product_id") references "product" ("product_id") on delete cascade
                            );
                            """
        });
    }

    public void GetDownExpressions(IMigrationContext context)
    {
        context.Expressions.Add(new ExecuteSqlStatementExpression
        {
            SqlStatement = $"""
                            drop table if exists viewed;
                            drop table if exists purchased;
                            drop table if exists report;
                            drop table if exists product;
                            drop table if exists "user";
                            """
        });
    }

    public string ConnectionString => throw new NotSupportedException();
}