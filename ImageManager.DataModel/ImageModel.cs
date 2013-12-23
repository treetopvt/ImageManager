using ExifLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageManager.DataModel
{
    public class ImageModel
    {
        public enum ImageSize
        {
            Thumbnail = 0,
            Small = 1,
            Medium = 2,
            Large = 3,
            Original = 4
        }


        public ImageModel()
        {

        }
        public ImageModel(FileInfo file, string rootPath)
        {
            Id = Guid.NewGuid();//will have to change to a DB entry later?
            FileName = file.Name;
            var rtPath = rootPath.TrimEnd('\\');
            RelativePath = file.DirectoryName.Replace(rtPath, "");//gets relative directory structure
            DirectoryName = file.Directory.Name;
            GetExifs(file);
        }

        public Guid Id { get; set; }
        private FolderModel _folder;
        public FolderModel Folder
        {
            get { return _folder; }
            set
            {
                this.FolderId = value.Id;
                _folder = value;
            }
        }
        public Guid FolderId { get; set; }
        public string FileName { get; set; }
        public string RelativePath { get; set; }
        public string DirectoryName { get; set; }
        public DateTime? DateTaken { get; set; }
        
        //public Dictionary<string, string> Tags { get; set; }

        private void GetExifs(FileInfo file)
        {
            var extension = file.Extension.ToLowerInvariant();
            if (extension == ".jpg" || extension == ".png")
            {
                try
                {
                    ExifReader reader = new ExifReader(file.FullName);
                    // Extract the tag data using the ExifTags enumeration
                    DateTime datePictureTaken;
                    if (reader.GetTagValue<DateTime>(ExifTags.DateTimeDigitized,
                                        out datePictureTaken))
                    {
                        DateTaken = datePictureTaken;
                    }

                    string comment = string.Empty;
                    if (reader.GetTagValue<string>(ExifTags.UserComment, out comment))
                    {
                        //Tags.Add("Comment", comment);
                    }

            //look to add GPS, and artist?

                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}