using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace server.Data;

public class TheDailyLensContext: IdentityDbContext<ApplicationUser>{


    public TheDailyLensContext(DbContextOptions<TheDailyLensContext> options): base(options){

    } 


}