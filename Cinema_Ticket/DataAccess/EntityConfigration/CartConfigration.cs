
namespace Cinema_Ticket.DataAccess.EntityConfigration
{
    public class CartConfigration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => new { c.MovieId , c.UserId});
        }
    }
}
