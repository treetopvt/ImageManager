using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Linq;

using Breeze.ContextProvider;
using Breeze.ContextProvider.EF6;
using ImageManager.DataModel;
using ExifLib;
using log4net;
using System.Reflection;
using System.Threading.Tasks;
using ImageProcessor;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageManager.DataAccess
{
	public class ImageManagerRepository
	{
		private IList<ImageModel> _ImageList;
		private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private IList<FolderModel> _FolderList;
        private FolderModel _RootFolder;

		private string _rootPath = "";

		public ImageManagerRepository()
		{
			_ImageList = new List<ImageModel>();
            _FolderList = new List<FolderModel>();
		}

		public ImageManagerRepository(string rootPath)
		{
			_rootPath = rootPath;
			//_ImageList = FolderModel.GetImageList(rootPath); //only the root images
            _RootFolder = new FolderModel(rootPath);
            _ImageList = _RootFolder.Images;
            _FolderList = _RootFolder.GetAllSubFolders();
		}

		public IQueryable<ImageModel> Images {
			get{
				return _ImageList.AsQueryable(); ;
		}
			set{
			_ImageList = value.ToList();
			}
		}

        public IQueryable<FolderModel> Folders
        {
            get {
                var rtnList = new List<FolderModel>();
                rtnList.Add(_RootFolder);
                rtnList.AddRange(_FolderList);
                return rtnList.AsQueryable();
            }
            set { _FolderList = value.ToList(); }
        }
        public FolderModel RootFolder { get; set; }

        /// <summary>
        /// will breadth-first search image tree for matching image
        /// </summary>
        /// <param name="imageGuid"></param>
        /// <param name="desiredSize"></param>
        /// <returns></returns>
		public async Task<byte[]> GetImageBytes(Guid imageGuid, ImageManager.DataModel.ImageModel.ImageSize desiredSize)
		{
            string imgPath = GetImagePath(imageGuid);

            return await GetImageBytes(imgPath, desiredSize);
		}

        public async Task<byte[]> GetImageBytes(Guid folderGuid, Guid imageGuid, ImageManager.DataModel.ImageModel.ImageSize desiredSize)
        {
            string imgPath = GetImagePath(folderGuid, imageGuid);

            return await GetImageBytes(imgPath, desiredSize);
        }


        private async Task<byte[]> GetImageBytes(string imgPath, ImageManager.DataModel.ImageModel.ImageSize desiredSize)
        {
            byte[] photoBytes = null;// _ImageRepository.GetImageThumbnail(id); //use the imageresizer for thumbnails
            if (string.IsNullOrEmpty(imgPath))
            {
                return null;
            }
            using (var file = new FileStream(imgPath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                photoBytes = new byte[file.Length];
                await file.ReadAsync(photoBytes, 0, (int)file.Length);
            }
            //using (var fs = new FileStream(imgPath, FileMode.Open, useAsync:true))
            //{
            //    //settings.Add("quality", quality.ToString());
            //    ImageBuilder.Current.Build(fs, bStream, settings);
            //}
            //resized = outStream.ToArray();

            byte[] outBytes = null;

            if (photoBytes != null && photoBytes.Length > 0)
            {
                ImageFormat format = ImageFormat.Jpeg;
                Size size = new Size(150, 150);
                int quality = 70;
                bool resize = true;

                switch (desiredSize)
                {
                    case ImageModel.ImageSize.Thumbnail:
                        size = new Size(45, 45);
                        quality = 50;
                        break;
                    case ImageModel.ImageSize.Original:
                        resize = false;
                        quality = 100;
                        break;
                    default:
                        size = new Size(500, 500);
                        break;
                }

                using (var inStream = new MemoryStream(photoBytes))
                {
                    using (var outStream = new MemoryStream())
                    {
                        using (ImageFactory imageFactory = new ImageFactory())
                        {
                            // Load, resize, set the format and quality and save an image.
                            //imageFactory.Load(inStream)
                            //    .Format(format)
                            //    .Quality(quality)
                            //    .Constrain(size)
                            //    .Save(outStream);
                            var fact = imageFactory.Load(inStream)
                                        .Format(format)
                                        .Quality(quality);
                            if (resize)
                            {
                                fact.Constrain(size);
                            }
                            fact.Save(outStream);
                        }

                        // Do something with the stream.
                        outStream.Position = 0;
                        outBytes = new Byte[outStream.Length];
                        await outStream.ReadAsync(outBytes, 0, (int)outStream.Length);
                        return outBytes;

                    }
                }
            }
            return null;
        }
		public string GetImagePath(Guid id)
		{
			var img = _ImageList.FirstOrDefault(i => i.Id == id);
			if (img !=null){
				return _rootPath + img.RelativePath + img.FileName;
			}
			return string.Empty;
		}

        private string GetImagePath(Guid folderGuid, Guid imageGuid)
        {
            var folder = _FolderList.FirstOrDefault(f => f.Id == folderGuid);
            if (folder != null)
            {
                var img = folder.Images.FirstOrDefault(i=>i.Id == imageGuid);
                if (img !=null){
                    return _rootPath + folder.RelativePath + img.FileName;
                }
            }
            return string.Empty;
        }

		public string MetaData
		{
			get
			{
				try
				{
					return new EFContextProvider<EF.ImageManagerContext>().Metadata();
				}
				catch (Exception ex)
				{
					//throw ex;
					log.Debug("Error getting metadata", ex);
				}
				return "Error retrieveing Metadata";
			}
		}

	}
}