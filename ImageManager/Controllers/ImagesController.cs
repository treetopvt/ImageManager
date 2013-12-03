﻿using Breeze.WebApi2;
using ImageManager.DataAccess;
using ImageManager.DataModel;
using ImageResizer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.OData;
using System.Web.Http.OData.Query;

namespace ImageManager.Controllers
{
    //ApiController
    [BreezeController]
    public class ImagesController : EntitySetController<ImageModel, Guid>
    {

        private static ImageManagerRepository _ImageRepository = new ImageManagerRepository(GetImagePath(IsDebug()));
        // GET api/<controller>
        [Queryable(AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<ImageModel> GetImages()
        {

            //return new string[] { "value1", "value2" };
            return _ImageRepository.Images;
        }

        [HttpGet]
        public string Metadata()
        {
            return _ImageRepository.MetaData;
        }

        // GET api/<controller>/5
        public HttpResponseMessage GetImage(int id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var imagePath = GetImagePath(IsDebug());
            var filePath = imagePath + "dance magic dance.jpg";
            //String filePath = HostingEnvironment.MapPath("~/Images/HT.jpg");
            
            FileStream fileStream = new FileStream(filePath, FileMode.Open);
            Image image = Image.FromStream(fileStream);
            MemoryStream memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Jpeg);
            result.Content = new ByteArrayContent(memoryStream.ToArray());
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            return result;
        }

        [Queryable]
        public SingleResult<ImageModel> GetImageModel([FromODataUri] Guid key)
        {
            return SingleResult.Create(_ImageRepository.Images.Where(imagemodel => imagemodel.Id == key).AsQueryable());
        }

        [HttpGet]
        public HttpResponseMessage GetImageBinary(Guid id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            string imgPath = _ImageRepository.GetImagePath(id);
            if (string.IsNullOrEmpty(imgPath))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            } 
            //String filePath = HostingEnvironment.MapPath("~/Images/HT.jpg");

            using (var fs = new FileStream(imgPath, FileMode.Open))
            {
                Image image = Image.FromStream(fs);
                using (var memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, ImageFormat.Jpeg);
                    result.Content = new ByteArrayContent(memoryStream.ToArray());
                }
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            }
            return result;
        }

        [HttpGet]
        public HttpResponseMessage GetImageThumbnail(Guid id)
        {
            
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            byte[] bytes = null;// _ImageRepository.GetImageThumbnail(id); //use the imageresizer for thumbnails
            MemoryStream bStream = new MemoryStream();
            if (bytes == null)
            {
                string imgPath = _ImageRepository.GetImagePath(id);
                if (string.IsNullOrEmpty(imgPath)){
                     return new HttpResponseMessage(HttpStatusCode.NotFound);
                }
                var settings = new ResizeSettings
                {
                    MaxWidth = 45,
                    MaxHeight = 45,
                    Format = "jpg"
                };
                using (var fs = new FileStream(imgPath, FileMode.Open))
                {
                    //settings.Add("quality", quality.ToString());
                    ImageBuilder.Current.Build(fs, bStream, settings);
                }
                //resized = outStream.ToArray();

            }else{
                bStream = new MemoryStream(bytes);
            }

            if (bStream != null && bStream.Length > 0){
                Image image = Image.FromStream(bStream);
                MemoryStream memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Jpeg);
                result.Content = new ByteArrayContent(memoryStream.ToArray());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                return result;
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }

        #region "Helper Methods"

        private static bool IsDebug()
        {
            bool isDebug = false;
 
            #if (DEBUG)
            isDebug = true;
            #else
            isDebug = false;
            #endif
                                
            return isDebug;
        }

 
        private static string GetImagePath(bool isDev){

            if (ConfigurationSettings.AppSettings.HasKeys())
			{
                string valuePath = isDev ? "DevImagePath" : "ImagePath";
                string[] values= ConfigurationSettings.AppSettings.GetValues("DevImagePath");
                if (values.Any())
                    return values[0].EndsWith(@"\") ? values[0] : values[0] + @"\";
                else
                {
                    Console.WriteLine("No customsetting1 application string");
                    return "";
                }
			}
            return "";
        }
        #endregion
    }
}