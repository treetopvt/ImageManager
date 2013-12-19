using ImageManager.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManager.DataAccess.EF.Configurations
{
    internal class FolderModelConfiguration : EntityTypeConfiguration<FolderModel>
    {
        public FolderModelConfiguration()
        {
            //Property(i=>i.Id).IsRequired();
            Property(i=>i.Name).IsRequired();
            HasMany(f => f.ChildFolders)
                .WithRequired(f => f.ParentFolder)
                .HasForeignKey(f => f.ParentFolderId)
                .WillCascadeOnDelete(true);
            HasMany(f => f.Images)
                .WithOptional()
                .HasForeignKey(i => i.FolderId)
                .WillCascadeOnDelete(true);
            
        }
    }
}
