//               HTTP Request
//                     │                    dotnet run --project TmsApi.Api --urls
//                     ▼
//               Program.cs
//           app.MapControllers()
//                     │
//                     ▼
//           CoursesController
//                     │
//                     ▼
//       CreateCourseRequest DTO
//       (Model Binding + Validation)
//                     │
//                     ▼
//             ICourseService
//                     │
//                     ▼
//             CourseService
//      (Business Logic & Rules)
//                     │
//                     ▼
//              TmsDbContext
//                     │
//                     ▼
//                 EF Core
//                     │
//                     ▼
//              PostgreSQL Database
//                     │
//                     ▼
//              CourseResponseDto
//                     │
//                     ▼
//           CoursesController
//                     │
//                     ▼
//             201 Created
//                     │
//                     ▼
//                  Scalar