using ImageManager.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManager.DataAccess.EF.Configurations
{
    internal class ImageModelConfiguration : EntityTypeConfiguration<ImageModel>
    {
        public ImageModelConfiguration()
        {
            //Property(i=>i.Id).IsRequired();
            Property(i=>i.FileName).IsRequired();
            

        }
    }
}
