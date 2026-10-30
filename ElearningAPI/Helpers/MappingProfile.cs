using AutoMapper;
using ElearningAPI.Models;
using ElearningAPI.Models.DTOs;

namespace ElearningAPI.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth mapping
            CreateMap<ApplicationUser, UserDto>();
            CreateMap<RegisterDto, ApplicationUser>();

            // Course mapping
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.TeacherName, 
                    opt => opt.MapFrom(src => $"{src.Teacher.FirstName} {src.Teacher.LastName}"))
                .ForMember(dest => dest.StudentCount,
                    opt => opt.MapFrom(src => src.UserCourses.Count))
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.UserCourses
                        .Where(uc => uc.Rating.HasValue)
                        .Average(uc => uc.Rating)));

            CreateMap<Course, CourseDetailsDto>()
                .IncludeBase<Course, CourseDto>();

            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Module mapping
            CreateMap<Module, ModuleDto>();
            CreateMap<CreateModuleDto, Module>();

            // Lesson mapping
            CreateMap<Lesson, LessonDto>();
            CreateMap<CreateLessonDto, Lesson>();

            // Quiz mapping
            CreateMap<Quiz, QuizDto>();
            CreateMap<CreateQuizDto, Quiz>();

            // Question mapping
            CreateMap<Question, QuestionDto>();
            
            // AnswerOption mapping
            CreateMap<AnswerOption, AnswerOptionDto>();
            // Module mapping
            CreateMap<Module, ModuleDto>();
            CreateMap<CreateModuleDto, Module>();
            CreateMap<UpdateModuleDto, Module>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // ÚJ

            // Lesson mapping
            CreateMap<Lesson, LessonDto>();
            CreateMap<CreateLessonDto, Lesson>();
            CreateMap<UpdateLessonDto, Lesson>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); // ÚJ

            // Forum mapping
            CreateMap<ForumPost, ForumPostDto>()
                .ForMember(dest => dest.Author,
                    opt => opt.MapFrom(src => src.User));
                    
            CreateMap<CreateForumPostDto, ForumPost>();

            // Progress mapping
            CreateMap<Progress, ProgressDto>();

            // UserCourse mapping
            CreateMap<UserCourse, EnrollmentDto>();

            // QuizAttempt mapping
            CreateMap<QuizAttempt, QuizAttemptDto>();
        }
    }


}