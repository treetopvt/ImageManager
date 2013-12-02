using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ImageManager.Models
{
    [MetadataTypeAttribute(typeof(ImageModel.ImageModelMetadata))]
    public partial class ImageModel
    {
        internal sealed class ImageModelMetadata
        {
            //metadata classes should not be instantiated
            private ImageModelMetadata()
            {

            }
            //add all the properties that exist on the public model

            [Key]
            public Guid Id { get; set; }
            [Required]
            public string FileName { get; set; }
            public string RelativePath { get; set; }
            public string DirectoryName { get; set; }
            public DateTime? DateTaken { get; set; }

        }
    }
}