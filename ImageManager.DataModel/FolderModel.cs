using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageManager.DataModel
{
    public class FolderModel
    {
        public FolderModel()
        {
            Id = Guid.NewGuid();//will have to change to a DB entry later?
            ChildFolders = new List<FolderModel>();
            Images = new List<ImageModel>();
        }

        /// <summary>
        /// constructor for generating a folder model with no parent folder to track
        /// </summary>
        /// <param name="rootPath"></param>
        public FolderModel(string rootPath):this()
        {
            var dir = new DirectoryInfo(rootPath);
            ParentFolder =null;
            Name = dir.Name;
            foreach (var d in dir.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly))
            {
                ChildFolders.Add(new FolderModel(d, this));
            }

            Images = GetImageList(dir);
        }

        public FolderModel(DirectoryInfo directoryToPopulateFrom, FolderModel parentFolder):this()
        {
            Name = directoryToPopulateFrom.Name;
            ParentFolder = parentFolder;
            if (parentFolder != null)
            {
                ParentFolderId = parentFolder.Id;
            }
            foreach (var dir in directoryToPopulateFrom.EnumerateDirectories("*.*", SearchOption.TopDirectoryOnly))
            {
                ChildFolders.Add(new FolderModel(dir, this));
            }

            Images = GetImageList(directoryToPopulateFrom);
        }

        public Guid Id { get; set; }
        public Guid? ParentFolderId { get; set; }
        public string Name { get; set; }
        public string RelativePath { get{
            if (ParentFolder != null)
            {
                return ParentFolder.RelativePath + @"\" + ParentFolder.Name + @"\";
            }
            return "";
        }
            private set { }
        }
        public virtual FolderModel ParentFolder { get; set; }
        public virtual IList<FolderModel> ChildFolders { get; set; }
        public virtual IList<ImageModel> Images { get; set; }

        /// <summary>
        /// flattens all sub folders of the current folder and returns them as a list
        /// </summary>
        /// <returns></returns>
        public IList<FolderModel> GetAllSubFolders(){
            return returnChildFoldersRecursive(this);
        }

        #region Helper Methods

        private static IList<FolderModel> returnChildFoldersRecursive(FolderModel folder){
            var rtnList = new List<FolderModel>();
            foreach (var f in folder.ChildFolders)
	        {
                rtnList.Add(f);
                rtnList.AddRange(returnChildFoldersRecursive(f));
	        }
            return rtnList;
        }

        public List<ImageModel> GetImageList(string imagePath)
        {
            var dir = new DirectoryInfo(imagePath);
            return GetImageList(dir);
        }
        private List<ImageModel> GetImageList(DirectoryInfo dir)
        {
            var rtnImage = new List<ImageModel>();
            var images = dir.GetFiles().Where(f => IsImage(f));
            foreach (var image in images)
            {
                rtnImage.Add(new ImageModel(image, dir.FullName) { Folder = this});
            }
            return rtnImage;
        }
        public static bool IsImage(FileInfo f)
        {
            var extension = f.Extension.ToLowerInvariant().TrimStart('.');
            switch (extension)
            {
                case "jpg":
                case "png":
                case "bmp":
                case "gif":
                    return true;
                default:
                    return false;
                    break;
            }

        }
        #endregion
    }

}
