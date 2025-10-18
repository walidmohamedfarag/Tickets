using Cinema_Ticket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_Ticket.DataAccess.EntityConfigration
{
    public class MovieActorConfigration : IEntityTypeConfiguration<MovieActor>
    {
        public void Configure(EntityTypeBuilder<MovieActor> builder)
        {
            builder.HasKey(k => new { k.MovieId, k.ActorId });
        }
    }
}
