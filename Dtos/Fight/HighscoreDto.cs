namespace dotnet_rpg.Dtos
{
    public class HighscoreDto
    {
        public int MyProperty { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Fights { get; set; }
        public int Victories { get; set; }
        public int Defeats { get; set; }
    }
}