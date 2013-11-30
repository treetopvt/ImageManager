    using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.OData;
using System.Web.Http.OData.Routing;
using ImageManager.Models;
using System.IO;
using System.Configuration;
using System.Web.Http.OData.Query;

namespace ImageManager.Controllers
{
    /*
    To add a route for this controller, merge these statements into the Register method of the WebApiConfig class. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using ImageManager.Models;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<ImageModel>("Images2");
    config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());
    */
    public class Images2Controller : ODataController
    {
       // private ImageManagerContext db = new ImageManagerContext();

        private static List<Models.ImageModel> _ImageList = GetImageList();

        // GET odata/Images2
        [Queryable(AllowedQueryOptions = AllowedQueryOptions.All)]
        public IQueryable<ImageModel> GetImages2()
        {
            return _ImageList.AsQueryable();
        }

        // GET odata/Images2(5)
        [Queryable]
        public SingleResult<ImageModel> GetImageModel([FromODataUri] Guid key)
        {
            return SingleResult.Create(_ImageList.Where(imagemodel => imagemodel.Id == key).AsQueryable());
        }

        // PUT odata/Images2(5)
        public IHttpActionResult Put([FromODataUri] Guid key, ImageModel imagemodel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (key != imagemodel.Id)
            //{
            //    return BadRequest();
            //}

            //db.Entry(imagemodel).State = EntityState.Modified;

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ImageModelExists(key))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return Updated(imagemodel);
            return Updated(new ImageModel());
        }

        // POST odata/Images2
        public IHttpActionResult Post(ImageModel imagemodel)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.ImageModels.Add(imagemodel);

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateException)
            //{
            //    if (ImageModelExists(imagemodel.Id))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return Created(new Models.ImageModel());
        }

        // PATCH odata/Images2(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public IHttpActionResult Patch([FromODataUri] Guid key, Delta<ImageModel> patch)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            ImageModel imagemodel = _ImageList.FirstOrDefault(i => i.Id == key);
            //if (imagemodel == null)
            //{
            //    return NotFound();
            //}

            //patch.Patch(imagemodel);

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ImageModelExists(key))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            return Updated(imagemodel);
        }

        // DELETE odata/Images2(5)
        public IHttpActionResult Delete([FromODataUri] Guid key)
        {
            //ImageModel imagemodel = db.ImageModels.Find(key);
            //if (imagemodel == null)
            //{
            //    return NotFound();
            //}

            //db.ImageModels.Remove(imagemodel);
            //db.SaveChanges();

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ImageModelExists(Guid key)
        {
            return _ImageList.Count(e => e.Id == key) > 0;
        }

        #region "Helper Methods"

        private static List<Models.ImageModel> GetImageList()
        {
            var rtnImage = new List<Models.ImageModel>();
            var imagePath = GetImagePath(IsDebug());//@"D:\Pictures\";
            var dir = new DirectoryInfo(imagePath);
            var images = dir.GetFiles().Where(f => IsImage(f));
            foreach (var image in images)
            {
                rtnImage.Add(new Models.ImageModel(image, imagePath));
            }
            return rtnImage;
        }

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
        private static string GetImagePath(bool isDev)
        {

            if (ConfigurationSettings.AppSettings.HasKeys())
            {
                string valuePath = isDev ? "DevImagePath" : "ImagePath";
                string[] values = ConfigurationSettings.AppSettings.GetValues("DevImagePath");
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
