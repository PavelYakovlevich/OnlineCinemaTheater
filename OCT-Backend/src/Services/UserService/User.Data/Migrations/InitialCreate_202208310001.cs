using FluentMigrator;

namespace User.Data.Migrations;

[Migration(202208310001)]
public class InitialCreate_202208310001 : Migration
{
    private const string userTableName = "Users";

    public override void Down()
    {
        Delete.Table(userTableName);
    }

    public override void Up()
    {
        Create.Table(userTableName)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
            .WithColumn("name").AsString(50).Nullable()
            .WithColumn("middlename").AsString(50).Nullable()
            .WithColumn("surname").AsString(50).Nullable()
            .WithColumn("birthday").AsDate().Nullable()
            .WithColumn("photo").AsString(int.MaxValue).Nullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable();
    }
}
