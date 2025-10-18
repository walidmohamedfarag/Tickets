using Cinema_Ticket.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinema_Ticket.DataAccess.EntityConfigration
{
    public class MovieSubImgConfigration : IEntityTypeConfiguration<MovieSubImg>
    {
        public void Configure(EntityTypeBuilder<MovieSubImg> builder)
        {
            builder.HasKey(k => new { k.MovieId, k.Img });
        }
    }
}
