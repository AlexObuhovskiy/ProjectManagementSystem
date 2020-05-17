Import database using ./Data/ProjectManagementSystem.bacpac file.

to scaffold DBContext 


1. Open the console in Visual Studio using the Tools > NuGet Package Manager > Package Manager Console command.
2. Change Default Project to `ProjectManagementSystem.DataAccess`
3. Run this command in Package Manager Console for Rob.DataAccess project (you might change the `sa` <password>):

`Scaffold-DbContext "Persist Security Info=False;User ID=sa;Password=testtest;Initial Catalog=ProjectManagementSystem;Server=." Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -t Project, Task -f`

3.1 If error occurs (sometimes it does) with StartUp project, then set `ProjectManagementSystem.DataAccess` as StartUp
4. Remove 'void OnConfiguring(DbContextOptionsBuilder optionsBuilder)' method