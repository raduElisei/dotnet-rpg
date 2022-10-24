namespace dotnet_rpg.Dtos
{
    public class AttackResultDto
    {
        public string Attacker { get; set; } = string.Empty;
        public string Defender { get; set; } = string.Empty;
        public int AttackerHP { get; set; }
        public int DefenderHP { get; set; }
        public int Damage { get; set; }
    }
}