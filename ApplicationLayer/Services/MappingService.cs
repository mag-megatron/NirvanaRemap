using System.Collections.Generic;
// Para IEnumerable
using Core.Events.Inputs;        // Para ControllerInput
using Core.Events.Outputs;
using Core.Interfaces; // Para MappedOutput
using Core.Services;             // Para ProfileMapping (do Core)

namespace ApplicationLayer.Services
{
    /// <summary>
    /// Serviço da camada de aplicação responsável por processar entradas de controlador
    /// através de um perfil de mapeamento definido no Core.
    /// </summary>
    public class MappingService : IMappingService
    {
        private readonly ProfileMapping _profile; // Instância do ProfileMapping do Core

        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="MappingService"/>.
        /// </summary>
        /// <param name="profile">
        /// O perfil de mapeamento (<see cref="ProfileMapping"/> do Core) que contém as regras
        /// para transformar entradas em saídas. Este perfil já deve estar configurado
        /// com os mapeamentos desejados.
        /// </param>
        public MappingService(ProfileMapping profile)
        {
            _profile = profile;
        }

        /// <summary>
        /// Processa uma dada <see cref="ControllerInput"/> usando o perfil de mapeamento configurado.
        /// </summary>
        /// <param name="input">A entrada do controlador a ser processada.</param>
        /// <returns>
        /// Uma coleção de <see cref="MappedOutput"/> resultante da aplicação das regras de mapeamento
        /// do perfil à entrada. Pode ser vazia se nenhuma regra corresponder.
        /// </returns>
        public IEnumerable<MappedOutput> Map(ControllerInput input)
        {
            return _profile.Apply(input);
        }
        
    }
}