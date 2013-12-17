﻿using System;
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


		private string _rootPath = "";

		public ImageManagerRepository()
		{
			_ImageList = new List<ImageModel>();
		}

		public ImageManagerRepository(string rootPath)
		{
			_rootPath = rootPath;
			_ImageList = GetImageList(rootPath);
		}

		public IQueryable<ImageModel> Images {
			get{
				return _ImageList.AsQueryable(); ;
		}
			set{
			_ImageList = value.ToList();
			}
		}

		//public byte[] GetImageThumbnail(Guid imageGuid){
		//    try
		//    {
		//        var img = _ImageList.FirstOrDefault(i=> i.Id == imageGuid);
		//        if (img !=null){
		//            ExifReader reader = new ExifReader(_rootPath + img.RelativePath + img.FileName);
		//            return reader.GetJpegThumbnailBytes();
		//        }
		//        return null;
		//    }catch(Exception ex)
		//    {
		//        log.Error("Error retrieving thumbnail using ExifReader", ex);
		//        //do nothing here, no exif data existed
		//    }
		//    return null;
		//}


		public async Task<byte[]> GetImageBytes(Guid imageGuid, ImageManager.DataModel.ImageModel.ImageSize desiredSize)
		{
			byte[] photoBytes = null;// _ImageRepository.GetImageThumbnail(id); //use the imageresizer for thumbnails

			string imgPath = GetImagePath(imageGuid);
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
						size = new Size(45,45);
						quality = 50;
						break;
					case ImageModel.ImageSize.Original:
						resize = false;
						quality= 100;
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
                            imageFactory.Load(inStream)
                                .Format(format)
                                .Quality(quality)
                                .Resize(size)
                                .Save(outStream);
                            //var fact = imageFactory.Load(inStream)
                            //            .Format(format)
                            //            .Quality(quality);
                            //if (resize)
                            //{
                            //    fact.Resize(size);
                            //}
                            //    fact.Save(outStream);
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

		#region Helper Methods

		private static List<ImageModel> GetImageList(string imagePath)
		{
			var rtnImage = new List<ImageModel>();
			var dir = new DirectoryInfo(imagePath);
			var images = dir.GetFiles().Where(f => IsImage(f));
			foreach (var image in images)
			{
				rtnImage.Add(new ImageModel(image, imagePath));
			}
			return rtnImage;
		}

		private static bool IsImage(FileInfo f)
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

		//protected override string BuildJsonMetadata()
		//{

		//    ODataModelBuilder mb = new ODataConventionModelBuilder();
		//    mb.EntitySet<ImageModel>("Images");
		//    mb.Namespace = "ImageManager.Models";  // DON'T FORGET THIS! //WebAPIODataWithBreezeConsumer
			
		//    var edmModel = mb.GetEdmModel();
		//    IEnumerable<EdmError> errors;
		//    String csdl;
		//    using (var swriter = new StringWriter())
		//    {
		//        using (var xwriter = new XmlTextWriter(swriter))
		//        {
		//            //edmModel.TryWriteCsdl(xwriter, out errors);
					
		//            // CsdlWriter.TryWriteCsdl(edmModel, xwriter, out errors);
		//            EdmxWriter.TryWriteEdmx(edmModel, xwriter, EdmxTarget.OData, out errors);
		//            csdl = swriter.ToString();

		//        }
		//    }
		//    var xele = XElement.Parse(csdl);
		//    var ns = xele.Name.Namespace;
		//    var dataServicesEle = xele.Descendants(ns + "DataServices").First();
		//    var xDoc = XDocument.Load(dataServicesEle.CreateReader());
		//    var json = ContextProvider.XDocToJson(xDoc);
		//    return json;
		//}



	}
}