using AutoMapper;
using dotbet_rpg.Models;
using dotnet_rpg.Data;
using dotnet_rpg.Dtos;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Services
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FightService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto request)
        {
            var response = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };

            try
            {
                var characters = await _context.Characters
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => request.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defeated = false;

                while (!defeated)
                {
                    foreach (Character attacker in characters)
                    {
                        var defenders = characters.Where(c => c.Id != attacker.Id).ToList();
                        var defender = defenders[new Random().Next(defenders.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker, defender);
                        }
                        else
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, defender, skill);
                        }

                        response.Data.Log
                            .Add($"{attacker.Name} attacks {defender.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage.");

                        if (defender.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            defender.Defeats++;
                            response.Data.Log.Add($"{defender.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });

                await _context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var defender = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.DefenderId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);

                if (skill == null)
                {
                    response.Success = false;
                    response.Message = "Character doesn't know that skill.";
                    return response;
                }

                int damage = DoSkillAttack(attacker, defender, skill);
                if (defender.HitPoints <= 0)
                    response.Message = $"{defender.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Defender = defender.Name,
                    AttackerHP = attacker.HitPoints,
                    DefenderHP = defender.HitPoints,
                    Damage = damage
                };
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoSkillAttack(Character? attacker, Character? defender, Skill? skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(defender.Defense);

            if (damage > 0)
                defender.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto request)
        {
            var response = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == request.AttackerId);
                var defender = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == request.DefenderId);
                int damage = DoWeaponAttack(attacker, defender);
                if (defender.HitPoints <= 0)
                    response.Message = $"{defender.Name} has been defeated!";

                await _context.SaveChangesAsync();

                response.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Defender = defender.Name,
                    AttackerHP = attacker.HitPoints,
                    DefenderHP = defender.HitPoints,
                    Damage = damage
                };
            }
            catch (System.Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        private static int DoWeaponAttack(Character? attacker, Character? defender)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(defender.Defense);

            if (damage > 0)
                defender.HitPoints -= damage;
            return damage;
        }

        public async Task<ServiceResponse<List<HighscoreDto>>> GetHighscore()
        {
            var characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();

            var response = new ServiceResponse<List<HighscoreDto>>
            {
                Data = characters.Select(c => _mapper.Map<HighscoreDto>(c)).ToList()
            };

            return response;
        }
    }
}