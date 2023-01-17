using FluentMigrator;

namespace MediaInfo.Data.Migrations;

[Migration(202209070001)]
public class InitialCreate_202209070001 : Migration
{
    private const string MediasTable = "Medias";
    private const string GenresTable = "Genres";
    private const string ParticipantsTable = "Participants";
    private const string FilmsParticipantsTable = "Participants_Medias";
    private const string FilmsGenresTable = "Genres_Medias";

    public override void Down()
    {
        Delete.Table(MediasTable);
        Delete.Table(GenresTable);
        Delete.Table(ParticipantsTable);
        Delete.Table(FilmsParticipantsTable);
        Delete.Table(FilmsGenresTable);
    }

    public override void Up()
    {
        Execute.Sql("CREATE EXTENSION IF NOT EXISTS \"uuid-ossp\"");

        Create.Table(MediasTable)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("name").AsString(50).Nullable()
            .WithColumn("picture").AsString(int.MaxValue).Nullable()
            .WithColumn("budget").AsCurrency().Nullable()
            .WithColumn("aid").AsCurrency().Nullable()
            .WithColumn("description").AsString(int.MaxValue)
            .WithColumn("issueDate").AsDate()
            .WithColumn("isFree").AsBoolean()
            .WithColumn("ageRating").AsInt32()
            .WithColumn("isTVSerias").AsBoolean()
            .WithColumn("isVisible").AsBoolean()
            .WithColumn("country").AsString(3);

        Create.Table(GenresTable)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("name").AsString(50);

        Create.Table(ParticipantsTable)
            .WithColumn("id").AsGuid().NotNullable().PrimaryKey().WithDefault(SystemMethods.NewGuid)
            .WithColumn("name").AsString(50)
            .WithColumn("surname").AsString(50)
            .WithColumn("birthday").AsDate().Nullable()
            .WithColumn("description").AsString(int.MaxValue).Nullable()
            .WithColumn("picture").AsString(int.MaxValue).Nullable()
            .WithColumn("role").AsInt32()
            .WithColumn("country").AsString(3);

        Create.Index().OnTable(ParticipantsTable).OnColumn("name");
        Create.Index().OnTable(ParticipantsTable).OnColumn("surname");
        Create.Index().OnTable(ParticipantsTable).OnColumn("role");

        Create.Table(FilmsParticipantsTable)
            .WithColumn("participantId").AsGuid().NotNullable().PrimaryKey().ForeignKey(ParticipantsTable, "id")
                .OnDelete(System.Data.Rule.Cascade)
            .WithColumn("mediaId").AsGuid().NotNullable().PrimaryKey().ForeignKey(MediasTable, "id")
                .OnDelete(System.Data.Rule.Cascade);

        Create.Table(FilmsGenresTable)
            .WithColumn("genreId").AsGuid().NotNullable().PrimaryKey().ForeignKey(GenresTable, "id")
            .WithColumn("mediaId").AsGuid().NotNullable().PrimaryKey().ForeignKey(MediasTable, "id")
                .OnDelete(System.Data.Rule.Cascade);
    }
}
