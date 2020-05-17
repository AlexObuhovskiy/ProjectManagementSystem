using System.Threading.Tasks;
using AutoMapper;
using ProjectManagementSystem.DataAccess.Interfaces;
using ProjectManagementSystem.Domain.Interfaces;
using ProjectManagementSystem.Domain.Models.Task;

namespace ProjectManagementSystem.Domain.Services
{
    /// <summary>
    /// Class TaskService for work with tasks.
    /// Implements the <see cref="ITaskService" />
    /// </summary>
    /// <seealso cref="ITaskService" />
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<DataAccess.Models.Task> _taskRepository;
        private readonly IMapper _mapper;
        private readonly IProjectStateService _projectStateService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="projectStateService">The project state service.</param>
        public TaskService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IProjectStateService projectStateService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _projectStateService = projectStateService;
            _taskRepository = _unitOfWork.GetRepository<DataAccess.Models.Task>();
        }

        /// <inhertidoc/>
        public async Task<TaskResponseDto[]> GetAllTasks()
        {
            var tasks = await _taskRepository.Get();
            var responseDtoArray = _mapper.Map<TaskResponseDto[]>(tasks);

            return responseDtoArray;
        }
    }
}
