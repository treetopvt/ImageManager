using Breeze.ContextProvider;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.OData.Builder;
using System.Xml;
using System.Xml.Linq;

namespace ImageManager.Models
{
    public class ImagesDataContext: ContextProvider
    {
        private IList<ImageModel> _ImageList;

        public ImagesDataContext()
        {
            _ImageList = new List<ImageModel>();
        }

        public ImagesDataContext(string rootPath)
        {
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

        #region Helper Methods

        private static List<Models.ImageModel> GetImageList(string imagePath)
        {
            var rtnImage = new List<Models.ImageModel>();
            var dir = new DirectoryInfo(imagePath);
            var images = dir.GetFiles().Where(f => IsImage(f));
            foreach (var image in images)
            {
                rtnImage.Add(new Models.ImageModel(image, imagePath));
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

        protected override string BuildJsonMetadata()
        {

            ODataModelBuilder mb = new ODataConventionModelBuilder();
            mb.EntitySet<ImageModel>("Images");
            mb.Namespace = "ImageManager.Models";  // DON'T FORGET THIS! //WebAPIODataWithBreezeConsumer
            
            var edmModel = mb.GetEdmModel();
            IEnumerable<EdmError> errors;
            String csdl;
            using (var swriter = new StringWriter())
            {
                using (var xwriter = new XmlTextWriter(swriter))
                {
                    //edmModel.TryWriteCsdl(xwriter, out errors);
                    
                    // CsdlWriter.TryWriteCsdl(edmModel, xwriter, out errors);
                    EdmxWriter.TryWriteEdmx(edmModel, xwriter, EdmxTarget.OData, out errors);
                    csdl = swriter.ToString();

                }
            }
            var xele = XElement.Parse(csdl);
            var ns = xele.Name.Namespace;
            var dataServicesEle = xele.Descendants(ns + "DataServices").First();
            var xDoc = XDocument.Load(dataServicesEle.CreateReader());
            var json = ContextProvider.XDocToJson(xDoc);
            return json;
        }

        protected override void CloseDbConnection()
        {
            throw new NotImplementedException();
        }

        public override System.Data.IDbConnection GetDbConnection()
        {
            throw new NotImplementedException();
        }

        protected override void OpenDbConnection()
        {
            throw new NotImplementedException();
        }

        protected override void SaveChangesCore(SaveWorkState saveWorkState)
        {
            throw new NotImplementedException();
        }
    }
}