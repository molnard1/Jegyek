namespace Jegyek
{
    public class Dtos
    {
        public record PostDto(int Jegy, string Leiras);
        public record PutDto(Guid Id, int Jegy, string Leiras);
    }
}
