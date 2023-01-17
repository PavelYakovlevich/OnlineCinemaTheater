using Authentication.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Authentication.Data.Context.EntityConfigurations;

internal class EmailConfirmationTokenConfiguration : IEntityTypeConfiguration<EmailConfirmationToken>
{
    public void Configure(EntityTypeBuilder<EmailConfirmationToken> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne<Account>()
            .WithMany()
            .HasForeignKey(x => x.AccountId);

        builder.Property(x => x.IssueDate)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Token)
            .IsRequired();
    }
}
