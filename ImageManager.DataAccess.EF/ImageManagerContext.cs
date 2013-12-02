using ImageManager.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManager.DataAccess.EF
{
    internal class ImageManagerContext :DbContext
    {
            
        static ImageManagerContext()
        {
            // Prevent attempt to initialize a database for this context
            Database.SetInitializer<ImageManagerContext>(null);
        }
 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new Configurations.ImageModelConfiguration());
            //modelBuilder.Configurations.Add(new OrderDtoConfiguration());
            //modelBuilder.Configurations.Add(new OrderDetailDtoConfiguration());
            //modelBuilder.Configurations.Add(new ProductDtoConfiguration());
        }
 
        public DbSet<ImageModel> Images { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<Product> Products { get; set; }
     }
}
