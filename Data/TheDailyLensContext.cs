using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace server.Data;

public class TheDailyLensContext: IdentityDbContext{


    public TheDailyLensContext(DbContextOptions<TheDailyLensContext> options): base(options){

    } 


}