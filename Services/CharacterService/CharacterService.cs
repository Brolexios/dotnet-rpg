namespace dotnet_rpg.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper mapper;
        private readonly DataContext dcontext;
        public CharacterService(IMapper mapper, DataContext dcontext)
        {
            this.mapper = mapper;
            this.dcontext = dcontext;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var character = mapper.Map<Character>(newCharacter);
            dcontext.Characters.Add(character);
            await dcontext.SaveChangesAsync();
            serviceResponse.Data = await dcontext.Characters.Select(c => mapper.Map<GetCharacterDto>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            var dbCharacters = await dcontext.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => mapper.Map<GetCharacterDto>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            var dbCharacter = await dcontext.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = mapper.Map<GetCharacterDto>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                var character = await dcontext.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id)
                    ?? throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
                character.Name = updatedCharacter.Name;
                character.HitPoints = updatedCharacter.HitPoints;
                character.Strength = updatedCharacter.Strength;
                character.Defense = updatedCharacter.Defense;
                character.Intelligence = updatedCharacter.Intelligence;
                character.Class = updatedCharacter.Class;

                await dcontext.SaveChangesAsync();
                serviceResponse.Data = mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();
            try
            {
                var character = await dcontext.Characters.FirstOrDefaultAsync(c => c.Id == id)
                    ?? throw new Exception($"Character with Id '{id}' not found.");
                dcontext.Characters.Remove(character);
                await dcontext.SaveChangesAsync();
                serviceResponse.Data = await dcontext.Characters.Select(c => mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}